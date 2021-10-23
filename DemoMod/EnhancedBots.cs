using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using uLink;
using HoldfastGame;
using System.Reflection;
using UnityEngine;

namespace DemoMod
{
    public class ForceFacingDirection
    {
        public int playerId;
        public UnityEngine.Vector3 facingDirection;
        public double stopRotatingTime;
    }
    public class SlavePlayer
    {
        public int playerId;
        public int ownerId;
        public FactionCountry factionCountry;
        public PlayerClass playerClass;
        public Vector3 currentAimForward;
        public bool follow = false;
        public bool charge = false;
        public int index = -1;
        public bool isAiming= false;
        public bool isCrouching = false;
        public bool isAlive = true;
        public bool isAimingExact = true;
        public bool isFiringAtWill = false;
        public bool isReloading = false;
        public bool hasAmmo = true;
        public string group = "all";
    }
   
    public class DemoTransform
    {
        public Vector3 position;
        public Vector3 forward;
        public float lastDistance = 9999f;
        public Vector3 lastPosition = Vector3.zero;
        public bool isCollistion = false;
        public bool checkDistanceBlocking = true;
    }

    public class DemoInfantryLine
    {
        public RoundPlayer ownerRoundPlayer;
        public LinkedList<SlavePlayer>[] lines;                    //每个位置存放当前位置的SlavePlayer(Store slavePlayer in line)
        public List<DemoTransform>[] linesTransform;
        public bool isDoubleRow = false;
        public int oneRow = 0;
        public bool doUpdate = false;
        public DemoInfantryLine(bool _isDoubleRow, int _oneRow, RoundPlayer _ownerRoundPlayer)
        {
            //Debug.Log("demo: dump args");
            //Debug.Log(string.Format("demo: _isDoubleRow: {0}, oneRow: {1}, ownerPos: {2}", _isDoubleRow, oneRow, _ownerRoundPlayer.PlayerTransform.position.ToString()));
            this.lines  = new LinkedList<SlavePlayer>[2];
            this.linesTransform = new List<DemoTransform>[2];
            this.isDoubleRow = _isDoubleRow;
            this.oneRow = _oneRow;
            this.ownerRoundPlayer = _ownerRoundPlayer;
            if (!_isDoubleRow)
            {
                this.lines[0] = new LinkedList<SlavePlayer>();
                this.lines[1] = new LinkedList<SlavePlayer>();
                this.linesTransform[0] = new List<DemoTransform>();
                this.linesTransform[1] = new List<DemoTransform>();
                Vector3 tmp = _ownerRoundPlayer.PlayerTransform.position;
                Vector3 ownerPosition = new Vector3(tmp.x, tmp.y, tmp.z);
                Vector3 ownerRight = _ownerRoundPlayer.PlayerTransform.right * 0.62f;
                for (int idx =0; idx< _oneRow; ++idx)
                {
                    Vector3 targetPosition = ownerPosition + (ownerRight *(idx+1));
                    Vector3 ownerForwardTmp = _ownerRoundPlayer.PlayerTransform.forward;
                    Vector3 targetForward = new Vector3(ownerForwardTmp.x, ownerForwardTmp.y, ownerForwardTmp.z);
                    this.linesTransform[0].Add(new DemoTransform() { position = targetPosition, forward = targetForward  });
                }

            }
            else //双排线列
            {
                this.lines[0] = new LinkedList<SlavePlayer>();
                this.lines[1] = new LinkedList<SlavePlayer>();
                this.linesTransform[0] = new List<DemoTransform>();
                this.linesTransform[1] = new List<DemoTransform>();
                Vector3 ownerPosition = new Vector3(_ownerRoundPlayer.PlayerTransform.position.x, _ownerRoundPlayer.PlayerTransform.position.y, _ownerRoundPlayer.PlayerTransform.position.z);
                Vector3 ownerRight = _ownerRoundPlayer.PlayerTransform.right * 0.66f;
                Vector3 ownerForward = new Vector3(_ownerRoundPlayer.PlayerTransform.forward.x, _ownerRoundPlayer.PlayerTransform.forward.y, _ownerRoundPlayer.PlayerTransform.forward.z) ;
                ownerForward.Normalize();
                for(int idx = 0; idx< _oneRow; ++idx)
                {
                    Vector3 l0_targetPosition = ownerPosition + (ownerRight * (idx + 1));
                    Vector3 l1_targetPosition = ownerPosition + (0.5f * ownerForward) + (ownerRight * (idx + 1));
                    Vector3 targetForward = ownerForward;
                    this.linesTransform[0].Add(new DemoTransform() { position = l0_targetPosition, forward = targetForward, checkDistanceBlocking = false });
                    this.linesTransform[1].Add(new DemoTransform() { position = l1_targetPosition, forward = targetForward, checkDistanceBlocking = false });

                }
            }
        }
   
        public void updateTransform()
        {
            int _oneRow = this.lines[0].Count + this.lines[1].Count;
            _oneRow = this.isDoubleRow? (_oneRow % 2 == 0 ? _oneRow / 2 : _oneRow / 2 + 1) : (_oneRow);
            this.oneRow = _oneRow;
            if (!this.isDoubleRow)
            {
                this.lines[0] = new LinkedList<SlavePlayer>();
                this.lines[1] = new LinkedList<SlavePlayer>();
                this.linesTransform[0] = new List<DemoTransform>();
                this.linesTransform[1] = new List<DemoTransform>();
                Vector3 ownerPosition = new Vector3(this.ownerRoundPlayer.PlayerTransform.position.x, this.ownerRoundPlayer.PlayerTransform.position.y, this.ownerRoundPlayer.PlayerTransform.position.z);
                Vector3 ownerRight = this.ownerRoundPlayer.PlayerTransform.right * 0.62f;
                for (int idx = 0; idx < _oneRow; ++idx)
                {
                    Vector3 targetPosition = ownerPosition + (ownerRight * (idx + 1));
                    Vector3 targetForward = new Vector3(this.ownerRoundPlayer.PlayerTransform.forward.x, this.ownerRoundPlayer.PlayerTransform.forward.y, this.ownerRoundPlayer.PlayerTransform.forward.z);
                    this.linesTransform[0].Add(new DemoTransform() { position = targetPosition, forward = targetForward });
                }

            }
            else //双排线列(Double rank)
            {
                this.lines[0] = new LinkedList<SlavePlayer>();
                this.lines[1] = new LinkedList<SlavePlayer>();
                this.linesTransform[0] = new List<DemoTransform>();
                this.linesTransform[1] = new List<DemoTransform>();
                Vector3 ownerPosition = new Vector3(this.ownerRoundPlayer.PlayerTransform.position.x, this.ownerRoundPlayer.PlayerTransform.position.y, this.ownerRoundPlayer.PlayerTransform.position.z);
                Vector3 ownerRight = this.ownerRoundPlayer.PlayerTransform.right * 0.66f;
                Vector3 ownerForward = new Vector3(this.ownerRoundPlayer.PlayerTransform.forward.x, this.ownerRoundPlayer.PlayerTransform.forward.y, this.ownerRoundPlayer.PlayerTransform.forward.z);
                ownerForward.Normalize();
                for (int idx = 0; idx < _oneRow; ++idx)
                {
                    Vector3 l0_targetPosition = ownerPosition + (ownerRight * (idx + 1));
                    Vector3 l1_targetPosition = ownerPosition + (0.5f * ownerForward) + (ownerRight * (idx + 1));
                    Vector3 targetForward = ownerForward;
                    this.linesTransform[0].Add(new DemoTransform() { position = l0_targetPosition, forward = targetForward, checkDistanceBlocking = false });
                    this.linesTransform[1].Add(new DemoTransform() { position = l1_targetPosition, forward = targetForward, checkDistanceBlocking = false });

                }
            }
        }
    }


    public class EnhancedBots
    {
        public static Dictionary<int, LinkedList<SlavePlayer>> slaveOwnerDictionary = new Dictionary<int, LinkedList<SlavePlayer>>();
        public static Dictionary<int, DemoInfantryLine> slaveOwnerInfantryLine = new Dictionary<int, DemoInfantryLine>();
        public static Dictionary<int, SlavePlayer> slavePlayerDictionary = new Dictionary<int, SlavePlayer>();
        public static Dictionary<int, Queue<DemoTransform>> slavePlayerTargetTransforms = new Dictionary<int, Queue<DemoTransform>>();
        public static List<ForceFacingDirection> faceDirections = new List<ForceFacingDirection>();
        public static bool inuse = false;
        public static int currentRequestOwner = -1;
        public static int currentSpawnIndex = -1;
        public static List<DemoSpawnStruct> currentSpawns = null;
        public static ServerRoundPlayerManager serverRoundPlayerManager = null;
        public static ServerCarbonPlayersManager serverCarbonPlayersManager = null;
        public static ServerRoundTimerManager serverRoundTimerManager = null;
        public static ServerBannedPlayersManager serverBannedPlayersManager = null;
        public static ServerGameManager serverGameManager = null;
        public static ServerWeaponHolderManager serverWeaponHolderManager = null;
        public static SpawnSectionManager spawnSectionManager = null;
        public static Dictionary<int, CarbonPlayerRepresentation> carbonPlayers = null; //Game internal refs
        public static dfList<int> carbonPlayerIDs = null;  //Game internal refs
        private static MethodInfo ref_SpawnCarbonPlayer = null;
        private static MethodInfo ref_IsPlayerActionMeleeBlock = null;
        private static MethodInfo ref_UpdateCarbonPlayerInput = null;


