using System;
using System.Threading;
using _ChristmasFarmMono.Source.Scripts.Extensions;
using _ChristmasFarmMono.Source.Scripts.Utils;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects
{
    public abstract class HandheldObject
    {
        protected HandheldObjectView _handheldObjectView;
        
        protected HandheldControlData _currentTemporaryContainer;

        private bool _canPlaceSpecimen;
        
        protected Vector3 _lastCellPosition;
        private CancellationTokenSource _visualizationLoopTokenSource;

        public virtual void Initialize(HandheldObjectView handheldObjectView)
        {
            _handheldObjectView = handheldObjectView;
        }

        public void ShowCellVisualization(Func<SpatialEntityDTO> getData)
        {
            SpatialEntityDTO? data = getData?.Invoke();
            
            if (data is null)
                throw new ArgumentNullException($"The variable 'data' in {nameof(ShowCellVisualization)} is null");
            
            ShowAtStartPosition(data.Value);
            VisualizationLoop(getData);
        }

        public void PlaceSpecimen(Vector3 originPosition, Vector3 forward)
        {
            if (!_canPlaceSpecimen) return;
            
            originPosition.y = _currentTemporaryContainer.ChildObject.transform.localScale.y / 4;
            var position = CellCalculator.CalculateNextCellPosition(originPosition, forward);
                
            SpawnAtPoint(position, Quaternion.identity);

            _canPlaceSpecimen = IsCellOccupiedAt(position);
        }
        
        public void HideCellVisualization()
        {
            _currentTemporaryContainer.SetActive(false);
            _visualizationLoopTokenSource.Cancel();
        }
        
        private void ShowAtStartPosition(in SpatialEntityDTO data)
        {
            var position = CellCalculator.CalculateNextCellPosition(data.OriginVector, data.ForwardVector);
            
            _currentTemporaryContainer.ParentObject.transform.position = position;
            _currentTemporaryContainer.SetActive(true);
        }

        private async void VisualizationLoop(Func<SpatialEntityDTO> getData)
        {
            _visualizationLoopTokenSource = new CancellationTokenSource();
            
            while (Application.isPlaying && !_visualizationLoopTokenSource.Token.IsCancellationRequested)
            {
                CellVisualization(getData);
                await Awaitable.NextFrameAsync();
            }
        }

        private void CellVisualization(Func<SpatialEntityDTO> getData)
        {
            SpatialEntityDTO actualData = getData!.Invoke();
            Vector3 updatedPosition = CellCalculator.CalculateNextCellPosition(actualData.OriginVector, actualData.ForwardVector);
            updatedPosition.y = _currentTemporaryContainer.ParentObject.transform.localScale.y * .5f;

            _currentTemporaryContainer.ParentObject.transform.position = Vector3.MoveTowards(_currentTemporaryContainer.ParentObject.transform.position,
                updatedPosition, 5f * Time.deltaTime);
            
            _canPlaceSpecimen = !IsCellOccupiedAt(updatedPosition);

            _handheldObjectView.SetStateFree(_canPlaceSpecimen);
        }

        protected event Action<Vector3> UpdateActiveCell;
        
        private bool IsCellOccupiedAt(Vector3 point)
        {
            var distanceThresholdSquared = 0.01f * 0.01f;

            if (_lastCellPosition != point && (_currentTemporaryContainer.ParentObject.transform.position - point)
                .sqrMagnitude < distanceThresholdSquared)
            {
                _lastCellPosition = point;
                Debug.Log("Bebra".Color(Color.red));
                
                UpdateActiveCell?.Invoke(point);
                
                return HasObjectAtPoint(point);
            }

            return !_canPlaceSpecimen;
        }

        protected abstract bool HasObjectAtPoint(Vector3 point);
        protected abstract void SpawnAtPoint(Vector3 point, Quaternion rotation);
    }
}