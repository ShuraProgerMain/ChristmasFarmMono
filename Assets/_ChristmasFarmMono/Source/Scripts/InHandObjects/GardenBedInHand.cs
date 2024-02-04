using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _ChristmasFarmMono.Source.Scripts.GardenBed;
using _ChristmasFarmMono.Source.Scripts.InHandObjects.PathTilesInHand;
using UnityEngine;
using VContainer;
using GameConfigs = _ChristmasFarmMono.Source.Scripts.Configs.GameConfigs;
using Object = UnityEngine.Object;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects
{
    public readonly struct SpatialEntityDTO
    {
        public readonly Vector3 OriginVector;
        public readonly Vector3 ForwardVector;

        public SpatialEntityDTO(Vector3 originVector, Vector3 forwardVector)
        {
            OriginVector = originVector;
            ForwardVector = forwardVector;
        }
    }

    public sealed record HandheldControlData
    {
        public GameObject ParentObject { get; init; }
        public GameObject ChildObject { get; init; }

        public void SetActive(bool value)
        {
            ParentObject.SetActive(value);
            ChildObject.SetActive(value);
        }
    }

    public sealed class HandheldObjectView
    {
        private readonly HandledObjectViewConfig _config;
        
        private readonly GameObject _viewObject;
        
        private readonly Material _viewObjectMaterial;

        public HandheldObjectView(HandledObjectViewConfig config)
        {
            _config = config;
            _viewObject = Object.Instantiate(_config.HandledObjectView);
            _viewObjectMaterial = _viewObject.GetComponent<MeshRenderer>().material;
        }

        public HandheldControlData UpdateHandheldControlData(GameObject handheldObjectInstance)
        {
            handheldObjectInstance.transform.SetParent(_viewObject.transform);

            var offsetY = -.5f + handheldObjectInstance.transform.localScale.y;
            
            handheldObjectInstance.transform.localPosition = new Vector3(0, offsetY, 0);
            
            return new HandheldControlData()
            {
                ParentObject = _viewObject,
                ChildObject = handheldObjectInstance
            };
        }
        
        public void SetStateFree(bool isFree)
        {
            _viewObjectMaterial.color = isFree
                ? new Color(0.7490196f, 0.7490196f, 0.7490196f, .3f)
                : new Color(0.7490196f, 0, 0, .3f);
        }
    }
    
    public sealed class GardenBedInHand : HandheldObject
    {
        private GardenBedItemConfig _config;
        private GardenBedsSpawner _gardenBedsSpawner;
        
        private HandheldObjectView _handheldObjectView;

        private bool _canPlaceSpecimen;
        private CancellationTokenSource _cancellationTokenSource;

        private IPointChecker _pointChecker;

        [Inject]
        private void Construct(GardenBedsSpawner gardenBedsSpawner, GameConfigs gameConfigs)
        {
            _gardenBedsSpawner = gardenBedsSpawner;
            _pointChecker = gardenBedsSpawner;
            
            _config = gameConfigs.GardenBedItemConfig;
        }

        public override void Initialize(HandheldObjectView handheldObjectView)
        {
            base.Initialize(handheldObjectView);

            var instance = Object.Instantiate(_config?.GardenBedMediator.gameObject);
            _currentTemporaryContainer = handheldObjectView.UpdateHandheldControlData(instance);
            _currentTemporaryContainer.SetActive(false);
            
            Object.Destroy(_currentTemporaryContainer.ChildObject.GetComponentInChildren<GardenBedMediator>());
            Object.Destroy(_currentTemporaryContainer.ChildObject.GetComponentInChildren<Collider>());
        }

        protected override bool HasObjectAtPoint(Vector3 point)
        {
            return _pointChecker.HasObjectAtPoint(new Vector3(point.x, 
                _currentTemporaryContainer.ChildObject.transform.localScale.y / 4, point.z));
        }

        protected override void SpawnAtPoint(Vector3 point, Quaternion rotation)
        {
            _gardenBedsSpawner.SpawnGardenBed(point, Quaternion.identity);
        }
    }

    public sealed class GardenBedsSpawner : IPointChecker
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

        public bool HasObjectAtPoint(Vector3 point)
        {
            return _gardensInPlace.ContainsKey(point);
        }

        public bool HasObjectAtPoint(string groupKey, Vector3 point)
        {
            Debug.LogWarning($"{nameof(GardenBedsSpawner)} don't have {nameof(HasObjectAtPoint)} with groupKey implementation");
            return _gardensInPlace.ContainsKey(point);
        }
    }
}