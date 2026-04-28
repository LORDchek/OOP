using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using OOP.Shapes;

namespace OOP
{
    /// <summary>
    /// Handles drawing of shapes on a Graphics object
    /// Separate from shape classes to follow separation of concerns
    /// </summary>
    public static class ShapeDrawer
    {
        /// <summary>
        /// Draws a single shape on the specified graphics context
        /// </summary>
        public static void DrawShape(Graphics g, IShape shape)
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

                // Draw based on shape type
                switch (shape)
                {
                    case PointShape point:
                        DrawPoint(g, pen, point);
                        break;
                    case Line line:
                        DrawLine(g, pen, line);
                        break;
                    case Square square:
                        DrawSquare(g, pen, square);
                        break;
                    case Circle circle:
                        DrawCircle(g, pen, circle);
                        break;
                    case RectangleShape rect:
                        DrawRectangle(g, pen, rect);
                        break;
                    case Ellipse ellipse:
                        DrawEllipse(g, pen, ellipse);
                        break;
                    case Triangle triangle:
                        DrawTriangle(g, pen, triangle);
                        break;
                }
            }
        }

        private static void DrawPoint(Graphics g, Pen pen, PointShape point)
        {
            g.DrawEllipse(pen, point.Location.X - 2, point.Location.Y - 2, 4, 4);
            g.FillEllipse(new SolidBrush(point.Color), point.Location.X - 2, point.Location.Y - 2, 4, 4);
        }

        private static void DrawLine(Graphics g, Pen pen, Line line)
        {
            g.DrawLine(pen, line.Location, line.EndPoint);
        }

        private static void DrawRectangle(Graphics g, Pen pen, RectangleShape rect)
        {
            g.DrawRectangle(pen, rect.Location.X, rect.Location.Y, rect.Width, rect.Height);
        }

        private static void DrawEllipse(Graphics g, Pen pen, Ellipse ellipse)
        {
            g.DrawEllipse(pen, ellipse.Location.X, ellipse.Location.Y, ellipse.Width, ellipse.Height);
        }

        private static void DrawTriangle(Graphics g, Pen pen, Triangle triangle)
        {
            Point[] points = { triangle.Location, triangle.EndPoint, triangle.ThirdPoint };
            g.DrawPolygon(pen, points);
        }

        private static void DrawSquare(Graphics g, Pen pen, Square square)
        {
            g.DrawRectangle(pen, square.Location.X, square.Location.Y, square.Side, square.Side);
        }

        private static void DrawCircle(Graphics g, Pen pen, Circle circle)
        {
            g.DrawEllipse(pen,
                circle.Location.X - circle.Radius,
                circle.Location.Y - circle.Radius,
                circle.Radius * 2,
                circle.Radius * 2);
        }

        /// <summary>
        /// Draws all shapes in the list
        /// </summary>
        public static void DrawAllShapes(Graphics g, ShapeList shapes)
        {
            foreach (var shape in shapes)
            {
                DrawShape(g, shape);
            }
        }
    }
}