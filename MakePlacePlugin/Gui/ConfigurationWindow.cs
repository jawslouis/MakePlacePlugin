// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.Gui.ConfigurationWindow
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Lumina.Text;
using MakePlacePlugin.Objects;

namespace MakePlacePlugin.Gui; 

public class ConfigurationWindow : Window<MakePlacePlugin> {
    private readonly Dictionary<uint, uint> iconToFurniture = new();
    private readonly Vector4 PURPLE = new(0.26275f, 0.21569f, 0.56863f, 1f);
    private readonly Vector4 PURPLE_ALPHA = new(0.26275f, 0.21569f, 0.56863f, 0.5f);
    private string CustomTag = string.Empty;

    public ConfigurationWindow(MakePlacePlugin plugin)
        : base(plugin) {
        this.FileDialogManager = new FileDialogManager {
            AddedWindowFlags = (ImGuiWindowFlags)2097184
        };
    }

    public Configuration Config => this.Plugin.Config;

    private FileDialogManager FileDialogManager { get; }

    protected void DrawAllUi() {
        if (!ImGui.Begin(this.Plugin.Name ?? "", ref this.WindowVisible, (ImGuiWindowFlags)16))
            return;
        if (ImGui.BeginChild("##SettingsRegion")) {
            this.DrawGeneralSettings();
            if (ImGui.BeginChild("##ItemListRegion")) {
                ImGui.PushStyleColor((ImGuiCol)24, this.PURPLE_ALPHA);
                ImGui.PushStyleColor((ImGuiCol)25, this.PURPLE);
                ImGui.PushStyleColor((ImGuiCol)26, this.PURPLE);
                if (ImGui.CollapsingHeader("Interior Furniture", (ImGuiTreeNodeFlags)32)) {
                    ImGui.PushID("interior");
                    this.DrawItemList(this.Plugin.InteriorItemList);
                    ImGui.PopID();
                }

                if (ImGui.CollapsingHeader("Exterior Furniture", (ImGuiTreeNodeFlags)32)) {
                    ImGui.PushID("exterior");
                    this.DrawItemList(this.Plugin.ExteriorItemList);
                    ImGui.PopID();
                }

                if (ImGui.CollapsingHeader("Interior Fixtures", (ImGuiTreeNodeFlags)32)) {
                    ImGui.PushID("interiorFixture");
                    this.DrawFixtureList(this.Plugin.Layout.interiorFixture);
                    ImGui.PopID();
                }

                if (ImGui.CollapsingHeader("Exterior Fixtures", (ImGuiTreeNodeFlags)32)) {
                    ImGui.PushID("exteriorFixture");
                    this.DrawFixtureList(this.Plugin.Layout.exteriorFixture);
                    ImGui.PopID();
                }

                if (ImGui.CollapsingHeader("Unused Furniture", (ImGuiTreeNodeFlags)32)) {
                    ImGui.PushID("unused");
                    this.DrawItemList(this.Plugin.UnusedItemList, true);
                    ImGui.PopID();
                }

                ImGui.PopStyleColor(3);
                ImGui.EndChild();
            }

            ImGui.EndChild();
        }

        this.FileDialogManager.Draw();
    }

    protected override void DrawUi() {
        ImGui.PushStyleColor((ImGuiCol)11, this.PURPLE);
        ImGui.PushStyleColor((ImGuiCol)22, this.PURPLE_ALPHA);
        ImGui.PushStyleColor((ImGuiCol)23, this.PURPLE_ALPHA);
        ImGui.SetNextWindowSize(new Vector2(530f, 450f), (ImGuiCond)4);
        this.DrawAllUi();
        ImGui.PopStyleColor(3);
        ImGui.End();
    }

    public void DrawIcon(ushort icon, Vector2 size) {
        if (icon >= 65000)
            return;
        ImGui.Image(DalamudApi.TextureProvider.GetIcon(icon).ImGuiHandle, size);
    }

