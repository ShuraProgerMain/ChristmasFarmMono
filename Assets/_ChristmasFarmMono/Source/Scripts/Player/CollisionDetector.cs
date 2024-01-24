using System;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Player
{
    public class CollisionDetector : MonoBehaviour
    {
        public Action<Collider> TriggerEnter;
        public Action<Collider> TriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            if (enabled)
            {
                TriggerEnter?.Invoke(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (enabled)
            {
                TriggerExit?.Invoke(other);
            }
        }
    }
}