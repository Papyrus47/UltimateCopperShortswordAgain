using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;

namespace UltimateCopperShortsword.Content.Projs.Bosses.UltimateCopperShortswordProj
{
    public abstract class UCSProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * MathF.Pow(Projectile.velocity.Length() + Projectile.ai[0] * 5, 0.5f);
        }
        public override bool ShouldUpdatePosition() => Projectile.ai[0] >= 30;
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1f;
            if (Main.expertMode)
                modifiers.SourceDamage /= 2;
            if (Main.masterMode)
                modifiers.SourceDamage /= 3;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            for(int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos2 = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size * 0.5f;
                sb.Draw(texture, drawPos2, null, Color.Lerp(Color.White, Color.Purple, i / (float)Projectile.oldPos.Length) * (1
                    - (i / (float)Projectile.oldPos.Length)), Projectile.oldRot[i], texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            sb.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale,
                    SpriteEffects.None, 0);

            #region 预警线
            if (Projectile.ai[0] < 30)
            {
                Color color = Color.OrangeRed;
                if (this is UCSProj2)
                    color = Color.LightGreen;
                Color drawColor = color * MathHelper.SmoothStep(0, 1f, Projectile.ai[0] / 30f - 0.5f);
                Utils.DrawLine(sb, Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(default) * 10000, drawColor, drawColor, 4);
            }
            #endregion
            return false;
        }
    }
    public class UCSProj1 : UCSProj
    {

    }
    public class UCSProj2 : UCSProj
    {

    }
}
