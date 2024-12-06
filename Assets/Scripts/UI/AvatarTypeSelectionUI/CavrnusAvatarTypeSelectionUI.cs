using UnityEngine;

namespace CavrnusSdk.Experimental
{
    public class CavrnusAvatarTypeSelectionUI : CavrnusStringSelectionUIBase
    {
        [SerializeField] private CavrnusAvatarTypes avatars;
        
        protected override void PopulateItems()
        {
            foreach (var at in avatars.AvatarTypes) {
                var go = Instantiate(Prefab, Container);
                Items.Add(go.GetComponent<CavrnusStringSelectionItem>());
                go.Setup(at.Name, PostValue);
            }
        }
    }
}