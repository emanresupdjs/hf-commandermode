using System;
using BepInEx;
using HarmonyLib;
using HoldfastGame;
using uLink;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DemoMod
{
    [BepInPlugin(pluginGuid, pluginName, version)]
    public class Demomod : BaseUnityPlugin
    {
        public const String pluginGuid = "home.5dsj.plugins.DemoMod";
        public const String version = "1.0.0.0";
        public const String pluginName = "DemoMod";
        void Awake()
        {
            String msg = "Start Loading DemoMOd";
            UnityEngine.Debug.Log(msg);
            var harmony = new Harmony(pluginGuid);

            //var scpcc_Execute_o = AccessTools.Method(typeof(HoldfastGame.ServerCarbonPlayersConsoleCommand), "Execute");
            //var scpcc_Execute_pre = AccessTools.Method(typeof(EnhancedBots), "scpcc_Execute_pre");
            //var scpcc_Execute_post = AccessTools.Method(typeof(EnhancedBots), "scpcc_Execute_post");
            //harmony.Patch(scpcc_Execute_o, prefix: new HarmonyMethod(scpcc_Execute_pre) , postfix: new HarmonyMethod(scpcc_Execute_post));

            var scpm_AddCarbonPlayers_o = AccessTools.Method(typeof(ServerCarbonPlayersManager), "AddCarbonPlayers", new[] { typeof(FactionCountry), typeof(PlayerClass) });
            var scpm_AddCarbonPlayers_pre = AccessTools.Method(typeof(EnhancedBots), "scpm_AddCarbonPlayers_specific_pre");
            var scpm_AddCarbonPlayers_post = AccessTools.Method(typeof(EnhancedBots), "scpm_AddCarbonPlayers_specific_post");
            harmony.Patch(scpm_AddCarbonPlayers_o, prefix: new HarmonyMethod(scpm_AddCarbonPlayers_pre), postfix: new HarmonyMethod(scpm_AddCarbonPlayers_post));

            var scpm_InitiateCarbonPlayer_o = AccessTools.Method(typeof(ServerCarbonPlayersManager), "InitiateCarbonPlayer");
            var scpm_InitiateCarbonPlayer_pre = AccessTools.Method(typeof(EnhancedBots), "scpm_InitiateCarbonPlayer_pre");
            var scpm_InitiateCarbonPlayer_post = AccessTools.Method(typeof(EnhancedBots), "scpm_InitiateCarbonPlayer_post");
            harmony.Patch(scpm_InitiateCarbonPlayer_o, prefix: new HarmonyMethod(scpm_InitiateCarbonPlayer_pre), postfix: new HarmonyMethod(scpm_InitiateCarbonPlayer_post));

            var scpm_GenerateClientChosenSpawnSettings_o = AccessTools.Method(typeof(ServerCarbonPlayersManager), "GenerateClientChosenSpawnSettings");
            var scpm_GenerateClientChosenSpawnSettings_post = AccessTools.Method(typeof(EnhancedBots), "scpm_GenerateClientChosenSpawnSettings_post");
            harmony.Patch(scpm_GenerateClientChosenSpawnSettings_o,  postfix: new HarmonyMethod(scpm_GenerateClientChosenSpawnSettings_post));

            var scpm_SpawnCarbonPlayer_o = AccessTools.Method(typeof(ServerCarbonPlayersManager), "SpawnCarbonPlayer");
            var scpm_SpawnCarbonPlayer_pre = AccessTools.Method(typeof(EnhancedBots), "scpm_SpawnCarbonPlayer_pre");
            harmony.Patch(scpm_SpawnCarbonPlayer_o, prefix: new HarmonyMethod(scpm_SpawnCarbonPlayer_pre));

            var rtm_Update_o = AccessTools.Method(typeof(RoundTimerManager), "Update");
            var rtm_Update_post = AccessTools.Method(typeof(EnhancedBots), "rtm_Update_post");
            harmony.Patch(rtm_Update_o, postfix: new HarmonyMethod(rtm_Update_post));
            
            var srcam_RequestLogin_o = AccessTools.Method(typeof(ServerRemoteConsoleAccessManager), "RequestLogin");
            var srcam_RequestLogin_pre = AccessTools.Method(typeof(EnhancedRC), "srcam_RequestLogin_pre");
            var srcam_RequestLogin_post = AccessTools.Method(typeof(EnhancedRC), "srcam_RequestLogin_post");
            harmony.Patch(srcam_RequestLogin_o, prefix: new HarmonyMethod(srcam_RequestLogin_pre), postfix: new HarmonyMethod(srcam_RequestLogin_post));

            var srcam_RequestCommandExecute_o = AccessTools.Method(typeof(ServerRemoteConsoleAccessManager), "RequestCommandExecute");
            var srcam_RequestCommandExecute_pre = AccessTools.Method(typeof(EnhancedRC), "srcam_RequestCommandExecute_pre");
            harmony.Patch(srcam_RequestCommandExecute_o, prefix: new HarmonyMethod(srcam_RequestCommandExecute_pre));

            var sch_SendChatMessage_o = AccessTools.Method(typeof(ServerChatHandler), "SendChatMessage");
            var sch_SendChatMessage_pre = AccessTools.Method(typeof(EnhancedRC), "sch_SendChatMessage_pre");
            harmony.Patch(sch_SendChatMessage_o, prefix: new HarmonyMethod(sch_SendChatMessage_pre));


            var spdm_RegisterFiringAction_o = AccessTools.Method(typeof(ServerPlayerDamageManager), "RegisterFiringAction", new[] { typeof(uLinkStrictPlatformerCreator), typeof(OwnerPacketToServer) });
            var spdm_RegisterFiringAction_post = AccessTools.Method(typeof(EnhancedBots), "spdm_RegisterFiringAction_post");
            harmony.Patch(spdm_RegisterFiringAction_o, postfix: new HarmonyMethod(spdm_RegisterFiringAction_post));

            var ulspc_HandleOwnerPacketToServer_o = AccessTools.Method(typeof(uLinkStrictPlatformerCreator), "HandleOwnerPacketToServer");
            var ulspc_HandleOwnerPacketToServer_post = AccessTools.Method(typeof(EnhancedBots), "ulspc_HandleOwnerPacketToServer_post");
            harmony.Patch(ulspc_HandleOwnerPacketToServer_o, postfix: new HarmonyMethod(ulspc_HandleOwnerPacketToServer_post));

            var sgm_ChangeGameMode_o = AccessTools.Method(typeof(ServerGameManager), "ChangeGameMode", new[] { typeof(GameDetails) });
            var sgm_ChangeGameMode_pre = AccessTools.Method(typeof(Demomod), "sgm_ChangeGameMode_pre");
            harmony.Patch(sgm_ChangeGameMode_o, prefix: new HarmonyMethod(sgm_ChangeGameMode_pre));

            var scpm_UpdateCarbonPlayerInput_o = AccessTools.Method(typeof(ServerCarbonPlayersManager), "UpdateCarbonPlayerInput");
            var scpm_UpdateCarbonPlayerInput_pre = AccessTools.Method(typeof(EnhancedBots), "scpm_UpdateCarbonPlayerInput_pre");
            harmony.Patch(scpm_UpdateCarbonPlayerInput_o, prefix: new HarmonyMethod(scpm_UpdateCarbonPlayerInput_pre));

            var scvm_HandleClientVoicePlayRequest_o = AccessTools.Method(typeof(ServerCharacterVoicesManager), "HandleClientVoicePlayRequest");
            var scvm_HandleClientVoicePlayRequest_post = AccessTools.Method(typeof(EnhancedBots), "scvm_HandleClientVoicePlayRequest_post");
            harmony.Patch(scvm_HandleClientVoicePlayRequest_o, postfix: new HarmonyMethod(scvm_HandleClientVoicePlayRequest_post));

            var scm_uLink_OnPlayerDisconnected_o = AccessTools.Method(typeof(ServerConnectionManager), "uLink_OnPlayerDisconnected");
            var scm_uLink_OnPlayerDisconnected_pre = AccessTools.Method(typeof(Demomod), "scm_uLink_OnPlayerDisconnected_pre");
            harmony.Patch(scm_uLink_OnPlayerDisconnected_o, prefix: new HarmonyMethod(scm_uLink_OnPlayerDisconnected_pre));

            var scpm_FixedUpdate_o = AccessTools.Method(typeof(ServerCarbonPlayersManager), "FixedUpdate");
            var scpm_FixedUpdate_post = AccessTools.Method(typeof(EnhancedBots), "scpm_FixedUpdate_post");
            harmony.Patch(scpm_FixedUpdate_o, postfix: new HarmonyMethod(scpm_FixedUpdate_post));

            var soom_HandleRequestStartOfficerOrder_o = AccessTools.Method(typeof(ServerOfficerOrderManager), "HandleRequestStartOfficerOrder");
            var soom_HandleRequestStartOfficerOrder_post = AccessTools.Method(typeof(EnhancedBots), "soom_HandleRequestStartOfficerOrder_post");
            harmony.Patch(soom_HandleRequestStartOfficerOrder_o, postfix: new HarmonyMethod(soom_HandleRequestStartOfficerOrder_post));


            var spdm_UpdatePlayerHealth_revivePolicy_o = AccessTools.Method(typeof(ServerPlayerDamageManager), "_UpdatePlayerHealth");
            var spdm_UpdatePlayerHealth_revivePolicy = AccessTools.Method(typeof(DemoGameMode), "spdm_UpdatePlayerHealth_revivePolicy");
            harmony.Patch(spdm_UpdatePlayerHealth_revivePolicy_o, postfix: new HarmonyMethod(spdm_UpdatePlayerHealth_revivePolicy));


            var csnuo_CreateProxyPacket_o = AccessTools.Method(typeof(CharacterServerNetworkUpdateableObject), "CreateProxyPacket");
            var csnuo_CreateProxyPacket_post = AccessTools.Method(typeof(Demomod), "csnuo_CreateProxyPacket_post");
            //harmony.Patch(csnuo_CreateProxyPacket_o, postfix: new HarmonyMethod(csnuo_CreateProxyPacket_post));


            msg = "DemoMod success loaded!";
            UnityEngine.Debug.Log(msg);

        }



        public static void sgm_ChangeGameMode_pre(GameDetails gameDetails)
        {
            foreach (var entry in EnhancedRC.loggedOnPlayers)
            {
                var user = entry.Value;
                user.slaveOwned = 0;
                user.botsAdded = false;
            }
            EnhancedBots.sgm_ChangeGameMode_pre(gameDetails);
        } //On mapRotation

        public static void scm_uLink_OnPlayerDisconnected_pre(NetworkPlayer networkPlayer)
        {
            EnhancedBots.scm_uLink_OnPlayerDisconnected_pre(networkPlayer);
            EnhancedRC.scm_uLink_OnPlayerDisconnected_pre(networkPlayer);
        }

        public static void csnuo_CreateProxyPacket_post(ServerPacketToProxy __result)
        {
            if (!EnhancedRC.loggedOnPlayerIDs.Contains(__result.PlayerID))
            {
                return;
            }
            Debug.Log(string.Format("demo :    timestamp: {0}\n  Position: {1}\n    Pitch: {2}\n    Yaw: {3}\n    rotaitionY: {4}\n", __result.Timestamp, __result.Position, __result.Pitch, __result.Yaw, __result.RotationYCompressed));
        }
    }

}
