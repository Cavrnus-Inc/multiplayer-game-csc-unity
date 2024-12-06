﻿using System;
using System.Collections.Generic;
using CavrnusSdk.API;
using UnityEngine;

namespace CavrnusSdk.Experimental
{
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
        
        public string ContainerName;
        public string PropertyName;
        public T DefaultValue;
        
        public bool UseUserContainer;
        public bool IsUserMetadata;
        
        public bool IsTransient;
        public bool AllowRepeatTransientValues;
        
        // Keep track of the caller and it's subsequent Property Objects 
        private readonly Dictionary<object, Dictionary<string, PropertyContextData>> callerContainerMap = new();
        
        public void Initialize(object caller, Action<CavrnusSpaceConnection> onConnected = null, string spaceTag = "")
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
            if (IsTransient)
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
            if (callerContainerMap.TryGetValue(caller, out var entry)) {
                if (entry.TryGetValue(PropertyName, out var ctx))
                    return ctx;
            }
            
            entry = new Dictionary<string, PropertyContextData>();
            entry.TryAdd(PropertyName, new PropertyContextData());
            callerContainerMap[caller] = entry;

            return entry[PropertyName];
        }
        
        protected string GetContainerName(object caller)
        {
            if (callerContainerMap.TryGetValue(caller, out var propertyMap) &&
                propertyMap.TryGetValue(PropertyName, out var ctx)) {

                if (UseUserContainer)
                    return ctx.User.ContainerId;
                
                return ctx.ContainerName;
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
            
            callerContainerMap.Remove(caller);
        }
    }
}