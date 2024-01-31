﻿
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
            _currentVelocity = Vector2.SmoothDamp(CurrentVelocity, velocity, ref _smoothInputVelocity, .05f);

            var moveSpeed = speed * deltaTime;
            var newPosition = (directTransform.forward * (CurrentVelocity.y * moveSpeed)) +
                              (directTransform.right * (CurrentVelocity.x * moveSpeed));
            newPosition.y = 0;

            return newPosition;
        }

        internal Quaternion SmoothRotation(Vector2 velocity, Quaternion characterRotation, float maxDegreesDelta)
        {
            if (velocity.magnitude < 0.0001f)
            {
                return characterRotation;
            }
            
            Quaternion targetRotation;

            var localMovementDirection = new Vector3(velocity.x, 0f, velocity.y);

            var forward = Quaternion.Euler(0f, 0f, 0f) * Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            if (Mathf.Approximately(Vector3.Dot(localMovementDirection, Vector3.forward), -1.0f))
            {
                targetRotation = Quaternion.LookRotation(-forward);
            }
            else
            {
                targetRotation = Quaternion.LookRotation(localMovementDirection);
            }

            targetRotation = Quaternion.RotateTowards(characterRotation, targetRotation, maxDegreesDelta);

            return targetRotation;
        }
    }
}