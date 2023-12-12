// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HookWrapper`1
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using Dalamud.Hooking;

namespace MakePlacePlugin; 

public class HookWrapper<T> : IHookWrapper, IDisposable where T : Delegate {
    private readonly Hook<T> wrappedHook;
    private bool disposed;

    public HookWrapper(Hook<T> hook) {
        this.wrappedHook = hook;
    }

    public T Original => this.wrappedHook.Original;

    public IntPtr Address => this.wrappedHook.Address;

    public void Enable() {
        if (this.disposed)
            return;
        this.wrappedHook?.Enable();
    }

    public void Disable() {
        if (this.disposed)
            return;
        this.wrappedHook?.Disable();
    }

    public void Dispose() {
        DalamudApi.PluginLog.Info("Disposing of {cdelegate}", typeof(T).Name);
        this.Disable();
        this.disposed = true;
        this.wrappedHook?.Dispose();
    }

    public bool IsEnabled => this.wrappedHook.IsEnabled;

    public bool IsDisposed => this.wrappedHook.IsDisposed;
}
