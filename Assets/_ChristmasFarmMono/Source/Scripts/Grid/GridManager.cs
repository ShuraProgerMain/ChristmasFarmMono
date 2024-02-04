using _ChristmasFarmMono.Source.Scripts.Player;
using _ChristmasFarmMono.Source.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace _ChristmasFarmMono.Source.Scripts.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] public Vector2 cellOffset;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private PlayerController playerController;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var current = CalculatePlayerCellPosition();
            Gizmos.DrawCube(current, Vector3.one * .5f);
            
            var leftBottom = new Vector3(-.5f, 0, -.5f);
            var size = .5f;
            var row = 3;
            var column = 3;
            
            for (int i = 0; i < row * column; i++)
            {
                float x = (i % column);
                int y = i / column;
                var position1 = transform.position;
                var position = new Vector3((position1.x) + x, position1.y, (position1.z) + y);

                var nextPoint = leftBottom.x + (size * (x));
                var nextPointY = leftBottom.z + (size * (y));
                Debug.Log($"Next point: {new Vector2(nextPoint, nextPointY)} with X: {x} and Y: {y}".Color(Color.green));
                Gizmos.DrawCube(position, Vector3.one);
            }
        }

        private Vector3 CalculatePlayerCellPosition()
        {
            var currentDirection = playerTransform.forward;
            currentDirection *= .5f;
            var value = playerTransform.position + currentDirection;

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
