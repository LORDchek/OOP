using System;
using System.Drawing;

namespace OOP.Shapes
{
    /// <summary>
    /// Represents an ellipse shape
    /// </summary>
    public class Ellipse : ShapeBase
    {
        public override string Name => "Ellipse";

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
            set { }
        }

        public Ellipse(Point location, int width, int height, Color color) : base(location, color)
        {
            this.width = width;
            this.height = height;
        }
    }
}