using Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIButtonSoundTrigger : MonoBehaviour, IPointerEnterHandler
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_button.interactable) return;
            GameEventManager.Instance.UIEventHandler.InvokeButtonSelected();
        }
    }
}
