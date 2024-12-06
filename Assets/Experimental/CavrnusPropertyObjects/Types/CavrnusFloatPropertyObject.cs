using System;
using System.Globalization;
using CavrnusSdk.API;
using UnityEngine;

namespace CavrnusSdk.Experimental
{
    [CreateAssetMenu(fileName = "CavrnusFloatPropertyObject_", menuName = "Cavrnus/Properties/Float", order = 0)]
    public class CavrnusFloatPropertyObject : CavrnusPropertyObject<float>
    {
        [SerializeField] private Vector2 minMaxSliderLimits;
        public Vector2 MinMaxSliderLimits => minMaxSliderLimits;
        
        protected override CavrnusLivePropertyUpdate<float> SetUpTransient(object caller, float value)
        {
            return GetSpaceConnection(caller)?.BeginTransientFloatPropertyUpdate(GetContainerName(caller), PropertyName, value);
        }
        
        protected override void PostValue(object caller,float value)
        {
            if (IsUserMetadata)
                CavrnusFunctionLibrary.UpdateLocalUserMetadataString(PropertyName, value.ToString(CultureInfo.InvariantCulture));
            else
                GetSpaceConnection(caller)?.PostFloatPropertyUpdate(GetContainerName(caller), PropertyName, value);
        }

        public override float GetValue(object caller)
        {
            if (IsUserMetadata) {
                var md = GetUser(caller).GetUserMetadata(PropertyName);
                return md.ToFloat();
            }
        
            return GetSpaceConnection(caller).GetFloatPropertyValue(GetContainerName(caller), PropertyName);
        }
        
        public override void DefineDefaultValue(object caller)
        {
            GetSpaceConnection(caller).DefineFloatPropertyDefaultValue(GetContainerName(caller), PropertyName, DefaultValue);
        }
        
        protected override IDisposable SetBinding(object caller, Action<float> onPropertyUpdated)
        {
            if (IsUserMetadata)
                return GetUser(caller).BindToUserMetadata(PropertyName, val => {
                    if (string.IsNullOrWhiteSpace(val) || val.Equals(GetValue(caller).ToString(CultureInfo.InvariantCulture)))
                        return;
                    
                    SendUpdateEvent(val.ToFloat(), onPropertyUpdated);
                });
        
            if (PropertyObjectContainerType == PropertyObjectContainerTypeEnum.User)
                return GetUser(caller).BindFloatPropertyValue(PropertyName, s => SendUpdateEvent(s, onPropertyUpdated));
            
            return GetSpaceConnection(caller)?.BindFloatPropertyValue(GetContainerName(caller), PropertyName, f => SendUpdateEvent(f, onPropertyUpdated));
        }
        
        public override void TransientUpdateWithNewData(object caller, float sliderValue)
        {
            GetTransientUpdater(caller)?.UpdateWithNewData(sliderValue);
        }
        
        public override void BeginTransientUpdate(object caller, float sliderValue)
        {
            if (GetTransientUpdater(caller) == null)
                SetTransientUpdater(caller, GetSpaceConnection(caller).BeginTransientFloatPropertyUpdate(GetContainerName(caller), PropertyName, sliderValue));
        }
        
        public override void FinishTransient(object caller)
        {
            GetTransientUpdater(caller)?.Finish();
            SetTransientUpdater(caller, null);
        }
    }
}