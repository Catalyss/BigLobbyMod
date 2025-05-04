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
        public static MelonPreferences_Category configCategory;
        public static MelonPreferences_Entry<int> maxPlayers;
        public static MelonPreferences_Entry<bool> allowPermaJoin;
        public static MelonPreferences_Entry<string> lobbyname;
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("BIG LOBBY MOD Initialized.");
            // Create config category and entry
            configCategory = MelonPreferences.CreateCategory("BigLobbyMod");
            maxPlayers = configCategory.CreateEntry<int>("MaxPlayers", 32, "Max Players", "Maximum number of players allowed in a lobby");
            allowPermaJoin = configCategory.CreateEntry<bool>("allowPermaJoin", false, "Allow Perma Join", "Make the lobby always joinnable");
            lobbyname = configCategory.CreateEntry<string>("lobbyname", "${PlayerName}'s Lobby", "Change lobby name", "Change the default lobby name use \"${PlayerName}\" to pickup the player's name");

            MelonLogger.Msg($"Config loaded. MaxPlayers = {maxPlayers.Value}");
        }
    }

    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Global.LimitPlayerCount=false;
            Global._LimitPlayerCount_k__BackingField=false;
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
            // Force maxPlayers to 32 (or any desired value)
            maxPlayers = Core.maxPlayers.Value;
        }
    }

    [HarmonyPatch(typeof(ISteamMatchmaking), "CreateLobby")]
    class Patch_CreateLobby
    {
        static void Prefix(ref LobbyType eLobbyType, ref int cMaxMembers)
        {
            cMaxMembers = Core.maxPlayers.Value;
        }
    }
    [HarmonyPatch(typeof(ISteamMatchmaking), "_CreateLobby")]
    class Patch__CreateLobby
    {
        static void Prefix(IntPtr self, LobbyType eLobbyType, int cMaxMembers)
        {
            cMaxMembers = Core.maxPlayers.Value;
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
            // Force maxPlayers to 32 (or any desired value)
            count = Core.maxPlayers.Value;
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
            name = "[BL] " + Core.lobbyname.Value.Replace("${PlayerName}", SteamClient.Name);
            MelonLogger.Msg($"Forced name to: {name}");
        }
    }


    [HarmonyPatch(typeof(ISteamMatchmaking), "SetLobbyMemberLimit")]
    public static class SteamMatchmakingPatch
    {
        // This will run before SetLobbyMemberLimit to set the player limit to 32
        static void Prefix(SteamId steamIDLobby, ref int cMaxMembers)
        {
            // Force the lobby size to 32
            cMaxMembers = Core.maxPlayers.Value;
        }
    }
    [HarmonyPatch(typeof(SteamServer), "set_MaxPlayers")]
    public static class SteamMatchmakingPatches
    {
        // This will run before set_MaxPlayers to set the player limit to 32
        static void Prefix(ref int value)
        {
            // Force the lobby size to 32
            value = Core.maxPlayers.Value;
        }
    }


    [HarmonyPatch(typeof(Lobby), "set_MaxMembers")]
    public static class set_MaxMembersPatches
    {
        // This will run before set_MaxPlayers to set the player limit to 32
        static void Prefix(ref int value)
        {
            // Force the lobby size to 32
            value = Core.maxPlayers.Value;
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
            // Force the lobby size to 32
            b = true;
        }
    }

    [HarmonyPatch(typeof(Global), "get_LimitPlayerCount")]
    public static class Patch_LimitPlayerCount
    {
        static bool Prefix(ref bool __result)
        {
            __result = false; // Disable player limit
            return false; // Skip original
        }
    }


    [HarmonyPatch(typeof(Global), "set_LimitPlayerCount")]
    public static class Patch_SetLimitPlayerCount
    {
        static bool Prefix(ref bool value)
        {
            value = false; // Force disable
            return true; // Call original with our override
        }
    }



    // Replace with the correct full type name if needed
    [HarmonyPatch(typeof(Il2Cpp.GameManager), nameof(Il2Cpp.GameManager.IsJoinable))]
    public static class JoinablePatch
    {
        static bool Prefix(ref Il2Cpp.GameManager.JoinResult __result)
        {
            //Make sure the limit is indeed off
            Global.LimitPlayerCount=false;
            Global._LimitPlayerCount_k__BackingField=false;
            if (Core.allowPermaJoin.Value)
            {
                __result = Il2Cpp.GameManager.JoinResult.IsJoinable; // Always return Success
                MelonLogger.Msg($"GameManager.JoinResult.IsJoinable");
                return false; // Skip original method
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(GameManager), "Start")] // or any known method that runs early
    public static class Patch_GameManager_Start
    {
        static void Postfix()
        {
            if (GameManager.players != null && GameManager.players.Capacity < Core.maxPlayers.Value)
            {
                GameManager.players.Capacity = Core.maxPlayers.Value;
            }
        }
    }
}