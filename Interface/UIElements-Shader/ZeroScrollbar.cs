﻿using ImproveGame.Common.Animations;
using Terraria.GameInput;

namespace ImproveGame.Interface.UIElements_Shader
{
    /// <summary>
    /// 宽度默认 20
    /// </summary>
    public class ZeroScrollbar : UIElement
    {
        private float viewPosition; // 滚动条当前位置
        private float maxViewPoisition;

        private float viewSize = 1f; // 显示出来的高度
        private float maxViewSize = 20f; // 控制元素的高度
        private float viewScale;

        // 用于拖动内滚动条
        private float offsetY;
        public bool dragging;

        public float ViewPosition
        {
            get => viewPosition;
            set => viewPosition = MathHelper.Clamp(value, 0f, maxViewSize - viewSize);
        }

        private float _bufferViewPosition;
        /// <summary>
        /// 缓冲距离, 不想使用动画就直接设置 <see cref="ViewPosition"/>
        /// </summary>
        public float BufferViewPosition
        {
            get => _bufferViewPosition;
            set => _bufferViewPosition = value;
        }

        public ZeroScrollbar()
        {
            Width.Pixels = 20;
            MaxWidth.Pixels = 20;
            SetPadding(5);
        }

        public override void Update(GameTime gameTime)
        {
            Main.NewText($"ViewPosition: {ViewPosition}  BufferViewPosition: {BufferViewPosition}");
            Main.NewText($"ViewSize: {viewSize}  MaxViewSize: {maxViewSize}");
            Main.NewText($"Height: {Height.Pixels}");
            base.Update(gameTime);
            if (dragging)
            {
                CalculatedStyle InnerDimensions = GetInnerDimensions();
                Main.NewText($"offset.Y: {offsetY}");
                Main.NewText($"剩余距离: {Main.MouseScreen.Y - InnerDimensions.Y}");
                ViewPosition = (Main.MouseScreen.Y - InnerDimensions.Y - offsetY) / (InnerDimensions.Height * (1 - viewScale)) * maxViewPoisition;
            }

            if (BufferViewPosition != 0)
            {
                ViewPosition -= BufferViewPosition * 0.2f;
                BufferViewPosition *= 0.8f;
                if (MathF.Abs(BufferViewPosition) < 0.1f)
                {
                    ViewPosition = MathF.Round(ViewPosition, 1);
                    BufferViewPosition = 0;
                }
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            if (evt.Target == this)
            {
                CalculatedStyle InnerDimensions = GetInnerDimensions();

                if (InnerDimensions.Contains(Main.MouseScreen))
                {
                    dragging = true;
                    offsetY = evt.MousePosition.Y - InnerDimensions.Y - (InnerDimensions.Height * viewScale * (viewPosition / maxViewPoisition));
                }
                BufferViewPosition = 0;
            }
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            base.MouseUp(evt);
            dragging = false;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            PlayerInput.LockVanillaMouseScroll("ModLoader/UIScrollbar");
        }

        public readonly Color background1 = new(43, 56, 101);
        public readonly Color borderColor2 = new(93, 88, 93);
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimension = GetDimensions();
            Vector2 position = dimension.Position();
            Vector2 size = dimension.Size();

            // 滚动条背板
            PixelShader.DrawBox(Main.UIScaleMatrix, position, size, size.X / 2, 3, Color.Black, background1);

            CalculatedStyle innerDimensions = GetInnerDimensions();
            Vector2 innerPosition = innerDimensions.Position();
            Vector2 innerSize = innerDimensions.Size();
            innerPosition.Y += (innerSize.Y - innerSize.Y * viewScale) * (ViewPosition / maxViewPoisition);
            innerSize.Y *= viewScale;

            // 滚动条拖动块
            PixelShader.DrawBox(Main.UIScaleMatrix, innerPosition, innerSize, innerSize.X / 2, 0, Color.White, Color.White);
        }

        public void SetView(float viewSize, float maxViewSize)
        {
            viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
            viewPosition = MathHelper.Clamp(viewPosition, 0f, maxViewSize - viewSize);
            this.maxViewPoisition = maxViewSize - viewSize;

            this.viewSize = viewSize;
            this.maxViewSize = maxViewSize;
            viewScale = viewSize / maxViewSize;
        }
    }
}