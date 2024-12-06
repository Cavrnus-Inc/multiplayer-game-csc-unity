using System;
using System.Collections.Generic;
using CavrnusSdk.API;
using UnityEngine;

namespace CavrnusSdk.Experimental
{
    public enum PropertyObjectContainerTypeEnum
    {
        User = 0,
        Space = 1
    }
    
    public enum PropertyObjectJournalTypeEnum
    {
        Saved = 0,
        Transient = 1
    }
    
    public abstract class CavrnusPropertyObject<T> : ScriptableObject
    {
        // We need object containing contextual data for each object/caller since Property Objects are shared instances
        private class PropertyContextData
        {
            public CavrnusUser User;
            public string ContainerName;
            public CavrnusSpaceConnection SpaceConnection;
            public Action<T> Callback;
            public IDisposable Binding;
            public CavrnusLivePropertyUpdate<T> TransientUpdater;
        }

        public PropertyObjectContainerTypeEnum PropertyObjectContainerType;
        public PropertyObjectJournalTypeEnum PropertyObjectJournalType;
        
        public string ContainerName;
        public string PropertyName;
        public T DefaultValue;
        
        public bool IsUserMetadata;
        public bool AllowRepeatTransientValues;
        
        // Keep track of the caller and it's Property Objects 
        private readonly Dictionary<object, PropertyContextData> callerContextMap = new();
        
        public void AwaitInitialize(object caller, Action<CavrnusSpaceConnection> onConnected = null, string spaceTag = "")
        {
            CavrnusFunctionLibrary.AwaitSpaceConnectionByTag(spaceTag, sc =>
            {
                sc.AwaitLocalUser(localUser =>
                {
                    var ctx = GetPropertyContext(caller);
                    ctx.User = localUser;
                    ctx.SpaceConnection = sc;
                    
                    onConnected?.Invoke(sc);
                });
            });
        }

        public void InitializeForRemoteUser(object caller, CavrnusUser remoteUser, Action<CavrnusSpaceConnection> onConnected = null, string spaceTag = "")
        {
            CavrnusFunctionLibrary.AwaitSpaceConnectionByTag(spaceTag, sc =>
            {
                var ctx = GetPropertyContext(caller);
                ctx.User = remoteUser;
                ctx.SpaceConnection = sc;
                
                // remoter user containerId
                
                onConnected?.Invoke(sc);
            });
        }
        
        public void BindProperty(object caller, Action<T> onPropertyUpdated)
        {
            var ctx = GetPropertyContext(caller);
            
            var binding = SetBinding(caller, onPropertyUpdated);
            ctx.Callback = onPropertyUpdated;
            ctx.Binding = binding;
        }
        
        public void DisposeProperty(object caller, Action<T> onPropertyUpdated)
        {
            UnregisterCaller(caller, onPropertyUpdated);
        }

        public abstract T GetValue(object caller);
        public abstract void DefineDefaultValue(object caller);
        
        protected abstract CavrnusLivePropertyUpdate<T> SetUpTransient(object caller, T value);
        protected abstract IDisposable SetBinding(object caller, Action<T> onPropertyUpdated);
        
        protected abstract void PostValue(object caller, T value);

        public void PostOrUpdateValue(object caller, T value)
        {
            if (PropertyObjectJournalType == PropertyObjectJournalTypeEnum.Transient)
                UpdateTransientData(caller, value);
            else
                PostValue(caller, value);
        }
        
        protected void SendUpdateEvent(T value, Action<T> onPropertyUpdated)
        {
            onPropertyUpdated?.Invoke(value);
        }

        public void UpdateTransientData(object caller, T value)
        {
            var ctx = GetPropertyContext(caller);
            ctx.TransientUpdater ??= SetUpTransient(caller, value);
            ctx.TransientUpdater?.UpdateWithNewData(value);
    
            if (AllowRepeatTransientValues) {
                ctx.TransientUpdater?.Cancel();
                ctx.TransientUpdater = null;
            }
        }

        public abstract void TransientUpdateWithNewData(object caller, T val);
        public abstract void BeginTransientUpdate(object caller, T val);
        public abstract void FinishTransient(object caller);
        
        protected CavrnusUser GetUser(object caller)
        {
            return GetPropertyContext(caller).User;
        }
        
        protected CavrnusSpaceConnection GetSpaceConnection(object caller)
        {
            return GetPropertyContext(caller).SpaceConnection;
        }
        
        protected CavrnusLivePropertyUpdate<T> GetTransientUpdater(object caller)
        {
            return GetPropertyContext(caller).TransientUpdater;
        }
        
        protected void SetTransientUpdater(object caller, CavrnusLivePropertyUpdate<T> updater)
        {
            GetPropertyContext(caller).TransientUpdater = updater;
        }
        
        private PropertyContextData GetPropertyContext(object caller)
        {
            if (!callerContextMap.TryGetValue(caller, out var context))
            {
                context = new PropertyContextData();
                callerContextMap[caller] = context;
            }
            return context;
        }
        
        protected string GetContainerName(object caller)
        {
            if (callerContextMap.TryGetValue(caller, out var ctx)) {
                if (PropertyObjectContainerType == PropertyObjectContainerTypeEnum.User)
                    return ctx.User.ContainerId;
            }

            return ContainerName;
        }
        
        private void UnregisterCaller(object caller, Action<T> onPropertyUpdated)
        {
            var ctx = GetPropertyContext(caller);
            if (ctx.Callback != null)
                ctx.Callback -= onPropertyUpdated;
                    
            ctx.Binding?.Dispose();
            ctx.TransientUpdater = null;
            
            callerContextMap.Remove(caller);
        }
    }
}