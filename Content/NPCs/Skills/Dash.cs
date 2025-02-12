using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using UltimateCopperShortsword.Core.SkillsNPC;

namespace UltimateCopperShortsword.Content.NPCs.Skills
{
    public class Dash : UCS_Skills
    {
        public float dashSpeed = 10f;
        public int dashTime = 30;
        public Dash(NPC npc) : base(npc)
        {
        }

        public override void AI()
        {
            if ((int)NPC.ai[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item4 with { Pitch = 0.5f}, NPC.position);
                for (float i = 0; i <= 6.28f; i += 0.1f)
                {
                    Dust.NewDustPerfect(NPC.Center, DustID.Copper, Vector2.One.RotatedBy(i) * 5).noGravity = true;
                }
                SyncNPC();
            }
            NPC.ai[0]++;
            if ((int)NPC.ai[0] == dashTime * 0.2f)
            {
                NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(default) * dashSpeed;
            }
            else if((int)NPC.ai[0] < dashTime * 0.2f)
            {
                Vector2 pos = Target.Center + (Target.Center - NPC.Center).RotatedBy(0.1f).SafeNormalize(default) * 100;
                NPC.velocity = (pos - NPC.Center).SafeNormalize(default) * 9;
            }
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
            if (NPC.ai[0] > dashTime * 1.2f)
            {
                SkillTimeOut = true;
                SyncNPC();
            }
            else if (NPC.ai[0] > dashTime * 0.8f)
            {
                NPC.velocity = (NPC.velocity * 20 + (Target.Center - NPC.Center).SafeNormalize(default) * dashSpeed) / 21f;
                SyncNPC();
            }
        }
        public override bool ActivationCondition(NPCSkills activeSkill)
        {
            if(activeSkill is Dash)
            {
                return true;
            }
            return Main.rand.NextBool(2, 5);
        }

        public override bool SwitchCondition(NPCSkills changeToSkill)
        {
            return NPC.ai[0] > dashTime;
        }
    }
}
