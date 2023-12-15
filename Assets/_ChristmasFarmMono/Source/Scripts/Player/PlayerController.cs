using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float rotateSpeed = 50f;
        
        [SerializeField] private Transform viewTransform;
        [SerializeField] private Animator animator;

        private MoveCalculator _moveCalculator;
        private GameplayActions _gameplayActions;

        private void OnEnable()
        {
            _moveCalculator = new MoveCalculator();
            _gameplayActions = new GameplayActions();
        }

        private void Start()
        {
            _gameplayActions.Enable();
        }

        private void Update()
        {
            Move(_gameplayActions.Character.Movement.ReadValue<Vector2>());
        }

        private void Move(Vector2 moveDirection)
        {
            transform.position += _moveCalculator.Move(moveDirection, 
                            transform, moveSpeed, Time.deltaTime);
                        
            viewTransform.rotation = _moveCalculator.SmoothRotation(moveDirection, viewTransform.rotation, rotateSpeed * Time.deltaTime);
            
            animator.SetFloat("Speed", moveDirection.magnitude);
        }


        private void OnDisable()
        {
            _gameplayActions.Disable();
            _gameplayActions.Dispose();
        }
    }
}
