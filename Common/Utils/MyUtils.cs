﻿using ImproveGame.Common.Configs;
using ImproveGame.Common.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static ImproveGame.Common.GlobalItems.ImproveItem;
using static Microsoft.Xna.Framework.Vector2;

namespace ImproveGame
{
    /// <summary>
    /// 局长自用工具
    /// </summary>
    partial class MyUtils
    {
        public static Vector2 GetStringSize(string text) {
            return FontAssets.MouseText.Value.MeasureString(text);
        }

        public static void DrawString(Vector2 position, string text, Color textColor, Color borderColor, float scale = 1f) {
            DrawString(position, text, textColor, borderColor, Zero, scale);
        }

        public static void DrawString(Vector2 position, string text, Color textColor, Color borderColor, Vector2 origin, float scale) {
            var sb = Main.spriteBatch;
            var font = FontAssets.MouseText.Value;
            Utils.DrawBorderStringFourWay(sb, font, text, position.X, position.Y, textColor, borderColor, origin, scale);
        }

        public static Color[] GetColors(Texture2D texture) {
            var w = texture.Width;
            var h = texture.Height;
            var cs = new Color[w * h]; // 创建一个能容下整个贴图颜色信息的 Color[]
            texture.GetData(cs); // 获取颜色信息
            return cs;
        }

        /// <summary>
        /// 旋转物品使用时候的贴图
        /// </summary>
        /// <param name="player">被操作的玩家实例</param>
        /// <param name="shouldSync">是否应该进行网络同步</param>
        public static void ItemRotation(Player player, bool shouldSync = true) {
            // 旋转物品
            Vector2 rotaion = (Main.MouseWorld - player.Center).SafeNormalize(Zero);
            player.direction = Main.MouseWorld.X < player.Center.X ? -1 : 1;
            player.itemRotation = MathF.Atan2(rotaion.Y * player.direction, rotaion.X * player.direction);
            //if (shouldSync && Main.netMode != NetmodeID.SinglePlayer) {
            //    NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
            //    NetMessage.SendData(MessageID.ItemAnimation, -1, -1, null, player.whoAmI);
            //}
        }

        /// <summary>
        /// 限制 Rect 的大小
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>end的位置</returns>
        public static Point LimitRect(Point start, Point end, int width, int height) {
            width--;
            height--;
            if (end.X - start.X < -width)
                end.X = start.X - width;
            else if (end.X - start.X > width)
                end.X = start.X + width;

            if (end.Y - start.Y < -height)
                end.Y = start.Y - height;
            else if (end.Y - start.Y > height)
                end.Y = start.Y + height;
            return end;
        }

