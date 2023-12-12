// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.PluginUi
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using MakePlacePlugin.Gui;

namespace MakePlacePlugin; 

public class PluginUi : IDisposable {
    private readonly MakePlacePlugin _plugin;

    public PluginUi(MakePlacePlugin plugin) {
        this.ConfigWindow = new ConfigurationWindow(plugin);
        this._plugin = plugin;
        DalamudApi.PluginInterface.UiBuilder.Draw += this.Draw;
        DalamudApi.PluginInterface.UiBuilder.OpenConfigUi += this.OnOpenConfigUi;
    }

    public ConfigurationWindow ConfigWindow { get; }

    public void Dispose() {
        DalamudApi.PluginInterface.UiBuilder.Draw -= this.Draw;
        DalamudApi.PluginInterface.UiBuilder.OpenConfigUi -= this.OnOpenConfigUi;
    }

    private void Draw() {
        this.ConfigWindow.Draw();
    }

    private void OnOpenConfigUi() {
        this.ConfigWindow.Visible = true;
        this.ConfigWindow.CanUpload = false;
        this.ConfigWindow.CanImport = false;
    }
}
