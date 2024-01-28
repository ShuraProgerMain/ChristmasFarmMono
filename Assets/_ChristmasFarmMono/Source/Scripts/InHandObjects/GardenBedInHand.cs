using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _ChristmasFarmMono.Source.Scripts.GardenBed;
using UnityEngine;
using VContainer;
using GameConfigs = _ChristmasFarmMono.Source.Scripts.Configs.GameConfigs;
using Object = UnityEngine.Object;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects
{
    public readonly struct GardenBedInHandDTO
    {
        public readonly Vector3 OriginVector;
        public readonly Vector3 ForwardVector;

        public GardenBedInHandDTO(Vector3 originVector, Vector3 forwardVector)
        {
            OriginVector = originVector;
            ForwardVector = forwardVector;
        }
    }

    public interface IHandheldObject
    {
        public void Initialize(HandledObjectView handledObjectView, GameConfigs gameConfigs);
        public void ShowCellVisualization(Func<GardenBedInHandDTO> getData);
        public void HideCellVisualization();
        public void PlaceSpecimen(Vector3 originPosition, Vector3 forward);
    }

    public sealed class HandledObjectView
    {
        private readonly HandledObjectViewConfig _config;
        private readonly GameObject _viewObject;
        private readonly Material _viewObjectMaterial;

        public HandledObjectView(HandledObjectViewConfig config)
        {
            _config = config;
            _viewObject = Object.Instantiate(_config.HandledObjectView);
            _viewObjectMaterial = _viewObject.GetComponent<MeshRenderer>().material;
        }
        
        public GameObject GetHandledObjectView(GameObject handledObject)
        {
            GameObject newHandledObject = Object.Instantiate(handledObject, _viewObject.transform, true);
            
            var offsetY = -.5f + newHandledObject.transform.localScale.y;
            
            newHandledObject.transform.localScale = new Vector3(newHandledObject.transform.localScale.x * .9f,
                newHandledObject.transform.localScale.y, newHandledObject.transform.localScale.z * .9f);
            
            newHandledObject.transform.localPosition = new Vector3(0, offsetY, 0);
            
            return _viewObject;
        }

        public void SetStateFree(bool isFree)
        {
            _viewObjectMaterial.color = isFree
                ? new Color(0.7490196f, 0.7490196f, 0.7490196f, .3f)
                : new Color(0.7490196f, 0, 0, .3f);
        }
    }
    
    public sealed class GardenBedInHand : IHandheldObject
    {
        private GardenBedItemConfig _config;
        private GardenBedsSpawner _gardenBedsSpawner;
        private GameObject _gardenForVisualize;
        private HandledObjectView _handledObjectView;

        private bool _canPlaceSpecimen;
        private CancellationTokenSource _cancellationTokenSource;

        [Inject]
        private void Construct(GardenBedsSpawner gardenBedsSpawner)
        {
            _gardenBedsSpawner = gardenBedsSpawner;
        }

        public void Initialize(HandledObjectView handledObjectView, GameConfigs gameConfigs)
        {
            _handledObjectView = handledObjectView;
            _config = gameConfigs.GardenBedItemConfig;
            
            var tempComponent = handledObjectView.GetHandledObjectView(_config?.GardenBedMediator.gameObject);
            _gardenForVisualize = tempComponent.gameObject;
            
            Object.Destroy(tempComponent.GetComponentInChildren<GardenBedMediator>());
            Object.Destroy(_gardenForVisualize.GetComponentInChildren<Collider>());
            
            _gardenForVisualize.SetActive(false);
        }

        public async void ShowCellVisualization(Func<GardenBedInHandDTO> getData)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            GardenBedInHandDTO? data = getData?.Invoke();

            if (data is null)
                throw new ArgumentNullException($"The variable 'data' in {nameof(ShowCellVisualization)} is null");
            
            var position = CalculateNextCellPosition(data.Value.OriginVector, data.Value.ForwardVector);
            _gardenForVisualize.transform.position = position;
            _gardenForVisualize.gameObject.SetActive(true);
            
            while (Application.isPlaying && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                GardenBedInHandDTO actualData = getData!.Invoke();
                Vector3 updatedPosition = CalculateNextCellPosition(actualData.OriginVector, actualData.ForwardVector);
                updatedPosition.y = _gardenForVisualize.transform.localScale.y * .5f;

                _gardenForVisualize.transform.position = Vector3.MoveTowards(_gardenForVisualize.transform.position,
                    updatedPosition, 6.5f * Time.deltaTime);

                var hasGardenBedInPoint =
                    _gardenBedsSpawner.HasGardenBedAtPoint(new Vector3(updatedPosition.x, 0, updatedPosition.z));

                _canPlaceSpecimen = !hasGardenBedInPoint;
                
                _handledObjectView.SetStateFree(!hasGardenBedInPoint);
                
                await Awaitable.NextFrameAsync();
            }

            _gardenForVisualize.gameObject.SetActive(false);
        }

        public void HideCellVisualization()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void PlaceSpecimen(Vector3 originPosition, Vector3 forward)
        {
            if (!_canPlaceSpecimen) return;
            
            var position = CalculateNextCellPosition(originPosition, forward);
            _gardenBedsSpawner.SpawnGardenBed(position, Quaternion.identity);
        }
        
        private Vector3 CalculateNextCellPosition(Vector3 originPosition, Vector3 forward)
        {
            var currentDirection = forward;
            currentDirection *= .5f;
            var value = originPosition + currentDirection;

            var rounded = new Vector3(
                x: RoundToNearestCell(value.x),
                y: value.y,
                z: RoundToNearestCell(value.z));

            return rounded;
        }
        
        private float RoundToNearestCell(float value, float cellSize = .5f)
        {
            return Mathf.Round(value / cellSize) * cellSize;
        }
    }

    public sealed class GardenBedsSpawner
    {
        private readonly GardenBedMediator _gardenBedTemplate;
        private readonly Dictionary<Vector3, GardenBedMediator> _gardensInPlace = new ();

        public event Action<GardenBedMediator> CreatedGardenBed;
        
        public GardenBedsSpawner(GameConfigs gameConfigs)
        {
            _gardenBedTemplate = gameConfigs.GardenBedItemConfig?.GardenBedMediator;
        }

        public void SpawnGardenBed(Vector3 position, Quaternion rotation)
        {
            var gardenBed = Object.Instantiate(_gardenBedTemplate, position, rotation);

            CreatedGardenBed?.Invoke(gardenBed);
            _gardensInPlace.Add(position, gardenBed);
        }

        public bool HasGardenBedAtPoint(Vector3 point)
        {
            return _gardensInPlace.ContainsKey(point);
        }

        public GardenBedMediator GetGardenBedMediator(string gardenId)
            => _gardensInPlace.First(x => x.Value.Identifier == gardenId).Value;
    }
}