using TerraSticker.Players;
using TerraSticker.UIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraSticker.Items
{
    public class StickerBubble : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item75;
            Item.useStyle = ItemUseStyleID.HoldUp;


        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Terrarium, 1);
            recipe.AddIngredient(ItemID.Bunny, 10);
            recipe.AddIngredient(ItemID.Cloud, 10);
            recipe.AddIngredient(ItemID.Marshmallow, 10);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }


        public override bool CanUseItem(Player player)
        {
            StickerManagerUISystem Stickers = ModContent.GetInstance<StickerManagerUISystem>();
            // 如果 UI 是活动的，那么返回 false，否则返回 true
            return Stickers.IsCurrentStateNull();
        }

        public override bool? UseItem(Player player)
        {
            if (player == Main.LocalPlayer)
            {
                ModContent.GetInstance<StickerManagerUISystem>().RefreshManagerUI();
            }
            return true;
        }
    }
}
