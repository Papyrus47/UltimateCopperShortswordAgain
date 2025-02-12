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
        public Player TargetPlayer => Main.player[NPC.FindClosestPlayer()];
        public static int Music1;
        public static int Music2;
        public static int Music3;
        /// <summary>
        /// 一阶段游走
        /// </summary>
        public Move Move_One;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Music1 = MusicLoader.GetMusicSlot("UltimateCopperShortsword/Assets/Sounds/Music/Phase1");
            Music2 = MusicLoader.GetMusicSlot("UltimateCopperShortsword/Assets/Sounds/Music/Phase2");
            Music3 = MusicLoader.GetMusicSlot("UltimateCopperShortsword/Assets/Sounds/Music/Phase3");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 9;
            NPC.defense = 16;
            NPC.lifeMax = 66000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 100000;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            Music = Music1;
        }
        public override void AI()
        {
            base.AI();
            if(CurrentMode is TwoLevel)
            {
                Music = Music2;
            }
            else if(CurrentMode is ThreeLevel)
            {
                Music = Music3;
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            base.ModifyHitPlayer(target, ref modifiers);
            modifiers.ScalingArmorPenetration += 1;
        }
        public override void OnSkillTimeOut()
        {
            OldSkills.Enqueue(CurrentSkill);
            if (OldSkills.Count > 10)
                OldSkills.Dequeue();
            if (CurrentMode is TwoLevel)
            {
                Move_One.OnSkillActive(CurrentSkill);
                CurrentSkill.OnSkillDeactivate(Move_One);
                CurrentSkill = Move_One;
            }
            else if(CurrentMode is ThreeLevel)
            {
                Move_One.OnSkillActive(CurrentSkill);
                CurrentSkill.OnSkillDeactivate(Move_One);
                CurrentSkill = Move_One;
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
            Move_One = new(NPC, 10);

            Dash dash1_phase1 = new(NPC)
            {
                dashSpeed = 10f,
                dashTime = 60,
            };
            Dash dash2_phase1 = new(NPC)
            {
                dashSpeed = 10f,
                dashTime = 60,
            };

            Swing swing1_phase1 = new(NPC, Vector2.UnitY, MathHelper.Pi, 1f, 2f, 15)
            {
                rand = 30
            };
            Swing swing2_phase1 = new(NPC, -Vector2.UnitY, -MathHelper.Pi, 0.5f, 2f, 20)
            {
                rand = 75
            };
            SkillNPC.Register(start, Move_One, dash1_phase1, dash2_phase1,swing1_phase1,swing2_phase1);
            #region 链接技能
            start.AddSkill(Move_One);

            Move_One.AddSkill(swing1_phase1).AddSkill(swing2_phase1);
            Move_One.AddSkill(dash1_phase1).AddSkill(dash2_phase1);
            #endregion

            CurrentSkill = start;
            #endregion
        }
    }
}
