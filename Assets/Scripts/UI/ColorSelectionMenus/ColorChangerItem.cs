using System;
using UnityEngine;
using UnityEngine.UI;

namespace CavrnusSdk.Experimental
{
    public class ColorChangerItem : MonoBehaviour
    {
        public Color Color{ get; private set; }
        
        [SerializeField] private Image image;
        [SerializeField] private GameObject selectedBorder;

        public CavrnusColorCollection.ColorDataObject Info{ get; private set; }
        private Action<CavrnusColorCollection.ColorDataObject> onSelected;

        public void Setup(CavrnusColorCollection.ColorDataObject info, Action<CavrnusColorCollection.ColorDataObject> onSelected)
        {
            Info = info;
            this.onSelected = onSelected;

            Color = info.Color;
            image.color = info.Color;
            
            selectedBorder.SetActive(false);
        }
        
        public void SetSelectionState(bool state)
        {
            selectedBorder.SetActive(state);
        }

        public void Select()
        {
            onSelected?.Invoke(Info);
        }
    }
}