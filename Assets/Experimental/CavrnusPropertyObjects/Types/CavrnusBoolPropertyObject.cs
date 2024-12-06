using System;
using CavrnusSdk.API;
using UnityEngine;

namespace CavrnusSdk.Experimental
{
    [CreateAssetMenu(fileName = "CavrnusBoolPropertyObject_", menuName = "Cavrnus/Properties/Bool", order = 0)]
    public class CavrnusBoolPropertyObject : CavrnusPropertyObject<bool>
    {
        protected override CavrnusLivePropertyUpdate<bool> SetUpTransient(object caller, bool value)
        {
            return GetSpaceConnection(caller).BeginTransientBoolPropertyUpdate(GetContainerName(caller), PropertyName, value);
        }

        protected override void PostValue(object caller, bool value)
        {
            if (IsUserMetadata)
                CavrnusFunctionLibrary.UpdateLocalUserMetadataString(PropertyName, value.ToString());
            else
                GetSpaceConnection(caller)?.PostBoolPropertyUpdate(GetContainerName(caller), PropertyName, value);
        }

        public override bool GetValue(object caller)
        {
            if (IsUserMetadata) {
                var md = GetUser(caller).GetUserMetadata(PropertyName);
                return md.ToLowerInvariant().Equals("true");
            }
            
            return GetSpaceConnection(caller).GetBoolPropertyValue(GetContainerName(caller), PropertyName);
        }

        public override void DefineDefaultValue(object caller)
        {
            GetSpaceConnection(caller).DefineBoolPropertyDefaultValue(GetContainerName(caller), PropertyName, DefaultValue);
        }

        protected override IDisposable SetBinding(object caller, Action<bool> onPropertyUpdated)
        {
            if (IsUserMetadata) {
                return GetUser(caller).BindToUserMetadata(PropertyName, val => {
                    if (string.IsNullOrWhiteSpace(val) || val.Equals(GetValue(caller).ToString()))
                        return;
                    
                    SendUpdateEvent(val.ToString().ToLowerInvariant().Equals("true"), onPropertyUpdated);
                });
            }
            
            if (PropertyObjectContainerType == PropertyObjectContainerTypeEnum.User)
                return GetUser(caller).BindBoolPropertyValue(PropertyName, s => SendUpdateEvent(s, onPropertyUpdated));
            
            return GetSpaceConnection(caller).BindBoolPropertyValue(GetContainerName(caller), PropertyName, val => {
                SendUpdateEvent(val, onPropertyUpdated);
            });
        }
        
        public override void TransientUpdateWithNewData(object caller, bool val)
        {
            GetTransientUpdater(caller)?.UpdateWithNewData(val);
        }
        
        public override void BeginTransientUpdate(object caller, bool val)
        {
            if (GetTransientUpdater(caller) == null)
                SetTransientUpdater(caller, GetSpaceConnection(caller).BeginTransientBoolPropertyUpdate(GetContainerName(caller), PropertyName, val));
        }
        
        public override void FinishTransient(object caller)
        {
            GetTransientUpdater(caller)?.Finish();
            SetTransientUpdater(caller, null);
        }
    }
}