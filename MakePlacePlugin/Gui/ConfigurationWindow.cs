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

        private readonly Vector4 PURPLE = new(0.26275f, 0.21569f, 0.56863f, 1f);
        private readonly Vector4 PURPLE_ALPHA = new(0.26275f, 0.21569f, 0.56863f, 0.5f);

        public ConfigurationWindow(MakePlacePlugin plugin) : base(plugin)
        {

        }

        protected override void DrawUi()
        {
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, PURPLE);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, PURPLE_ALPHA);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, PURPLE_ALPHA);
            ImGui.SetNextWindowSize(new Vector2(530, 450), ImGuiCond.FirstUseEver);
            if (!ImGui.Begin($"{Plugin.Name}", ref WindowVisible, ImGuiWindowFlags.NoScrollWithMouse))
            {
                ImGui.PopStyleColor(3);
                ImGui.End();
                return;
            }
            if (ImGui.BeginChild("##SettingsRegion"))
            {
                DrawGeneralSettings();
                if (ImGui.BeginChild("##ItemListRegion"))
                {
                    ImGui.PushStyleColor(ImGuiCol.Header, PURPLE_ALPHA);
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, PURPLE);
                    ImGui.PushStyleColor(ImGuiCol.HeaderActive, PURPLE);


                    if (ImGui.CollapsingHeader("Interior Furniture", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.PushID("interior");
                        DrawItemList(Plugin.InteriorItemList);
                        ImGui.PopID();
                    }
                    if (ImGui.CollapsingHeader("Exterior Furniture", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.PushID("exterior");
                        DrawItemList(Plugin.ExteriorItemList);
                        ImGui.PopID();
                    }

                    if (ImGui.CollapsingHeader("Interior Fixtures", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.PushID("interiorFixture");
                        DrawFixtureList(Plugin.Layout.interiorFixture);
                        ImGui.PopID();
                    }

                    if (ImGui.CollapsingHeader("Exterior Fixtures", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.PushID("exteriorFixture");
                        DrawFixtureList(Plugin.Layout.exteriorFixture);
                        ImGui.PopID();
                    }
                    if (ImGui.CollapsingHeader("Unused Furniture", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.PushID("unused");
                        DrawItemList(Plugin.UnusedItemList, true);
                        ImGui.PopID();
                    }

                    ImGui.PopStyleColor(3);
                    ImGui.EndChild();
                }
                ImGui.EndChild();
            }

            ImGui.PopStyleColor(3);

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
                    MakePlacePlugin.LayoutManager.ExportLayout();
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

                if (!IsDecorMode())
                {
                    LogError("Unable to load layouts outside of Layout mode");
                    LogError("(Housing -> Indoor/Outdoor Furnishings)");

                }
                else
                {

                    try
                    {
                        SaveLayoutManager.ImportLayout(Config.SaveLocation);
                        Plugin.MatchLayout();
                        Config.ResetRecord();
                        Log(String.Format("Imported {0} items", Plugin.InteriorItemList.Count + Plugin.ExteriorItemList.Count));
                    }
                    catch (Exception e)
                    {
                        LogError($"Load Error: {e.Message}", e.StackTrace);
                    }
                }
            }
            if (Config.ShowTooltips && ImGui.IsItemHovered()) ImGui.SetTooltip("Load layout from file");

            ImGui.Dummy(new Vector2(0, 15));

            ImGui.Text("Selected Floors");

            if (ImGui.Checkbox("Basement", ref Config.Basement))
            {
                Plugin.MatchLayout();
                Config.Save();
            }
            ImGui.SameLine(); ImGui.Dummy(new Vector2(10, 0)); ImGui.SameLine();

            if (ImGui.Checkbox("Ground Floor", ref Config.GroundFloor))
            {
                Plugin.MatchLayout();
                Config.Save();
            }
            ImGui.SameLine(); ImGui.Dummy(new Vector2(10, 0)); ImGui.SameLine();

            if (ImGui.Checkbox("Upper Floor", ref Config.UpperFloor))
            {
                Plugin.MatchLayout();
                Config.Save();
            }

            ImGui.Dummy(new Vector2(0, 15));

            ImGui.Text("Layout Actions");

            var inOut = Memory.Instance.IsOutdoors() ? "Exterior" : "Interior";

            if (ImGui.Button($"Get {inOut} Layout"))
            {
                if (IsDecorMode())
                {
                    try
                    {
                        Plugin.LoadLayout();
                    }
                    catch (Exception e)
                    {
                        LogError($"Error: {e.Message}", e.StackTrace);
                    }
                }
                else
                {
                    LogError("Unable to load layouts outside of Layout mode");
                    LogError("(Housing -> Indoor/Outdoor Furnishings)");

                }
            }

            ImGui.SameLine();

            if (ImGui.Button($"Apply {inOut} Layout"))
            {
                if (IsDecorMode() && IsRotateMode())
                {
                    try
                    {
                        Plugin.MatchLayout();
                        Plugin.ApplyLayout();
                    }
                    catch (Exception e)
                    {
                        LogError($"Error: {e.Message}", e.StackTrace);
                    }
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

            ImGui.Dummy(new Vector2(0, 15));
            Config.Save();

        }

        private void DrawRow(int i, HousingItem housingItem, bool showSetPosition = true, int childIndex = -1)
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

            if (showSetPosition)
            {
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


        }

        private void DrawFixtureList(List<Fixture> fixtureList)
        {

            try
            {


                if (ImGui.Button("Clear"))
                {
                    fixtureList.Clear();
                    Config.Save();
                }

                ImGui.Columns(3, "FixtureList", true);
                ImGui.Separator();

                ImGui.Text("Level"); ImGui.NextColumn();
                ImGui.Text("Fixture"); ImGui.NextColumn();
                ImGui.Text("Item"); ImGui.NextColumn();

                ImGui.Separator();

                foreach (var fixture in fixtureList)
                {
                    ImGui.Text(fixture.level); ImGui.NextColumn();
                    ImGui.Text(fixture.type); ImGui.NextColumn();


                    var item = Data.GetExcelSheet<Item>().GetRow(fixture.itemId);
                    if (item != null)
                    {
                        DrawIcon(item.Icon, new Vector2(20, 20));
                        ImGui.SameLine();
                    }
                    ImGui.Text(fixture.name); ImGui.NextColumn();

                    ImGui.Separator();
                }

                ImGui.Columns(1);

            }
            catch (Exception e)
            {
                LogError(e.Message, e.StackTrace);
            }

        }

        private void DrawItemList(List<HousingItem> itemList, bool isUnused = false)
        {



            if (ImGui.Button("Sort"))
            {
                itemList.Sort((x, y) =>
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
            if (ImGui.Button("Clear"))
            {
                itemList.Clear();
                Config.Save();
            }

            if (!isUnused)
            {
                ImGui.SameLine();
                ImGui.Text("Note: Missing items, incorrect dyes, and items on unselected floors are grayed out");
            }

            // name, position, r, color, set
            int columns = isUnused ? 4 : 5;


            ImGui.Columns(columns, "ItemList", true);
            ImGui.Separator();
            ImGui.Text("Item"); ImGui.NextColumn();
            ImGui.Text("Position (X,Y,Z)"); ImGui.NextColumn();
            ImGui.Text("Rotation"); ImGui.NextColumn();
            ImGui.Text("Dye"); ImGui.NextColumn();

            if (!isUnused)
            {
                ImGui.Text("Set Position"); ImGui.NextColumn();
            }

            ImGui.Separator();
            for (int i = 0; i < itemList.Count(); i++)
            {
                var housingItem = itemList[i];
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
                DrawRow(i, housingItem, !isUnused);

                if (housingItem.ItemStruct == IntPtr.Zero)
                {
                    ImGui.PopStyleColor();
                }

                ImGui.Separator();
            }

            ImGui.Columns(1);

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

            if (Memory.Instance == null) return;

            var itemList = Memory.Instance.IsOutdoors() ? Plugin.ExteriorItemList : Plugin.InteriorItemList;

            for (int i = 0; i < itemList.Count(); i++)
            {
                var playerPos = MakePlacePlugin.ClientState.LocalPlayer.Position;
                var housingItem = itemList[i];

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