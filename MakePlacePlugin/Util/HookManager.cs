// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HookManager
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace MakePlacePlugin; 

public class HookManager {
    public static List<IHookWrapper> HookList = new();

    public static void Dispose() {
        foreach (var hookWrapper in HookList.Where((Func<IHookWrapper, bool>)(hook => !hook.IsDisposed))) {
            if (hookWrapper.IsEnabled)
                hookWrapper.Disable();
            hookWrapper.Dispose();
        }

        HookList.Clear();
    }

    public static HookWrapper<T> Hook<T>(
        string signature,
        T detour,
        bool enable = true,
        int addressOffset = 0)
        where T : Delegate {
        return HookAddress(DalamudApi.SigScanner.ScanText(signature), detour, enable, addressOffset);
    }

    public static HookWrapper<T> HookAddress<T>(
        IntPtr addr,
        T detour,
        bool enable = true,
        int addressOffset = 0)
        where T : Delegate {
        DalamudApi.PluginLog.Info("Hooking " + detour.Method.Name + " at " + addr.ToString("X"), Array.Empty<object>());
        var hookWrapper = new HookWrapper<T>(DalamudApi.Hooks.HookFromAddress(addr + addressOffset, detour));
        if (enable)
            hookWrapper.Enable();
        HookList.Add(hookWrapper);
        return hookWrapper;
    }
}
