using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace CavrnusSdk.Experimental
{
    public class CavrnusStringSelectionItem : MonoBehaviour
    {
        public string StringValue{ get; private set; }
        
        private Action<string> onSelected;

        [SerializeField] private UnityEvent<bool> onUnitySelectedState;
        [SerializeField] private TextMeshProUGUI tmPro;
        
        public void Setup(string stringVal, Action<string> onSelected)
        {
            StringValue = stringVal;
            this.onSelected = onSelected;
            tmPro.text = stringVal;
            
            SetSelectionState(false);
        }
        
        public void SetSelectionState(bool state)
        {
            onUnitySelectedState?.Invoke(state);
        }

        public void Select()
        {
            onSelected?.Invoke(StringValue);
        }
    }
}