        public static bool scpm_AddCarbonPlayers_specific_pre(FactionCountry factionCountry, PlayerClass playerClass) //ServerCarbonPlayersManager
        {
            SlavePlayer target = new SlavePlayer()
            {
                playerId = -1,
                ownerId = currentRequestOwner,
                factionCountry = factionCountry,
                playerClass = playerClass,
                currentAimForward = Vector3.zero,
                index = currentSpawnIndex

            };
            if(currentSpawns != null)
            {
                target.group = currentSpawns[currentSpawnIndex% currentSpawns.Count].group;
            }
            slaveOwnerDictionary[currentRequestOwner].AddLast(target);
            return true;
        }
        public static void scpm_AddCarbonPlayers_specific_post(ServerCarbonPlayersManager __instance, Dictionary<int, CarbonPlayerRepresentation> ___carbonPlayers , dfList<int> ___carbonPlayerIDs, bool __result)
        {
            currentSpawnIndex++;
            if(carbonPlayers == null || carbonPlayerIDs == null)
            {
                carbonPlayers = ___carbonPlayers;
                carbonPlayerIDs = ___carbonPlayerIDs;
            }
        }


        public static  bool scpm_InitiateCarbonPlayer_pre(ref NetworkPlayer __result)
        {
            NetworkPlayer networkPlayer = Network.NetworkClientAllocator.Allocate();
            networkPlayer.isCarbonPlayer = true;
            ClientAuthSettings clientAuthSettings = new ClientAuthSettings
            {
                SteamID = 0UL,
                SteamCBAuthTicket = 0,
                SteamPAuthTicket = new byte[0],
                MachineID = SystemInfo.deviceUniqueIdentifier,
                MachineSecurityID = StringCipher.Hash(SystemInfo.deviceUniqueIdentifier, 0UL)
            };
            bool flag = false;
            bool isPlayerWithWeakWeapons = UnityEngine.Random.Range(0f, 1f) > 0.5f;
            ServerRoundPlayer ownerRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(currentRequestOwner);
            string regimentTag = ownerRoundPlayer.PlayerRoundInformation.InitialDetails.RegimentTag;
            string regimentBannerCode = ownerRoundPlayer.PlayerRoundInformation.InitialDetails.RegimentBannerCode;
            int regimentScore = ownerRoundPlayer.PlayerRoundInformation.InitialDetails.RegimentScore;
            string namePrefix = currentRequestOwner.ToString();
            if(currentSpawns != null)
            {
                namePrefix = currentSpawns[0].namePrefix;
            }
            PlayerInitialDetails playerInitialDetails = new PlayerInitialDetails
            {
                Name = string.Format("{0} {1}", namePrefix, currentSpawnIndex.ToString()),
                CharacterVoicePitch = 1f,
                IsLoyalistPlayerBadge = flag,
                IsLoyalistPlayerHorse = flag,
                IsLoyalistPlayerWeapons = flag,
                IsPlayerWithWeakWeapons = isPlayerWithWeakWeapons,
                IsRegimentsOfTheLineDlc = true,
                IsHighCommandDlc = true,
                IsRegimentsOfTheGuardDlc = true,
                RegimentTag = regimentTag,
                RegimentBannerCode = regimentBannerCode,
                RegimentScore = regimentScore
            };
            if(serverGameManager == null)
            {
                initComponent();
            }
            serverGameManager.HandlePlayerSentDetails(clientAuthSettings, playerInitialDetails, networkPlayer);
            __result = networkPlayer;
            return false;
        }
        public static void scpm_InitiateCarbonPlayer_post(ref uLink.NetworkPlayer __result)
        {
            //执行Bot 初始化(init slave object)
            if (slaveOwnerDictionary.ContainsKey(currentRequestOwner)) 
            {
                slaveOwnerDictionary[currentRequestOwner].Last().playerId = __result.id;
                slavePlayerDictionary[__result.id] = slaveOwnerDictionary[currentRequestOwner].Last();
                slavePlayerTargetTransforms[__result.id] = new Queue<DemoTransform>();
            }
        }
        public static void scpm_GenerateClientChosenSpawnSettings_post(ref ClientChosenSpawnSettings __result, ModelUniformDataRepository ___modelUniformDataRepository)
        {
            if(currentSpawns == null || currentSpawnIndex >= currentSpawns.Count)
            {
                //Debug.Log("demo: " + currentSpawns.Count + " index; " + currentSpawnIndex);
                return;
            }
 //           int spawnSectionId = __result.SpawnSectionID;
 //          SpawnSection section = spawnSectionManager.availableSpawnSections[spawnSectionId];
//FIX spawn section invalid here
            try
            {
                int temp = currentSpawnIndex % currentSpawns.Count;
                FactionCountry factionCountry = currentSpawns[temp].factionCountry;
                PlayerClass playerClass = currentSpawns[temp].playerClass;
                int idx = currentSpawns[temp].uniformId;
                List<ModelUniformDataItem> list = ___modelUniformDataRepository.ResolveUniformsFor(factionCountry, playerClass);

                if (idx >= list.Count)
                {
                    Debug.Log("demo: out of Range Index.");
                    return ;
                }
                __result.CharacterUniformIdentifier = (byte)list[idx].UniformId;
                __result.ClassMedalID = 17;
                __result.LeaderboardMedalID = 32;
                int rand = UnityEngine.Random.Range(0, list[idx].HeadIds.Length);
                __result.CharacterHeadIdentifier = (byte)list[idx].HeadIds[rand];
            }
            catch(Exception ex)
            {
                Debug.Log("demo: exception in scpm_GenerateClientChosenSpawnSettings_post" + ex.ToString());
            }
        }

        public static void scpm_SpawnCarbonPlayer_pre(NetworkPlayer networkPlayer, ref FactionCountry factionCountry, ref PlayerClass playerClass) //Bots 信息设置
        {
            int carbonPlayerId = networkPlayer.id;
            if (slavePlayerDictionary.ContainsKey(carbonPlayerId)) // bots复活
            {
                factionCountry = slavePlayerDictionary[carbonPlayerId].factionCountry;
                playerClass = slavePlayerDictionary[carbonPlayerId].playerClass;
            }

        }

        public static void sgm_InformPlayerAboutUnableToSpawn_post(NetworkPlayer networkPlayer)
        {
            try
            {
                if (networkPlayer.isCarbonPlayer)
                {
                    SlavePlayer slave = slavePlayerDictionary[networkPlayer.id];
                    int ownerID = slave.ownerId;
                    slavePlayerDictionary.Remove(slave.playerId);
                    slavePlayerTargetTransforms.Remove(slave.playerId);
                    slaveOwnerDictionary[ownerID].Remove(slave);
                    serverCarbonPlayersManager.RemoveCarbonPlayer(slave.playerId);
                }
            }catch ( Exception ex)
            {
                Debug.Log("demo: Exception at sgm_InformPlayerAboutUnableToSpawn_post " + ex.ToString());
            }

        }

        public static void scpm_FixedUpdate_post() 
        {
            try
            {

                if (serverRoundPlayerManager == null)
                {
                    initComponent();
                }

                // Per-slave Routines
                // 死亡检测 新版 (death detection new)
                foreach (var pair in slaveOwnerDictionary)
                {
                    FixedUpdate_deathDetection(pair);
                    FixedUpdate_handleFireAtWill(pair);
                }

                
                //
                // Per-Line Routines
                //形成线列 (make a line)
                foreach (var pair in slaveOwnerInfantryLine)
                {
                    FixedUpdate_formInfantryLine(pair);
                }
            }
            catch (Exception  ex)
            {
                Debug.Log("demo exception in scpm_FixedUpdate_post: " + ex.ToString());
            }
           
        }