        public static void DrawBorderRect(Rectangle tileRectangle, Color backgroundColor, Color borderColor) {
            Texture2D texture = TextureAssets.MagicPixel.Value;
            Vector2 position = tileRectangle.TopLeft() * 16f - Main.screenPosition;
            Vector2 scale = new(tileRectangle.Width, tileRectangle.Height);
            Main.spriteBatch.Draw(
                    texture,
                    position,
                    new(0, 0, 1, 1),
                    backgroundColor,
                    0f,
                    Zero,
                    16f * scale,
                    SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(
                texture,
                position + UnitX * -2f + UnitY * -2f,
                new(0, 0, 1, 1),
                borderColor, 0f, Zero,
                new Vector2(2f, 16f * scale.Y + 4),
                SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture,
                position + UnitX * 16f * scale.X + UnitY * -2f,
                new(0, 0, 1, 1),
                borderColor, 0f, Zero,
                new Vector2(2f, 16f * scale.Y + 4), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture,
                position + UnitY * -2f,
                new(0, 0, 1, 1),
                borderColor, 0f, Zero,
                new Vector2(16f * scale.X, 2f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture,
                position + UnitY * 16f * scale.Y,
                new(0, 0, 1, 1),
                borderColor, 0f, Zero,
                new Vector2(16f * scale.X, 2f), SpriteEffects.None, 0f);
        }

        /// <summary>
        /// 获取 HJson 文字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetText(string str, params object[] arg) {
            string text = Language.GetTextValue($"Mods.ImproveGame.{str}", arg);

            // 支持输入<left>和<right>，就和ItemTooltip一样（原版只有Tooltip支持）
            if (text.Contains("<right>")) {
                InputMode inputMode = InputMode.XBoxGamepad;
                if (PlayerInput.UsingGamepad)
                    inputMode = InputMode.XBoxGamepadUI;

                if (inputMode == InputMode.XBoxGamepadUI) {
                    KeyConfiguration keyConfiguration = PlayerInput.CurrentProfile.InputModes[inputMode];
                    string input = PlayerInput.BuildCommand("", true, keyConfiguration.KeyStatus["MouseRight"]);
                    input = input.Replace(": ", "");
                    text = text.Replace("<right>", input);
                }
                else {
                    text = text.Replace("<right>", Language.GetTextValue("Controls.RightClick"));
                }
            }
            if (text.Contains("<left>")) {
                InputMode inputMode2 = InputMode.XBoxGamepad;
                if (PlayerInput.UsingGamepad)
                    inputMode2 = InputMode.XBoxGamepadUI;

                if (inputMode2 == InputMode.XBoxGamepadUI) {
                    KeyConfiguration keyConfiguration2 = PlayerInput.CurrentProfile.InputModes[inputMode2];
                    string input = PlayerInput.BuildCommand("", true, keyConfiguration2.KeyStatus["MouseLeft"]);
                    input = input.Replace(": ", "");
                    text = text.Replace("<left>", input);
                }
                else {
                    text = text.Replace("<left>", Language.GetTextValue("Controls.LeftClick"));
                }
            }

            return text;
        }

        public static Asset<Texture2D> GetTexture(string path) {
            return ModContent.Request<Texture2D>($"ImproveGame/Assets/Images/{path}", AssetRequestMode.ImmediateLoad);
        }

        public static Asset<Effect> GetEffect(string path) {
            return ModContent.Request<Effect>($"ImproveGame/Assets/Effect/{path}", AssetRequestMode.ImmediateLoad);
        }

        /// <summary>
        /// 将存储float液体量转换为原版int整数液体量
        /// <br>0.5%(0.005f) -> 255</br>
        /// </summary>
        /// <param name="amount">float液体量</param>
        /// <returns>整数液体量</returns>
        public static int LiquidAmountToInt(float amount) => (int)Math.Round(amount / 0.005f * 255);

        /// <summary>
        /// 将原版int整数液体量转换为存储float液体量
        /// <br>255 -> 0.05%(0.005f)</br>
        /// </summary>
        /// <param name="amount">整数液体量</param>
        /// <returns>float液体量</returns>
        public static float LiquidAmountToFloat(int amount) => amount / 255f * 0.005f;

        /// <summary>
        /// 绘制一个方框
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="dimensions"></param>
        /// <param name="texture"></param>
        /// <param name="color"></param>
        public static void DrawPanel(SpriteBatch sb, CalculatedStyle dimensions, Texture2D texture, Color color) {
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Point point2 = new Point(point.X + (int)dimensions.Width - 12, point.Y + (int)dimensions.Height - 12);
            int width = point2.X - point.X - 12;
            int height = point2.Y - point.Y - 12;
            sb.Draw(texture, new Rectangle(point.X, point.Y, 12, 12), new Rectangle(0, 0, 12, 12), color);
            sb.Draw(texture, new Rectangle(point2.X, point.Y, 12, 12), new Rectangle(12 + 4, 0, 12, 12), color);
            sb.Draw(texture, new Rectangle(point.X, point2.Y, 12, 12), new Rectangle(0, 12 + 4, 12, 12), color);
            sb.Draw(texture, new Rectangle(point2.X, point2.Y, 12, 12), new Rectangle(12 + 4, 12 + 4, 12, 12), color);
            sb.Draw(texture, new Rectangle(point.X + 12, point.Y, width, 12), new Rectangle(12, 0, 4, 12), color);
            sb.Draw(texture, new Rectangle(point.X + 12, point2.Y, width, 12), new Rectangle(12, 12 + 4, 4, 12), color);
            sb.Draw(texture, new Rectangle(point.X, point.Y + 12, 12, height), new Rectangle(0, 12, 12, 4), color);
            sb.Draw(texture, new Rectangle(point2.X, point.Y + 12, 12, height), new Rectangle(12 + 4, 12, 12, 4), color);
            sb.Draw(texture, new Rectangle(point.X + 12, point.Y + 12, width, height), new Rectangle(12, 12, 4, 4), color);
        }

        /// <summary>
        /// 堆叠物品到仓库
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="item"></param>
        /// <returns>堆叠后剩余的物品</returns>
        public static Item ItemStackToInventory(Item[] inventory, Item item, bool hint = true, int EndIndex = -1) {
            EndIndex = (EndIndex == -1 ? inventory.Length : EndIndex);
            // 先填充和物品相同的
            for (int i = 0; i < EndIndex; i++) {
                item = ItemStackToInventoryItem(inventory, i, item, hint);
                if (item.IsAir) {
                    Recipe.FindRecipes();
                    return item;
                }
            }
            // 后填充空位
            for (int i = 0; i < EndIndex; i++) {
                if (inventory[i].IsAir) {
                    if (hint) {
                        PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, item, item.stack);
                        SoundEngine.PlaySound(SoundID.Grab);
                    }
                    (inventory[i], item) = (item, inventory[i]);
                    Recipe.FindRecipes();
                    return item;
                }
            }
            Recipe.FindRecipes();
            return item;
        }

