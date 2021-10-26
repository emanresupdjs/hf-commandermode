using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoldfastGame;
using System.Collections;
using uLink;
using HarmonyLib;

namespace DemoMod
{
    class DemoLoginUser
    {
        public int playerId = -1;
        public int slaveOwned = -1;
        public bool isServerLogon = false;
        public bool botsAdded = false;
        public static int MAX_DEFAULT = 20;
        public static Dictionary<PlayerClass, int> playerClassSlaveThreshold = new Dictionary<PlayerClass, int> { 
            {PlayerClass.ArmyInfantryOfficer, 20} };
    }
    class EnhancedRC //ServerRemoteConsoleAccessManager
    {
        public static string passwd = "321";
        public static HashSet<int> loggedOnPlayerIDs = new HashSet<int>();
        public static Dictionary<int, DemoLoginUser> loggedOnPlayers = new Dictionary<int, DemoLoginUser>();
        public static ServerGameManager serverGameManager = null;
        public static ServerRoundPlayerManager serverRoundPlayerManager = null;
        public static ServerWeaponHolderManager serverWeaponHolderManager = null;
        public static NetworkView networkView = null;
        private static bool modIsEnabled = true;
        public static bool srcam_RequestLogin_pre(ServerRemoteConsoleAccessManager __instance, string password, NetworkMessageInfo messageInfo)
        {
            ServerComponentReferenceManager serverInstance = ServerComponentReferenceManager.ServerInstance;
            serverGameManager = serverInstance.serverGameManager;
            serverRoundPlayerManager = serverInstance.serverRoundPlayerManager;
            serverWeaponHolderManager = serverInstance.serverWeaponHolderManager;
            if (password != passwd)
            {
                return true;
            }
            string[] list = { "none"};
            NetworkPlayer sender = messageInfo.sender;
            int id = sender.id;
            loggedOnPlayerIDs.Add(id);
            loggedOnPlayers.Add(id, new DemoLoginUser()
            {
                playerId = id,
                slaveOwned = 0
            }) ;
            if(networkView == null)
            {
                 networkView = Traverse.Create(__instance).Field("_networkView").GetValue() as NetworkView;
            }
            networkView.RPC("RemoteConsoleLoginAck", sender, new object[] { 
            password,
            true,
            list
            });
            return false;
            
        }
        public static void srcam_RequestLogin_post(ServerRemoteConsoleAccessManager __instance, HashSet<int> ___loggedOnPlayerIDs)
        {
           //Server logon admins
            foreach(var id in ___loggedOnPlayerIDs)
            {
                if (!loggedOnPlayerIDs.Contains(id))
                {
                    loggedOnPlayerIDs.Add(id);
                    loggedOnPlayers.Add(id, new DemoLoginUser()
                    {
                        playerId = id,
                        slaveOwned = 0,
                        isServerLogon = true
                    }) ;
                }
                else
                {
                    loggedOnPlayers[id].isServerLogon = true;
                }
            }
            if (networkView == null)
            {
                networkView = Traverse.Create(__instance).Field("_networkView").GetValue() as NetworkView;
            }


        }