        private static void FixedUpdate_formInfantryLine(KeyValuePair<int , DemoInfantryLine> pair)
        {
            int ownerId = pair.Key;
            DemoInfantryLine line = pair.Value;
            LinkedList<SlavePlayer>[] lineInfantry = line.lines;
            if (!slaveOwnerDictionary.ContainsKey(ownerId)) { return; };
            RoundPlayer ownerRoundPlayer = serverRoundPlayerManager.ResolveRoundPlayer(ownerId);
            bool hasDeath0 = false;
            bool hasDeath1 = false;
            //List<SlavePlayer> temp = slaveOwnerDictionary[ownerId];

            //形成线列
            try
            {
                List<DemoTransform> l0_transforms = line.linesTransform[0];
                List<DemoTransform> l1_transforms = line.linesTransform[1];

                //在线列上检查死亡Bot (Detect dead slave on line)
                List<SlavePlayer> old = new List<SlavePlayer>(lineInfantry[0]);
                foreach (SlavePlayer slave in old)//检测bot死亡
                {
                    if (!slave.isAlive)
                    {
                        lineInfantry[0].Remove(slave);
                        slavePlayerTargetTransforms[slave.playerId].Clear();
                        hasDeath0 = true;
                    }
                }
                old = new List<SlavePlayer>(lineInfantry[1]);
                foreach (SlavePlayer slave in old)//检测bot死亡
                {
                    if (!slave.isAlive)
                    {
                        lineInfantry[1].Remove(slave);
                        slavePlayerTargetTransforms[slave.playerId].Clear();
                        hasDeath1 = true;
                    }
                }

                if (line.doUpdate) //更新Transform (Update slave transform)
                {
                    line.updateTransform();
                }
                List<SlavePlayer> slaves = new List<SlavePlayer>(); // 存活的slave (Alive slaves only)
                foreach (SlavePlayer slave in slaveOwnerDictionary[ownerId])
                {
                    if (slave.isAlive)
                    {
                        slaves.Add(slave);
                    }
                }

                //初始化 (init line)
                if (!line.isDoubleRow)
                {
                    if (lineInfantry[0].Count == 0)//初始化
                    {
                        for (int i = 0; i < slaves.Count; ++i)
                        {
                            var slave = slaves[i];
                            slave.follow = false;
                            if (slavePlayerTargetTransforms.ContainsKey(slave.playerId))
                            {
                                slavePlayerTargetTransforms[slave.playerId].Clear();
                            }
                            walkTo(slave.playerId, l0_transforms[i]);
                            lineInfantry[0].AddLast(slave);
                        }
                    }
                }
                else // 双排 (double rank line init)
                {
                    if (lineInfantry[0].Count == 0 && lineInfantry[1].Count == 0)//初始化
                    {
                        for (int i = 0; i < slaves.Count; ++i)
                        {
                            SlavePlayer slave = slaves[i];
                            slave.follow = false;
                            if (slavePlayerTargetTransforms.ContainsKey(slave.playerId))
                            {
                                slavePlayerTargetTransforms[slave.playerId].Clear();
                            }
                            int num = i < line.oneRow ? 0 : 1;
                            if (num == 0)
                            {
                                if (line.doUpdate)
                                {
                                    walkTo(slave.playerId, l0_transforms[i]);
                                }
                                else
                                {
                                    serverCarbonPlayersManager.StartCoroutine(waitWalkTo(4, slave.playerId, l0_transforms[i]));
                                }
                            }
                            else
                            {
                                var rp = serverRoundPlayerManager.ResolveRoundPlayer(slave.playerId);
                                if (line.doUpdate)
                                {
                                    walkTo(slave.playerId, l1_transforms[i - line.oneRow]);
                                }
                                else
                                {
                                    serverCarbonPlayersManager.StartCoroutine(waitWalkTo((i - line.oneRow) * 0.6f, slave.playerId, new DemoTransform() { position = rp.PlayerTransform.position + rp.PlayerTransform.forward * 0.8f, forward = rp.PlayerTransform.right * -1f, checkDistanceBlocking = false }));
                                    serverCarbonPlayersManager.StartCoroutine(waitWalkTo(0.9f * (i - line.oneRow) + 1f, slave.playerId, l1_transforms[i - line.oneRow]));
                                    serverCarbonPlayersManager.StartCoroutine(waitAction(slaves.Count / 2 + 1f, ownerId, PlayerActions.StartCrouching.ToString(), slaveId: slave.playerId));
                                }
                            }
                            lineInfantry[num].AddLast(slave);
                        }
                    }
                }

                //补齐空位 (Fill the dead body position)
                List<SlavePlayer> newListLineInfantry = new List<SlavePlayer>(lineInfantry[0]); //第一排
                for (int i = 0; i < newListLineInfantry.Count && hasDeath0; ++i)
                {
                    var slave = newListLineInfantry[i];
                    slave.follow = false;
                    if (slavePlayerTargetTransforms.ContainsKey(slave.playerId))
                    {
                        slavePlayerTargetTransforms[slave.playerId].Clear();
                    }
                    var tempTransform = new DemoTransform() { position = l0_transforms[i].position, forward = l0_transforms[i].forward, checkDistanceBlocking = false };
                    serverCarbonPlayersManager.StartCoroutine(waitWalkTo((0.7f * i) + 0.5f, slave.playerId, tempTransform));
                }
                newListLineInfantry = new List<SlavePlayer>(lineInfantry[1]); //第二排
                for (int i = 0; i < newListLineInfantry.Count && hasDeath1; ++i)
                {
                    var slave = newListLineInfantry[i];
                    slave.follow = false;
                    if (slavePlayerTargetTransforms.ContainsKey(slave.playerId))
                    {
                        slavePlayerTargetTransforms[slave.playerId].Clear();
                    }
                    var tempTransform = new DemoTransform() { position = l1_transforms[i].position, forward = l1_transforms[i].forward, checkDistanceBlocking = false };
                    serverCarbonPlayersManager.StartCoroutine(waitWalkTo((0.7f * i) + 0.5f, slave.playerId, tempTransform));
                }
                hasDeath0 = false;
                hasDeath1 = false;
            }
            catch (Exception ex)
            {
                Debug.Log("demo: Exception at FixedUpdate_formInfantryLine : " + ex);
            }

        }

        private static void FixedUpdate_deathDetection(KeyValuePair<int, LinkedList<SlavePlayer>> pair)
        {
            int ownerId = pair.Key;
            LinkedList<SlavePlayer> slaves = pair.Value;
            List<SlavePlayer> temp = new List<SlavePlayer>(slaves);
            int idx = 0;
            foreach (SlavePlayer slave in temp)
            {
                ServerRoundPlayer serverRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(slave.playerId);
                if (serverRoundPlayer == null) continue;
                slave.isAlive = serverRoundPlayer.PlayerBase.SpawnedAndAlive;
                if (!slave.isAlive)
                {
                    slaves.Remove(slave);
                    slaves.AddLast(slave);
                }
            }
            foreach (SlavePlayer s in slaves) //更新index (update index)
            {
                s.index = idx;
                idx++;
            }
        }

        private static void FixedUpdate_handleFireAtWill(KeyValuePair<int, LinkedList<SlavePlayer>> pair)
        {
            int ownerId = pair.Key;

            foreach(SlavePlayer slave in pair.Value)
            {
                int slaveId = slave.playerId;
                if (!slave.isFiringAtWill)
                {
                    break; // Fire at will is set to all bots!
                }
                //serverRoundPlayerManager.ResolveRoundPlayer(slaveId).WeaponHolder.ActiveWeaponData.
                if (slave.isAlive && slave.isFiringAtWill && slave.hasAmmo && !slave.isReloading   && !slave.follow)
                {
                    action(ownerId, slaveId.ToString(), "randomAim");
                    serverCarbonPlayersManager.StartCoroutine(waitAction(0.5f, ownerId, "fire", slaveId: slaveId));
                }
                if(slave.isAlive && slave.isFiringAtWill && !slave.hasAmmo && !slave.isReloading)
                {
                    serverCarbonPlayersManager.StartCoroutine(waitAction(1f, ownerId, "reload", slaveId: slaveId));

                }
            }
            
        }

