using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;

namespace MakePlacePlugin
{
    public class HookManager
    {
        public static List<IHookWrapper> HookList = new();
        public static ISigScanner Scanner;
        public static IGameInteropProvider InteropProvider;

        public static void Init(ISigScanner s, IGameInteropProvider interopProvider)
        {
            Scanner = s;
            InteropProvider = interopProvider;
        }

        public static void Dispose()
        {
            foreach (var hook in HookList.Where(hook => !hook.IsDisposed))
            {
                if (hook.IsEnabled)
                    hook.Disable();
                hook.Dispose();
            }

            HookList.Clear();

        }

        public static HookWrapper<T> Hook<T>(string signature, T detour, bool enable = true, int addressOffset = 0)
    where T : Delegate
        {
            var addr = Scanner.ScanText(signature);

            return HookAddress(addr, detour, enable, addressOffset);
        }

        public static HookWrapper<T> HookAddress<T>(IntPtr addr, T detour, bool enable = true, int addressOffset = 0) where T : Delegate
        {
            PluginLog.Information($"Hooking {detour.Method.Name} at {addr.ToString("X")}");
            
            var h = InteropProvider.HookFromAddress(addr + addressOffset, detour);
            var wh = new HookWrapper<T>(h);
            if (enable) wh.Enable();
            HookList.Add(wh);
            return wh;
        }

    }
}
