using System;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public sealed class FirTreeInput
    {
        public void SetInput(Action firTreeInputResult)
        {
            Debug.Log("Suetaaa");
            firTreeInputResult?.Invoke();
        }
    }
}