        /// <summary>
        /// 获取所有物品栏的物品
        /// </summary>
        /// <param name="player">玩家实例</param>
        /// <param name="ignoreInventory">是否不获取原物品栏的物品</param>
        /// <returns>包含全部物品数组的List</returns>
        public static List<Item[]> GetAllInventoryItems(Player player, bool ignoreInventory) {
            List<Item[]> items = new() {
                player.bank.item,
                player.bank2.item,
                player.bank3.item,
                player.bank4.item,
            };
            if (!ignoreInventory) {
                items.Insert(0, player.inventory);
            }
            if (Config.SuperVault && player.TryGetModPlayer<DataPlayer>(out var modPlayer)) {
                items.Add(modPlayer.SuperVault);
            }
            return items;
        }

        /// <summary>
        /// 获取所有物品栏的物品
        /// </summary>
        /// <param name="player">玩家实例</param>
        /// <param name="ignoreInventory">是否不获取原物品栏的物品</param>
        /// <returns>包含全部物品的List</returns>
        public static List<Item> GetAllInventoryItemsList(Player player, bool ignoreInventory) {
            var item = new List<Item>();
            var items = GetAllInventoryItems(player, ignoreInventory);
            foreach (var itemArray in items) {
                for (int i = 0; i < itemArray.Length; i++) {
                    item.Add(itemArray[i]);
                }
            }
            return item;
        }

