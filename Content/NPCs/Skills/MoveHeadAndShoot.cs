using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Skills
{
    public class MoveHeadAndShoot : UCS_Skills
    {
        public MoveHeadAndShoot(NPC npc) : base(npc)
        {
        }
        public override void AI()
        {
            Vector2 toTarget = (Target.Center - NPC.Center);
            if (NPC.ai[0]++ < 30f) // 小于30帧,飞向玩家头上
            {
                Vector2 vector2 = Target.Center - Vector2.UnitY * 500 - NPC.Center;
                if (vector2.Length() > 100f)
                    NPC.velocity = vector2.SafeNormalize(default) * 30;
                else
                    NPC.velocity = (NPC.velocity * 3f + toTarget.SafeNormalize(default) * 20f) / 4f;
            }
            else if (NPC.ai[0] < 90f) // 射一秒弹幕,半秒可以截断
            {
                NPC.velocity = (NPC.velocity * 2f + toTarget.SafeNormalize(default) * 3) / 3f;
                if ((int)NPC.ai[0] % 7 == 0)
                {
                    SyncNPC();
                    for (int i = -1; i <= 1; i++)
                    {
                        Shoot(default, NPC.velocity.SafeNormalize(default).RotatedBy(i * 0.3f), 8);
                    }
                }
            }
            else
            {
                SkillTimeOut = true;
            }
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
        }
        public override bool ActivationCondition(NPCSkills activeSkill) => Main.rand.Next(10) < (Target.Center - NPC.Center).Length() / 100;
        public override bool SwitchCondition(NPCSkills changeToSkill)
        {
            return NPC.ai[0] >= 60;
        }
    }
}
