using Terraria.Graphics.Effects;
using UltimateCopperShortsword.Assets.Skys;

namespace UltimateCopperShortsword
{
    // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public class UltimateCopperShortsword : Mod
    {
        public override void Load()
        {
            base.Load();
            SkyManager.Instance[nameof(UCSSky)] = new UCSSky();//加载我们写的天空
        }
    }
}
