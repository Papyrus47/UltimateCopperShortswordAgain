using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;

namespace LogSpiralLibrary.ForFun.StabProj
{
    public class StabProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 15;
            Projectile.width = Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            base.SetDefaults();
        }
        public override void AI()
        {
            base.AI();
            if ((int)Projectile.ai[2] == 0)
            {
                Projectile.ai[2] = 70;
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1f;
            if (Main.expertMode)
                modifiers.SourceDamage /= 2;
            if (Main.masterMode)
                modifiers.SourceDamage /= 3;
        }
        public virtual bool Colliding(Rectangle targetRect)
        {
            for (float i = 0; i <= 1f; i += 0.03f)
            {
                Rectangle rect = new((int)(Projectile.position.X - Projectile.Size.X / 2), (int)(Projectile.position.Y - Projectile.Size.Y / 2), (int)Projectile.Size.X, (int)Projectile.Size.Y);
                rect.Offset((int)(Projectile.ai[0].ToRotationVector2().X * Projectile.width * Projectile.ai[2] * 0.5f * i), (int)(Projectile.ai[0].ToRotationVector2().Y * Projectile.height * Projectile.ai[2] * 0.5f * i));
                if (targetRect.Intersects(rect))
                {
                    return true;
                }
            }
            return false;
        }
        public override string Texture => "Terraria/Images/Extra_98";
        public override bool PreDraw(ref Color lightColor)
        {
            float t = Projectile.timeLeft / 15f;
            float fac = (1 - MathF.Cos(MathHelper.TwoPi * MathF.Sqrt(t))) * .5f;
            Vector2 unit = Projectile.ai[0].ToRotationVector2();
            Color mainColor = Main.hslToRgb(new(Projectile.ai[1], 1, 0.5f));
            Main.EntitySpriteDraw(TextureAssets.Extra[98].Value, Projectile.Center - Main.screenPosition, null, mainColor with { A = 0 } * fac,  Projectile.ai[0] + MathHelper.PiOver2, new(36), new Vector2(1.5f, Projectile.ai[2]) * fac * Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(TextureAssets.Extra[98].Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * fac, Projectile.ai[0] + MathHelper.PiOver2, new(36), new Vector2(1.5f, Projectile.ai[2]) * fac * .75f * Projectile.scale, 0, 0);
            //Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 1, 1), Color.HotPink, 0, new Vector2(.5f), 16, 0, 0);
            return false;
        }
    }
}
