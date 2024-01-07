using System;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public sealed class FirTreeInput
    {
        public void SetInput(Action firTreeInputResult)
        {
            firTreeInputResult?.Invoke();
        }
    }
}