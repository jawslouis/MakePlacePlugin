using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace MakePlacePlugin;

public class DalamudApi
{
    public static void Initialize(IDalamudPluginInterface pluginInterface) => pluginInterface.Create<DalamudApi>();

    // [PluginService] public static IAetheryteList AetheryteList { get; private set; } = null;
    // [PluginService] public static IBuddyList BuddyList { get; private set; } = null;    
    [PluginService] public static IChatGui ChatGui { get; private set; } = null;
    [PluginService] public static IClientState ClientState { get; private set; } = null;
    [PluginService] public static ICommandManager CommandManager { get; private set; } = null;
    // [PluginService] public static ICondition Condition { get; private set; } = null;
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null;
    [PluginService] public static IDataManager DataManager { get; private set; } = null;
    [PluginService] public static ITextureProvider TextureProvider { get; private set; }
    // [PluginService] public static IDtrBar DtrBar { get; private set; } = null;
    // [PluginService] public static IFateTable FateTable { get; private set; } = null;
    // [PluginService] public static IFlyTextGui FlyTextGui { get; private set; } = null;
    // [PluginService] public static IFramework Framework { get; private set; } = null;
    [PluginService] public static IGameGui GameGui { get; private set; } = null;
    // [PluginService] public static IGameNetwork GameNetwork { get; private set; } = null;
    // [PluginService] public static IGamepadState GamePadState { get; private set; } = null;
    // [PluginService] public static IJobGauges JobGauges { get; private set; } = null;
    // [PluginService] public static IKeyState KeyState { get; private set; } = null;
    // [PluginService] public static ILibcFunction LibcFunction { get; private set; } = null;
    //[PluginService] public static IObjectTable ObjectTable { get; private set; } = null;
    // [PluginService] public static IPartyFinderGui PartyFinderGui { get; private set; } = null;
    // [PluginService] public static IPartyList PartyList { get; private set; } = null;
    [PluginService] public static ISigScanner SigScanner { get; private set; } = null;
    // [PluginService] public static ITargetManager TargetManager { get; private set; } = null;
    // [PluginService] public static IToastGui ToastGui { get; private set; } = null;
     [PluginService] public static IGameInteropProvider Hooks { get; private set; } = null;
    [PluginService] public static IPluginLog PluginLog { get; private set; } = null;
    // [PluginService] public static ITitleScreenMenu TitleScreenMenu { get; private set; } = null;
}