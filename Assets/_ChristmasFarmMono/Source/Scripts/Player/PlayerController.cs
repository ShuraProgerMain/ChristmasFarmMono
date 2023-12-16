using _ChristmasFarmMono.Source.Scripts.Extensions;
using _ChristmasFarmMono.Source.Scripts.GardenBed;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _ChristmasFarmMono.Source.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float rotateSpeed = 50f;
        
        [SerializeField] private Transform viewTransform;
        [SerializeField] private Animator animator;

        [SerializeField] private CollisionDetector collisionDetector;

        private MoveCalculator _moveCalculator;
        private GameplayActions _gameplayActions;
        private IInteractive _currentInteractObject;
        
        private readonly int _speed = Animator.StringToHash("Speed");

        private void OnEnable()
        {
            _moveCalculator = new MoveCalculator();
            _gameplayActions = new GameplayActions();

            _gameplayActions.Character.Interact.performed += Interact;

            collisionDetector.TriggerEnter += OnTriggerEnterCustom;
            collisionDetector.TriggerExit += OnTriggerExitCustom;
        }

        private void Start()
        {
            _gameplayActions.Enable();
        }

        private void Update()
        {
            Move(_gameplayActions.Character.Movement.ReadValue<Vector2>());
        }


        private void OnTriggerEnterCustom(Collider other)
        {
            _currentInteractObject?.TryDropSelect();

            if (other.TryGetComponent(out IInteractive gardenBedMediator))
            {
                _currentInteractObject = gardenBedMediator;
                _currentInteractObject.TrySelect();
            }
        }

        private void OnTriggerExitCustom(Collider other)
        {
            if (other.TryGetComponent(out IInteractive gardenBedMediator))
            {
                gardenBedMediator.TryDropSelect();
            }
        }

        private void Move(Vector2 moveDirection)
        {
            transform.position += _moveCalculator.Move(moveDirection, 
                            transform, moveSpeed, Time.deltaTime);
                        
            viewTransform.rotation = _moveCalculator.SmoothRotation(moveDirection, viewTransform.rotation, rotateSpeed * Time.deltaTime);
            
            animator.SetFloat(_speed, moveDirection.magnitude);
        }

        private void Interact(InputAction.CallbackContext context)
        {
            Debug.Log("Player interact");
            _currentInteractObject?.Interact();
        }

        private void OnDisable()
        {
            _gameplayActions.Disable();
            _gameplayActions.Dispose();
        }
    }
}
