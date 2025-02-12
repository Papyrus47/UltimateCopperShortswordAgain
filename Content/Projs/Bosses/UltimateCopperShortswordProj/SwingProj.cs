using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent;
using UltimateCopperShortsword.Content.NPCs;
using UltimateCopperShortsword.Content.NPCs.Modes;
using UltimateCopperShortsword.Core.SwingHelper;

namespace UltimateCopperShortsword.Content.Projs.Bosses.UltimateCopperShortswordProj
{
    /// <summary>
    /// ai0:time
    /// ai1:maxTime
    /// ai2:Rotation
    /// localai0:挥舞起始向量的弧度
    /// localai1:挥舞Y缩放向量
    /// scale:长度缩放
    /// </summary>
    public class SwingProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.CopperShortsword}";
        public SwingHelper swingHelper;
        public CopperShortsword copperShortsword;
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.extraUpdates = 4;
            Projectile.aiStyle = -1;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            swingHelper.SendData(writer);
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(Projectile.localAI[2]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            swingHelper.RendData(reader);
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            Projectile.localAI[2] = reader.ReadSingle();
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1f;
            if (Main.expertMode)
                modifiers.SourceDamage /= 2;
            if (Main.masterMode)
                modifiers.SourceDamage /= 3;
        }
        public override void OnKill(int timeLeft)
        {
            swingHelper = null;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            if (copperShortsword != null && !copperShortsword.NPC.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Projectile.rotation = 0;
            Player player = Main.player[Projectile.owner];
            if (swingHelper == null) // 注册化
            {
                swingHelper = new(Projectile, 40, TextureAssets.Item[ItemID.CopperShortsword])
                {
                    DrawTrailCount = 3,
                    DrawItem_ScaleMoreThanOne = false
                };
                Vector2 vector2 = new(1, Projectile.localAI[1]);
                swingHelper.Change(Projectile.localAI[0].ToRotationVector2(), vector2 * Projectile.scale, vector2.Y - 1);
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 vel = (player.Center - Projectile.Center).SafeNormalize(default);
                    Projectile.localAI[2] = (vel.X > 0).ToDirectionInt();
                    swingHelper.SetRotVel((int)Projectile.localAI[2] == 1 ? (Projectile.Center - player.Center).ToRotation() : -(player.Center - Projectile.Center).ToRotation());
                    Projectile.netUpdate = true;
                }
                SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
            }

            Projectile.direction = (int)Projectile.localAI[2];
            Projectile.spriteDirection = Projectile.direction * (Projectile.ai[2] >= 0).ToDirectionInt();

            Projectile.ai[0]++;
            if ((int)Projectile.ai[0] % 5 == 0)
            {
                Projectile.netUpdate = true;
            }
            float time = Projectile.ai[0] / (Projectile.ai[1] * (Projectile.extraUpdates + 1));
            if (time > 1f)
            {
                Projectile.Kill();
                return;
            }
            time = MathHelper.SmoothStep(0, 1f, MathF.Pow(time, 2.5f));

            //swingHelper.ProjFixedPos(Projectile.Center);
            swingHelper.SetSwingActive();
            swingHelper.SwingAI(Projectile.Size.Length() * Projectile.scale, Projectile.direction, time * Projectile.ai[2]);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            if (copperShortsword != null && !copperShortsword.NPC.active)
                return false;
            if (copperShortsword == null)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.ModNPC is CopperShortsword shortsword)
                    {
                        copperShortsword = shortsword;
                        break;
                    }
                }
            }
            else
            {
                if(copperShortsword.CurrentMode is TwoLevel)
                {
                    lightColor = Color.Lerp(Color.Green, lightColor, 0.5f);
                }
                else if(copperShortsword.CurrentMode is ThreeLevel)
                {
                    lightColor = Color.Green;
                }
            }
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, TextureAssets.Extra[209].Value, (facotr) =>
            {
                if (copperShortsword.CurrentMode is TwoLevel)
                {
                    return Color.Lerp(Color.OrangeRed, Color.Green, facotr) with { A = 0 };
                }
                else if (copperShortsword.CurrentMode is ThreeLevel)
                {
                    return Color.Lerp(Color.Green, Color.OrangeRed, facotr) with { A = 0 };
                }
                return Color.Lerp(Color.OrangeRed, Color.Purple, facotr) with { A = 0 };
            });
            return false;
        }
    }
}
