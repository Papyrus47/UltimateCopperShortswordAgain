using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateCopperShortsword.Content.NPCs.Modes;
using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Skills
{
    public class RoungTargetAndShoot : UCS_Skills
    {
        public RoungTargetAndShoot(NPC npc) : base(npc)
        {
        }
        public override void AI()
        {
            Vector2 toTarget = (Target.Center - NPC.Center);
            if (NPC.ai[0]++ < 30f) // 小于30帧,飞向玩家头上
            {
                float dis = 700;
                if (copperShortsword.CurrentMode is TwoLevel)
                    dis = 500;
                else if(copperShortsword.CurrentMode is ThreeLevel)
                    dis = 450;
                Vector2 vector2 = Target.Center - (Target.Center - NPC.Center).SafeNormalize(default) * dis - NPC.Center;
                if (vector2.Length() > 100f)
                    NPC.velocity = vector2 * 0.3f;
                else
                    NPC.velocity = (NPC.velocity * 3f + toTarget.SafeNormalize(default) * 10f) / 4f;
            }
            else if (NPC.ai[0] < 90f) // 射一秒弹幕,半秒可以截断
            {
                Vector2 pos = Target.Center + (NPC.Center - Target.Center).RotatedBy(1 / 60f * MathHelper.TwoPi);
                NPC.velocity = pos - NPC.Center;
                if ((int)NPC.ai[0] % 5 == 0)
                {
                    SyncNPC();
                    float speed = 8;
                    if (copperShortsword.CurrentMode is TwoLevel)
                        speed = 6;
                    else if (copperShortsword.CurrentMode is ThreeLevel)
                        speed = 4;
                    Shoot(default, (Target.Center - NPC.Center).SafeNormalize(default), speed);
                }
            }
            else
            {
                SkillTimeOut = true;
            }
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
        }
        public override bool ActivationCondition(NPCSkills activeSkill) => Main.rand.Next(15) < (Target.Center - NPC.Center).Length() / 100;
        public override bool SwitchCondition(NPCSkills changeToSkill) => false;
    }
}