        public static bool scpm_UpdateCarbonPlayerInput_pre(ServerCarbonPlayersManager __instance, CarbonPlayerRepresentation carbonPlayer, ref ServerRoundPlayer player, ref PlayerActions playerAction, MeleeStrikeType meleeStrike, MeleeStrikeManager ___meleeStrikeManager)
        {

            try
            {
                if ((playerAction != PlayerActions.None && (playerAction != PlayerActions.FireFirearm)))
                { return true; }
                int slaveId = carbonPlayer.playerID;
                if (!slavePlayerDictionary.ContainsKey(slaveId))
                {
                    return true;
                }
                //更新bot朝向 (update slave forward)
                SlavePlayer slave = slavePlayerDictionary[slaveId];
                player.PlayerTransform.forward = slave.currentAimForward;
                //更新bot 跟随 (update follow status)
                if (slave.follow)
                {
                    if (slavePlayerTargetTransforms.ContainsKey(slaveId))
                    {
                        slavePlayerTargetTransforms[slaveId].Clear();
                    }
                    RoundPlayer ownerRoundPlayer = serverRoundPlayerManager.ResolveRoundPlayer(slave.ownerId);
                    Vector3 ownerPosition = new Vector3(ownerRoundPlayer.PlayerTransform.position.x, ownerRoundPlayer.PlayerTransform.position.y, ownerRoundPlayer.PlayerTransform.position.z);
                    Vector3 ownerBack = ownerRoundPlayer.PlayerTransform.forward * -1f;
                    Vector3 targetPosition = ownerPosition + (ownerBack * 0.9f * (slave.index + 1.5f));
                    Vector3 targetForward = new Vector3(ownerRoundPlayer.PlayerTransform.forward.x, ownerRoundPlayer.PlayerTransform.forward.y, ownerRoundPlayer.PlayerTransform.forward.z);
                    walkTo(slave.playerId, new DemoTransform() { position = targetPosition, forward = targetForward });
                }

                //更新bot 自由射击 (update fire at will)
                if (slave.isFiringAtWill && slave.isAlive && !slave.follow)
                {
                }

                if (!__instance.updateInput) { return true; }

                Vector2 inputAxis;
                float y;
                OwnerPacketToServer ownerPacketToServer = ComponentReferenceManager.genericObjectPools.ownerPacketToServer.Obtain();
                RoundPlayer carbonPlayerRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(slaveId);
                byte spawnInstance = player.PlayerBase.PlayerStartData.SpawnInstance;
                Queue<DemoTransform> targetQueue;
                //获取slave移动队列
                if (slavePlayerTargetTransforms.ContainsKey(slaveId))
                {
                    targetQueue = slavePlayerTargetTransforms[slaveId];
                }
                else
                {
                    return true;
                }
                //获取移动 (update slave movement)
                DemoTransform target = targetQueue.Count != 0 ? targetQueue.Peek() : null;
                if (target == null)
                {
                    return true;
                }
                else
                {
                    float rand2 = UnityEngine.Random.Range(0f, 1f);
                    if (rand2 > 0.7 && target.lastPosition != Vector3.zero)
                    {
                        //碰撞检测 (collision detection)
                        Vector3 forwardSet = new Vector3(slave.currentAimForward.x, slave.currentAimForward.y, slave.currentAimForward.z);
                        Vector3 nowForward = player.PlayerTransform.position - target.lastPosition;
                        nowForward.Normalize();
                        forwardSet.Normalize();
                        nowForward.y = 0;
                        forwardSet.y = 0;
                        if (Vector3.Distance(target.lastPosition, player.PlayerTransform.position) < 0.00006 && (target.checkDistanceBlocking))
                        {
                            //Debug.Log("demo: distance moved: " + Vector3.Distance(target.lastPosition, player.PlayerTransform.position));
                            Vector3 newForward = (carbonPlayerRoundPlayer.PlayerTransform.right) + (-1f * carbonPlayerRoundPlayer.PlayerTransform.forward);
                            Vector3 f1 = (-1f * carbonPlayerRoundPlayer.PlayerTransform.forward);
                            f1.Normalize();
                            Vector3 p1 = (player.PlayerTransform.position + 0.2f * f1);
                            newForward.Normalize();
                            slave.currentAimForward = newForward;
                            DemoTransform temp = targetQueue.Dequeue();
                            float multiplier = 0.15f;
                            while (temp.isCollistion)
                            {
                                temp = targetQueue.Dequeue();
                            }
                            targetQueue.Enqueue(new DemoTransform() { forward = f1, position = p1, isCollistion = true });
                            targetQueue.Enqueue(new DemoTransform() { forward = newForward, position = newForward * multiplier + player.PlayerTransform.position, isCollistion = true });
                            targetQueue.Enqueue(temp);
                        }
                        else if (nowForward != Vector3.zero && Vector3.Distance(nowForward, forwardSet) > 0.09)
                        {
                            //Debug.Log("demo: vector distance: " + Vector3.Distance(nowForward, forwardSet));
                            DemoTransform temp = targetQueue.Dequeue();
                            float multiplier = 0.4f;
                            while (temp.isCollistion)
                            {
                                temp = targetQueue.Dequeue();
                                multiplier += 0.15f;
                            }
                            slave.currentAimForward = nowForward;
                            targetQueue.Enqueue(new DemoTransform() { forward = nowForward, position = nowForward * multiplier + player.PlayerTransform.position, isCollistion = true });
                            if (!temp.isCollistion) { targetQueue.Enqueue(temp); }
                        }
                    }
                    target.lastPosition = player.PlayerTransform.position;

                    float distance = Vector2.Distance(new Vector2(target.position.x, target.position.z), new Vector2(player.PlayerTransform.position.x, player.PlayerTransform.position.z));
                    //Debug.Log(string.Format("demo: distance: {0} last: {1}", distance, target.lastDistance));
                    if (distance > target.lastDistance)
                    {
                        //重新计算forward (re-calculate slave forward)
                        Vector3 forward = (target.position - player.PlayerTransform.position);
                        forward.y = 0;
                        forward.Normalize();
                        slave.currentAimForward = forward;
                        //Debug.Log("demo: caculate new forward: " + forward);
                    }
                    target.lastDistance = distance;
                    if (distance > 0.2)
                    {
                        inputAxis = new Vector2(0, 1);
                        EnumCollection<PlayerActions> enumCollection = ComponentReferenceManager.genericObjectPools.playerActionsEnumCollection.Obtain();
                        enumCollection.Add((int)PlayerActions.Run);
                        ownerPacketToServer.ActionCollection = enumCollection;
                    }
                    else if (distance > 0.065)
                    {
                        inputAxis = new Vector2(0, 1);
                    }
                    else
                    {
                        targetQueue.Dequeue();
                        inputAxis = Vector2.zero;
                        //Debug.Log(string.Format("demo:{3} reach position: {0} forward: {1} isCollision: {2}", target.position, target.forward, target.isCollistion, carbonPlayer.playerID));
                        if (!target.isCollistion) { slavePlayerTargetTransforms[slaveId].Clear(); }
                        slave.currentAimForward = target.forward;
                    }
                }

                y = carbonPlayerRoundPlayer.PlayerObject.transform.rotation.eulerAngles.y;
                ownerPacketToServer.Instance = new byte?(spawnInstance);
                ownerPacketToServer.OwnerInputAxis = new Vector2?(inputAxis);
                ownerPacketToServer.OwnerRotationY = new float?(y);
                ownerPacketToServer.Swimming = player.PlayerBase.State.isSwimming;
                if (playerAction == PlayerActions.FireFirearm)
                {
                    EnumCollection<PlayerActions> enumCollection = ComponentReferenceManager.genericObjectPools.playerActionsEnumCollection.Obtain();
                    enumCollection.Add((int)playerAction);
                    double networkTime = uLinkNetworkConnectionsCollection.networkTime;
                    ownerPacketToServer.CameraForward = new Vector3?(slave.currentAimForward);
                    ownerPacketToServer.CameraPosition = new Vector3?(player.uLinkStrictPlatformerCreator.rewinderHitboxGroup.BodyHitboxes[1].Transform.position);
                    ownerPacketToServer.PacketTimestamp = new double?(networkTime);
                    ownerPacketToServer.ActionCollection = enumCollection;

                }
                player.uLinkStrictPlatformerCreator.HandleOwnerPacketToServer(ownerPacketToServer);

                if ((meleeStrike != MeleeStrikeType.None))
                {
                    PlayerMeleeStrikePacket playerMeleeStrikePacket = ComponentReferenceManager.genericObjectPools.playerMeleeStrikePacket.Obtain();
                    playerMeleeStrikePacket.AttackTime = uLinkNetworkConnectionsCollection.networkTime;
                    playerMeleeStrikePacket.AttackingPlayerID = carbonPlayer.playerID;
                    playerMeleeStrikePacket.AttackingPlayerMeleeWeaponDamageDealerTypeID = player.WeaponHolder.ActiveWeaponDetails.damageDealerTypeID;
                    playerMeleeStrikePacket.MeleeStrikeType = meleeStrike;
                    Debug.Log(string.Format("demo: melee: {0}", meleeStrike.ToString()));
                    ___meleeStrikeManager.MeleeAttackStrike(playerMeleeStrikePacket);
                }
            }catch(Exception ex)
            {
                Debug.Log("Demo UpdateCarbonPlayerInput exception: " + ex.ToString());
                return false;
            }
            return false;

        }

        public static void rtm_Update_post()
        {
            try
            {
                updateSlaveYRotation();
            }catch(Exception ex)
            {
                Debug.Log("demo exception at rtm_Update_post: " + ex.ToString());
            }
        }

        public static void scm_uLink_OnPlayerDisconnected_pre(NetworkPlayer networkPlayer)
        {
            removeBots(networkPlayer.id);
        }

        public static void sgm_ChangeGameMode_pre(GameDetails gameDetails)
        {
            reinit();
        } //On mapRotation

        public static void spdm_RegisterFiringAction_post(uLinkStrictPlatformerCreator playerFiring, OwnerPacketToServer packet, CommonGlobalVariables ___commonGlobalVariables) //ServerPlayerDamangeManager
        {

            int playerId = playerFiring.playerBase.PlayerID;
            try
            {
                if (slaveOwnerDictionary.ContainsKey(playerId))
                {
                    Vector3 forward = packet.CameraForward == null ? Vector3.zero : (Vector3)packet.CameraForward;
                    Vector3 position = packet.CameraPosition == null ? Vector3.zero : (Vector3)packet.CameraPosition ;
                    RaycastHit raycastHit;
                    bool hasHits = false;
                    Ray ray = new Ray
                    {
                        origin = new Vector3(position.x, position.y, position.z),
                        direction = new Vector3(forward.x, forward.y, forward.z)
                    };
                    //int.Parse("FFFFFFFF", System.Globalization.NumberStyles.HexNumber)&
                    if (Physics.Raycast(ray.origin, ray.direction, out raycastHit, 1000, ___commonGlobalVariables.layers.bulletBlockers | ___commonGlobalVariables.layers.terrain | ___commonGlobalVariables.layers.player |
                        ___commonGlobalVariables.layers.ship | ___commonGlobalVariables.layers.walkablePlatform | ___commonGlobalVariables.layers.playerRagdollBodyPart |
                        ___commonGlobalVariables.layers.damageableCollider | ___commonGlobalVariables.layers.bulletBlockers | ___commonGlobalVariables.layers.walkable |
                        ___commonGlobalVariables.layers.characterToCharacterCollider | ___commonGlobalVariables.layers.moveableArtilleryColliders | ___commonGlobalVariables.layers.water |
                        ___commonGlobalVariables.layers.outOfBoundsBorderNotificationLayer | ___commonGlobalVariables.layers.vehicleLayer

                     ))
                    {
                        /*Debug.Log(string.Format("demo: get hit! orig: {0}, point: {1}, distance: {2}, layer: {3}" ,ray.origin,raycastHit.point.ToString(), raycastHit.distance, ___commonGlobalVariables.layers.bulletBlockers | ___commonGlobalVariables.layers.terrain | ___commonGlobalVariables.layers.player |
                        ___commonGlobalVariables.layers.ship | ___commonGlobalVariables.layers.walkablePlatform | ___commonGlobalVariables.layers.playerRagdollBodyPart |
                        ___commonGlobalVariables.layers.damageableCollider | ___commonGlobalVariables.layers.bulletBlockers | ___commonGlobalVariables.layers.walkable |
                        ___commonGlobalVariables.layers.characterToCharacterCollider | ___commonGlobalVariables.layers.moveableArtilleryColliders | ___commonGlobalVariables.layers.water |
                        ___commonGlobalVariables.layers.outOfBoundsBorderNotificationLayer | ___commonGlobalVariables.layers.vehicleLayer));
                        */
                        hasHits = true;
                    }
                    foreach (var slave in slaveOwnerDictionary[playerId])
                    {
                        slave.currentAimForward = new Vector3(forward.x, forward.y, forward.z);
                        if (slave.isAimingExact && hasHits)
                        {
                            var roundPlayer = serverRoundPlayerManager.ResolveRoundPlayer(slave.playerId);
                            if (roundPlayer == null)
                            {
                                continue;
                            }
                            Vector3 slavePosition = new Vector3(roundPlayer.PlayerTransform.position.x, roundPlayer.PlayerTransform.position.y, roundPlayer.PlayerTransform.position.z) ;
                            Vector3 cameraP = new Vector3(slavePosition.x, slavePosition.y + 1.3f, slavePosition.z);
                            Vector3 temp = raycastHit.point - cameraP;
                            temp.Normalize();
                            slave.currentAimForward = temp;
                            roundPlayer.PlayerBase.Pitch = playerFiring.playerBase.Pitch;
                        }
                    }
                    action(playerId, "all", "sectionFire");

                }

            }
            catch (Exception ex)
            {
                Debug.Log("demo: Exception in spdm_RegisterFiringAction_post: " + ex.ToString());
            }

        }

