using LogSpiralLibrary.ForFun.StabProj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using UltimateCopperShortsword.Content.NPCs.Modes;
using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Skills
{
    public class Spurt : UCS_Skills
    {
        /// <summary>
        /// 突刺到的位置
        /// </summary>
        public Func<Vector2> SpurtToPos;
        public Spurt(NPC npc, Func<Vector2> spurtToPos) : base(npc)
        {
            SpurtToPos = spurtToPos;
        }
        public override void AI()
        {
            NPC.ai[0]++;
            Vector2 pos = new Vector2(NPC.ai[1], NPC.ai[2]);
            if (NPC.ai[0] < 60)
            {
                if (NPC.ai[0] < 58)
                {
                    NPC.ai[1] = SpurtToPos().X;
                    NPC.ai[2] = SpurtToPos().Y;
                }
                if ((int)NPC.ai[0] == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item28 with { Pitch = 1f }, NPC.position);
                }
                if (copperShortsword.OldSkills.Count > 0 && copperShortsword.OldSkills.ToArray()[^1] is Spurt && NPC.ai[0] < 40)
                {
                    NPC.ai[0] = 40;
                }
                NPC.rotation += 0.5f;
                NPC.velocity = (NPC.velocity * 10 + (pos - NPC.Center).SafeNormalize(default) * 5) / 11f;

            }
            else if (NPC.ai[0] < 80)
            {
                SyncNPC();
                NPC.velocity = (pos - NPC.Center) * 0.1f;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
                if ((int)NPC.ai[0] == 61 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Color color = Color.OrangeRed;
                    if (copperShortsword.CurrentMode is ThreeLevel)
                        color = Color.Green;
                    
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.velocity * 9, NPC.velocity.SafeNormalize(default), ModContent.ProjectileType<StabProj>(), 0, 0, Target.whoAmI,NPC.velocity.ToRotation(),Main.rgbToHsl(color).X);
                }
            }
            else if (NPC.ai[0] > 120)
            {
                SkillTimeOut = true;
            }
            else if (NPC.ai[0] < 120)
            {
                NPC.velocity *= 0.9f;
            }
        }
        public override bool ActivationCondition(NPCSkills activeSkill)
        {
            Vector2 vel = NPC.Center - Target.Center;
            float dis = vel.Length();
            if (activeSkill is not Spurt)
                return Main.rand.NextFloat(10) * 90 < dis;
            else
            {
                if (activeSkill == this)
                    return Main.rand.NextBool(5, 100);
                else
                    return true;
            }
            return false;
        }
        public override bool SwitchCondition(NPCSkills changeToSkill) => NPC.ai[0] > 100;
        public override void OnSkillActive(NPCSkills activeSkill)
        {
            base.OnSkillActive(activeSkill);
            Vector2 vel = NPC.Center - Target.Center;
            float dis = vel.Length();
            NPC.velocity = vel.SafeNormalize(Vector2.UnitX) * (400 / dis);
        }
    }
}
