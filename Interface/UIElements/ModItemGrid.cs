﻿namespace ImproveGame.Interface.UIElements
{
    // 一个有滚动条的视图有三部分.
    // 一个最大的元素, 用来包裹显示内容部分和滚动条. 最大的元素负责隐藏超过范围的元素.
    public class ModItemGrid : UIElement
    {
        public readonly Vector2 SlotSize;

        public ModItemList ItemList;
        public ModScrollbar Scrollbar;

        // 可以在 new 的时候将其他元素也初始化, 或者在执行 Active() 的时候初始化.
        public ModItemGrid(UserInterface userInterface)
        {
            SlotSize = TextureAssets.InventoryBack.Size();
            SetPadding(0);
            OverflowHidden = true; // 隐藏超过显示范围的部分, 计算 padding 后的.

            ItemList = new(SlotSize);
            Append(ItemList);

            Scrollbar = new(userInterface);
            Scrollbar.Height.Pixels = ItemList.Height() - 12f;
            Scrollbar.HAlign = 1f;
            Scrollbar.VAlign = 0.5f;
            Append(Scrollbar);

            Width.Pixels = ItemList.Width() + Scrollbar.Width() + 10f;
            Height.Pixels = ItemList.Height();
        }

        public void SetInventory(Item[] items)
        {
            Scrollbar.SetView(Height.Pixels, SlotSize.Y * (items.Length / ModItemList.HCount) + 10f * (items.Length / ModItemList.HCount) - 10f);
            ItemList.SetInventory(items);
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            Scrollbar.SetViewPosition(evt.ScrollWheelValue);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Scrollbar != null)
            {
                ItemList.Top.Set(-Scrollbar.GetValue(), 0);
            }
            ItemList.Recalculate();
        }
    }
}