        public static void ulspc_HandleOwnerPacketToServer_post(uLinkStrictPlatformerCreator __instance, OwnerPacketToServer op)
        {
            int playerId = __instance.playerBase.PlayerID;
            if (slaveOwnerDictionary.ContainsKey(playerId))
            {
                EnumCollection<PlayerActions> enumCollection = ComponentReferenceManager.genericObjectPools.playerActionsEnumCollection.Obtain();
                if (enumCollection == null)
                {
                    enumCollection = ComponentReferenceManager.genericObjectPools.playerActionsEnumCollection.Obtain();
                }
                op.ActionCollection.CloneInto(enumCollection);
                if (enumCollection.Has((int)PlayerActions.StartReloadFirearm))
                {
                    action(playerId, "all", "reload");
                }
            }
        }

        public  static void soom_HandleRequestStartOfficerOrder_post(RequestStartOfficerOrderPacket currentRequestPacket)
        {
            int officerId = currentRequestPacket.officerNetworkPlayer.id;
            if(!slaveOwnerDictionary.ContainsKey(officerId))
            {
                return;
            }
            OfficerOrderType officerOrderType = currentRequestPacket.officerOrderType;

            if (officerOrderType == OfficerOrderType.FormLine)
            {
                //action(officerId, "all", "onground");
                //serverCarbonPlayersManager.StartCoroutine(waitAction(3, officerId, "forward"));

            }
            else if(officerOrderType == OfficerOrderType.Fire)
            {
                action(officerId, "all", "fire");
            }else if (officerOrderType == OfficerOrderType.CeaseFire)
            {
                action(officerId, "all", PlayerActions.StopAimingFirearm.ToString());
            }else if (officerOrderType == OfficerOrderType.MakeReady)
            {
                action(officerId, "all", PlayerActions.StartAimingFirearm.ToString());
            }


        }
        public static void scvm_HandleClientVoicePlayRequest_post(int playerID, CharacterVoicePhrase phrase)
        {
            if (slaveOwnerDictionary.Keys.ToArray().Contains(playerID))
            {
                if( phrase == CharacterVoicePhrase.ReadyGuns)
                {
                    action(playerID, "all", "reload");
                    serverCarbonPlayersManager.StartCoroutine(waitAction(3 ,playerID, PlayerActions.StartAimingFirearm.ToString()));
                }
                if(phrase == CharacterVoicePhrase.TakeAim || phrase == CharacterVoicePhrase.PresentArms )
                {
                    action(playerID, "all", PlayerActions.DisableCombatStance.ToString());
                    action(playerID, "all", PlayerActions.StartAimingFirearm.ToString());
                }
                else if( phrase == CharacterVoicePhrase.PrimeLoad )
                {
                    action(playerID, "all", "reload");
                }
                else if(phrase == CharacterVoicePhrase.Fire)
                {
                    action(playerID, "all", "fire");
                }
                else if( phrase == CharacterVoicePhrase.ShoulderArms)
                {
                    action(playerID, "all", "onground");
                }
                else if(phrase == CharacterVoicePhrase.StandGround )
                {
                    action(playerID, "all", "onground2");
                }
                else if(phrase == CharacterVoicePhrase.Insult)
                {
                    action(playerID, "all", "ongroundOld");
                }
                else if(phrase == CharacterVoicePhrase.CeaseFire)
                {
                    action(playerID, "all", PlayerActions.StopAimingFirearm.ToString());
                    action(playerID, "all", "stopFireAtWill");
                }
                else if(phrase == CharacterVoicePhrase.PatrioticCheer)
                {
                    action(playerID, "all", "follow");
                }
                else if(phrase == CharacterVoicePhrase.RemoveBayonet)
                {
                    action(playerID, "all", PlayerActions.StartBayonetDetach.ToString());
                    serverCarbonPlayersManager.StartCoroutine(waitAction(4, playerID, PlayerActions.FinishBayonetDetach.ToString()));
                }
                else if(phrase == CharacterVoicePhrase.FixBayonet)
                {
                    action(playerID, "all", PlayerActions.StartBayonetAttach.ToString());
                    serverCarbonPlayersManager.StartCoroutine(waitAction(4, playerID, PlayerActions.FinishBayonetAttach.ToString()));
                }
                else if(phrase == CharacterVoicePhrase.AimHigher)
                {
                    action(playerID, "all", "switchAimMode");
                }else if( phrase == CharacterVoicePhrase.SectionsFire)
                {
                    action(playerID, "all", "sectionFire");
                }else if(phrase == CharacterVoicePhrase.Warcry || phrase == CharacterVoicePhrase.StayCalm)
                {
                    action(playerID, "all", "inlineFollow");
                }else if(phrase == CharacterVoicePhrase.FireAtWill)
                {
                    action(playerID, "all", "startFireAtWill");
                }



            }
        }


        private static bool initSlaveOwner(int playerId)
        {
            if (slaveOwnerDictionary.ContainsKey(playerId)) { return false; }
            slaveOwnerDictionary[playerId] = new LinkedList<SlavePlayer>();
            if (serverCarbonPlayersManager == null)
            {
                initComponent();
            }
            return true;
        }
        
        private static void reinit() //刷图时调用
        {
            var keys = slaveOwnerDictionary.Keys.ToArray();
            slaveOwnerDictionary = new Dictionary<int, LinkedList<SlavePlayer>>();
            foreach( var key in keys)
            {
                slaveOwnerDictionary[key] = new LinkedList<SlavePlayer>();
            }
            slaveOwnerInfantryLine = new Dictionary<int, DemoInfantryLine>();
            slavePlayerDictionary = new Dictionary<int, SlavePlayer>();
            slavePlayerTargetTransforms = new Dictionary<int, Queue<DemoTransform>>();
            faceDirections = new List<ForceFacingDirection>();

            currentRequestOwner = -1;
            currentSpawnIndex = -1;
            carbonPlayers = null;
            carbonPlayerIDs = null;
            initComponent();
        }
        private static void updateSlaveYRotation()
        {
            if (serverRoundTimerManager == null) { return; }
            float currentTime = serverRoundTimerManager.secondsToRoundEnd;
            for (int i = faceDirections.Count - 1; i >= 0; i--)
            {
                var currentPlayerRequested = faceDirections[i];
                RoundPlayer slaveRoundPlayer = serverRoundPlayerManager.ResolveRoundPlayer(currentPlayerRequested.playerId);
                if (slaveRoundPlayer == null)
                {
                    continue;
                }
                if (currentTime >= currentPlayerRequested.stopRotatingTime)
                {
                    slaveRoundPlayer.PlayerObject.transform.eulerAngles = new Vector3(currentPlayerRequested.facingDirection.x, currentPlayerRequested.facingDirection.y, currentPlayerRequested.facingDirection.z);
                    //UnityEngine.Debug.Log("demo: " + currentTime + " Setting rotation " + currentPlayerRequested.facingDirection.ToString());
                }
                else
                {
                    faceDirections.RemoveAt(i);
                    //UnityEngine.Debug.Log("demo: get yrot: " + slaveRoundPlayer.PlayerObject.transform.eulerAngles.ToString());
                    //UnityEngine.Debug.Log("demo: " + currentTime + " Removing at i");
                }
            }
        }


        public static string addBots(int ownerId, int count, FactionCountry factionCountry, PlayerClass playerClass)
        {
            //初始化
            if (serverCarbonPlayersManager == null)
            {
                initComponent();
            }
            if(ownerId <1 || count < 1)
            {
                return "Invalid arguments";
            }
            var spawnLock = new object();
            while (inuse) { return "try Again!"; }
            inuse = true;
            {
                if(currentRequestOwner == -1)
                {
                    currentRequestOwner = ownerId;
                }
                else
                {
                    inuse = false;
                    return "Please try again!";
                }
                bool ownerExists = !initSlaveOwner(ownerId);
                currentSpawnIndex = ownerExists ? slaveOwnerDictionary[ownerId].Count: 0;
                serverCarbonPlayersManager.forceInputAxis = true;
                serverCarbonPlayersManager.forceInputRotation = true;
                serverCarbonPlayersManager.IgnoreAutoControls(true);
                for (int i = 0; i < count; ++i)
                {
                    serverCarbonPlayersManager.AddCarbonPlayers(factionCountry, playerClass);
                }
                currentRequestOwner = -1;
                inuse = false;
            }
            serverCarbonPlayersManager.StartCoroutine(waitFormLine(1f, ownerId));
            return string.Format("success add {0} bots of {1} to {2} .", count, factionCountry + " " + playerClass, ownerId);

        }

        public static string addBots(int ownerId, List<DemoSpawnStruct> spawns)
        {
            if (serverCarbonPlayersManager == null)
            {
                initComponent();
            }
            if (ownerId < 1 || spawns.Count < 1)
            {
                return "Invalid arguments";
            }
            var spawnLock = new object();
            while (inuse) { return "try Again!"; }
            inuse = true;
            try{
                if (currentRequestOwner == -1)
                {
                    currentRequestOwner = ownerId;
                }
                else
                {
                    inuse = false;
                    return "Please try again!";
                }
                currentSpawns = spawns;
                bool ownerExists = !initSlaveOwner(ownerId);
                currentSpawnIndex = ownerExists ? slaveOwnerDictionary[ownerId].Count : 0;
                serverCarbonPlayersManager.forceInputAxis = true;
                serverCarbonPlayersManager.forceInputRotation = true;
                serverCarbonPlayersManager.IgnoreAutoControls(true);
                for (int i = 0; i < spawns.Count; ++i)
                {
                    serverCarbonPlayersManager.AddCarbonPlayers(spawns[i].factionCountry, spawns[i].playerClass);
                }
                currentRequestOwner = -1;
                inuse = false;
            }catch(Exception ex)
            {
                Debug.Log("demo: exception " + ex.ToString());
                inuse = false;
                currentSpawns = null;
                return "exception!";
            }
            serverCarbonPlayersManager.StartCoroutine(waitFormLine(1f, ownerId));
            serverCarbonPlayersManager.StartCoroutine(waitAction( 8f,ownerId, "switchWeapon", group: "flag"));
            currentSpawns = null;
            return string.Format("success add {0} bots of template to {1} .", spawns.Count,  ownerId);
        }

