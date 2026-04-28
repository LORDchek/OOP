using System;
using System.Drawing;

namespace OOP.Shapes
{
    /// <summary>
    /// Represents a rectangle shape
    /// </summary>
    public class RectangleShape : ShapeBase
    {
        public override string Name => "Rectangle";

        private int width;
        private int height;

        public override int Width
        {
            get => width;
            set => width = value;
        }

        public override int Height
        {
            get => height;
            set => height = value;
        }

        public override Point EndPoint
        {
            get => new Point(Location.X + width, Location.Y + height);
            set { } // Calculated property
        }

        public RectangleShape(Point location, int width, int height, Color color) : base(location, color)
        {
            this.width = width;
            this.height = height;
        }
    }
}