        public static bool srcam_RequestCommandExecute_pre(string consoleCommand, NetworkMessageInfo messageInfo)
        {
            try
            {
                NetworkPlayer sender = messageInfo.sender;
                int id = sender.id;
                string text = consoleCommand.Trim();
                if (!isModLoggedOn(id))
                {
                    return true;
                }
                if (loggedOnPlayers[id].isServerLogon)
                {
                    if (text.Contains("dm "))
                    {
                        text = text.Substring(3);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
                UnityEngine.Debug.Log(string.Format("demo: get command from id {0} {1}", id, text) );
                string output = handleCommand(text, sender, loggedOnPlayers[id].isServerLogon);
                RemoteConsoleCommandOuput(text, output, sender);
            }
            catch (Exception exception)
            {
                RPCExceptionLogger.LogRPCException(exception);
                return false;
            }
            return false;
        }

        public static bool sch_SendChatMessage_pre(ServerChatHandler __instance, string entryText, int textChannelID, NetworkMessageInfo messageInfo)
        {
            if (!entryText.StartsWith("dm "))
            {
                return true;
            }

            entryText = entryText.Substring(3);
            try
            {
                NetworkPlayer sender = messageInfo.sender;
                int id = sender.id;
                if (!loggedOnPlayerIDs.Contains(id))
                {
                    loggedOnPlayerIDs.Add(id);
                    loggedOnPlayers.Add(id, new DemoLoginUser()
                    {
                        playerId = id,
                        slaveOwned = 0,
                        isServerLogon = false
                    }) ;
                }
                ServerComponentReferenceManager serverInstance = ServerComponentReferenceManager.ServerInstance;
                serverRoundPlayerManager = serverInstance.serverRoundPlayerManager;
                RoundPlayer roundPlayer = serverRoundPlayerManager.ResolveRoundPlayer(sender);
                if (roundPlayer != null)
                {
                    string output = handleCommand(entryText, sender, false);
                    ChatEntry chatEntry = ComponentReferenceManager.genericObjectPools.chatEntry.Obtain();
                    chatEntry.PlayerID = roundPlayer.NetworkPlayerID;
                    chatEntry.Text = output;
                    chatEntry.TextChannel = (TextChatChannel)textChannelID;
                    dfList<NetworkPlayer> dfList = dfList<NetworkPlayer>.Obtain();
                    dfList.Add(sender);
                    if (networkView == null)
                    {
                        networkView = Traverse.Create(__instance).Field("_networkView").GetValue() as NetworkView;
                    }
                    networkView.RPC<ChatEntry>("BroadcastChatMessage", dfList, chatEntry);
                    dfList.Release();
                    ComponentReferenceManager.genericObjectPools.chatEntry.Release(chatEntry);
                }
                return false;
            }
            catch (Exception exception)
            {
                RPCExceptionLogger.LogRPCException(exception);
                return false;
            }
        }

        public static void scm_uLink_OnPlayerDisconnected_pre(NetworkPlayer networkPlayer)
        {
            EnhancedRC.loggedOnPlayers.Remove(networkPlayer.id);
            EnhancedRC.loggedOnPlayerIDs.Remove(networkPlayer.id);
        }

        private static bool isModLoggedOn(int playerId)
        {
            return loggedOnPlayerIDs.Contains(playerId);
        }
        private static void RemoteConsoleCommandOuput(string command, string msg, NetworkPlayer sender)
        {
            if (networkView == null)
            {
                return;
            }
            networkView.RPC("RemoteConsoleCommandOuput", sender, new object[] {
                command,
                msg,
                true,
                true});
        }
        private static string handleCommand(string command, NetworkPlayer sender, bool isServerLogon = false)
        {
            string[] arguments = command.Split(' ');
            switch (arguments[0])
            {
                case "addBots":
                    {
                        if (!modIsEnabled)
                        {
                            return "Mod is disabled.";
                        }
                        if (loggedOnPlayers[sender.id].botsAdded) { return "You can only addBots once!"; }
                        if (arguments.Length < 3) { return "usage: addBots <template> <count> [<name-prefix> <uniformId>]"; }
                        int template ;
                        int count;
                        PlayerClass pClass, ownerClass;
                        FactionCountry fCountry;
                        
                        try
                        {
                            if (!int.TryParse(arguments[1], out template) && template < 0) { return "templateId must > 0"; }
                            if(int.TryParse(arguments[2], out count) && count < 1) { return "count must > 1"; }
                            fCountry = serverRoundPlayerManager.ResolveServerRoundPlayer(sender.id).SpawnData.Faction;
                            if (fCountry == FactionCountry.None) { return "you need to spawn first!"; }
                            ownerClass = serverRoundPlayerManager.ResolveServerRoundPlayer(sender.id).SpawnData.ClassType;
                            int max_slave = DemoLoginUser.playerClassSlaveThreshold.ContainsKey(ownerClass) ? DemoLoginUser.playerClassSlaveThreshold[ownerClass] : DemoLoginUser.MAX_DEFAULT;
                            count =  count +loggedOnPlayers[sender.id].slaveOwned > max_slave ? max_slave - loggedOnPlayers[sender.id].slaveOwned : count;
                            pClass = HomelessMethods.ParseEnum<PlayerClass>(arguments[2].ToString());
                            string namePrefix;
                            int uniformId;
                            if (arguments.Length == 5)
                            {
                                namePrefix = arguments[3].Trim();
                                namePrefix = namePrefix.Substring(0, namePrefix.Length > 10? 10: namePrefix.Length);
                                uniformId = 0;
                                int.TryParse(arguments[4].Trim(), out uniformId);
                            }
                            else
                            {
                                namePrefix = sender.id.ToString();
                                uniformId = 0;
                            }
                            List<DemoSpawnStruct> spawns;
                            switch (template)
                            {
                                case 0:
                                    spawns = DemoGameMode.lineInfantry(count: count, fc: fCountry, namePrefix: namePrefix, uniformId: uniformId);
                                    break;
                                case 1:
                                    spawns = DemoGameMode.lineInfantryWithAuxiliary(count: count, fc: fCountry, namePrefix: namePrefix, uniformId: uniformId);
                                    break;
                                case 2:
                                    spawns = DemoGameMode.guards(count: count, fc: fCountry, namePrefix: namePrefix, uniformId: uniformId);
                                    break;
                                case 3:
                                    spawns = DemoGameMode.grenadiers(count: count, fc: fCountry, namePrefix: namePrefix, uniformId: uniformId);
                                    break;
                                default:
                                    spawns = DemoGameMode.lineInfantry(count: count, fc: fCountry, namePrefix: namePrefix, uniformId: uniformId);
                                    break;
                            }
                            EnhancedBots.addBots(sender.id, spawns);
                            loggedOnPlayers[sender.id].slaveOwned += count;
                            loggedOnPlayers[sender.id].botsAdded = true;
                            return string.Format("success add {1} bots for {0}",sender.id, count);

                        }
                        catch(Exception ex)
                        {
                            return "Error parse FactionCountry or PlayerClass." + ex.ToString();
                        }
                    }
                case "add":
                    {
                        if(!isServerLogon)
                        {
                            return "not allowed";
                        }
                        if (arguments.Length < 6) { return "usage: add <count> <FactionCountry> <PlayerClass> <namePrefix> <uniformId>"; }
                        int count;
                        PlayerClass pClass;
                        FactionCountry fCountry;
                        List<DemoSpawnStruct> spawns = new List<DemoSpawnStruct>();
                        try
                        {
                            if (!int.TryParse(arguments[1], out count) && count < 1) { return "count must > 0"; }
                            fCountry = HomelessMethods.ParseEnum<FactionCountry>(arguments[2].ToString());
                            pClass = HomelessMethods.ParseEnum<PlayerClass>(arguments[3].ToString());
                            for(int i = 0; i< count; ++i)
                            {
                                spawns.Add(new DemoSpawnStruct() { namePrefix=arguments[4], factionCountry = fCountry, playerClass= pClass, uniformId= int.Parse(arguments[5])});
                            }
                        }
                        catch
                        {
                            return "Error parse FactionCountry or PlayerClass.";
                        }

                        return EnhancedBots.addBots(sender.id, spawns);

                    }
                case "removeBots":
                    {
                        if(!isServerLogon)
                        {
                            return "not allowed!";
                        }
                        if(arguments.Length< 2)
                        {
                            loggedOnPlayers[sender.id].slaveOwned = 0;
                            loggedOnPlayers[sender.id].botsAdded = false;
                            return EnhancedBots.removeBots(sender.id);
                        }
                        else
                        {
                            int id;
                            if(int.TryParse(arguments[1].Trim(), out id))
                            {
                                loggedOnPlayers[id].slaveOwned = 0;
                                loggedOnPlayers[id].botsAdded = false;
                                return EnhancedBots.removeBots(id);
                            }
                            return "No such player";
                        }
                    }
                case "formLine":
                    {
                        if (isServerLogon)
                            return EnhancedBots.formLine(sender.id, "all");
                        else
                            return "not allowed!";
                    }
                case "do":
                    {
                        string action;
                        string group;
                        if (arguments.Length < 3) { return "usage: do <group> <PlayerActions> [<additional>]"; }

                        group = arguments[1].Trim();
                        action = arguments[2].Trim();

                        if(arguments.Length == 4)
                        {
                            return EnhancedBots.action(sender.id, group, action, arguments[3].Trim());
                        }
                        return EnhancedBots.action(sender.id, group, action);
                    }
                case "do2":
                    {
                        string action;
                        string sId;
                        if (arguments.Length < 3) { return "usage: do <id> <PlayerActions> [<additional>]"; }

                        sId = arguments[1].Trim();
                        action = arguments[2].Trim();
                        int slaveId;
                        if (!int.TryParse(sId, out slaveId)) { return "Invalid slaveId."; };
                        //if (arguments.Length == 4)
                        //{
                        //    return EnhancedBots.action_n(,  action, arguments[3].Trim());
                        //}
                        //return EnhancedBots.action_n(slave, action);
                        return "";
                    }
                case "dump":
                    {
                        if (arguments.Length < 2) { return "usage: dump <id>"; }
                        try
                        {
                            SlavePlayer slave = EnhancedBots.slavePlayerDictionary[int.Parse(arguments[1])];
                            string result = string.Format("id: {0} |owner: {1} | isAiming: {2} | isAlive: {3} | isCrouching: {4} | aiming: {5}", slave.playerId, slave.ownerId, slave.isAiming, slave.isAlive, slave.isCrouching, slave.currentAimForward.ToString());
                            return result;
                        }
                        catch
                        {
                            return "error";
                        }
                        
                    }
                case "disable":
                    {
                        if (isServerLogon)
                        {
                            foreach(var k in loggedOnPlayers.Keys)
                            {
                                loggedOnPlayers[k].slaveOwned = 0;
                                EnhancedBots.removeBots(k);
                            }
                            return (modIsEnabled = false).ToString();
                        }
                        else
                            return "not allowed!";
                    }
                case "enable":
                    {
                        if (isServerLogon)
                            return (modIsEnabled = true).ToString();
                        else
                            return "not allowed!";
                    }
                case "set":
                    {
                        if (!isServerLogon)
                        {
                            return "not allowed!";
                        }
                        if (arguments.Length < 3) { return "usage: set <variable> <value> [<additional>]"; }
                        try
                        {
                            string varName = arguments[1].Trim();
                            if (varName == "MAX_SLAVE")
                            {
                                if (arguments.Length < 4) { return "rc dm set MAX_SLAVE <PlayerClass> <Count>"; }
                                PlayerClass ownerClass;
                                int value;
                                if (!HomelessMethods.TryParseEnum<PlayerClass>(arguments[2], out ownerClass)) { return "rc dm set MAX_SLAVE <PlayerClass> <Count>"; }
                                if (!int.TryParse(arguments[3], out value)) { return "rc dm set MAX_SLAVE <PlayerClass> <Count>"; }
                                if (DemoLoginUser.playerClassSlaveThreshold.ContainsKey(ownerClass))
                                {
                                    DemoLoginUser.playerClassSlaveThreshold[ownerClass] = value;
                                }
                                else
                                {
                                    DemoLoginUser.playerClassSlaveThreshold.Add(ownerClass, value);
                                }
                            }
                            else if(varName == "MAX_DEFAULT")
                            {
                                int value;
                                if (!int.TryParse(arguments[2], out value))
                                {
                                    return "error";
                                }
                                DemoLoginUser.MAX_DEFAULT = value;
                            }
                        }
                        catch (Exception ex) { return "error parsing variable"+ex; }
                        return "ok";
                    }
                case "weapon":
                    {
                        WeaponType t = WeaponType.Musket_SeaServiceBrownBess;
                        HomelessMethods.TryParseEnum<WeaponType>(arguments[1], out t);
                        if(serverWeaponHolderManager == null)
                        {
                            ServerComponentReferenceManager serverInstance = ServerComponentReferenceManager.ServerInstance;
                            serverWeaponHolderManager = serverInstance.serverWeaponHolderManager;
                        }
                        serverWeaponHolderManager.BroadcastSwitchWeapon(sender.id, t);
                        break;
                    }
                case "help":
                    {
                        return @"在聊天频道内输入指令以操作Mod (作者: [RGF] 我的手机)
dm addBots < 模板 > < 数量 > [<名字前缀> <制服Id>] 每人每局仅调用一次
例如: dm addBots 0 10 //十个线列兵
dm addBots 0 10 [RGF|3rd] 0 //十个名字以RGF开头的bot，制服ID为0

按 B 键 跟随
按 N 键 左右两边形成线列

在V键菜单中操作Bot
Shoulder Arms: 在右手边形成单排线列
Stand Ground: 在右手边形成双排线列(建议先形成单排线列再双排)
Load: 装填
Unfix Bayonet: 卸载刺刀
Present Arms: 瞄准";
                    }
            }
           

            return "ok";
        }

    }
}

