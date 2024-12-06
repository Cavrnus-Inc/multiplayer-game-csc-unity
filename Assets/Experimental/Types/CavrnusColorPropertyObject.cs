using System;
using CavrnusSdk.API;
using UnityEngine;

namespace CavrnusSdk.Experimental
{
    [CreateAssetMenu(fileName = "CavrnusColorPropertyObject_", menuName = "Cavrnus/Properties/Color", order = 0)]
    public class CavrnusColorPropertyObject : CavrnusPropertyObject<Color>
    {
        protected override CavrnusLivePropertyUpdate<Color> SetUpTransient(object caller, Color value)
        {
            return GetSpaceConnection(caller).BeginTransientColorPropertyUpdate(GetContainerName(caller), PropertyName, value);
        }
        
        protected override void PostValue(object caller, Color value)
        {
            if (IsUserMetadata)
                CavrnusFunctionLibrary.UpdateLocalUserMetadataString(PropertyName, value.ToString());
            else
                GetSpaceConnection(caller).PostColorPropertyUpdate(GetContainerName(caller), PropertyName, value);
        }

        public override Color GetValue(object caller)
        {
            if (IsUserMetadata) {
                var md = GetUser(caller).GetUserMetadata(PropertyName);
                return md.ToColor();
            }
            
            return GetSpaceConnection(caller).GetColorPropertyValue(GetContainerName(caller), PropertyName);
        }

        public override void DefineDefaultValue(object caller)
        {
            GetSpaceConnection(caller).DefineColorPropertyDefaultValue(GetContainerName(caller), PropertyName, DefaultValue);
        }

        protected override IDisposable SetBinding(object caller, Action<Color> onPropertyUpdated)
        {
            if (IsUserMetadata) {
                return GetUser(caller).BindToUserMetadata(PropertyName, val => {
                    if (string.IsNullOrWhiteSpace(val) || val.Equals(GetValue(caller).ToString()))
                        return;
                    
                    SendUpdateEvent(val.ToColor(), onPropertyUpdated);
                });
            }
            
            if (UseUserContainer)
                return GetUser(caller).BindColorPropertyValue(PropertyName, s => SendUpdateEvent(s, onPropertyUpdated));
            
            return GetSpaceConnection(caller)?.BindColorPropertyValue(GetContainerName(caller), PropertyName, color => SendUpdateEvent(color, onPropertyUpdated));
        }
        
        public override void TransientUpdateWithNewData(object caller, Color val)
        {
            GetTransientUpdater(caller)?.UpdateWithNewData(val);
        }
        
        public override void BeginTransientUpdate(object caller, Color val)
        {
            if (GetTransientUpdater(caller) == null)
                SetTransientUpdater(caller, GetSpaceConnection(caller).BeginTransientColorPropertyUpdate(GetContainerName(caller), PropertyName, val));
        }
        
        public override void FinishTransient(object caller)
        {
            GetTransientUpdater(caller)?.Finish();
            SetTransientUpdater(caller, null);
        }
    }
}