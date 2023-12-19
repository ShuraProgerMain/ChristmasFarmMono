using System;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public struct ProductionState
    {
        public string ProductionItemId;
        public DateTime ProductionEndTime;
        public DateTime ProductionStartTime;
        public Action<ProductionResult> ProductionComplete;
    }
}