        public static string removeBots(int ownerId)
        {
            inuse = true;
            if (slaveOwnerDictionary.ContainsKey(ownerId))
            {
                foreach( SlavePlayer slave in slaveOwnerDictionary[ownerId])
                {
                    if(serverRoundPlayerManager == null)
                    {
                        initComponent();
                    }
                    serverBannedPlayersManager.RevivePlayer(ownerId, slave.playerId, "");
                    serverRoundPlayerManager.RemovePlayer(carbonPlayers[slave.playerId].networkPlayer, false);
                    EnhancedRC.networkView.RPC<int>("RemotePlayerLeftRound", uLinkNetworkConnectionsCollection.connections, slave.playerId);
                    slavePlayerDictionary.Remove(slave.playerId);
                    slavePlayerTargetTransforms.Remove(slave.playerId);
                    if(carbonPlayers != null)
                    {
                        carbonPlayers.Remove(slave.playerId);
                    }
                }
                slaveOwnerInfantryLine.Remove(ownerId);
                slaveOwnerDictionary.Remove(ownerId);
                inuse = false;
                return "all bots removed.";
                
            }
            inuse = false;
            return "no slave!";
        }
        public static string formLine(int ownerId, string groupName)
        {
            int[] targets;
            List<int> ids = new List<int>();
            RoundPlayer roundPlayer = null;
            RaycastHit[] resultHits = new RaycastHit[3];
            if (groupName == "all")
            {
                LinkedList<SlavePlayer> slaves;
                if (!slaveOwnerDictionary.TryGetValue(ownerId, out slaves)) { return "no slave!"; };
                foreach (SlavePlayer slave in slaves)
                {
                    ids.Add(slave.playerId);
                }
                targets = ids.ToArray();

            }
            else
            {
                return "Invalid arguments.";
            }

            if ((roundPlayer = serverRoundPlayerManager.ResolveRoundPlayer(ownerId)) == null)
            {
                return "error: no roundplayer.";
            }
            UnityEngine.Vector3 right = roundPlayer.PlayerTransform.right * 0.55f;
            UnityEngine.Vector3 forward = new Vector3(roundPlayer.PlayerTransform.forward.x, roundPlayer.PlayerTransform.forward.y, roundPlayer.PlayerTransform.forward.z);
            UnityEngine.Vector3 position = roundPlayer.PlayerTransform.position + (0.5f * forward);
            float yRotation = roundPlayer.PlayerTransform.eulerAngles.y;
            UnityEngine.Vector3 eulerAngles = new UnityEngine.Vector3(0, yRotation, 0);
            //UnityEngine.Debug.Log("demo: get forward: " + roundPlayer.PlayerTransform.forward.ToString());
            //UnityEngine.Debug.Log("demo: set yrot: " + eulerAngles.ToString());
            //UnityEngine.Debug.Log("demo: my pos:" + roundPlayer.PlayerTransform.position + (0.5f * forward));
            for (int i = 0; i < targets.Length; ++i)
            {
                int slaveId = targets[i];
                
                var targetPosition = position + (right * (i-(targets.Length/2)));
                targetPosition.Set(targetPosition.x, targetPosition.y + 0.3f, targetPosition.z);
                //UnityEngine.Debug.Log(string.Format("demo: {0} pos: {3} right: {2} tpos: {1}", i, targetPosition.ToString(), right, position));
                
                var hits = Physics.RaycastNonAlloc(targetPosition, Vector3.down, resultHits, 3);
                if (hits > 0)
                {
                    var closestDistance = resultHits[0].distance;
                    var closestPoint = resultHits[0].point;
                    for (int h = 1; h < hits; h++)
                    {
                        var raycastHit = resultHits[h];
                        if (closestDistance < raycastHit.distance)
                        {
                            closestDistance = raycastHit.distance;
                            closestPoint = raycastHit.point;
                        }
                    }
                    closestPoint.Set(closestPoint.x, closestPoint.y + 2f, closestPoint.z);
                    targetPosition = closestPoint;
                }
                else
                {
                    hits = UnityEngine.Physics.RaycastNonAlloc(targetPosition, Vector3.up, resultHits, 3);
                    if (hits > 0)
                    {
                        var closestDistance = resultHits[0].distance;
                        var closestPoint = resultHits[0].point;
                        for (int h = 1; h < hits; h++)
                        {
                            var raycastHit = resultHits[h];
                            if (closestDistance < raycastHit.distance)
                            {
                                closestDistance = raycastHit.distance;
                                closestPoint = raycastHit.point;
                            }
                        }
                        closestPoint.Set(closestPoint.x, closestPoint.y + 2f, closestPoint.z);
                        targetPosition = closestPoint;
                    }
                }
                
                var slaveRoundPlayer = serverRoundPlayerManager.ResolveRoundPlayer(slaveId);
                if (slaveRoundPlayer == null)
                {
                    continue;
                }

                slaveRoundPlayer.PlayerObject.transform.position = targetPosition;
                slaveRoundPlayer.PlayerObject.transform.forward = forward;
                slavePlayerDictionary[slaveId].currentAimForward = forward;
                slaveRoundPlayer.PlayerBase.UpdateRotation(yRotation);


                faceDirections.Add(new ForceFacingDirection()
                {
                    playerId = slaveId,
                    facingDirection = eulerAngles,
                    stopRotatingTime = serverRoundTimerManager.SecondsToRoundEnd - 5
                });
            }
            return "success!";
        }

