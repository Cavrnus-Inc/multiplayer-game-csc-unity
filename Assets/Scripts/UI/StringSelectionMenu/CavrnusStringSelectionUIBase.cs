using System.Collections.Generic;
using UnityEngine;

namespace CavrnusSdk.Experimental
{
    public abstract class CavrnusStringSelectionUIBase : MonoBehaviour
    {
        [SerializeField] private CavrnusPropertyObject<string> propertyObject;
        
        [Space]
        [SerializeField] protected CavrnusStringSelectionItem Prefab;
        [SerializeField] protected Transform Container;

        protected readonly List<CavrnusStringSelectionItem> Items = new();

        protected virtual void Start()
        {
            propertyObject.AwaitInitialize(this, _ => {
                PopulateItems();
                BindProperty();
            });
        }

        protected abstract void PopulateItems();

        private void BindProperty() => propertyObject.BindProperty(this, OnPropertyUpdated);
        protected void OnPropertyUpdated(string severVal) => SetSelectedItem(severVal);

        protected void SetSelectedItem(string selectedVal)
        {
            if (!string.IsNullOrWhiteSpace(selectedVal)) {
                foreach (var item in Items)
                    item.SetSelectionState(item.StringValue.ToLowerInvariant().Trim().Equals(selectedVal.ToLowerInvariant().Trim()));
            }
        }

        protected void PostValue(string val)
        {
            SetSelectedItem(val);
            propertyObject.PostOrUpdateValue(this, val);
        }
        
        private void OnDestroy() => propertyObject.DisposeProperty(this, OnPropertyUpdated);
    }
}