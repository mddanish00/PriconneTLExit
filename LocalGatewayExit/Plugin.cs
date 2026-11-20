/*
    PriconneTLLocalGatewayExit is a BepInEx plugin to automatically close LocalGateway launched by LocalGatewayEndpoint when closing Priconne.
    Copyright (C) 2026  mddanishh00
    Copyright (C) 2023  Kevga

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

    This plugin is based on SugoiExitPatch from PriconneTLFixup (https://github.com/Kevga/PriconneTLFixup).
*/
using System.Diagnostics;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using Cute;
using HarmonyLib;
using LocalGateway;

namespace LocalGatewayExit;


[BepInPlugin("LocalGatewayExit", "PriconneTLLocalGatewayExit by mddanish00", "1.2.0")] //x-release-please-version
[BepInProcess("PrincessConnectReDive.exe")]
public class Plugin : BasePlugin
{
    private readonly Harmony harmony = new("com.github.mddanish00.PriconneTLLocalGatewayExit");

    public override void Load()
    {
        LocalGatewayExit.Log.BieLogger = Log;

        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }
        catch (Exception e)
        {
            LocalGatewayExit.Log.Warn("Failed to set console encoding to UTF8. Japanese characters may not display correctly.");
            LocalGatewayExit.Log.Warn(e);
        }

        var sw = Stopwatch.StartNew();
        harmony.PatchAll();
        sw.Stop();
        LocalGatewayExit.Log.Debug($"Patching took {sw.ElapsedMilliseconds}ms");
        LocalGatewayExit.Log.Info("LocalGatewayExit loaded!");
    }

    public override bool Unload()
    {
        LocalGatewayExit.Log.Info("Shutting down...");
        harmony.UnpatchSelf();
        return true;
    }
}

[HarmonyPatch(typeof(LocalGatewayEndpoint), "StartProcess")]
public class LocalGatewayExitPatch
{
    internal static Process? process;

    public static void Postfix(Process ___process)
    {
        Log.Info("LocalGatewayEndpoint.StartProcess");
        process = ___process;
    }
}

[HarmonyPatch(typeof(Toolbox), nameof(Toolbox.ApplicationQuit))]
public class LocalGatewayExitPatch2
{
    public static void Prefix()
    {
        Log.Info("Application quitting");
        LocalGatewayExitPatch.process?.Kill();
        LocalGatewayExitPatch.process = null;
    }
}