        /// <summary>
        /// 堆叠物品到仓库某位置
        /// </summary>
        /// <param name="inventory">仓库</param>
        /// <param name="slot">槽位</param>
        /// <param name="item">来自外来物品</param>
        /// <returns>堆叠后剩余的物品</returns>
        public static Item ItemStackToInventoryItem(Item[] inventory, int slot, Item item, bool hint) {
            if (!inventory[slot].IsAir && inventory[slot].type == item.type) {
                if (inventory[slot].stack + item.stack > inventory[slot].maxStack) {
                    if (hint) {
                        PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, item, inventory[slot].maxStack - inventory[slot].stack);
                        SoundEngine.PlaySound(SoundID.Grab);
                    }
                    item.stack -= inventory[slot].maxStack - inventory[slot].stack;
                    inventory[slot].stack = inventory[slot].maxStack;
                    return item;
                }
                else {
                    if (hint) {
                        PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, item, item.stack, noStack: false);
                        SoundEngine.PlaySound(SoundID.Grab);
                    }
                    inventory[slot].stack += item.stack;
                    item.SetDefaults(0);
                    return item;
                }
            }
            return item;
        }

        public static readonly List<int> Bank2Items = new() { ItemID.PiggyBank, ItemID.MoneyTrough, ItemID.ChesterPetItem };
        public static readonly List<int> Bank3Items = new() { ItemID.Safe };
        public static readonly List<int> Bank4Items = new() { ItemID.DefendersForge };
        public static readonly List<int> Bank5Items = new() { ItemID.VoidLens, ItemID.VoidVault };

        public static bool IsBankItem(int type) => Bank2Items.Contains(type) || Bank3Items.Contains(type) || Bank4Items.Contains(type) || Bank5Items.Contains(type);

        /// <summary>
        /// 判断指定 Item[] 中是否有 item 能用的空间
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool HasItemSpace(Item[] inv, Item item, int indexMax = 0) {
            for (int i = 0; i < (indexMax > 0 ? indexMax : inv.Length); i++) {
                if (inv[i].type == ItemID.None || (inv[i].type == item.type && inv[i].stack > 0 && inv[i].stack < inv[i].maxStack)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断指定 Item[] 中是否有 item
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool HasItem(Item[] inv, Item item, int indexMax = 0) {
            for (int i = 0; i < (indexMax > 0 ? indexMax : inv.Length); i++) {
                if (inv[i].type == item.type && inv[i].stack > 0) {
                    return true;
                }
            }
            return false;
        }

        // 获取配置
        public static ImproveConfigs Config => ModContent.GetInstance<ImproveConfigs>();

        /// <summary>
        /// 获取平台总数
        /// </summary>
        /// <param name="inv"></param>
        /// <param name="count">平台的数量</param>
        /// <returns>是否有不会被消耗的平台</returns>
        public static bool GetPlatformCount(Item[] inv, ref int count) {
            bool consumable = true;
            for (int i = 0; i < 50; i++) {
                Item item = inv[i];
                if (item.createTile != -1 && TileID.Sets.Platforms[item.createTile]) {
                    count += item.stack;
                    if (!item.consumable || !ItemLoader.ConsumeItem(item, Main.player[item.playerIndexTheItemIsReservedFor])) {
                        consumable = false;
                    }
                }
            }
            return consumable;
        }

        // 获取墙总数
        public static bool GetWallCount(Item[] inv, ref int count) {
            bool consumable = true;
            for (int i = 0; i < 50; i++) {
                Item item = inv[i];
                if (item.createWall > 0) {
                    count += item.stack;
                    if (!item.consumable || !ItemLoader.ConsumeItem(item, Main.player[item.playerIndexTheItemIsReservedFor])) {
                        consumable = false;
                    }
                }
            }
            return consumable;
        }

        // 获取背包第一个平台
        public static Item GetFirstPlatform(Player player) {
            for (int i = 0; i < 50; i++) {
                Item item = player.inventory[i];
                if (item.stack > 0 && item.createTile > -1 && TileID.Sets.Platforms[item.createTile]) {
                    return item;
                }
            }
            return new Item();
        }

        // 获取背包第一个平台
        public static Item GetFirstWall(Player player) {
            for (int i = 0; i < 50; i++) {
                Item item = player.inventory[i];
                if (item.stack > 0 && item.createWall > 0) {
                    return item;
                }
            }
            return null;
        }

        // 加载前缀
        public static void LoadPrefixInfo() {
            PrefixLevel.Add(1, 1);
            PrefixLevel.Add(2, 1);
            PrefixLevel.Add(3, 1);
            PrefixLevel.Add(4, 2);
            PrefixLevel.Add(5, 1);
            PrefixLevel.Add(6, 1);
            PrefixLevel.Add(7, 0);
            PrefixLevel.Add(8, 0);
            PrefixLevel.Add(9, 0);
            PrefixLevel.Add(10, 0);
            PrefixLevel.Add(11, 0);
            PrefixLevel.Add(12, 1);
            PrefixLevel.Add(13, 0);
            PrefixLevel.Add(14, 1);
            PrefixLevel.Add(15, 1);
            // 射手
            PrefixLevel.Add(16, 1);
            PrefixLevel.Add(17, 2);
            PrefixLevel.Add(18, 2);
            PrefixLevel.Add(19, 1);
            PrefixLevel.Add(20, 2);
            PrefixLevel.Add(21, 1);
            PrefixLevel.Add(22, 0);
            PrefixLevel.Add(23, 0);
            PrefixLevel.Add(24, 0);
            PrefixLevel.Add(25, 1);
            // 法师
            PrefixLevel.Add(26, 2);
            PrefixLevel.Add(27, 1);
            PrefixLevel.Add(28, 2);
            PrefixLevel.Add(29, 0);
            PrefixLevel.Add(30, 0);
            PrefixLevel.Add(31, 0);
            PrefixLevel.Add(32, 0);
            PrefixLevel.Add(33, 1);
            PrefixLevel.Add(34, 1);
            PrefixLevel.Add(35, 1);
            // 通用
            PrefixLevel.Add(36, 1);
            PrefixLevel.Add(37, 2);
            PrefixLevel.Add(38, 1);
            PrefixLevel.Add(39, 0);
            PrefixLevel.Add(40, 0);
            PrefixLevel.Add(41, 0);
            // 公共
            PrefixLevel.Add(42, 1);
            PrefixLevel.Add(43, 2);
            PrefixLevel.Add(44, 1);
            PrefixLevel.Add(45, 1);
            PrefixLevel.Add(46, 1);
            PrefixLevel.Add(47, 0);
            PrefixLevel.Add(48, 0);
            PrefixLevel.Add(49, 0);
            PrefixLevel.Add(50, 0);
            PrefixLevel.Add(51, 1);

            PrefixLevel.Add(52, 1);

            PrefixLevel.Add(53, 1);
            PrefixLevel.Add(54, 1);
            PrefixLevel.Add(55, 1);
            PrefixLevel.Add(56, 0);
            PrefixLevel.Add(57, 1);

            // 暴怒
            PrefixLevel.Add(58, 0);
            // 公共
            PrefixLevel.Add(59, 2);
            PrefixLevel.Add(60, 2);
            PrefixLevel.Add(61, 1);

            // 顶级前缀
            PrefixLevel.Add(81, 3);
            PrefixLevel.Add(82, 3);
            PrefixLevel.Add(83, 3);
            PrefixLevel.Add(84, 3);
            // 饰品
            PrefixLevel.Add(62, 1);
            PrefixLevel.Add(69, 1);
            PrefixLevel.Add(73, 1);
            PrefixLevel.Add(77, 1);
            PrefixLevel.Add(63, 2);
            PrefixLevel.Add(70, 2);
            PrefixLevel.Add(74, 2);
            PrefixLevel.Add(78, 2);
            PrefixLevel.Add(67, 2);
            PrefixLevel.Add(64, 3);
            PrefixLevel.Add(71, 3);
            PrefixLevel.Add(75, 3);
            PrefixLevel.Add(79, 3);
            PrefixLevel.Add(65, 4);
            PrefixLevel.Add(72, 4);
            PrefixLevel.Add(76, 4);
            PrefixLevel.Add(80, 4);
            PrefixLevel.Add(68, 4);
            PrefixLevel.Add(66, 4);
        }

        /// <summary>
        /// 判断有没有足量的此类物品
        /// </summary>
        public static int EnoughItem(Player player, Func<Item, bool> judge, int amount = 1) {
            int oneIndex = -1;
            int num = 0;
            for (int i = 0; i < 50; i++) {
                Item item = player.inventory[i];
                if (item.type != ItemID.None && item.stack > 0 && judge(item)) {
                    if (oneIndex == -1)
                        oneIndex = i;
                    if (!item.consumable || !ItemLoader.ConsumeItem(item, player))
                        return oneIndex;
                    num += item.stack;
                }
            }
            if (num >= amount) {
                return oneIndex;
            }
            return -1;
        }

        /// <summary>
        /// 从物品栏里找到对应物品，返回值为在<see cref="Player.inventory"/>中的索引
        /// </summary>
        /// <param name="player">对应玩家</param>
        /// <param name="shouldPick">选取物品的依据</param>
        /// <param name="tryConsume">是否尝试消耗</param>
        /// <returns></returns>
        public static int PickItemInInventory(Player player, Func<Item, bool> shouldPick, bool tryConsume) {
            for (int i = 0; i < 50; i++) {
                ref Item item = ref player.inventory[i];
                if (!item.IsAir && shouldPick.Invoke(item)) {
                    if (tryConsume) {
                        TryConsumeItem(ref item, player);
                    }
                    return i;
                }
            }
            return -1;
        }

        public static bool TryConsumeItem(ref Item item, Player player) {
            if (!item.IsAir && item.consumable && ItemLoader.ConsumeItem(item, player)) {
                item.stack--;
                if (item.stack < 1) {
                    item.TurnToAir();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 强制性的可否放置判断，只有满足条件才能放置物品，没有例外
        /// </summary>
        /// <param name="slotItem">槽内物品</param>
        /// <param name="mouseItem">手持物品</param>
        /// <returns>
        /// 强制判断返回值，判断放物类型
        /// <br>0: 不可放物</br>
        /// <br>1: 两物品不同，应该切换</br>
        /// <br>2: 两物品相同，应该堆叠</br>
        /// </returns>
        public static byte CanPlaceInSlot(Item slotItem, Item mouseItem) {
            if (mouseItem.type != slotItem.type || mouseItem.prefix != slotItem.prefix)
                return 1;
            if (!slotItem.IsAir && slotItem.stack < slotItem.maxStack && ItemLoader.CanStack(slotItem, mouseItem))
                return 2;
            return 0;
        }

        /// <summary>
        /// 普遍性的可否放置判断
        /// </summary>
        /// <param name="slotItem">槽内物品</param>
        /// <param name="mouseItem">手持物品</param>
        /// <returns>一般判断返回值</returns>
        public static bool SlotPlace(Item slotItem, Item mouseItem) => slotItem.type == mouseItem.type || mouseItem.IsAir;
    }
}