        public static string action(int ownerId, string groupName, string action, string additionalArgs = "BearingFlag")
        {
            RoundPlayer ownerRoundPlayer;
            List<SlavePlayer> slaves = new List<SlavePlayer>();
            PlayerActions targetAction = PlayerActions.None;

           
            var update = HarmonyLib.AccessTools.Method(typeof(ServerCarbonPlayersManager), "UpdateCarbonPlayerInput");

            //检查owner是否初始化
            if (!slaveOwnerDictionary.ContainsKey(ownerId))
            {
                return "error: no slave.";
            }
            List<SlavePlayer> allSlaves = slaveOwnerDictionary[ownerId].ToList();
            //初始化 目标
            int tmp1;
            if (groupName == "all")
            {
                foreach(var s in allSlaves)
                {
                    if(s.isAlive)
                    {
                        slaves.Add(s);
                    }
                    //slaves.Add(s);
                }
            }
            else if (!int.TryParse(groupName, out tmp1))
            {
                slaves = new List<SlavePlayer>();
                foreach(var s in allSlaves)
                {
                    if(s.group == groupName)
                    {
                        slaves.Add(s);
                    }
                }

            }
            else
            {
                slaves = new List<SlavePlayer>();
                try
                {
                    slaves.Add(slavePlayerDictionary[int.Parse(groupName)]);
                }catch (Exception ex)
                {
                    Debug.Log("demo: Exception in EnhancedBots.action" + ex.ToString());
                }
            }
            if (slaves.Count == 0)
            {
                return string.Format("demo: no alive slave!");
            }

            //处理自定义命令
            switch (action)
            {
                case "fire":
                    {
                        foreach(SlavePlayer slave in slaves)
                        {
                            if (!carbonPlayers.ContainsKey(slave.playerId)) { continue; }
                            CarbonPlayerRepresentation carbonPlayerRepresentation = carbonPlayers[slave.playerId];
                            ServerRoundPlayer serverRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(slave.playerId);
                            if (serverRoundPlayer == null) { continue; }
                            if (!slave.isAiming)
                            {
                                update.Invoke(serverCarbonPlayersManager, new object[] { carbonPlayerRepresentation, serverRoundPlayer, PlayerActions.StartAimingFirearm, MeleeStrikeType.None });
                                slave.isAiming = true;
                                serverCarbonPlayersManager.StartCoroutine(waitAction(0.7f, ownerId, PlayerActions.FireFirearm.ToString(), slave.playerId));
                                serverCarbonPlayersManager.StartCoroutine(waitAction(1f, ownerId, PlayerActions.StopAimingFirearm.ToString(), slave.playerId));
                            }
                            else
                            {
                                update.Invoke(serverCarbonPlayersManager, new object[] { carbonPlayerRepresentation, serverRoundPlayer, PlayerActions.FireFirearm, MeleeStrikeType.None });
                                serverCarbonPlayersManager.StartCoroutine(waitAction(0.3f, ownerId, PlayerActions.StopAimingFirearm.ToString(), slave.playerId));
                                slave.hasAmmo = false;
                            }
                        }
                        return string.Format("Bot {0} execute {1} requested by {2}", groupName, "fire", ownerId);
                    }
                case "sectionFire":
                    {
                        int idx = 0;
                        foreach (SlavePlayer slave in slaves)
                        {
                            if (!carbonPlayers.ContainsKey(slave.playerId)) { continue; }
                            CarbonPlayerRepresentation carbonPlayerRepresentation = carbonPlayers[slave.playerId];
                            ServerRoundPlayer serverRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(slave.playerId);
                            if (serverRoundPlayer == null) { continue; }
                            int delayMultiplier = UnityEngine.Random.Range(0, 10);
                            if (!slave.isAiming)
                            {
                                update.Invoke(serverCarbonPlayersManager, new object[] { carbonPlayerRepresentation, serverRoundPlayer, PlayerActions.StartAimingFirearm, MeleeStrikeType.None });
                                slave.isAiming = true;
                                serverCarbonPlayersManager.StartCoroutine(waitAction(0.7f + 0.1f * delayMultiplier, ownerId, PlayerActions.FireFirearm.ToString(), slave.playerId));
                                serverCarbonPlayersManager.StartCoroutine(waitAction(1f + 0.1f * delayMultiplier, ownerId, PlayerActions.StopAimingFirearm.ToString(), slave.playerId));
                            }
                            else
                            {
                                serverCarbonPlayersManager.StartCoroutine(waitAction( 0.1f * delayMultiplier, ownerId, PlayerActions.FireFirearm.ToString(), slave.playerId));
                                serverCarbonPlayersManager.StartCoroutine(waitAction(0.3f + 0.1f * delayMultiplier, ownerId, PlayerActions.StopAimingFirearm.ToString(), slave.playerId));
                            }
                            idx++;
                        }
                        return string.Format("Bot {0} execute {1} requested by {2}", groupName, "sectionFire", ownerId);
                    }
                case "reload":
                    {
                        foreach (SlavePlayer slave in slaves)
                        {
                            if (!carbonPlayers.ContainsKey(slave.playerId)) { continue; }
                            CarbonPlayerRepresentation carbonPlayerRepresentation = carbonPlayers[slave.playerId];
                            ServerRoundPlayer serverRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(slave.playerId);
                            if (serverRoundPlayer == null) { continue; }
                            if (!slave.isAiming)
                            {
                                update.Invoke(serverCarbonPlayersManager, new object[] { carbonPlayerRepresentation, serverRoundPlayer, PlayerActions.StartReloadFirearm, MeleeStrikeType.None });
                            }
                            else
                            {
                                update.Invoke(serverCarbonPlayersManager, new object[] { carbonPlayerRepresentation, serverRoundPlayer, PlayerActions.StopAimingFirearm, MeleeStrikeType.None });
                                slave.isAiming = false;
                                serverCarbonPlayersManager.StartCoroutine(waitAction(0.4f, ownerId, PlayerActions.StartReloadFirearm.ToString(), slave.playerId));
                            }
                            serverCarbonPlayersManager.StartCoroutine(waitAction(12f, ownerId, PlayerActions.FinishReloadFirearm.ToString(), slave.playerId));
                        }
                        return string.Format("Bot {0} execute {1} requested by {2}", groupName, "reload", ownerId);  
                    }
                case "aim":
                    {
                        foreach (SlavePlayer slave in slaves)
                        {
                            if (!carbonPlayers.ContainsKey(slave.playerId)) { continue; }
                            CarbonPlayerRepresentation carbonPlayerRepresentation = carbonPlayers[slave.playerId];
                            ServerRoundPlayer serverRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(slave.playerId);
                            if (serverRoundPlayer == null) { continue; }

                            update.Invoke(serverCarbonPlayersManager, new object[] { carbonPlayerRepresentation, serverRoundPlayer, PlayerActions.StartAimingFirearm, MeleeStrikeType.None });
                            slave.isAiming = true;
                        }
                        return string.Format("Bot {0} execute {1} requested by {2}", groupName, "aim", ownerId);
                    }
                case "switchAimMode":
                    {
                        foreach (SlavePlayer slave in slaves)
                        {
                            slave.isAimingExact = !slave.isAimingExact;
                        }
                        return string.Format("Bot {0} execute {1} requested by {2}", groupName, "switchAimMode", ownerId);
                    }
                case "ongroundOld":
                    {
                        slaveOwnerInfantryLine.Remove(ownerId);
                        EnhancedBots.action(ownerId, "all", PlayerActions.StopCrouching.ToString());
                        foreach (SlavePlayer slave in slaves)
                        {
                            slave.follow = false;
                            if (slavePlayerTargetTransforms.ContainsKey(slave.playerId))
                            {
                                slavePlayerTargetTransforms[slave.playerId].Clear();
                            }
                            ownerRoundPlayer = serverRoundPlayerManager.ResolveRoundPlayer(ownerId);
                            Vector3 ownerPosition = new Vector3(ownerRoundPlayer.PlayerTransform.position.x, ownerRoundPlayer.PlayerTransform.position.y, ownerRoundPlayer.PlayerTransform.position.z);
                            Vector3 ownerRight = ownerRoundPlayer.PlayerTransform.right * 0.65f;
                            int idx = slave.index;
                            Vector3 targetPosition = ownerPosition + (ownerRight * (idx < (slaves.Count) / 2 ? idx - (slaves.Count / 2) : 1 + idx - (slaves.Count / 2)));
                            Vector3 targetForward = new Vector3(ownerRoundPlayer.PlayerTransform.forward.x, ownerRoundPlayer.PlayerTransform.forward.y, ownerRoundPlayer.PlayerTransform.forward.z);

                            walkTo(slave.playerId, new DemoTransform() { position = targetPosition, forward = targetForward });
                        }
                        return string.Format("Bot {0} execute onTheGround requested by {1}", groupName, ownerId);
                    }
                case "follow":
                    {
                        slaveOwnerInfantryLine.Remove(ownerId);
                        EnhancedBots.action(ownerId, "all", PlayerActions.StopCrouching.ToString());
                        foreach (SlavePlayer slave in slaves)
                        {
                            if (slavePlayerTargetTransforms.ContainsKey(slave.playerId))
                            {
                                slavePlayerTargetTransforms[slave.playerId].Clear();
                            }
                            slave.follow = !slave.follow;
                        }
                        return string.Format("Bot {0} execute follow requested by {1}", groupName,  ownerId);
                    }
                case "crouch":
                    { return ""; }
                case "onground":
                    {
                        EnhancedBots.action(ownerId, "all", PlayerActions.StopCrouching.ToString());
                        DemoInfantryLine line = new DemoInfantryLine(_isDoubleRow: false, _oneRow: slaves.Count, _ownerRoundPlayer: serverRoundPlayerManager.ResolveRoundPlayer(ownerId));
                        slaveOwnerInfantryLine[ownerId] = line;
                        return "make single line";
                    }
                case "onground2":
                    {
                        EnhancedBots.action(ownerId, "all", PlayerActions.StopCrouching.ToString());
                        List<SlavePlayer> aliveSlavePlayers = new List<SlavePlayer>();
                        foreach(var s in slaves)
                        {
                            if (s.isAlive)
                                aliveSlavePlayers.Add(s);
                        }
                        DemoInfantryLine line = new DemoInfantryLine(_isDoubleRow: true, _oneRow: aliveSlavePlayers.Count %2 ==0 ? aliveSlavePlayers.Count/2: aliveSlavePlayers.Count/2 +1, _ownerRoundPlayer: serverRoundPlayerManager.ResolveRoundPlayer(ownerId));
                        slaveOwnerInfantryLine[ownerId] = line;
                        return "make double line";
                    }
                case "forward":
                    {
                        slaveOwnerInfantryLine.Remove(ownerId);
                        EnhancedBots.action(ownerId, "all", PlayerActions.StopCrouching.ToString());
                        foreach (SlavePlayer slave in slaves)
                        {
                            slave.follow = false;
                            if (slavePlayerTargetTransforms.ContainsKey(slave.playerId))
                            {
                                slavePlayerTargetTransforms[slave.playerId].Clear();
                            }
                            var slaveRp = serverRoundPlayerManager.ResolveRoundPlayer(slave.playerId);
                            Vector3 slavePosition = new Vector3(slaveRp.PlayerTransform.position.x, slaveRp.PlayerTransform.position.y, slaveRp.PlayerTransform.position.z);
                            var temp = slaveRp.PlayerTransform.forward;
                            Vector3 slaveForward = new Vector3(temp.x , temp.y , temp.z);
                            slaveForward.Normalize();
                            int idx = slave.index;
                            Vector3 targetPosition = slavePosition + 1.2f*slaveForward;
                            Vector3 targetForward = slaveForward ;

                            walkTo(slave.playerId, new DemoTransform() { position = targetPosition, forward = targetForward });
                        }
                        return string.Format("Bot {0} execute onTheGround requested by {1}", groupName, ownerId);
                    }
                case "inlineFollow":
                    {
                        if (!slaveOwnerInfantryLine.ContainsKey(ownerId))
                        {
                            return "make a line first";
                        }
                        EnhancedBots.action(ownerId, "all", PlayerActions.StopCrouching.ToString());
                        bool flag = slaveOwnerInfantryLine[ownerId].doUpdate = !slaveOwnerInfantryLine[ownerId].doUpdate;
                        bool flag2 = slaveOwnerInfantryLine[ownerId].isDoubleRow;
                        if (!flag && flag2)
                        {
                            EnhancedBots.action(ownerId, "all", "l1crouch");
                        }
                        return "inline follow";
                    }
                case "l1crouch":
                    {
                        if (!slaveOwnerInfantryLine.ContainsKey(ownerId))
                        {
                            return "make a line first";
                        }
                        DemoInfantryLine line = slaveOwnerInfantryLine[ownerId];
                        LinkedList<SlavePlayer> l1 = line.lines[1];
                        foreach(var slave in l1)
                        {
                            EnhancedBots.action(ownerId, slave.playerId.ToString(), PlayerActions.StartCrouching.ToString());
                        }
                        return "ok";
                    }
                case "switchWeapon":
                    {
                        foreach (var s in slaves)
                        {
                            WeaponType t = WeaponType.Musket_SeaServiceBrownBess;
                            HomelessMethods.TryParseEnum<WeaponType>(additionalArgs, out t);
                            serverWeaponHolderManager.BroadcastSwitchWeapon(s.playerId, t);
                        }
                        return "Switch weapon ok.";
                    }
                case "meleeStrike":
                    {
                        MeleeStrikeType mType = MeleeStrikeType.MeleeStrikeHigh;
                        PlayerActions mAction = PlayerActions.ExecuteMeleeWeaponStrike;
                        HomelessMethods.TryParseEnum<MeleeStrikeType>(additionalArgs, out mType);
                        Debug.Log(string.Format("demo:meleeStrike {0}", mType));
                        foreach (SlavePlayer slave in slaves)
                        {
                            if (!carbonPlayers.ContainsKey(slave.playerId)) { continue; }
                            CarbonPlayerRepresentation carbonPlayerRepresentation = carbonPlayers[slave.playerId];
                            ServerRoundPlayer serverRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(slave.playerId);
                            if (serverRoundPlayer == null) { continue; }
                            update.Invoke(serverCarbonPlayersManager, new object[] { carbonPlayerRepresentation, serverRoundPlayer, mAction, mType });
                            //Broadcast melee strike
                            Weapon activeWeaponDetails = serverRoundPlayer.CreatorWeaponHolder.ActiveWeaponDetails;
                            PlayerMeleeStrikePacket playerMeleeStrikePacket = ComponentReferenceManager.genericObjectPools.playerMeleeStrikePacket.Obtain();
                            playerMeleeStrikePacket.AttackingPlayerID = slave.playerId;
                            playerMeleeStrikePacket.AttackTime = uLinkNetworkConnectionsCollection.networkTime + 0.03;
                            playerMeleeStrikePacket.AttackingPlayerMeleeWeaponDamageDealerTypeID = activeWeaponDetails.GetDamageDealerTypeID(AttackType.MeleeAttack);
                            playerMeleeStrikePacket.MeleeStrikeType = mType;
                            EnhancedRC.networkView.RPC<PlayerMeleeStrikePacket>("MeleeAttackStrike", uLinkNetworkConnectionsCollection.connections, playerMeleeStrikePacket);
                            ComponentReferenceManager.genericObjectPools.playerMeleeStrikePacket.Release(playerMeleeStrikePacket);
                        }
                        return string.Format("demo:meleeStrike {0}", mType);
                    }
                case "randomAim":
                    {

                        SlavePlayer oneSlave = slaves[0];
                        ServerRoundPlayer serverRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(oneSlave.playerId);
                        Collider[] hitColliders = Physics.OverlapSphere(serverRoundPlayer.PlayerTransform.position, 40, 1 << 11);
                        Vector3 targetPosition;
                        try
                        {
                            Collider randCollider;
                            List<Collider> enemyColliders = new List<Collider>();
                            foreach(Collider co in hitColliders)
                            {
                                FactionCountry targetFaction = co.GetComponent<RigidbodyCharacter>().GetComponent<PlayerBase>().PlayerStartData.Faction;
                                if(targetFaction != serverRoundPlayer.PlayerStartData.Faction)
                                {
                                    enemyColliders.Add(co);
                                }
                            }

                            if(enemyColliders.Count == 0)
                            {
                                return "demo: randomFire no enemy found";
                            }else{
                                int rand = UnityEngine.Random.Range(0, enemyColliders.Count);
                                randCollider = enemyColliders[rand];
                            }

                            targetPosition = randCollider.gameObject.transform.position;

                            foreach ( SlavePlayer slave in slaves)
                            {
                                ServerRoundPlayer slaveRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(slave.playerId);
                                slave.currentAimForward = targetPosition - slaveRoundPlayer.PlayerTransform.position;
                            
                            }
                            return string.Format("demo: randomFire pos {0}", targetPosition);

                        }                        
                        catch (Exception ex)
                        {
                            Debug.Log("demo: excepiton in action:randomFire " + ex.ToString());
                            return "demo: excepiton in action:randomFire " + ex.ToString();
                        }
                    }
                case "startFireAtWill":
                    {
                        foreach( SlavePlayer slave in slaves)
                        {
                            slave.isFiringAtWill = true;
                        }
                        return string.Format("demo: isFiringAtWill set to  {0}", true);
                    }
                case "stopFireAtWill":
                    {
                        foreach (SlavePlayer slave in slaves)
                        {
                            slave.isFiringAtWill = false;
                        }
                        return string.Format("demo: isFiringAtWill set to  {0}", false);
                    }
                default:
                    break;
            }
            //处理默认指令
            foreach(SlavePlayer slave in slaves)
            {
                CarbonPlayerRepresentation carbonPlayerRepresentation = carbonPlayers[slave.playerId];
                ServerRoundPlayer serverRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(slave.playerId);
                try
                {
                    if (!HomelessMethods.TryParseEnum<PlayerActions>(action, out targetAction)) { continue; } ;
                    
                    switch (targetAction)
                    {
                        case PlayerActions.StartAimingFirearm:
                            {
                                slave.isAiming = true;
                                break;
                            }
                        case PlayerActions.StopAimingFirearm:
                            {
                                slave.isAiming = false;
                                break;
                            }
                        case PlayerActions.StartReloadFirearm:
                            {
                                slave.isReloading = true;
                                break;
                            }
                        case PlayerActions.FinishReloadFirearm:
                            {
                                slave.isReloading = false;
                                slave.hasAmmo = true;
                                break;
                            }
                        case PlayerActions.InterruptReloadFirearm:
                            {
                                slave.isReloading = false;
                                break;
                            }
                        case PlayerActions.FireFirearm:
                            {
                                slave.hasAmmo = false;
                                break;
                            }
                    }
                    update.Invoke(serverCarbonPlayersManager, new object[] { carbonPlayerRepresentation, serverRoundPlayer, targetAction, MeleeStrikeType.None }) ;
                   
                }
                catch(Exception ex)
                {
                    Debug.Log(string.Format("demo: Failed execute Command user: {0} action: {1} group: {2} Exception: {3}", ownerId, action, groupName, ex.ToString()));   
                    return "failed Invalid action "+ ex.ToString();
                }
            }

            return string.Format("Bot {0} execute {1} requested by {2}", groupName, action, ownerId);
        }
    
