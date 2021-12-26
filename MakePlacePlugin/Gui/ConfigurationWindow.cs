using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using MakePlacePlugin.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using static MakePlacePlugin.MakePlacePlugin;

namespace MakePlacePlugin.Gui
{
    public class ConfigurationWindow : Window<MakePlacePlugin>
    {

        public Configuration Config => Plugin.Config;

        private string CustomTag = string.Empty;
        private readonly Dictionary<uint, uint> iconToFurniture = new() { };


        public ConfigurationWindow(MakePlacePlugin plugin) : base(plugin)
        {

        }

        protected override void DrawUi()
        {
            ImGui.SetNextWindowSize(new Vector2(530, 450), ImGuiCond.FirstUseEver);
            if (!ImGui.Begin($"{Plugin.Name}", ref WindowVisible, ImGuiWindowFlags.NoScrollWithMouse))
            {
                ImGui.End();
                return;
            }
            if (ImGui.BeginChild("##SettingsRegion"))
            {
                DrawGeneralSettings();
                if (ImGui.BeginChild("##ItemListRegion"))
                {
                    DrawItemList();
                    ImGui.EndChild();
                }
                ImGui.EndChild();
            }
            ImGui.End();
        }

        #region Helper Functions
        public void DrawIcon(ushort icon, Vector2 size)
        {
            if (icon < 65000)
            {
                if (Plugin.TextureDictionary.ContainsKey(icon))
                {
                    var tex = Plugin.TextureDictionary[icon];
                    if (tex == null || tex.ImGuiHandle == IntPtr.Zero)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(1, 0, 0, 1));
                        ImGui.BeginChild("FailedTexture", size);
                        ImGui.Text(icon.ToString());
                        ImGui.EndChild();
                        ImGui.PopStyleColor();
                    }
                    else
                        ImGui.Image(Plugin.TextureDictionary[icon].ImGuiHandle, size);
                }
                else
                {
                    ImGui.BeginChild("WaitingTexture", size, true);
                    ImGui.EndChild();

                    Plugin.TextureDictionary[icon] = null;

                    Task.Run(() =>
                    {
                        try
                        {
                            var iconTex = MakePlacePlugin.Data.GetIcon(icon);
                            var tex = MakePlacePlugin.Interface.UiBuilder.LoadImageRaw(iconTex.GetRgbaImageData(), iconTex.Header.Width, iconTex.Header.Height, 4);
                            if (tex != null && tex.ImGuiHandle != IntPtr.Zero)
                                Plugin.TextureDictionary[icon] = tex;
                        }
                        catch
                        {
                        }
                    });
                }
            }
        }
        #endregion


        #region Basic UI
        unsafe private void DrawGeneralSettings()
        {

            if (ImGui.Checkbox("Label Furniture", ref Config.DrawScreen)) Config.Save();
            if (Config.ShowTooltips && ImGui.IsItemHovered())
                ImGui.SetTooltip("Show furniture names on the screen");

            ImGui.SameLine();
            ImGui.Dummy(new Vector2(10, 0));
            ImGui.SameLine();
            if (ImGui.Checkbox("##hideTooltipsOnOff", ref Config.ShowTooltips)) Config.Save();
            ImGui.SameLine();
            ImGui.TextUnformatted("Show Tooltips");

            ImGui.Dummy(new Vector2(0, 10));
            ImGui.Text("Save/Load file location");
            if (ImGui.InputText("##saveLocation", ref Config.SaveLocation, 100))
            {
                Config.Save();
            }

            if (ImGui.Button("Save"))
            {
                try
                {
                    MakePlacePlugin.HousePrinter.ExportLayout(Config.HousingItemList, Config);
                }
                catch (Exception e)
                {
                    LogError($"Save Error: {e.Message}", e.StackTrace);
                }
            }
            if (Config.ShowTooltips && ImGui.IsItemHovered()) ImGui.SetTooltip("Save current layout to file");

            ImGui.SameLine();
            if (ImGui.Button("Load"))
            {
                try
                {
                    Config.HousingItemList = LayoutExporter.ImportLayout(Config.SaveLocation);
                    Plugin.MatchLayout();
                    Config.ResetRecord();
                    Log(String.Format("Imported {0} items", Config.HousingItemList.Count));
                }
                catch (Exception e)
                {
                    LogError($"Load Error: {e.Message}", e.StackTrace);
                }
            }
            if (Config.ShowTooltips && ImGui.IsItemHovered()) ImGui.SetTooltip("Load layout from file");


            ImGui.Dummy(new Vector2(0, 15));

            ImGui.Text("Layout Actions");

            if (ImGui.Button("Clear"))
            {
                Config.HousingItemList.Clear();
                Config.Save();
            }
            ImGui.SameLine();
            if (ImGui.Button("Sort"))
            {
                Config.SelectedItemIndex = -1;
                Config.HousingItemList.Sort((x, y) =>
                {
                    if (x.Name.CompareTo(y.Name) != 0)
                        return x.Name.CompareTo(y.Name);
                    if (x.X.CompareTo(y.X) != 0)
                        return x.X.CompareTo(y.X);
                    if (x.Y.CompareTo(y.Y) != 0)
                        return x.Y.CompareTo(y.Y);
                    if (x.Z.CompareTo(y.Z) != 0)
                        return x.Z.CompareTo(y.Z);
                    if (x.Rotate.CompareTo(y.Rotate) != 0)
                        return x.Rotate.CompareTo(y.Rotate);
                    return 0;
                });
                Config.Save();
            }

            ImGui.SameLine();

            if (ImGui.Button("House Layout"))
            {
                if (IsDecorMode())
                {
                    try
                    {
                        Plugin.LoadLayout();
                    } catch (Exception e)
                    {
                        LogError($"Error: {e.Message}", e.StackTrace);
                    }
                }
                else
                {
                    LogError("Unable to load layouts outside of Layout mode");
                }
            }
            if (Config.ShowTooltips && ImGui.IsItemHovered()) ImGui.SetTooltip("Get the house layout");

            ImGui.SameLine();

            if (ImGui.Button("Apply Layout"))
            {
                if (IsDecorMode() && MakePlacePlugin.IsRotateMode())
                {
                    Plugin.ApplyLayout();

                }
                else
                {
                    LogError("Unable to apply layouts outside of Rotate Layout mode");
                }
            }

            ImGui.SameLine();

            ImGui.PushItemWidth(100);
            if (ImGui.InputInt("Placement Interval (ms)", ref Config.LoadInterval))
            {
                Config.Save();
            }
            ImGui.PopItemWidth();
            if (Config.ShowTooltips && ImGui.IsItemHovered()) ImGui.SetTooltip("Time interval between furniture placements when applying a layout. If this is too low (e.g. 200 ms), some placements may be skipped over.");



            ImGui.Text("Note: Missing items and incorrect dyes are grayed out");

            Config.Save();

        }

        private void DrawRow(int i, HousingItem housingItem, int childIndex = -1)
        {
            ImGui.Text($"{housingItem.X:N3}, {housingItem.Y:N3}, {housingItem.Z:N3}"); ImGui.NextColumn();
            ImGui.Text($"{housingItem.Rotate:N3}"); ImGui.NextColumn();
            var stain = MakePlacePlugin.Data.GetExcelSheet<Stain>().GetRow(housingItem.Stain);
            var colorName = stain?.Name;

            if (housingItem.Stain != 0)
            {
                Utils.StainButton("dye_" + i, stain, new Vector2(20));
                ImGui.SameLine();

                if (!housingItem.DyeMatch)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
                }

                ImGui.Text($"{colorName}");

                if (!housingItem.DyeMatch)
                    ImGui.PopStyleColor();

            }
            ImGui.NextColumn();
            string uniqueID = childIndex == -1 ? i.ToString() : i.ToString() + "_" + childIndex.ToString();

            if (housingItem.ItemStruct != IntPtr.Zero)
            {

                if (ImGui.Button("Set" + "##" + uniqueID))
                {
                    SetItemPosition(housingItem);
                }
            }
            else
            {
                //ImGui.NextColumn();
            }
            ImGui.NextColumn();



        }
        private void DrawItemList()
        {
            // name, position, r, color, set
            int columns = 5;


            ImGui.Columns(columns, "ItemList", true);
            ImGui.Separator();
            ImGui.Text("Item"); ImGui.NextColumn();
            ImGui.Text("Position (X,Y,Z)"); ImGui.NextColumn();
            ImGui.Text("Rotation"); ImGui.NextColumn();
            ImGui.Text("Dye"); ImGui.NextColumn();

            ImGui.Text("Set Position"); ImGui.NextColumn();


            ImGui.Separator();
            for (int i = 0; i < Config.HousingItemList.Count(); i++)
            {
                var housingItem = Config.HousingItemList[i];
                var displayName = housingItem.Name;

                var item = MakePlacePlugin.Data.GetExcelSheet<Item>().GetRow(housingItem.ItemKey);
                if (item != null)
                {
                    DrawIcon(item.Icon, new Vector2(20, 20));
                    ImGui.SameLine();
                }

                if (housingItem.ItemStruct == IntPtr.Zero)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
                }

                ImGui.Text(displayName);



                ImGui.NextColumn();
                DrawRow(i, housingItem);

                if (housingItem.ItemStruct == IntPtr.Zero)
                {
                    ImGui.PopStyleColor();
                }

                ImGui.Separator();
            }

        }

        #endregion


        #region Draw Screen
        protected override void DrawScreen()
        {
            if (Config.DrawScreen)
            {
                DrawItemOnScreen();
            }
        }

        private unsafe void DrawItemOnScreen()
        {
            for (int i = 0; i < Config.HousingItemList.Count(); i++)
            {
                var playerPos = MakePlacePlugin.ClientState.LocalPlayer.Position;
                var housingItem = Config.HousingItemList[i];

                if (housingItem.ItemStruct == IntPtr.Zero) continue;

                var itemStruct = (HousingItemStruct*)housingItem.ItemStruct;

                var itemPos = new Vector3(itemStruct->Position.X, itemStruct->Position.Y, itemStruct->Position.Z);
                if (Config.HiddenScreenItemHistory.IndexOf(i) >= 0) continue;
                if (Config.DrawDistance > 0 && (playerPos - itemPos).Length() > Config.DrawDistance)
                    continue;
                var displayName = housingItem.Name;
                if (MakePlacePlugin.GameGui.WorldToScreen(itemPos, out var screenCoords))
                {
                    ImGui.PushID("HousingItemWindow" + i);
                    ImGui.SetNextWindowPos(new Vector2(screenCoords.X, screenCoords.Y));
                    ImGui.SetNextWindowBgAlpha(0.8f);
                    if (ImGui.Begin("HousingItem" + i,
                        ImGuiWindowFlags.NoDecoration |
                        ImGuiWindowFlags.AlwaysAutoResize |
                        ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoMove |
                        ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav))
                    {

                        ImGui.Text(displayName);

                        ImGui.SameLine();

                        if (ImGui.Button("Set" + "##ScreenItem" + i.ToString()))
                        {
                            if (!IsDecorMode() || !IsRotateMode())
                            {
                                LogError("Unable to set position while not in rotate layout mode");
                                continue;
                            }

                            SetItemPosition(housingItem);
                            Config.HiddenScreenItemHistory.Add(i);
                            Config.Save();
                        }

                        ImGui.SameLine();

                        ImGui.End();
                    }

                    ImGui.PopID();
                }
            }
        }
        #endregion





    }
}