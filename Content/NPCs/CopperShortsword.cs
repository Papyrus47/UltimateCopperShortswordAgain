using System.IO;
using Terraria;
using Terraria.DataStructures;
using UltimateCopperShortsword.Content.NPCs.Modes;
using UltimateCopperShortsword.Content.NPCs.Skills;
using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs
{
    [AutoloadBossHead]
    public class CopperShortsword : BasicSkillNPC
    {
        public override string BossHeadTexture => Texture;
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
        public override string Texture => $"Terraria/Images/Item_{ItemID.CopperShortsword}";
        public Player TargetPlayer
        {
            get
            {
                if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    NPC.TargetClosest();
                return Main.player[NPC.target];
            }
        }

        public static int Music1;
        public static int Music2;
        public static int Music3;
        /// <summary>
        /// 伤害池
        /// </summary>
        public int DamagePool;
        /// <summary>
        /// 伤害池子上限
        /// </summary>
        public int DamagePoolMax = 30000;
        /// <summary>
        /// 一阶段游走
        /// </summary>
        public Move Move_One;
        /// <summary>
        /// 二阶段游走
        /// </summary>
        public Move Move_Two;
        /// <summary>
        /// 三阶段游走
        /// </summary>
        public Move Move_Three;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Music1 = MusicLoader.GetMusicSlot("UltimateCopperShortsword/Assets/Sounds/Music/Phase1");
            Music2 = MusicLoader.GetMusicSlot("UltimateCopperShortsword/Assets/Sounds/Music/Phase2");
            Music3 = MusicLoader.GetMusicSlot("UltimateCopperShortsword/Assets/Sounds/Music/Phase3");
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 9;
            NPC.defense = 16;
            NPC.lifeMax = 66000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 100000;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            Music = Music1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncNPC,-1,-1,null,NPC.whoAmI); // 同步生成
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(DamagePool);
            //writer.Write(NPC.rotation);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            DamagePool = reader.ReadInt32();
            //NPC.rotation = reader.ReadSingle();
        }
        public override void AI()
        {
            //if (Main.netMode == NetmodeID.MultiplayerClient)
            //    return;
            _ = TargetPlayer;
            if (TargetPlayer.dead)
            {
                NPC.active = false;
                return;
            }
            base.AI();
            if (Main.netMode == NetmodeID.Server)
            {
                // 因为服务器是控制台，所以要把信息写进控制台里
                Console.WriteLine(CurrentSkill.GetType().Name);
            }
            //NPC.life = (int)(NPC.lifeMax * 0.45f);
            if (NPC.life <= (int)(NPC.lifeMax * 0.2f)) // 清除
            {
                NPC.life = 0;
                NPC.checkDead();
                Main.NewText("之后还会更新更多，敬请期待！");
                return;
            }
            if (CurrentMode is TwoLevel)
            {
                Music = Music2;
            }
            else if(CurrentMode is ThreeLevel)
            {
                Music = Music3;
            }
            if(DamagePool < DamagePoolMax)
                DamagePool += 20;
            if (DamagePool < 0)
                DamagePool += -DamagePool / 3;
        }
        public override bool CheckActive() => false;
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            base.ModifyHitPlayer(target, ref modifiers);
            modifiers.ScalingArmorPenetration += 1;
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            base.ModifyIncomingHit(ref modifiers);
            if (DamagePool <= 100)
                modifiers.FinalDamage *= 0.9f;
            modifiers.ModifyHitInfo += ModifyHitInfo;
        }
        public void ModifyHitInfo(ref NPC.HitInfo info)
        {
            DamagePool -= info.Damage;
            if (DamagePool <= 0)
            {
                info.Damage = 1;
            }
        }

        public override void OnSkillTimeOut()
        {
            OldSkills.Enqueue(CurrentSkill);
            if (OldSkills.Count > 10)
                OldSkills.Dequeue();
            if (CurrentMode is TwoLevel)
            {
                Move_Two.OnSkillActive(CurrentSkill);
                CurrentSkill.OnSkillDeactivate(Move_Two);
                CurrentSkill = Move_Two;
            }
            else if(CurrentMode is ThreeLevel)
            {
                Move_Three.OnSkillActive(CurrentSkill);
                CurrentSkill.OnSkillDeactivate(Move_Three);
                CurrentSkill = Move_Three;
            }
            else
            {
                Move_One.OnSkillActive(CurrentSkill);
                CurrentSkill.OnSkillDeactivate(Move_One);
                CurrentSkill = Move_One;
            }
        }
        public override void Init()
        {
            SaveModes = new();
            SaveModesID = new();
            SaveSkills = new();
            SaveSkillsID = new();
            OldSkills = new();

            #region 注册状态
            OneLevel oneLevel = new(NPC);
            TwoLevel twoLevel = new(NPC);
            ThreeLevel threeLevel = new(NPC);
            SkillNPC.Register(oneLevel, twoLevel, threeLevel);

            CurrentMode = oneLevel;
            #endregion
            #region 注册 创建技能
            StartText start = new(NPC);
            Move_One = new(NPC, 5);
            Move_Two = new(NPC, 8);
            Move_Three = new(NPC, 10);
            #region 三阶段技能
            #region 技能创建
            Swing swing1_phase3 = new(NPC, Vector2.UnitY, MathHelper.Pi, 0.5f, 1.8f, 20)
            {
                rand = 10
            };
            SwingShoot swing2_phase3 = new(NPC, -Vector2.UnitY, -MathHelper.Pi, 1f, 2f, 20)
            {
                rand = 20
            };
            SwingShoot swing3_phase3 = new(NPC, Vector2.UnitY, MathHelper.Pi, 0.2f, 2.1f, 20)
            {
                rand = 30
            };
            SwingShoot swing4_phase3 = new(NPC, -Vector2.UnitY, -MathHelper.Pi, 1f, 2.2f, 20)
            {
                rand = 40
            };
            ChargedSlash chargedSlash = new(NPC, Vector2.UnitY, MathHelper.Pi, 0.2f, 4f, 40)
            {
                rand = 50
            };
            Spurt spurt1_phase3 = new(NPC, () => TargetPlayer.Center + (TargetPlayer.Center - NPC.Center).SafeNormalize(default) * 1000);
            Spurt spurt2_phase3 = new(NPC, () => TargetPlayer.Center + (TargetPlayer.Center - NPC.Center).SafeNormalize(default) * 500);
            Spurt spurt3_phase3 = new(NPC, () => TargetPlayer.Center + (TargetPlayer.Center - NPC.Center).SafeNormalize(default) * 500);

            MoveHeadAndShoot moveHeadAndShoot_phase3 = new(NPC);
            RoungTargetAndShoot roungTargetAndShoot_phase3 = new(NPC);
            MoveSwingShoot moveSwingShoot1_phase3 = new(NPC, Vector2.UnitY, MathHelper.Pi, 0.3f, 2.5f, 10)
            {
                rand = 50 // 一半几率触发
            };
            MoveSwingShoot moveSwingShoot2_phase3 = new(NPC, -Vector2.UnitY, -MathHelper.Pi, 0.3f, 2.5f, 10)
            {
                rand = 50 // 一半几率触发
            };
            #endregion
            SkillNPC.Register(Move_Three, swing1_phase3, swing2_phase3, swing3_phase3, swing4_phase3, chargedSlash, spurt1_phase3, spurt2_phase3, spurt3_phase3, moveHeadAndShoot_phase3, moveSwingShoot1_phase3);

            spurt1_phase3.AddBySkilles(swing1_phase3, swing2_phase3, swing3_phase3, swing4_phase3, chargedSlash);
            Move_Three.AddSkill(spurt1_phase3).AddSkill(spurt2_phase3).AddSkill(spurt3_phase3);
            Move_Three.AddSkill(swing1_phase3).AddSkill(swing2_phase3).AddSkill(swing3_phase3).AddSkill(swing4_phase3).AddSkill(chargedSlash);
            moveSwingShoot1_phase3.AddBySkilles(swing1_phase3, swing2_phase3, swing3_phase3, swing4_phase3, chargedSlash, spurt1_phase3, spurt2_phase3, spurt3_phase3, moveHeadAndShoot_phase3);
            moveSwingShoot1_phase3.AddSkill(moveSwingShoot2_phase3);

            swing1_phase3.AddBySkilles(spurt1_phase3, spurt2_phase3, spurt3_phase3);
            roungTargetAndShoot_phase3.AddBySkilles(spurt2_phase3,spurt3_phase3);
            Move_Three.AddSkilles(moveHeadAndShoot_phase3, roungTargetAndShoot_phase3);
            #endregion
            #region 二阶段技能
            #region 创建
            Dash dash1_phase2 = new(NPC)
            {
                dashSpeed = 20f,
                dashTime = 30,
            };
            Dash dash2_phase2 = new(NPC)
            {
                dashSpeed = 20f,
                dashTime = 40,
            };
            Dash dash3_phase2 = new(NPC)
            {
                dashSpeed = 20f,
                dashTime = 40,
            };
            Swing swing1_phase2 = new(NPC, Vector2.UnitY, MathHelper.Pi, 0.2f, 1.8f, 20)
            {
                rand = 10
            };
            Swing swing2_phase2 = new(NPC, -Vector2.UnitY, -MathHelper.Pi, 1f, 1.8f, 15)
            {
                rand = 30
            };
            Swing swing3_phase2 = new(NPC, Vector2.UnitY, MathHelper.Pi, 0.6f, 2f, 10)
            {
                rand = 30
            };
            Swing swing4_phase2 = new(NPC, -Vector2.UnitY, -MathHelper.Pi, 0.2f, 2.2f, 10)
            {
                rand = 30
            };
            MoveSwingShoot moveSwingShoot_phase2 = new(NPC, Vector2.UnitY, MathHelper.Pi, 0.3f, 2.5f, 20)
            {
                rand = 50 // 一半几率触发
            };
            Spurt spurt_phase2 = new(NPC, () => TargetPlayer.Center + (TargetPlayer.Center - NPC.Center).SafeNormalize(default) * 1000);
            RoungTargetAndShoot roungTargetAndShoot_phase2 = new(NPC);
            SkillNPC.Register(Move_Two, dash1_phase2, dash2_phase2, swing1_phase2, swing2_phase2, swing3_phase2, swing4_phase2, dash3_phase2, spurt_phase2, roungTargetAndShoot_phase2);
            #endregion
            #region 远程
            roungTargetAndShoot_phase2.AddBySkilles(swing1_phase2);
            Move_Two.AddSkill(roungTargetAndShoot_phase2);
            moveSwingShoot_phase2.AddBySkilles(swing3_phase2, dash3_phase2, spurt_phase2, roungTargetAndShoot_phase2);
            #endregion
            #region 近战

            dash3_phase2.AddSkill(spurt_phase2);
            dash2_phase2.AddSkill(spurt_phase2);
            dash1_phase2.AddSkill(spurt_phase2);
            Move_Two.AddSkill(spurt_phase2).AddSkill(spurt_phase2);
            swing3_phase2.AddSkill(dash1_phase2);
            Move_Two.AddSkill(dash1_phase2).AddSkill(dash2_phase2).AddSkill(dash3_phase2).AddSkill(swing3_phase2);
            Move_Two.AddSkill(swing1_phase2).AddSkill(swing2_phase2).AddSkill(swing3_phase2).AddSkill(swing4_phase2);
            #endregion
            #endregion
            #region 一阶段技能
            Dash dash1_phase1 = new(NPC)
            {
                dashSpeed = 20f,
                dashTime = 30,
            };
            Dash dash2_phase1 = new(NPC)
            {
                dashSpeed = 20f,
                dashTime = 40,
            };

            Swing swing1_phase1 = new(NPC, Vector2.UnitY, MathHelper.Pi, 1f, 2f, 15)
            {
                rand = 20
            };
            Swing swing2_phase1 = new(NPC, -Vector2.UnitY, -MathHelper.Pi, 0.5f, 2.3f, 20)
            {
                rand = 75
            };
            MoveSwingShoot moveSwingShoot_phase1 = new(NPC, Vector2.UnitY, MathHelper.Pi, 0.3f, 2.5f, 20)
            {
                rand = 50 // 一半几率触发
            };
            MoveHeadAndShoot moveHeadAndShoot = new(NPC);
            RoungTargetAndShoot roungTargetAndShoot = new(NPC);
            SkillNPC.Register(start, Move_One, dash1_phase1, dash2_phase1,swing1_phase1,swing2_phase1,moveSwingShoot_phase1);
            #region 链接技能
            start.AddSkill(Move_One);

            #region 远程弹幕判定
            Move_One.AddSkill(roungTargetAndShoot);
            dash2_phase1.AddSkill(roungTargetAndShoot);
            swing2_phase1.AddSkill(roungTargetAndShoot);
            moveHeadAndShoot.AddSkill(swing1_phase1);
            Move_One.AddSkill(moveHeadAndShoot).AddSkill(dash1_phase1);
            dash2_phase1.AddSkill(moveHeadAndShoot);
            swing2_phase1.AddSkill(moveHeadAndShoot);
            Move_One.AddSkill(moveSwingShoot_phase1);
            #endregion
            #region 近战攻击判定
            dash1_phase1.AddSkill(swing1_phase1);
            Move_One.AddSkill(swing1_phase1).AddSkill(swing2_phase1).AddSkill(dash1_phase1);
            Move_One.AddSkill(dash1_phase1).AddSkill(dash2_phase1).AddSkill(swing2_phase1);
            #endregion
            #endregion
            #endregion

            CurrentSkill = start;
            #endregion
        }
    }
}
