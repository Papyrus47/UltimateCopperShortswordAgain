using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using UltimateCopperShortsword.Content.Projs.Bosses.UltimateCopperShortswordProj;
using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Skills
{
    public class Swing : UCS_Skills
    {
        public Vector2 StartVel;
        public float Rot;
        public float ScaleY;
        public float ScaleAll;
        public float maxTime;
        /// <summary>
        /// 不要超过7，否则会出现bug
        /// </summary>
        public int rand = 1;
        public Swing(NPC npc, Vector2 startVel, float rot, float scaleY, float scaleAll, float maxTime) : base(npc)
        {
            StartVel = startVel;
            Rot = rot;
            ScaleY = scaleY;
            ScaleAll = scaleAll;
            this.maxTime = maxTime;
        }
        public override void AI()
        {
            NPC.ai[0]++;
            if ((int)NPC.ai[0] == 30) // 生成弹幕
            {
                NPC.ai[3] = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX, ModContent.ProjectileType<SwingProj>(), NPC.damage, 0f, Target.whoAmI, 0, maxTime * 0.8f, Rot);
                Projectile projectile = Main.projectile[(int)NPC.ai[3]];
                projectile.localAI[0] = StartVel.ToRotation();
                projectile.localAI[1] = ScaleY;
                projectile.scale = ScaleAll;
            }
            else if ((int)NPC.ai[0] > 30)
            {
                NPC.velocity = Vector2.Zero;
                SyncNPC();
            }
            else
            {
                float dis = NPC.Distance(Target.position);
                if(dis > 50)
                    NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(default) * 10;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
                if((int)NPC.ai[0] == 2)
                {
                    SoundEngine.PlaySound(SoundID.Item4 with { Pitch = 0.5f }, NPC.position);
                    for (float i = 0; i <= 6.28f; i += 0.1f)
                    {
                        Dust.NewDustPerfect(NPC.Center, DustID.Copper, Vector2.One.RotatedBy(i) * 5).noGravity = true;
                    }
                }
                SyncNPC();
            }
            if (NPC.ai[0] - 30 > maxTime)
            {
                SkillTimeOut = true;
                SyncNPC();
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => NPC.ai[0] < 30;
        public override bool ActivationCondition(NPCSkills activeSkill) => Main.rand.Next(100) < rand;
        public override bool SwitchCondition(NPCSkills changeToSkill)
        {
            if ((int)NPC.ai[0] > 30)
            {
                Projectile projectile = Main.projectile[(int)NPC.ai[3]];
                if (!projectile.active || projectile.ModProjectile is not SwingProj)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
