using HoldfastGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using uLink;
using UnityEngine;

namespace DemoMod
{
    public class DemoSpawnStruct
    {
        public string namePrefix = "";
        public FactionCountry factionCountry = FactionCountry.None;
        public PlayerClass playerClass = PlayerClass.None;
        public int uniformId = 0;
        public string group = "all";
    }
    class DemoGameMode
    {
        public static List<DemoSpawnStruct> lineInfantry(int count, FactionCountry fc, string namePrefix = null, int uniformId = 0)
        {
            List<DemoSpawnStruct> result = new List<DemoSpawnStruct>();
            for(int i =0; i<count; ++i)
            {
                result.Add(new DemoSpawnStruct() { namePrefix = namePrefix, factionCountry = fc, playerClass = PlayerClass.ArmyLineInfantry, uniformId= uniformId});
            }
            return result;
        }

        public static List<DemoSpawnStruct> lineInfantryWithAuxiliary(int count, FactionCountry fc, string namePrefix = null, int uniformId = 0)
        {
            List<DemoSpawnStruct> result = new List<DemoSpawnStruct>();
            int auxPosition = count % 2 == 0 ? count / 2 : count / 2 + 1;
            auxPosition = auxPosition / 2 ;
            if(auxPosition < 0)
            {
                auxPosition = 0;
            }
            int i = 0;
            for(i=0; i<auxPosition; ++i)
            {
                result.Add(new DemoSpawnStruct() { namePrefix = namePrefix, factionCountry = fc , playerClass = PlayerClass.ArmyLineInfantry, uniformId = uniformId });
            }
            //result.Add(new DemoSpawnStruct() { namePrefix = namePrefix, factionCountry = fc, playerClass = PlayerClass.Fifer , group= "music"});
            result.Add(new DemoSpawnStruct() { namePrefix = namePrefix, factionCountry = fc, playerClass = PlayerClass.FlagBearer , group = "flag"});
            //result.Add(new DemoSpawnStruct() { namePrefix = namePrefix, factionCountry = fc, playerClass = PlayerClass.Drummer, group = "music" });
            i += 1;
            for(; i<count; ++i)
            {
                result.Add(new DemoSpawnStruct() { namePrefix = namePrefix, factionCountry = fc, playerClass = PlayerClass.ArmyLineInfantry, uniformId = uniformId });
            }
            return result;
        }

        public static List<DemoSpawnStruct> guards(int count,  FactionCountry fc ,string namePrefix = null, int uniformId = 0)
        {
            List<DemoSpawnStruct> result = new List<DemoSpawnStruct>();
            for (int i = 0; i < count; ++i)
            {
                result.Add(new DemoSpawnStruct() { namePrefix = namePrefix, factionCountry = fc, playerClass = PlayerClass.Guard, uniformId = uniformId });
            }
            return result;
        }
        public static List<DemoSpawnStruct> grenadiers(int count, FactionCountry fc, string namePrefix = null, int uniformId = 0)
        {
            List<DemoSpawnStruct> result = new List<DemoSpawnStruct>();
            for (int i = 0; i < count; ++i)
            {
                result.Add(new DemoSpawnStruct() { namePrefix = namePrefix, factionCountry = fc, playerClass = PlayerClass.Grenadier, uniformId = uniformId });
            }
            return result;
        }


        public static void spdm_UpdatePlayerHealth_revivePolicy(RoundPlayer player, PlayerHealthChangedEventArgs playerHealthChangedEventArgs) //秽土转生
        {
            if (!EnhancedBots.slaveOwnerDictionary.ContainsKey(player.NetworkPlayerID))
            {
                return;
            }
            if (playerHealthChangedEventArgs.WasPlayerKilled)
            {
                LinkedList<SlavePlayer> list = EnhancedBots.slaveOwnerDictionary[player.NetworkPlayerID];
                SlavePlayer s_alive = null;
                foreach(var slave in list)
                {
                    if (!slave.isAlive)
                    {
                        continue;
                    }
                    s_alive = slave;
                }
                if(s_alive != null)
                {
                    EnhancedBots.serverBannedPlayersManager.SlayPlayer(player.NetworkPlayerID, s_alive.playerId, "revive owner");
                    EnhancedBots.serverCarbonPlayersManager.StartCoroutine(waitRevive(1f, player));
                }
            }
        }
   
