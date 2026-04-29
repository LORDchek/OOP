using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using OOP.Shapes;

namespace OOP
{
    /// <summary>
    /// Handles drawing of shapes on a Graphics object
    /// Separate from shape classes to follow separation of concerns
    /// </summary>
    public class ShapeDrawer
    {
        private readonly ShapeRendererRegistry rendererRegistry;

        public ShapeDrawer(ShapeRendererRegistry rendererRegistry)
        {
            this.rendererRegistry = rendererRegistry ?? throw new ArgumentNullException(nameof(rendererRegistry));
        }

        /// <summary>
        /// Draws a single shape on the specified graphics context
        /// </summary>
        public void DrawShape(Graphics g, IShape shape)
        {
            if (shape == null) return;

            using (Pen pen = new Pen(shape.Color, 2))
            {
                // Draw selection highlight if selected
                if (shape.IsSelected)
                {
                    pen.Color = Color.Red;
                    pen.Width = 3;
                    pen.DashStyle = DashStyle.Dash;
                }

                var renderer = rendererRegistry.Get(shape.Name);
                if (renderer == null) return;
                renderer.Draw(g, shape, pen);
            }
        }

        /// <summary>
        /// Draws all shapes in the list
        /// </summary>
        public void DrawAllShapes(Graphics g, ShapeList shapes)
        {
            foreach (var shape in shapes)
            {
                DrawShape(g, shape);
            }
        }
    }
}