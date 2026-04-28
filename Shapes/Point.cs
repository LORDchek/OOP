using System;
using System.Drawing;

namespace OOP.Shapes
{
    /// <summary>
    /// Represents a point shape
    /// </summary>
    public class PointShape : ShapeBase
    {
        public override string Name => "Point";

        // Point has zero width/height
        public override int Width
        {
            get => 0;
            set { }
        }

        public override int Height
        {
            get => 0;
            set { }
        }

        public override Point EndPoint
        {
            get => Location;
            set => Location = value;
        }

        public PointShape(Point location, Color color) : base(location, color)
        {
        }
    }
}