﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Mobile;

namespace Akavache.Mobile
{
    public class AkavacheDriver : ISuspensionDriver, IEnableLogger
    {
        public AkavacheDriver(string applicationName)
        {
            BlobCache.ApplicationName = applicationName;
            BlobCache.SerializerSettings = new JsonSerializerSettings() {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
            };
        }

        public IObservable<T> LoadState<T>() where T : class, IApplicationRootState
        {
            return BlobCache.UserAccount.GetObjectAsync<T>("__AppState");
        }

        public IObservable<Unit> SaveState<T>(T state) where T : class, IApplicationRootState
        {
            return BlobCache.UserAccount.InsertObject("__AppState", state)
                .SelectMany(BlobCache.UserAccount.Flush());
        }

        public IObservable<Unit> InvalidateState()
        {
            BlobCache.UserAccount.InvalidateObject<object>("__AppState");
            return Observable.Return(Unit.Default);
        }
    }
}