using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using GameConfigs = _ChristmasFarmMono.Source.Scripts.Configs.GameConfigs;
using Object = UnityEngine.Object;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects.PathTilesInHand
{
    public sealed class PathTileInHand : HandheldObject
    {
        private const string PathTileId = "path_tile_grass";
        
        private PathTileGroupsConfig _config;
        private PathTilesSpawner _pathTilesSpawner;
        
        private GameObject[] _temporaryPrefabs;
        private readonly Dictionary<PathTileType, GameObject> _temporaryTiles = new ();

        private IPointChecker _pointChecker;

        [Inject]
        public void Construct(PathTilesSpawner pathTilesSpawner, GameConfigs gameConfigs)
        {
            _pathTilesSpawner = pathTilesSpawner;
            _pointChecker = pathTilesSpawner;
            
            _config = gameConfigs.PathTilesItemConfig;
        }

        public override void Initialize(HandheldObjectView handheldObjectView)
        {
            base.Initialize(handheldObjectView);

            foreach (var tile in _config.PathTiles[PathTileId])
            {
                var t = Object.Instantiate(tile.Value.PathVariant);
                
                var localScale = t.transform.localScale;
                
                localScale = new Vector3(localScale.x * .9f,
                    localScale.y, localScale.z * .9f);
                
                t.transform.localScale = localScale;
                
                t.SetActive(false);
                _temporaryTiles.Add(tile.Key, t);
            }
            
            _currentTemporaryContainer = handheldObjectView
                .UpdateHandheldControlData(_temporaryTiles[PathTileType.Straight]);
            
            _currentTemporaryContainer.SetActive(false);
            
            UpdateActiveCell += UpdateHandheldChildObjectView;
        }
        
        protected override bool HasObjectAtPoint(Vector3 point)
        {
            return _pointChecker.HasObjectAtPoint(PathTileId, new Vector3(point.x, 
                _currentTemporaryContainer.ChildObject.transform.localScale.y / 4, point.z));
        }

        protected override void SpawnAtPoint(Vector3 point, Quaternion rotation)
        {
            _pathTilesSpawner.SpawnPathTile(PathTileId, point, Quaternion.identity);
        }

        private void UpdateHandheldChildObjectView(Vector3 point)
        {
            int neighbourCount = CheckHorizontal(point);
            neighbourCount += CheckVertical(point);

            _currentTemporaryContainer.ChildObject.SetActive(false);
            Debug.Log($"Nearby tiles count: {neighbourCount} with Path Tiles Type: {(PathTileType)neighbourCount}");
            _currentTemporaryContainer = _handheldObjectView
                .UpdateHandheldControlData(_temporaryTiles[neighbourCount >= 2 ? (PathTileType)neighbourCount 
                    : PathTileType.Straight]);
            _currentTemporaryContainer.ChildObject.SetActive(true);
        }

        private int CheckHorizontal(Vector3 point)
        {
            var plus = point + new Vector3(0.5f, 0, 0);
            int result = HasObjectAtPoint(plus) ? 1 : 0;
            var minus = point - new Vector3(0.5f, 0, 0);
            result += HasObjectAtPoint(minus) ? 1 : 0;
            
            return result;
        }
        
        private int CheckVertical(Vector3 point)
        {
            int result = HasObjectAtPoint(point + new Vector3(0, 0, 0.5f)) ? 1 : 0;
            result += HasObjectAtPoint(point - new Vector3(0, 0, 0.5f)) ? 1 : 0;

            return result;
        }
    }

    public interface IPointChecker
    {
        public bool HasObjectAtPoint(Vector3 point);
        public bool HasObjectAtPoint(string groupKey, Vector3 point);
    }
    
    public sealed class PathTilesSpawner : IPointChecker
    {
        private Dictionary<string, Dictionary<Vector3, GameObject>> _pathInPlace = new ();
            
        private readonly PathTileGroupsConfig _config;
        
        public event Action CreatedPathTile;

        public PathTilesSpawner(GameConfigs gameConfigs)
        {
            _config = gameConfigs.PathTilesItemConfig;
        }

        public void SpawnPathTile(string pathTileId, Vector3 position, Quaternion rotation)
        {
            var pathTile = _config.PathTiles[pathTileId][PathTileType.Straight].PathVariant;
            
            var tempObject = Object.Instantiate(pathTile, position, rotation);
            tempObject.SetActive(true);

            CreatedPathTile?.Invoke();

            if (_pathInPlace.TryGetValue(pathTileId, out Dictionary<Vector3, GameObject> tiles))
            {
                tiles.Add(position, tempObject);
            }
            else
            {
                _pathInPlace.Add(pathTileId, new Dictionary<Vector3, GameObject> { { position, tempObject } });
            }
        }

        public bool HasObjectAtPoint(Vector3 point)
        {
            foreach (var objectsAtPoints in _pathInPlace)
            {
                if (objectsAtPoints.Value.ContainsKey(point))
                    return true;
            }

            return false;
        }

        public bool HasObjectAtPoint(string groupKey, Vector3 point)
        {
            return _pathInPlace.Count > 0 && _pathInPlace[groupKey].ContainsKey(point);
        }
    }
}