    private void LogLayoutModeError() {
        MakePlacePlugin.LogError("Unable to load layouts outside of Layout mode");
        if (Memory.Instance.GetCurrentTerritory() == Memory.HousingArea.Island)
            MakePlacePlugin.LogError("(Manage Furnishings -> Place Furnishing Glamours)");
        else
            MakePlacePlugin.LogError("(Housing -> Indoor/Outdoor Furnishings)");
    }

    private void DrawGeneralSettings() {
        if (ImGui.Checkbox("Label Furniture", ref this.Config.DrawScreen))
            this.Config.Save();
        if (this.Config.ShowTooltips && ImGui.IsItemHovered())
            ImGui.SetTooltip("Show furniture names on the screen");
        ImGui.SameLine();
        ImGui.Dummy(new Vector2(10f, 0.0f));
        ImGui.SameLine();
        if (ImGui.Checkbox("##hideTooltipsOnOff", ref this.Config.ShowTooltips))
            this.Config.Save();
        ImGui.SameLine();
        ImGui.TextUnformatted("Show Tooltips");
        ImGui.Dummy(new Vector2(0.0f, 10f));
        ImGui.Text("Layout");
        if (!this.Config.SaveLocation.IsNullOrEmpty()) {
            ImGui.Text("Current file location: " + this.Config.SaveLocation);
            if (ImGui.Button("Save"))
                try {
                    MakePlacePlugin.LayoutManager.ExportLayout();
                } catch (Exception ex) {
                    MakePlacePlugin.LogError("Save Error: " + ex.Message, ex.StackTrace);
                }

            ImGui.SameLine();
        }

        if (ImGui.Button("Save As"))
            try {
                var str = "save";
                if (!this.Config.SaveLocation.IsNullOrEmpty())
                    str = Path.GetFileNameWithoutExtension(this.Config.SaveLocation);
                this.FileDialogManager.SaveFileDialog("Select a Save Location", ".json", str, "json", (ok, res) => {
                    if (!ok)
                        return;
                    this.Config.SaveLocation = res;
                    this.Config.Save();
                    MakePlacePlugin.LayoutManager.ExportLayout();
                }, Path.GetDirectoryName(this.Config.SaveLocation));
            } catch (Exception ex) {
                MakePlacePlugin.LogError("Save Error: " + ex.Message, ex.StackTrace);
            }

        ImGui.SameLine();
        ImGui.Text("Plugin → File           ");
        ImGui.SameLine();
        if (ImGui.Button("Load")) {
            if (!Memory.Instance.IsHousingMode())
                this.LogLayoutModeError();
            else
                try {
                    if (!this.Config.SaveLocation.IsNullOrEmpty())
                        Path.GetFileNameWithoutExtension(this.Config.SaveLocation);
                    this.FileDialogManager.OpenFileDialog("Select a Layout File", ".json", (ok, res) => {
                        if (!ok)
                            return;
                        this.Config.SaveLocation = res.FirstOrDefault<string>("");
                        this.Config.Save();
                        SaveLayoutManager.ImportLayout(this.Config.SaveLocation);
                        this.Plugin.MatchLayout();
                        this.Config.ResetRecord();
                        MakePlacePlugin.Log(string.Format("Imported {0} items",
                            this.Plugin.InteriorItemList.Count + this.Plugin.ExteriorItemList.Count));
                    }, 1, Path.GetDirectoryName(this.Config.SaveLocation));
                } catch (Exception ex) {
                    MakePlacePlugin.LogError("Load Error: " + ex.Message, ex.StackTrace);
                }
        }

        if (this.Config.ShowTooltips && ImGui.IsItemHovered())
            ImGui.SetTooltip("Load layout from file");
        ImGui.SameLine();
        ImGui.Text("File → Plugin");
        ImGui.Dummy(new Vector2(0.0f, 15f));
        if ((Memory.Instance.GetCurrentTerritory() != Memory.HousingArea.Indoors ? 1 :
                Memory.Instance.GetIndoorHouseSize().Equals("Apartment") ? 1 : 0) == 0) {
            ImGui.Text("Selected Floors");
            if (ImGui.Checkbox("Basement", ref this.Config.Basement)) {
                this.Plugin.MatchLayout();
                this.Config.Save();
            }

            ImGui.SameLine();
            ImGui.Dummy(new Vector2(10f, 0.0f));
            ImGui.SameLine();
            if (ImGui.Checkbox("Ground Floor", ref this.Config.GroundFloor)) {
                this.Plugin.MatchLayout();
                this.Config.Save();
            }

            ImGui.SameLine();
            ImGui.Dummy(new Vector2(10f, 0.0f));
            ImGui.SameLine();
            if (Memory.Instance.HasUpperFloor() && ImGui.Checkbox("Upper Floor", ref this.Config.UpperFloor)) {
                this.Plugin.MatchLayout();
                this.Config.Save();
            }

            ImGui.Dummy(new Vector2(0.0f, 15f));
        }

        ImGui.Text("Layout Actions");
        var str1 = "";
        switch (Memory.Instance.GetCurrentTerritory()) {
            case Memory.HousingArea.Indoors:
                str1 = "Interior";
                break;
            case Memory.HousingArea.Outdoors:
                str1 = "Exterior";
                break;
            case Memory.HousingArea.Island:
                str1 = "Island";
                break;
        }

        if (ImGui.Button("Get " + str1 + " Layout")) {
            if (Memory.Instance.IsHousingMode())
                try {
                    this.Plugin.LoadLayout();
                } catch (Exception ex) {
                    MakePlacePlugin.LogError("Error: " + ex.Message, ex.StackTrace);
                }
            else
                this.LogLayoutModeError();
        }

        ImGui.SameLine();
        ImGui.Text("Game → Plugin           ");
        ImGui.SameLine();
        if (ImGui.Button("Apply " + str1 + " Layout")) {
            if (Memory.Instance.CanEditItem())
                try {
                    this.Plugin.MatchLayout();
                    this.Plugin.ApplyLayout();
                } catch (Exception ex) {
                    MakePlacePlugin.LogError("Error: " + ex.Message, ex.StackTrace);
                }
            else
                MakePlacePlugin.LogError("Unable to apply layouts outside of Rotate Layout mode");
        }

        ImGui.SameLine();
        ImGui.Text("Plugin → Game           ");
        ImGui.SameLine();
        ImGui.PushItemWidth(100f);
        if (ImGui.InputInt("Placement Interval (ms)", ref this.Config.LoadInterval))
            this.Config.Save();
        ImGui.PopItemWidth();
        if (this.Config.ShowTooltips && ImGui.IsItemHovered())
            ImGui.SetTooltip(
                "Time interval between furniture placements when applying a layout. If this is too low (e.g. 200 ms), some placements may be skipped over.");
        ImGui.Dummy(new Vector2(0.0f, 15f));
        this.Config.Save();
    }

