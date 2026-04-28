using System;
using System.Drawing;

namespace OOP.Shapes
{
    /// <summary>
    /// Represents a circle shape (inherits from Ellipse)
    /// </summary>
    public class Circle : Ellipse
    {
        public override string Name => "Circle";

        public int Radius
        {
            get => Width / 2;
            set
            {
                Width = value * 2;
                Height = value * 2;
            }
        }

        // Center point for circle
        public Point Center
        {
            get => new Point(Location.X + Radius, Location.Y + Radius);
            set => Location = new Point(value.X - Radius, value.Y - Radius);
        }

        public Circle(Point center, int radius, Color color)
            : base(new Point(center.X - radius, center.Y - radius), radius * 2, radius * 2, color)
        {
        }

        // Override to ensure circle maintains equal width/height
        public override int Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                base.Height = value; // Keep circle shape
            }
        }

        public override int Height
        {
            get => base.Height;
            set
            {
                base.Height = value;
                base.Width = value; // Keep circle shape
            }
        }
    }
}