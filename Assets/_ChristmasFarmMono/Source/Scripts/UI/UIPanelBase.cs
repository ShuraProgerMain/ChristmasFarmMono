using _ChristmasFarmMono.Source.Scripts.Player;
using UnityEngine.UIElements;

namespace _ChristmasFarmMono.Source.Scripts.UI
{
    public abstract class UIPanelBase
    {
        protected VisualElement TopElement;

        private InputActionsService _inputActionsService;

        protected UIPanelBase(InputActionsService inputActionsService)
        {
            _inputActionsService = inputActionsService;
        }
        
        public virtual UIPanelBase Init(VisualElement topElement)
        {
            TopElement = topElement;
            
            return this;
        }
        
        protected virtual void ShowPanel()
        {
            _inputActionsService.EnableUI();
        }

        public virtual void HidePanel()
        {
            _inputActionsService.DisableUI();
        }
        
        protected void SimulateClick(Focusable element)
        {
            using var clickEvent = ClickEvent.GetPooled();
            clickEvent.target = element;
            element.SendEvent(clickEvent);
        }
    }
}