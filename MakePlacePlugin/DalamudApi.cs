// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.DalamudApi
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace MakePlacePlugin; 

public class DalamudApi {
    [PluginService] public static IChatGui ChatGui { get; }

    [PluginService] public static IClientState ClientState { get; }

    [PluginService] public static ICommandManager CommandManager { get; }

    [PluginService] public static DalamudPluginInterface PluginInterface { get; }

    [PluginService] public static IDataManager DataManager { get; }

    [PluginService] public static ITextureProvider TextureProvider { get; }

    [PluginService] public static IGameGui GameGui { get; }

    [PluginService] public static ISigScanner SigScanner { get; }

    [PluginService] public static IGameInteropProvider Hooks { get; }

    [PluginService] public static IPluginLog PluginLog { get; }

    public static void Initialize(DalamudPluginInterface pluginInterface) {
        pluginInterface.Create<DalamudApi>(Array.Empty<object>());
    }
}
