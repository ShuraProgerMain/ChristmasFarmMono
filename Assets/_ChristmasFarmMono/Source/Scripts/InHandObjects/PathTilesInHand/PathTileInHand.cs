using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _ChristmasFarmMono.Source.Scripts.Extensions;
using UnityEngine;
using VContainer;
using GameConfigs = _ChristmasFarmMono.Source.Scripts.Configs.GameConfigs;
using Object = UnityEngine.Object;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects.PathTilesInHand
{
    public sealed class PathTileInHand : IHandheldObject
    {
        private const string PathTileId = "path_tile_grass";
        
        private PathTilesItemConfig _config;
        private PathTilesSpawner _pathTilesSpawner;
        
        private HandledObjectView _handledObjectView;

        private GameObject[] _temporaryPrefabs;
        private HandheldControlData _currentTemporaryContainer;

        private bool _canPlaceSpecimen;
        private CancellationTokenSource _cancellationTokenSource;

        [Inject]
        public void Construct(PathTilesSpawner pathTilesSpawner, GameConfigs gameConfigs)
        {
            _pathTilesSpawner = pathTilesSpawner;
            _config = gameConfigs.PathTilesItemConfig;
        }
        
        public void Initialize(HandledObjectView handledObjectView, GameConfigs gameConfigs)
        {
            _handledObjectView = handledObjectView;
            
            _currentTemporaryContainer = handledObjectView.GetHandheldControlData(_config?.PathTileGroups
                .First(x => x.GroupIdentifier.Id == PathTileId).PathVariants[0]);
            _currentTemporaryContainer.ParentObject.SetActive(false);
            _currentTemporaryContainer.ChildObject.SetActive(false);
        }

        public async void ShowCellVisualization(Func<GardenBedInHandDTO> getData)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            GardenBedInHandDTO? data = getData?.Invoke();
            
            if (data is null)
                throw new ArgumentNullException($"The variable 'data' in {nameof(ShowCellVisualization)} is null");
            
            var position = CellCalculator.CalculateNextCellPosition(data.Value.OriginVector, data.Value.ForwardVector);
            _currentTemporaryContainer.ParentObject.transform.position = position;
            _currentTemporaryContainer.ParentObject.SetActive(true);
            _currentTemporaryContainer.ChildObject.SetActive(true);
            
            while (Application.isPlaying && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                GardenBedInHandDTO actualData = getData!.Invoke();
                Vector3 updatedPosition = CellCalculator.CalculateNextCellPosition(actualData.OriginVector, actualData.ForwardVector);
                updatedPosition.y = _currentTemporaryContainer.ParentObject.transform.localScale.y * .5f;

                _currentTemporaryContainer.ParentObject.transform.position = Vector3.MoveTowards(_currentTemporaryContainer.ParentObject.transform.position,
                    updatedPosition, 6.5f * Time.deltaTime);

                var hasGardenBedInPoint =
                    _pathTilesSpawner.HasPathTileAtPoint(PathTileId, new Vector3(updatedPosition.x, _currentTemporaryContainer.ChildObject.transform.localScale.y / 2f, updatedPosition.z));

                _canPlaceSpecimen = !hasGardenBedInPoint;
                
                _handledObjectView.SetStateFree(!hasGardenBedInPoint);
                
                await Awaitable.NextFrameAsync();
            }
        }

        public void HideCellVisualization()
        {
            _currentTemporaryContainer.ParentObject.SetActive(false);
            _currentTemporaryContainer.ChildObject.SetActive(false);
            _cancellationTokenSource.Cancel();
        }

        public void PlaceSpecimen(Vector3 originPosition, Vector3 forward)
        {
            if (!_canPlaceSpecimen) return;
            
            originPosition.y = _currentTemporaryContainer.ChildObject.transform.localScale.y / 4;
            var position = CellCalculator.CalculateNextCellPosition(originPosition, forward);
            
            _pathTilesSpawner.SpawnPathTile(PathTileId, position, Quaternion.identity);
        }
    }

    public sealed class PathTilesSpawner
    {
        private Dictionary<string, Dictionary<Vector3, GameObject>> _pathInPlace = new ();
            
        private readonly PathTilesItemConfig _config;
        
        public event Action CreatedPathTile;

        public PathTilesSpawner(GameConfigs gameConfigs)
        {
            _config = gameConfigs.PathTilesItemConfig;
        }

        public void SpawnPathTile(string pathTileId, Vector3 position, Quaternion rotation)
        {
            var pathTile = _config.PathTileGroups.First(x => x.GroupIdentifier.Id == pathTileId).PathVariants[0];
            Debug.Log($"Spawn position {position}");
            var tempObject = Object.Instantiate(pathTile, position, rotation);

            CreatedPathTile?.Invoke();

            if (_pathInPlace.TryGetValue(pathTileId, out Dictionary<Vector3, GameObject> tiles))
            {
                tiles.Add(position, tempObject);
            }
            else
            {
                _pathInPlace.Add(pathTileId, new Dictionary<Vector3, GameObject>() { { position, tempObject } });
            }
        }

        public bool HasPathTileAtPoint(string pathTileId, Vector3 point)
        {
            return _pathInPlace.Count > 0 && _pathInPlace[pathTileId].ContainsKey(point);
        }
    }
}