        public static void pdm_ProcessPlayerHealthChanged_horsePolicy(RoundPlayer player, PlayerHealthChangedEventArgs args) //Horse rule
        {
            if (args.ChangeReason != PlayerHealthChangedReason.HitByMeleeWeapon && args.ChangeReason != PlayerHealthChangedReason.HitByMeleeSecondaryAttack)
            {
                return;
            }
            HitByMeleeWeaponPlayerHealthChangedData hitByMeleeWeaponPlayerHealthChangedData = (HitByMeleeWeaponPlayerHealthChangedData)args.Packet.HealthChangedData;
            RoundPlayer roundPlayer = EnhancedBots.serverRoundPlayerManager.ResolveRoundPlayer(hitByMeleeWeaponPlayerHealthChangedData.AttackingPlayerID);
            if (roundPlayer == null) { return; }
            if (roundPlayer.PlayerStartData.ClassType != PlayerClass.Hussar && roundPlayer.PlayerStartData.ClassType != PlayerClass.CuirassierDragoon) { return; }

            try
            {

                int friendly = 0;
                Collider[] hitColliders = Physics.OverlapSphere(roundPlayer.PlayerTransform.position, 16, 1 << 11);
                foreach (Collider co in hitColliders)
                {
                    FactionCountry targetFaction = co.GetComponent<RigidbodyCharacter>().GetComponent<PlayerBase>().PlayerStartData.Faction;
                    if (targetFaction == roundPlayer.PlayerBase.PlayerStartData.Faction)
                    {
                        friendly += 1;
                    }
                }
                if (friendly < 2)
                {
                    EnhancedBots.serverCarbonPlayersManager.StartCoroutine(waitRevive(0.1f, player));
                    EnhancedBots.serverBannedPlayersManager.SlayPlayer(0, roundPlayer.NetworkPlayerID, "You kill out of range!");
                }
                //Debug.Log(string.Format("Demo: {0} friendly", friendly));

            }
            catch(Exception ex)
            {
                Debug.Log("Demo: exception: " + ex.ToString());
            }

        }

        public static bool gcp_ExecuteInput_pre(GameConsolePanel __instance, string input, int adminID)
        {
            object[] arguments;
            string text = __instance.ParseCommand(input.Trim(), out arguments);
            if(text == "dm")
            {
                string [] args = arguments.Cast<string>().ToArray<string>();
                if(args.Length == 0) { return false; }
                switch (args[0])
                {
                    case "MAX_SLAVE":
                        {
                            if(args.Length < 3) { break; }
                            PlayerClass ownerClass;
                            int value;
                            if(!HomelessMethods.TryParseEnum<PlayerClass>(args[1], out ownerClass)) { break; }
                            if(!int.TryParse(args[2], out value)) { break; }
                            if (DemoLoginUser.playerClassSlaveThreshold.ContainsKey(ownerClass))
                            {
                                DemoLoginUser.playerClassSlaveThreshold[ownerClass] = value;
                            }
                            else
                            {
                                DemoLoginUser.playerClassSlaveThreshold.Add(ownerClass, value);
                            }
                            Debug.Log("Demo: MAX_SLAVE updated!");
                            break;
                        }
                    case "enable":
                        {
                            break;
                        }
                    case "disable":
                        {
                            break;
                        }
                }
                return false;
            }
            return true;
        }

        private static IEnumerator waitRevive(float seconds, RoundPlayer player)
        {
            yield return new WaitForSeconds(seconds);
            EnhancedBots.serverBannedPlayersManager.RevivePlayer(player.NetworkPlayerID, player.NetworkPlayerID, "revive");
            yield break;
        }

        private static IEnumerator waitSlay(float seconds, RoundPlayer player)
        {
            yield return new WaitForSeconds(seconds);
            EnhancedBots.serverBannedPlayersManager.SlayPlayer(player.NetworkPlayerID, player.NetworkPlayerID, "revive");
            yield break;
        }



    }

}
