using System.Collections.Generic;
using UnityEngine;

namespace CavrnusSdk.Experimental
{
    public class CavrnusColorCollectionUI : MonoBehaviour
    {
        [SerializeField] private CavrnusPropertyObject<Color> propertyObject;

        [Space]
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform container;
        [SerializeField] protected CavrnusColorCollection ColorCollection;

        private readonly List<ColorChangerItem> items = new();

        private void Start()
        {
            propertyObject.AwaitInitialize(this, _ => {
                PopulateItems();
                BindProperty();
            });
        }
        
        private void PopulateItems()
        {
            foreach (var data in ColorCollection.ColorData) {
                var go = Instantiate(prefab, container);
                items.Add(go.GetComponent<ColorChangerItem>());
                go.GetComponent<ColorChangerItem>().Setup(data, OnSelected);
            }
        }

        private void OnSelected(CavrnusColorCollection.ColorDataObject data)
        {
            propertyObject.PostOrUpdateValue(this, data.Color);
        }

        private void BindProperty()
        {
            propertyObject.BindProperty(this, OnPropertyUpdated);
        }

        private void OnPropertyUpdated(Color color)
        {
            var serverData = ColorCollection.GetDataFromColor(color);
            SetSelectedItem(serverData);
        }

        private void SetSelectedItem(CavrnusColorCollection.ColorDataObject selectedDataObject)
        {
            if (selectedDataObject != null) {
                foreach (var item in items)
                    item.SetSelectionState(item.Info.DisplayName.ToLowerInvariant().Trim().Equals(selectedDataObject.DisplayName.ToLowerInvariant().Trim()));
            }
        }
        
        private void OnDestroy() => propertyObject.DisposeProperty(this, OnPropertyUpdated);
    }
}