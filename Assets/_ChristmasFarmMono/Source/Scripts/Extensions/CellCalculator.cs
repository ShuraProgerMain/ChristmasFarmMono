using JetBrains.Annotations;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Extensions
{
    public static class CellCalculator
    {
        [Pure]
        public static Vector3 CalculateNextCellPosition(Vector3 originPosition, Vector3 forward)
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
        
        private static float RoundToNearestCell(float value, float cellSize = .5f)
        {
            return Mathf.Round(value / cellSize) * cellSize;
        }
    }
}