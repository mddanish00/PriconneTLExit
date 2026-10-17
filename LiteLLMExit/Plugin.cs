/*
    PriconneTLLiteLLMExit is a BepInEx plugin to automatically close LiteLLM launched by LiteLLMEndpoint when closing Priconne.
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
using LiteLLM;

namespace LiteLLMExit;


[BepInPlugin("LiteLLMExit", "PriconneTLLiteLLMExit by mddanish00", "1.1.0")] //x-release-please-version
[BepInProcess("PrincessConnectReDive.exe")]
public class Plugin : BasePlugin
{
    private readonly Harmony harmony = new("com.github.mddanish00.PriconneTLLiteLLMExit");

    public override void Load()
    {
        LiteLLMExit.Log.BieLogger = Log;

        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }
        catch (Exception e)
        {
            LiteLLMExit.Log.Warn("Failed to set console encoding to UTF8. Japanese characters may not display correctly.");
            LiteLLMExit.Log.Warn(e);
        }

        var sw = Stopwatch.StartNew();
        harmony.PatchAll();
        sw.Stop();
        LiteLLMExit.Log.Debug($"Patching took {sw.ElapsedMilliseconds}ms");
        LiteLLMExit.Log.Info("LiteLLMExit loaded!");
    }

    public override bool Unload()
    {
        LiteLLMExit.Log.Info("Shutting down...");
        harmony.UnpatchSelf();
        return true;
    }
}

[HarmonyPatch(typeof(LiteLLMEndpoint), "StartProcess")]
public class LiteLLMExitPatch
{
    internal static Process? process;

    public static void Postfix(Process ___process)
    {
        Log.Info("LiteLLMEndpoint.StartProcess");
        process = ___process;
    }
}

[HarmonyPatch(typeof(Toolbox), nameof(Toolbox.ApplicationQuit))]
public class LiteLLMExitPatch2
{
    public static void Prefix()
    {
        Log.Info("Application quitting");
        LiteLLMExitPatch.process?.Kill();
        LiteLLMExitPatch.process = null;
    }
}


