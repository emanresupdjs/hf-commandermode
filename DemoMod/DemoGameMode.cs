using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoldfastGame;


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
                    EnhancedBots.serverBannedPlayersManager.RevivePlayer(player.NetworkPlayerID, player.NetworkPlayerID, "revive owner");
                }
            }
        }
    }

}
