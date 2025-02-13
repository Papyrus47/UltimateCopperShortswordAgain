using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using UltimateCopperShortsword.Content.NPCs.Modes;
using UltimateCopperShortsword.Content.Projs.Bosses.UltimateCopperShortswordProj;

namespace UltimateCopperShortsword.Content.NPCs.Skills
{
    public class SwingShoot : Swing
    {
        public SwingShoot(NPC npc, Vector2 startVel, float rot, float scaleY, float scaleAll, float maxTime) : base(npc, startVel, rot, scaleY, scaleAll, maxTime)
        {
        }

        public override void AI()
        {
            base.AI();

            if (Main.netMode != NetmodeID.MultiplayerClient) // 非客户端模式
            {
                if ((int)NPC.ai[0] > 30)
                {
                    Projectile projectile = Main.projectile[(int)NPC.ai[3]];
                    if (!projectile.active || projectile.ModProjectile is not SwingProj)
                        return;
                    Shoot(default, projectile.velocity.SafeNormalize(default), 15);

                }
                else if (NPC.ai[0] < 30 && (int)NPC.ai[0] % 3 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item122 with { Pitch = 0.5f }, NPC.position);
                    Color color = Color.OrangeRed;
                    if (copperShortsword.CurrentMode is ThreeLevel)
                        color = Color.Green;
                    for (float i = 0; i <= 6.28f; i += 0.1f)
                    {
                        Dust.NewDustPerfect(NPC.Center, DustID.FireworksRGB, Vector2.One.RotatedBy(i) * 5, 0, color).noGravity = true;
                    }
                }
            }
        }
    }
}
