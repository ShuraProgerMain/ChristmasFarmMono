using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _ChristmasFarmMono.Source.Scripts.UI;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public class SomeObjectProduction
    {
        private readonly Dictionary<string, ProductionState> _inProductionStates = new();

        private CancellationTokenSource _updateTimerTaskSource;
        private Task _updateTimerTask;
        
        public SomeObjectProduction(ItemsHolderShow itemsHolderShow)
        {
            
        }

        public void ShowProductionView(string productionItemId)
        {
            
        }

        public void HideProductionView()
        {
            
        }
    }

    public static class TimerView
    {
        public static async Task UpdateEverySecond(Action everySecondCallback, Func<bool> when, CancellationToken token)
        {
            try
            {
                while (when() && !token.IsCancellationRequested)
                {
                    everySecondCallback?.Invoke();
                    await Task.Delay(1000);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }
    }
}