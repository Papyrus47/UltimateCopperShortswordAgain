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
    public class ChargedSlash : Swing
    {
        public ChargedSlash(NPC npc, Vector2 startVel, float rot, float scaleY, float scaleAll, float maxTime) : base(npc, startVel, rot, scaleY, scaleAll, maxTime)
        {
        }
        public override void AI()
        {
            NPC.ai[0]++;
            if ((int)NPC.ai[0] == 30 && Main.netMode != NetmodeID.MultiplayerClient) // 生成弹幕
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
                if ((int)NPC.ai[0] % 2 == 0)
                    NPC.ai[0] -= 0.5f;
                if (copperShortsword.DamagePool <= copperShortsword.DamagePoolMax * 0.2f)
                {
                    SkillTimeOut = true;
                    return;
                }
                float dis = NPC.Distance(Target.position);
                if (dis > 600)
                    NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(default) * 10;
                else
                    NPC.velocity = (NPC.velocity * 10 + (Target.Center - NPC.Center).SafeNormalize(default)) / 11f;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
                if ((int)NPC.ai[0] == 2 || (int)NPC.ai[0] == 20)
                {
                    SoundEngine.PlaySound(SoundID.Item122 with { Pitch = -0.5f }, NPC.position);
                    Color color = Color.OrangeRed;
                    if (copperShortsword.CurrentMode is ThreeLevel)
                        color = Color.Green;
                    for (float i = 0; i <= 6.28f; i += 0.1f)
                    {
                        Dust.NewDustPerfect(NPC.Center, DustID.FireworksRGB, Vector2.One.RotatedBy(i) * 5, 0, color).noGravity = true;
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
    }
}
