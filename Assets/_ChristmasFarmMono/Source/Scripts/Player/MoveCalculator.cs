
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Player
{
    public sealed class MoveCalculator
    {
        private Vector2 _smoothInputVelocity;
        private Vector2 _currentVelocity;

        public Vector2 CurrentVelocity => _currentVelocity;

        internal Vector3 Move(Vector2 velocity, Transform directTransform, float speed, float deltaTime)
        {
            _currentVelocity = Vector2.SmoothDamp(CurrentVelocity, velocity, ref _smoothInputVelocity, .08f);

            var moveSpeed = (speed * velocity.magnitude) * deltaTime;
            var newPosition = directTransform.forward * (CurrentVelocity.y * moveSpeed) +
                              directTransform.right * (CurrentVelocity.x * moveSpeed);
            newPosition.y = 0;

            return newPosition;
        }

        internal Quaternion SmoothRotation(Vector2 velocity, Quaternion characterRotation, float maxDegreesDelta)
        {
            if (velocity == Vector2.zero) return characterRotation;

            var localMovementDirection = Quaternion.Euler(0f, Mathf.Atan2(velocity.x, velocity.y) 
                                                              * Mathf.Rad2Deg, 0f) * Vector3.forward;
            
            var forward = Quaternion.Euler(0f, 0f, 0f) * Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            var targetRotation = Mathf.Approximately(Vector3.Dot(localMovementDirection, Vector3.forward), -1.0f) 
                ? Quaternion.LookRotation(-forward) 
                : Quaternion.LookRotation(localMovementDirection);

            
            float angle = Quaternion.Angle(characterRotation, targetRotation);

            float rotationSpeed = Mathf.Lerp(0, maxDegreesDelta, angle / 180f);
            
            targetRotation = Quaternion.RotateTowards(characterRotation, targetRotation, rotationSpeed);

            return targetRotation;

        }
    }
}