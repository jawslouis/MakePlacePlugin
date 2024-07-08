global using Dalamud;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace DisPlacePlugin;

public class DalamudApi
{
    public static void Initialize(IDalamudPluginInterface pluginInterface) => pluginInterface.Create<DalamudApi>(); 
    [PluginService]
    public static IChatGui ChatGui { get; private set; } = null;
    [PluginService]
    public static IClientState ClientState { get; private set; } = null;
    [PluginService]
    public static ICommandManager CommandManager { get; private set; } = null;
    [PluginService]
    public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService]
    public static IDataManager DataManager { get; private set; } = null;
    [PluginService]
    public static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService]
    public static IGameGui GameGui { get; private set; } = null;
    [PluginService]
    public static ISigScanner SigScanner { get; private set; } = null;
    [PluginService]
    public static IGameInteropProvider Hooks { get; private set; } = null;
    [PluginService]
    public static IPluginLog PluginLog { get; private set; } = null;
}