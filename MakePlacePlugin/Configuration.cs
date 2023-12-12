// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.Configuration
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Collections.Generic;
using Dalamud.Configuration;

namespace MakePlacePlugin; 

[Serializable]
public class Configuration : IPluginConfiguration {
    public bool Basement = true;
    public float DrawDistance;
    public bool DrawScreen;
    public bool GroundFloor = true;
    public List<int> GroupingList = new();
    public List<int> HiddenScreenItemHistory = new();
    public int LoadInterval = 400;
    public bool PlaceAnywhere;
    public string SaveLocation;
    public bool ShowTooltips = true;
    public List<string> Tags = new();
    public List<bool> TagsSelectList = new();
    public bool UpperFloor = true;

    public int Version { get; set; }

    public void Save() {
        DalamudApi.PluginInterface.SavePluginConfig(this);
    }

    public void ResetRecord() {
        this.HiddenScreenItemHistory.Clear();
        this.GroupingList.Clear();
        this.Save();
    }
}
