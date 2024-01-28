using System;
using System.Threading.Tasks;
using _ChristmasFarmMono.Source.Scripts.Extensions;
using _ChristmasFarmMono.Source.Scripts.GardenBed;
using _ChristmasFarmMono.Source.Scripts.InHandObjects;
using _ChristmasFarmMono.Source.Scripts.InHandObjects.InHandsObjectsInventory;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace _ChristmasFarmMono.Source.Scripts.Player
{
    public readonly struct PlayerInteractiveActions
    {
        public readonly string InteractiveActionIdentifier;
        public readonly Func<Task> IntermediateAction;

        public PlayerInteractiveActions(string interactiveActionIdentifier, Func<Task> intermediateAction)
        {
            InteractiveActionIdentifier = interactiveActionIdentifier;
            IntermediateAction = intermediateAction;
        }
    }

    public sealed class InputActionsService : IDisposable
    {
        public GameplayActions GameplayActions { get; }

        public InputActionsService()
        {
            GameplayActions = new GameplayActions();
            GameplayActions.Enable();
        }

        public void EnableUI()
        {
            GameplayActions.Character.Disable();
            GameplayActions.UI.Enable();
        }
        
        public void DisableUI()
        {
            GameplayActions.Character.Enable();
            GameplayActions.UI.Disable();
        }

        public void Dispose()
        {
            GameplayActions?.Disable();
            GameplayActions?.Dispose();
        }
    }

    public sealed class PlayerController : MonoBehaviour, ITickable
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float rotateSpeed = 50f;
        
        [SerializeField] private Transform viewTransform;
        [SerializeField] private Animator animator;

        [SerializeField] private CollisionDetector collisionDetector;
        [SerializeField] private GardenBedItemConfig gardenBedItemConfig;
        [SerializeField] private HandledObjectViewConfig handledObjectViewConfig;

        private MoveCalculator _moveCalculator;
        private InputActionsService _inputActionsService;
        private IInteractive _currentInteractObject;
        
        private Action _interactiveAction;

        private Vector2 _moveVector;

        private Vector2 MoveVector
        {
            get => _moveVector;

            set
            {
                if (value.magnitude > 0)
                {
                    _moveVector = value;
                }
            }
        }

        private readonly int _speed = Animator.StringToHash("Speed");

        [Inject]
        private void Construct(InputActionsService inputActionsService, InHandsObjectInventoryController inHandsObjectInventoryController)
        {
            _inputActionsService = inputActionsService;
            inHandsObjectInventoryController.Initialize(OnSelectHandledObject);
            _moveCalculator = new MoveCalculator();

            _inputActionsService.GameplayActions.Character.Interact.performed += Interact;

            collisionDetector.TriggerEnter += OnTriggerEnterCustom;
            collisionDetector.TriggerExit += OnTriggerExitCustom;
        }
        
        public void Tick()
        {
            Move(_inputActionsService.GameplayActions.Character.Movement.ReadValue<Vector2>());
        }
        
        private void OnTriggerEnterCustom(Collider other)
        {
            _currentInteractObject?.TryDropSelect();

            if (other.TryGetComponent(out IInteractive gardenBedMediator))
            {
                _currentInteractObject = gardenBedMediator;
                _interactiveAction = _currentInteractObject.Interact;
                _currentInteractObject.TrySelect();
            }
        }

        private void OnTriggerExitCustom(Collider other)
        {
            if (other.TryGetComponent(out IInteractive gardenBedMediator))
            {
                if (gardenBedMediator == _currentInteractObject)
                    _currentInteractObject = null;
                
                gardenBedMediator.TryDropSelect();
            }
        }

        private void Move(Vector2 moveDirection)
        {
            MoveVector = moveDirection;
            
            transform.position += _moveCalculator.Move(moveDirection, 
                            transform, moveSpeed, Time.deltaTime);
                        
            viewTransform.rotation = _moveCalculator.SmoothRotation(moveDirection, viewTransform.rotation, rotateSpeed * Time.deltaTime);
            animator.SetFloat(_speed, _moveCalculator.CurrentVelocity.magnitude);
        }

        private void Interact(InputAction.CallbackContext context)
        {
            _interactiveAction?.Invoke();
        }

        private void OnSelectHandledObject([CanBeNull] IHandheldObject handheldObject)
        {
            if (handheldObject is null)
            {
                collisionDetector.enabled = true;
                return;
            }

            collisionDetector.enabled = false;
            _interactiveAction = () => handheldObject.PlaceSpecimen(viewTransform.position, viewTransform.forward);
            handheldObject.ShowCellVisualization(() => new GardenBedInHandDTO(viewTransform.position, viewTransform.forward));
        }
    }
}
