
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace UltimateCopperShortsword.Assets.Skys
{
    public class UCSSky : CustomSky //霓虹灯天空
    {
        public static Color DrawColor; // 设置的背景颜色
        public static int Timeleft = 100; //弄一个计时器，让天空能自己消失
        public override void Update(GameTime gameTime)//天空激活时的每帧更新函数
        {
            if (!Main.gamePaused)//游戏暂停时不执行
            {
                if (Timeleft > 0) 
                    Timeleft--;//只要激活时就会减少，这样就会在外部没赋值时自己消失了
                else
                {
                    if (SkyManager.Instance[nameof(UCSSky)].IsActive())
                    {
                        SkyManager.Instance.Deactivate(nameof(UCSSky));//消失
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (minDepth < 9 && maxDepth > 9)//绘制在背景景物后面，防止遮挡，当然你想的话，也可以去掉这个条件
            {
                Texture2D sky = ModContent.Request<Texture2D>(GetType().Namespace.Replace('.','/') + "/SkyEffect").Value;
                spriteBatch.Draw(sky, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),null, DrawColor * (Timeleft / 20f));
                //把一条带状的图片填满屏幕
            }
        }
        public override float GetCloudAlpha()
        {
            return 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
        }

        public override void Deactivate(params object[] args)
        {

        }

        public override void Reset()
        {

        }

        public override bool IsActive() => Timeleft > 0;

    }

}