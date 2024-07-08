using System;
using DisPlacePlugin.Gui;

namespace DisPlacePlugin
{
    public class PluginUi : IDisposable
    {
        private readonly DisPlacePlugin _plugin;
        public ConfigurationWindow ConfigWindow { get; }

        public PluginUi(DisPlacePlugin plugin)
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