using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using _ChristmasFarmMono.Source.Scripts.GardenBed;
using AddressableExtensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects
{
    public readonly struct GardenBedInHandDTO
    {
        public readonly Vector2 DirectionVector;
        public readonly Vector3 OriginVector;
        public readonly Vector3 ForwardVector;

        public GardenBedInHandDTO(Vector2 directionVector, Vector3 originVector, Vector3 forwardVector)
        {
            DirectionVector = directionVector;
            OriginVector = originVector;
            ForwardVector = forwardVector;
        }
    }

    public interface IHandheldObject
    {
        public void Initialize(HandledObjectView handledObjectView);
        public void ShowCellVisualization(Func<GardenBedInHandDTO> getData);
        public void HideCellVisualization();
        public void PlaceSpecimen(Vector3 originPosition, Vector3 forward);
    }
    
    public sealed class HandledObjectView
    {
        private HandledObjectViewConfig _config;

        public HandledObjectView()
        {
            // Need load config
        }
        
        public HandledObjectView(HandledObjectViewConfig config)
        {
            _config = config;
        }
        
        public GameObject GetHandledObjectView(GameObject handledObject)
        {
            GameObject viewObject = Object.Instantiate(_config.HandledObjectView);
            GameObject newHandledObject = Object.Instantiate(handledObject);
            var offsetY = -.5f + newHandledObject.transform.localScale.y;
            newHandledObject.transform.SetParent(viewObject.transform);
            newHandledObject.transform.localScale = new Vector3(newHandledObject.transform.localScale.x * .9f,
                newHandledObject.transform.localScale.y, newHandledObject.transform.localScale.z * .9f);
            newHandledObject.transform.localPosition = new Vector3(0, offsetY, 0);
            
            return viewObject;
        }
    }
    
    public sealed class GardenBedInHand : IHandheldObject
    {
        private readonly GardenBedItemConfig _config;
        private GameObject _gardenForVisualize;

        private CancellationTokenSource _cancellationTokenSource;

        public GardenBedInHand()
        {
            _config = Addressables.LoadAssetAsync<GardenBedInHandParameters>(Gameconfigs.Gardenbedinhandconfig).WaitForCompletion().itemConfig;
        }

        public void Initialize(HandledObjectView handledObjectView)
        {
            Debug.Log(handledObjectView);
            var tempComponent = handledObjectView.GetHandledObjectView(_config.GardenBedMediator.gameObject);
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
            var position = CalculateNextCellPosition(originPosition, forward);
            var gardenBed = Object.Instantiate(_config.GardenBedMediator, position, Quaternion.identity);
        }
        
        private Vector3 CalculateNextCellPosition(Vector3 originPosition, Vector3 forward)
        {
            var currentDirection = forward;
            currentDirection *= .5f;
            var value = originPosition + currentDirection;

            Debug.Log(value);

            var rounded = new Vector3(
                x: RoundToNearestCell(value.x),
                y: value.y,
                z: RoundToNearestCell(value.z));

            Debug.Log(rounded);
            return rounded;
        }
        
        private float RoundToNearestCell(float value, float cellSize = .5f)
        {
            return Mathf.Round(value / cellSize) * cellSize;
        }
    }
}