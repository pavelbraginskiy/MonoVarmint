using Microsoft.Xna.Framework;

namespace MonoVarmint.Widgets
{
    //--------------------------------------------------------------------------------------
    /// <summary>
    /// VarmintWidgetLabel - simple text widget
    /// </summary>
    //--------------------------------------------------------------------------------------
    [VarmintWidgetShortName("Label")]
    public class VarmintWidgetLabel : VarmintWidget
    {

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //--------------------------------------------------------------------------------------
        public VarmintWidgetLabel()
        {
            this.OnRender += Render;
        }

        //--------------------------------------------------------------------------------------
        /// <summary>
        /// Render
        /// </summary>
        //--------------------------------------------------------------------------------------
        void Render(GameTime gameTime, VarmintWidget widget)
        {
            var textToDisplay = (Content == null) ? "" : Content.ToString();

            Renderer.DrawBox(AbsoluteOffset, Size, RenderBackgroundColor);
            Vector2 alignedOffset = AbsoluteOffset;
            var margin = 0f;
            if (WrapContent) margin = Size.X;
            Vector2 textSize = Renderer.MeasureText(textToDisplay, FontName, FontSize, margin);
            switch (HorizontalContentAlignment)
            {
                case HorizontalContentAlignment.Left: break;
                case HorizontalContentAlignment.Center: alignedOffset.X += (Size.X - textSize.X) / 2; break;
                case HorizontalContentAlignment.Right: alignedOffset.X += (Size.X - textSize.X); break;
            }

            switch(VerticalContentAlignment)
            {
                case VerticalContentAlignment.Top: break;
                case VerticalContentAlignment.Center: alignedOffset.Y += (Size.Y - textSize.Y) / 2; break;
                case VerticalContentAlignment.Bottom: alignedOffset.Y += (Size.Y - textSize.Y);  break;
            }

            Renderer.DrawText(textToDisplay, FontName, FontSize, alignedOffset, RenderForegroundColor, margin);
        }
    }
}