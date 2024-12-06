using System;
using CavrnusSdk.API;
using UnityEngine;

namespace CavrnusSdk.Experimental
{
    [CreateAssetMenu(fileName = "CavrnusVectorPropertyObject_", menuName = "Cavrnus/Properties/Vector", order = 0)]
    public class CavrnusVectorPropertyObject : CavrnusPropertyObject<Vector4>
    {
        protected override void PostValue(object caller, Vector4 value)
        {
            if (IsUserMetadata)
                CavrnusFunctionLibrary.UpdateLocalUserMetadataString(PropertyName, value.ToString());
            else
                GetSpaceConnection(caller).PostVectorPropertyUpdate(GetContainerName(caller), PropertyName, value);
        }

        public override Vector4 GetValue(object caller)
        {
            if (IsUserMetadata) {
                var md = GetUser(caller)?.GetUserMetadata(PropertyName);
                return md.ToVector4();
            }
            
            return GetSpaceConnection(caller).GetVectorPropertyValue(GetContainerName(caller), PropertyName);
        }

        public override void DefineDefaultValue(object caller)
        {
            GetSpaceConnection(caller).DefineVectorPropertyDefaultValue(GetContainerName(caller), PropertyName, DefaultValue);
        }

        protected override IDisposable SetBinding(object caller, Action<Vector4> onPropertyUpdated)
        {
            if (IsUserMetadata)
                return GetUser(caller).BindToUserMetadata(PropertyName, val => {
                    if (string.IsNullOrWhiteSpace(val) || val.Equals(GetValue(caller).ToString()))
                        return;
                    
                    SendUpdateEvent(val.ToVector4(), onPropertyUpdated);
                });
            
            return GetSpaceConnection(caller)?.BindVectorPropertyValue(GetContainerName(caller), PropertyName, v => SendUpdateEvent(v, onPropertyUpdated));
        }

        protected override CavrnusLivePropertyUpdate<Vector4> SetUpTransient(object caller, Vector4 value)
        {
            return GetSpaceConnection(caller)?.BeginTransientVectorPropertyUpdate(GetContainerName(caller), PropertyName, value);
        }
                
        public override void TransientUpdateWithNewData(object caller, Vector4 val)
        {
            GetTransientUpdater(caller)?.UpdateWithNewData(val);
        }
        
        public override void BeginTransientUpdate(object caller, Vector4 val)
        {
            if (GetTransientUpdater(caller) == null)
                SetTransientUpdater(caller, GetSpaceConnection(caller).BeginTransientVectorPropertyUpdate(GetContainerName(caller), PropertyName, val));
        }
        
        public override void FinishTransient(object caller)
        {
            GetTransientUpdater(caller)?.Finish();
            SetTransientUpdater(caller, null);
        }
    }
}