using Microsoft.Xna.Framework.Graphics;
using TerraSticker.UIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerraSticker.Utils
{
    public static class UIUtiles
    {

        public static void DrawButtonsOnUIElement(this UIElement parent, Dictionary<int, UIStickerManagerButton> stickersList, int pages)
        {
            float imageHeight = 250;
            int index = 0;
            int itemsPerPage = 10;
            int startIndex = pages * itemsPerPage;
            int endIndex = startIndex + itemsPerPage;

            foreach (var sticker in stickersList)
            {
                if (index >= startIndex && index < endIndex)
                {
                    int positionIndex = index - startIndex;
                    var modulo = positionIndex % 5;
                    var quotient = positionIndex / 5;
                    sticker.Value.Left.Set((parent.Width.Pixels - 1000) / 6f * (modulo + 1) + 200 * modulo, parent.Width.Precent / 6f * (modulo + 1));
                    sticker.Value.Top.Set(quotient * (imageHeight + 20), 0f);
                    parent.Append(sticker.Value);
                }
                index++;
            }
        }
    }
}
