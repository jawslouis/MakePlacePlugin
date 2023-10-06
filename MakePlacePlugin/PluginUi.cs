using System;
using MakePlacePlugin.Gui;

namespace MakePlacePlugin
{
    public class PluginUi : IDisposable
    {
        private readonly MakePlacePlugin _plugin;
        public ConfigurationWindow ConfigWindow { get; }

        public PluginUi(MakePlacePlugin plugin)
        {
            ConfigWindow = new ConfigurationWindow(plugin);

            _plugin = plugin;
            DalamudApi.PluginInterface.UiBuilder.Draw += Draw;
            DalamudApi.PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        }

        private void Draw()
        {
            ConfigWindow.Draw();
        }
        private void OnOpenConfigUi()
        {
            ConfigWindow.Visible = true;
            ConfigWindow.CanUpload = false;
            ConfigWindow.CanImport = false;
        }

        public void Dispose()
        {
            DalamudApi.PluginInterface.UiBuilder.Draw -= Draw;
            DalamudApi.PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        }
    }
}