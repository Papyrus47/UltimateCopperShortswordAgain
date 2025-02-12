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
            Projectile.extraUpdates = 4;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
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
            return false;
        }
    }
    public class UCSProj1 : UCSProj
    {

    }
}
