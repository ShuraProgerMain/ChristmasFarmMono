using _ChristmasFarmMono.Source.Scripts.Player;
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
            
            // var size = 1;
            // var row = 4;
            // var column = 6;
            //
            // Gizmos.color = Color.white;
            // // r * column + column
            // for (int i = 0; i < row * column; i++)
            // {
            //     int x = (i % column) * size;
            //     int y = i / column;
            //     var position1 = transform.position;
            //     var position = new Vector3((position1.x) + x, position1.y, (position1.z) + y);
            //     Gizmos.DrawCube(position, Vector3.one);
            // }
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
