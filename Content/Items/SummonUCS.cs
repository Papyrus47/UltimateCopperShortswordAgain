using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using UltimateCopperShortsword.Content.NPCs;

namespace UltimateCopperShortsword.Content.Items
{
    public class SummonUCS : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.CopperShortsword}";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps sort inventory know that this is a boss summoning Item.

            // If this would be for a vanilla boss that has no summon item, you would have to include this line here:
            // NPCID.Sets.MPAllowedEnemies[NPCID.Plantera] = true;

            // Otherwise the UseItem code to spawn it will not work in multiplayer
        }
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.maxStack = 20;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }
        public override bool CanUseItem(Player player)
        {
            // 使用条件
            return !NPC.AnyNPCs(ModContent.NPCType<CopperShortsword>());
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<CopperShortsword>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPCDirect(player.GetSource_ItemUse(Item), player.position + new Vector2(0, -100), type);
                }
                else
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type); // 同步生成
                }
            }

            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CopperBar,10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
