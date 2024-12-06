using CavrnusSdk.Experimental;
using UnityEngine;

namespace CavrnusDemo.EmoteSelectionUI
{
    public class CavrnusEmoteSelectionUI : CavrnusStringSelectionUIBase
    {
        [SerializeField] private CavrnusEmoteCollection emotes;
        
        protected override void PopulateItems()
        {
            foreach (var at in emotes.EmoteData) {
                var go = Instantiate(Prefab, Container);
                Items.Add(go.GetComponent<CavrnusStringSelectionItem>());
                go.Setup(at.DisplayName, PostValue);
            }
        }
    }
}