    private void DrawRow(int i, HousingItem housingItem, bool showSetPosition = true, int childIndex = -1) {
        if (!housingItem.CorrectLocation)
            ImGui.PushStyleColor(0, new Vector4(0.5f, 0.5f, 0.5f, 1f));
        var interpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 3);
        interpolatedStringHandler.AppendFormatted(housingItem.X, "N3");
        interpolatedStringHandler.AppendLiteral(", ");
        interpolatedStringHandler.AppendFormatted(housingItem.Y, "N3");
        interpolatedStringHandler.AppendLiteral(", ");
        interpolatedStringHandler.AppendFormatted(housingItem.Z, "N3");
        ImGui.Text(interpolatedStringHandler.ToStringAndClear());
        if (!housingItem.CorrectLocation)
            ImGui.PopStyleColor();
        ImGui.NextColumn();
        if (!housingItem.CorrectRotation)
            ImGui.PushStyleColor(0, new Vector4(0.5f, 0.5f, 0.5f, 1f));
        interpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
        interpolatedStringHandler.AppendFormatted(housingItem.Rotate, "N3");
        ImGui.Text(interpolatedStringHandler.ToStringAndClear());
        ImGui.NextColumn();
        if (!housingItem.CorrectRotation)
            ImGui.PopStyleColor();
        var row1 = DalamudApi.DataManager.GetExcelSheet<Stain>().GetRow(housingItem.Stain);
        var name = row1?.Name;
        if (housingItem.Stain != 0) {
            Utils.StainButton("dye_" + i, row1, new Vector2(20f));
            ImGui.SameLine();
            if (!housingItem.DyeMatch)
                ImGui.PushStyleColor(0, new Vector4(0.5f, 0.5f, 0.5f, 1f));
            interpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
            interpolatedStringHandler.AppendFormatted<SeString>(name);
            ImGui.Text(interpolatedStringHandler.ToStringAndClear());
            if (!housingItem.DyeMatch)
                ImGui.PopStyleColor();
        } else if (housingItem.MaterialItemKey != 0U) {
            var row2 = DalamudApi.DataManager.GetExcelSheet<Item>().GetRow(housingItem.MaterialItemKey);
            if (row2 != null) {
                if (!housingItem.DyeMatch)
                    ImGui.PushStyleColor(0, new Vector4(0.5f, 0.5f, 0.5f, 1f));
                this.DrawIcon(row2.Icon, new Vector2(20f, 20f));
                ImGui.SameLine();
                ImGui.Text(row2.Name.ToString());
                if (!housingItem.DyeMatch)
                    ImGui.PopStyleColor();
            }
        }