        public static string setCamera(int ownerId, string groupName, UnityEngine.Vector3 cameraForward)
        {
            ServerRoundPlayer ownerRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(ownerId);
            UnityEngine.Vector3 ownerForward = new Vector3(ownerRoundPlayer.PlayerTransform.forward.x, ownerRoundPlayer.PlayerTransform.forward.y, ownerRoundPlayer.PlayerTransform.forward.z);
            UnityEngine.Vector3 ownerPosition = new Vector3(ownerRoundPlayer.PlayerTransform.position.x, ownerRoundPlayer.PlayerTransform.position.y, ownerRoundPlayer.PlayerTransform.position.z);
            return string.Format("get user camera status pos: {0}  forward: {1}", ownerPosition, ownerForward);
        }
    
        public static string walkTo( int slaveId, DemoTransform targetTransform)
        {
            ServerRoundPlayer slaveRoundPlayer;
            if (!slavePlayerDictionary.ContainsKey(slaveId))
            {
                Debug.Log("demo: access slaveId that is not exist!");
                return "no such slave Player";
            }
            if (!slavePlayerTargetTransforms.ContainsKey(slaveId))
            {
                slavePlayerTargetTransforms[slaveId] = new Queue<DemoTransform>();
            }
            slavePlayerTargetTransforms[slaveId].Clear();
            slaveRoundPlayer = serverRoundPlayerManager.ResolveServerRoundPlayer(slaveId);
            if(slaveRoundPlayer == default(RoundPlayer))
            {
                Debug.Log("demo: error Resolve RoundPlayer!");
                return "error Resolve RoundPlayer!";
            }
            DemoTransform target = targetTransform;
            Vector3 forward = target.position - slaveRoundPlayer.PlayerTransform.position;
            forward.y = 0;
            forward.Normalize();
            //slavePlayerDictionary[slaveId].currentAimForward = forward == Vector3.zero? slavePlayerDictionary[slaveId].currentAimForward:forward;
            slavePlayerDictionary[slaveId].currentAimForward =  forward;
            slavePlayerTargetTransforms[slaveId].Enqueue( target);
            //Debug.Log("demo: get forward: " + forward.ToString());
            return forward.ToString();
        }
    

        private static IEnumerator waitWalkTo(float seconds, int slaveId, DemoTransform targetTransform)
        {
            yield return new WaitForSeconds(seconds);
            walkTo(slaveId, targetTransform);
            yield break;
        }
        private static IEnumerator waitFormLine(float seconds, int ownerId)
        {
            yield return new WaitForSeconds(seconds);
            formLine(ownerId, "all");
            yield break;
        }

        private static IEnumerator waitAction(float seconds, int ownerId,  string _action, int slaveId = 0, string group = "none")
        {
            yield return new WaitForSeconds(seconds);
            if(slaveId != 0)
            {
                action(ownerId, slaveId.ToString(), _action);
            }
            else
            {
                if (group != "none")
                {
                    action(ownerId, group, _action);
                }
                else
                {
                    action(ownerId, "all", _action);
                }
                yield break;
            }
        }

   
        private static void initComponent()
        {
            
            ServerComponentReferenceManager serverInstance = ServerComponentReferenceManager.ServerInstance;
            serverCarbonPlayersManager = serverInstance.serverCarbonPlayersManager;
            serverRoundPlayerManager = serverInstance.serverRoundPlayerManager;
            serverRoundTimerManager = serverInstance.serverRoundTimerManager;
            serverBannedPlayersManager = serverInstance.serverBannedPlayersManager;
            serverGameManager = serverInstance.serverGameManager;
            serverWeaponHolderManager = serverInstance.serverWeaponHolderManager;
            spawnSectionManager = serverInstance.spawnSectionManager;
            ref_SpawnCarbonPlayer = HarmonyLib.AccessTools.Method(typeof(ServerCarbonPlayersManager), "SpawnCarbonPlayer");
            ref_IsPlayerActionMeleeBlock = HarmonyLib.AccessTools.Method(typeof(ServerCarbonPlayersManager), "IsPlayerActionMeleeBlock");
            ref_UpdateCarbonPlayerInput = HarmonyLib.AccessTools.Method(typeof(ServerCarbonPlayersManager), "UpdateCarbonPlayerInput");
        }
    
    }
}
