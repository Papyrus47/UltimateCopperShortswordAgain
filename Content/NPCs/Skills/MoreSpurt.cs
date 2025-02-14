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
    public class MoreSpurt : UCS_Skills
    {
        public MoreSpurt(NPC npc) : base(npc)
        {
        }
        public override void AI()
        {
            NPC.ai[0]++;
            Vector2 vel = Target.Center - NPC.Center;
            if (NPC.ai[0] < 10)
            {
                if(Main.netMode != NetmodeID.Server)
                {
                    SoundEngine.PlaySound(SoundID.Item28 with { Pitch = -0.5f }, NPC.position);
                    Color color = Color.OrangeRed;
                    if (copperShortsword.CurrentMode is ThreeLevel)
                        color = Color.Green;
                    for (float i = 0; i <= 6.28f; i += 0.1f)
                    {
                        Dust.NewDustPerfect(NPC.Center, DustID.FireworksRGB, Vector2.One.RotatedBy(i) * 5, 0, color).noGravity = true;
                    }
                }
                if (vel.Length() > 200)
                    NPC.velocity = (NPC.velocity * 10 + vel * 0.1f) / 11f;
                else
                    NPC.velocity *= 0.7f;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver4;
            }
            else if (NPC.ai[0] < 60)
            {

                if ((int)NPC.ai[0] % 2 == 0)
                {
                    NPC.velocity = NPC.velocity.SafeNormalize(default) * 45f;
                    if (Main.netMode != NetmodeID.Server) // 不在服务器播放声音
                        SoundEngine.PlaySound(SoundID.Item1 with { Pitch = 0.5f }, NPC.position);
                    if(Main.netMode != NetmodeID.MultiplayerClient) // 不在客户端产生弹幕
                    {
                        Color color = Color.OrangeRed;
                        if (copperShortsword.CurrentMode is ThreeLevel)
                            color = Color.Green;

                        float angle = NPC.velocity.RotatedByRandom(0.6f).ToRotation();
                        NPC.rotation = angle + MathHelper.PiOver4;
                        NPC.netUpdate = true;
                        float scale = Main.rand.NextFloat(3, 7);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.velocity.SafeNormalize(default) * scale * 30, NPC.velocity.SafeNormalize(default) * 5, ModContent.ProjectileType<StabProj>(), NPC.damage, 0, Target.whoAmI,angle , Main.rgbToHsl(color).X, scale);
                        if ((int)NPC.ai[0] % 3 == 0)
                            Shoot(default, angle.ToRotationVector2(), 4);
                    }
                }
                else
                    NPC.velocity = NPC.velocity.SafeNormalize(default) * 5f;
            }
            else if (NPC.ai[0] > 90)
            {
                SkillTimeOut = true;
            }
        }
        public override bool ActivationCondition(NPCSkills activeSkill)
        {
            Vector2 vel = NPC.Center - Target.Center;
            float dis = vel.Length();
            var array = copperShortsword.OldSkills.ToList();
            if (activeSkill is not MoreSpurt)
            {
                if (array.Count > 6 && array.FindAll(x => x is not MoreSpurt).Count > 6)
                    return true;
                return Main.rand.NextFloat(10) * 20 > dis;
            }
            else
            {
                if (array.Count > 3 && array.FindAll(x => x is MoreSpurt).Count < 3)
                    return true;
            }
            return false;
        }
        public override bool SwitchCondition(NPCSkills changeToSkill) => NPC.ai[0] > 70;
        public override void OnSkillActive(NPCSkills activeSkill)
        {
            base.OnSkillActive(activeSkill);
        }
    }
}
