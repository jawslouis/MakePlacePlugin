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
            MakePlacePlugin.Interface.UiBuilder.Draw += Draw;
            MakePlacePlugin.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
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
            MakePlacePlugin.Interface.UiBuilder.Draw -= Draw;
            MakePlacePlugin.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        }
    }
}