using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using Il2CppSteamworks;
using Il2CppSteamworks.Data;
using MelonLoader;

[assembly: MelonInfo(typeof(BigLobbyMod.Core), "BigLobbyMod", "1.0.0", "Catalyss", null)]
[assembly: MelonGame("Pigeons at Play", "Mycopunk")]

namespace BigLobbyMod
{


    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("BIG LOBBY MOD Initialized.");
        }
    }

    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            HarmonyInstance.PatchAll();
        }
    }


    [HarmonyPatch]
    public static class LobbyPatch
    {
        // Target the CreateLobby method in the Online class
        static MethodBase TargetMethod()
        {
            return AccessTools.Method("Online:CreateLobby");
        }

        // Prefix method that will run before CreateLobby
        static void Prefix(ref int maxPlayers)
        {
            // Log the original maxPlayers value for debugging
            MelonLogger.Msg($"Original maxPlayers: {maxPlayers}");

            // Force maxPlayers to 32 (or any desired value)
            maxPlayers = 32;
            MelonLogger.Msg($"Forced maxPlayers to: {maxPlayers}");
        }
    }

    [HarmonyPatch(typeof(ISteamMatchmaking), "CreateLobby")]
    class Patch_CreateLobby
    {
        static void Prefix(ref LobbyType eLobbyType, ref int cMaxMembers)
        {
            cMaxMembers = 32;
            MelonLogger.Msg("CreateLobby patched: max players forced to 32.");
        }
    }
    [HarmonyPatch(typeof(ISteamMatchmaking), "_CreateLobby")]
    class Patch__CreateLobby
    {
        static void Prefix(IntPtr self, LobbyType eLobbyType, int cMaxMembers)
        {
            cMaxMembers = 32;
            MelonLogger.Msg("CreateLobby patched: max players forced to 32.");
        }
    }


    [HarmonyPatch]
    public static class LobbyPatches
    {
        // Target the CreateLobby method in the Online class
        static MethodBase TargetMethod()
        {
            return AccessTools.Method("Online:SetMaxLobbyPlayers");
        }

        // Prefix method that will run before CreateLobby
        static void Prefix(ref int count)
        {
            // Log the original maxPlayers value for debugging
            MelonLogger.Msg($"Original count: {count}");

            // Force maxPlayers to 32 (or any desired value)
            count = 32;
            MelonLogger.Msg($"Forced count to: {count}");
        }
    }

    [HarmonyPatch]
    public static class LobbyPatching
    {
        // Target the CreateLobby method in the Online class
        static MethodBase TargetMethod()
        {
            return AccessTools.Method("Online:SetLobbyName");
        }

        // Prefix method that will run before CreateLobby
        static void Prefix(ref string name)
        {
            // Log the original maxPlayers value for debugging
            MelonLogger.Msg($"Original name: {name}");

            // Force maxPlayers to 32 (or any desired value)
            name = "[BigLobby] " + name;
            MelonLogger.Msg($"Forced name to: {name}");
        }
    }


    [HarmonyPatch(typeof(ISteamMatchmaking), "SetLobbyMemberLimit")]
    public static class SteamMatchmakingPatch
    {
        // This will run before SetLobbyMemberLimit to set the player limit to 32
        static void Prefix(SteamId steamIDLobby, ref int cMaxMembers)
        {
            // Log the current max member value for debugging
            MelonLogger.Msg($"Original max members: {cMaxMembers}");

            // Force the lobby size to 32
            cMaxMembers = 32;

            // Log the new value to confirm
            MelonLogger.Msg($"Forced max members to {cMaxMembers}");
        }
    }
    [HarmonyPatch(typeof(SteamServer), "set_MaxPlayers")]
    public static class SteamMatchmakingPatches
    {
        // This will run before set_MaxPlayers to set the player limit to 32
        static void Prefix(ref int value)
        {
            // Log the current max member value for debugging
            MelonLogger.Msg($"Original max members: {value}");

            // Force the lobby size to 32
            value = 32;

            // Log the new value to confirm
            MelonLogger.Msg($"Forced max members to {value}");
        }
    }


    [HarmonyPatch(typeof(Lobby), "set_MaxMembers")]
    public static class set_MaxMembersPatches
    {
        // This will run before set_MaxPlayers to set the player limit to 32
        static void Prefix(ref int value)
        {
            // Log the current max member value for debugging
            MelonLogger.Msg($"Original max members: {value}");

            // Force the lobby size to 32
            value = 32;

            // Log the new value to confirm
            MelonLogger.Msg($"Forceds to {value}");
        }
    }

    [HarmonyPatch(typeof(ISteamMatchmaking), "SetLobbyJoinable")]
    public static class SetLobbyJoinablePatches
    {
        // This will run before set_MaxPlayers to set the player limit to 32
        static void Prefix(SteamId steamIDLobby, ref bool bLobbyJoinable)
        {
            // Log the current max member value for debugging
            MelonLogger.Msg($"Original max members: {bLobbyJoinable}");

            // Force the lobby size to 32
            bLobbyJoinable = true;

            // Log the new value to confirm
            MelonLogger.Msg($"Forceds to {bLobbyJoinable}");
        }
    }



    [HarmonyPatch(typeof(Lobby), "SetJoinable")]
    public static class SetJoinablePatches
    {
        // This will run before set_MaxPlayers to set the player limit to 32
        static void Prefix(ref bool b)
        {
            // Log the current max member value for debugging
            MelonLogger.Msg($"Original : {b}");

            // Force the lobby size to 32
            b = true;

            // Log the new value to confirm
            MelonLogger.Msg($"Forced to {b}");
        }
    }


    // Replace with the correct full type name if needed
    [HarmonyPatch(typeof(Il2Cpp.GameManager), nameof(Il2Cpp.GameManager.IsJoinable))]
    public static class JoinablePatch
    {
        static bool Prefix(ref Il2Cpp.GameManager.JoinResult __result)
        {
            __result = Il2Cpp.GameManager.JoinResult.IsJoinable; // Always return Success
            MelonLogger.Msg($"GameManager.JoinResult.IsJoinable");
            return false; // Skip original method
        }
    }

    [HarmonyPatch(typeof(GameManager), "Start")] // or any known method that runs early
    public static class Patch_GameManager_Start
    {
        static void Postfix()
        {
            if (GameManager.players != null && GameManager.players.Capacity < 32)
            {
                GameManager.players.Capacity = 32;
                MelonLogger.Msg("Forced GameManager.players capacity to 32");
                MelonLogger.Msg($"Player list count: {GameManager.players.Count}, capacity: {GameManager.players.Capacity}");

            }
        }
    }
}