        ImGui.NextColumn();
        if (!showSetPosition)
            return;
        var str = childIndex == -1 ? i.ToString() : i + "_" + childIndex;
        if (housingItem.ItemStruct != IntPtr.Zero && ImGui.Button("Set##" + str)) {
            this.Plugin.MatchLayout();
            if (housingItem.ItemStruct != IntPtr.Zero)
                MakePlacePlugin.SetItemPosition(housingItem);
            else
                MakePlacePlugin.LogError("Unable to set position for " + housingItem.Name);
        }

        ImGui.NextColumn();
    }

    private void DrawFixtureList(List<Fixture> fixtureList) {
        try {
            if (ImGui.Button("Clear")) {
                fixtureList.Clear();
                this.Config.Save();
            }

            ImGui.Columns(3, "FixtureList", true);
            ImGui.Separator();
            ImGui.Text("Level");
            ImGui.NextColumn();
            ImGui.Text("Fixture");
            ImGui.NextColumn();
            ImGui.Text("Item");
            ImGui.NextColumn();
            ImGui.Separator();
            foreach (var fixture in fixtureList) {
                ImGui.Text(fixture.level);
                ImGui.NextColumn();
                ImGui.Text(fixture.type);
                ImGui.NextColumn();
                var row = DalamudApi.DataManager.GetExcelSheet<Item>().GetRow(fixture.itemId);
                if (row != null) {
                    this.DrawIcon(row.Icon, new Vector2(20f, 20f));
                    ImGui.SameLine();
                }

                ImGui.Text(fixture.name);
                ImGui.NextColumn();
                ImGui.Separator();
            }

            ImGui.Columns(1);
        } catch (Exception ex) {
            MakePlacePlugin.LogError(ex.Message, ex.StackTrace);
        }
    }

    private void DrawItemList(List<HousingItem> itemList, bool isUnused = false) {
        if (ImGui.Button("Sort")) {
            itemList.Sort((Comparison<HousingItem>)((x, y) => {
                if (x.Name.CompareTo(y.Name) != 0)
                    return x.Name.CompareTo(y.Name);
                if (x.X.CompareTo(y.X) != 0)
                    return x.X.CompareTo(y.X);
                if (x.Y.CompareTo(y.Y) != 0)
                    return x.Y.CompareTo(y.Y);
                if (x.Z.CompareTo(y.Z) != 0)
                    return x.Z.CompareTo(y.Z);
                return x.Rotate.CompareTo(y.Rotate) != 0 ? x.Rotate.CompareTo(y.Rotate) : 0;
            }));
            this.Config.Save();
        }

        ImGui.SameLine();
        if (ImGui.Button("Clear")) {
            itemList.Clear();
            this.Config.Save();
        }

        if (!isUnused) {
            ImGui.SameLine();
            ImGui.Text("Note: Missing items, incorrect dyes, and items on unselected floors are grayed out");
        }

        ImGui.Columns(isUnused ? 4 : 5, "ItemList", true);
        ImGui.Separator();
        ImGui.Text("Item");
        ImGui.NextColumn();
        ImGui.Text("Position (X,Y,Z)");
        ImGui.NextColumn();
        ImGui.Text("Rotation");
        ImGui.NextColumn();
        ImGui.Text("Dye/Material");
        ImGui.NextColumn();
        if (!isUnused) {
            ImGui.Text("Set Position");
            ImGui.NextColumn();
        }

        ImGui.Separator();
        for (var index = 0; index < itemList.Count(); ++index) {
            var housingItem = itemList[index];
            var name = housingItem.Name;
            var row = DalamudApi.DataManager.GetExcelSheet<Item>().GetRow(housingItem.ItemKey);
            if (row != null) {
                this.DrawIcon(row.Icon, new Vector2(20f, 20f));
                ImGui.SameLine();
            }

            if (housingItem.ItemStruct == IntPtr.Zero)
                ImGui.PushStyleColor(0, new Vector4(0.5f, 0.5f, 0.5f, 1f));
            ImGui.Text(name);
            ImGui.NextColumn();
            this.DrawRow(index, housingItem, !isUnused);
            if (housingItem.ItemStruct == IntPtr.Zero)
                ImGui.PopStyleColor();
            ImGui.Separator();
        }

        ImGui.Columns(1);
    }

    protected override void DrawScreen() {
        if (!this.Config.DrawScreen)
            return;
        this.DrawItemOnScreen();
    }

    private unsafe void DrawItemOnScreen() {
        if (Memory.Instance == null)
            return;
        var source = Memory.Instance.GetCurrentTerritory() == Memory.HousingArea.Indoors
            ? this.Plugin.InteriorItemList
            : this.Plugin.ExteriorItemList;
        for (var index = 0; index < source.Count(); ++index) {
            var position = ((GameObject)DalamudApi.ClientState.LocalPlayer).Position;
            var rowItem = source[index];
            if (rowItem.ItemStruct != IntPtr.Zero) {
                var itemStruct = (HousingItemStruct*)rowItem.ItemStruct;
                var vector3 = new Vector3(itemStruct->Position.X, itemStruct->Position.Y, itemStruct->Position.Z);
                if (this.Config.HiddenScreenItemHistory.IndexOf(index) < 0 && (this.Config.DrawDistance <= 0.0 ||
                                                                               (position - vector3).Length() <=
                                                                               (double)this.Config.DrawDistance)) {
                    var name = rowItem.Name;
                    if (DalamudApi.GameGui.WorldToScreen(vector3, out var vector2)) {
                        ImGui.PushID("HousingItemWindow" + index);
                        ImGui.SetNextWindowPos(new Vector2(vector2.X, vector2.Y));
                        ImGui.SetNextWindowBgAlpha(0.8f);
                        if (ImGui.Begin("HousingItem" + index, (ImGuiWindowFlags)790895)) {
                            ImGui.Text(name);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##ScreenItem" + index)) {
                                if (!Memory.Instance.CanEditItem()) {
                                    MakePlacePlugin.LogError("Unable to set position while not in rotate layout mode");
                                    continue;
                                }

                                MakePlacePlugin.SetItemPosition(rowItem);
                                this.Config.HiddenScreenItemHistory.Add(index);
                                this.Config.Save();
                            }

                            ImGui.SameLine();
                            ImGui.End();
                        }

                        ImGui.PopID();
                    }
                }
            }
        }
    }
}
