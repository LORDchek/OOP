using System;
using System.Drawing;

namespace OOP.Shapes
{
    /// <summary>
    /// Represents a line shape defined by start and end points
    /// </summary>
    public class Line : ShapeBase
    {
        public override string Name => "Line";

        private Point endPoint;

        public override Point EndPoint
        {
            get => endPoint;
            set => endPoint = value;
        }

        // Width and Height are calculated based on endpoints
        public override int Width
        {
            get => Math.Abs(endPoint.X - Location.X);
            set { } // Read-only property
        }

        public override int Height
        {
            get => Math.Abs(endPoint.Y - Location.Y);
            set { } // Read-only property
        }

        public Line(Point start, Point end, Color color) : base(start, color)
        {
            endPoint = end;
        }

        public override void MoveTo(Point newLocation)
        {
            int dx = newLocation.X - Location.X;
            int dy = newLocation.Y - Location.Y;
            Location = newLocation;
            endPoint = new Point(endPoint.X + dx, endPoint.Y + dy);
        }

        public override void Resize(int width, int height)
        {
            endPoint = new Point(Location.X + Math.Max(0, width), Location.Y + Math.Max(0, height));
        }
    }
}