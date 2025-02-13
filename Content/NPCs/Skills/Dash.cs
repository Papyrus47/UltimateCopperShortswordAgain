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
                SoundEngine.PlaySound(SoundID.Item122 with { Pitch = 0.5f}, NPC.position);
                Color color = Color.OrangeRed;
                if (copperShortsword.CurrentMode is ThreeLevel)
                    color = Color.Green;
                for (float i = 0; i <= 6.28f; i += 0.1f)
                {
                    Dust.NewDustPerfect(NPC.Center, DustID.FireworksRGB, Vector2.One.RotatedBy(i) * 5,0, color).noGravity = true;
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
                Vector2 pos = Target.Center + (NPC.Center - Target.Center).RotatedBy(0.01f).SafeNormalize(default) * 200;
                NPC.velocity = (NPC.velocity * 20 + (pos - NPC.Center).SafeNormalize(default) * 16f) / 21f;
            }
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
            if (NPC.ai[0] > dashTime * 1.1f)
            {
                SkillTimeOut = true;
                SyncNPC();
            }
            else if (NPC.ai[0] > dashTime * 0.8f)
            {
                NPC.velocity = (NPC.velocity * 20 + (NPC.Center - Target.Center).SafeNormalize(default) * dashSpeed * 0.5f) / 21f;
                SyncNPC();
            }
        }
        public override bool ActivationCondition(NPCSkills activeSkill)
        {
            if(NPC.Distance(Target.position) > 600)
                return false;
            if(activeSkill is Dash)
            {
                return true;
            }
            return Main.rand.Next(10) < 2;
        }

        public override bool SwitchCondition(NPCSkills changeToSkill)
        {
            return NPC.ai[0] > dashTime;
        }
    }
}
