using System;
using CavrnusSdk.API;
using UnityEngine;

namespace CavrnusSdk.Experimental
{
    [CreateAssetMenu(fileName = "CavrnusStringPropertyObject_", menuName = "Cavrnus/Properties/String", order = 0)]
    public class CavrnusStringPropertyObject : CavrnusPropertyObject<string>
    {
        protected override CavrnusLivePropertyUpdate<string> SetUpTransient(object caller, string value)
        {
            return GetSpaceConnection(caller)?.BeginTransientStringPropertyUpdate(GetContainerName(caller), PropertyName, value);
        }
        
        protected override void PostValue(object caller, string value)
        {
            if (IsUserMetadata)
                CavrnusFunctionLibrary.UpdateLocalUserMetadataString(PropertyName, value);
            else
                GetSpaceConnection(caller)?.PostStringPropertyUpdate(GetContainerName(caller), PropertyName, value);
        }

        public override string GetValue(object caller)
        {
            if (IsUserMetadata)
                return GetUser(caller).GetUserMetadata(PropertyName);
       
            return GetSpaceConnection(caller).GetStringPropertyValue(GetContainerName(caller), PropertyName);
        }

        public override void DefineDefaultValue(object caller)
        {
            GetSpaceConnection(caller).DefineStringPropertyDefaultValue(GetContainerName(caller), PropertyName, DefaultValue);
        }

        protected override IDisposable SetBinding(object caller, Action<string> onPropertyUpdated)
        {
            if (IsUserMetadata) {
                return GetUser(caller).BindToUserMetadata(PropertyName, val => {
                    if (string.IsNullOrWhiteSpace(val) || val.Equals(GetValue(caller))) 
                        return;
                    
                    SendUpdateEvent(val, onPropertyUpdated);
                });
            }
            
            if (PropertyObjectContainerType == PropertyObjectContainerTypeEnum.User)
                return GetUser(caller).BindStringPropertyValue(PropertyName, s => SendUpdateEvent(s, onPropertyUpdated));
        
            return GetSpaceConnection(caller).BindStringPropertyValue(GetContainerName(caller), PropertyName, s => SendUpdateEvent(s, onPropertyUpdated));
        }
        
        public override void TransientUpdateWithNewData(object caller, string val)
        {
            GetTransientUpdater(caller)?.UpdateWithNewData(val);
        }
        
        public override void BeginTransientUpdate(object caller, string val)
        {
            if (GetTransientUpdater(caller) == null)
                SetTransientUpdater(caller, GetSpaceConnection(caller).BeginTransientStringPropertyUpdate(GetContainerName(caller), PropertyName, val));
        }
        
        public override void FinishTransient(object caller)
        {
            GetTransientUpdater(caller)?.Finish();
            SetTransientUpdater(caller, null);
        }
    }
}