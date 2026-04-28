using System;
using System.Drawing;

namespace OOP.Shapes
{
    /// <summary>
    /// Represents a triangle shape defined by three points
    /// </summary>
    public class Triangle : ShapeBase
    {
        public override string Name => "Triangle";

        private Point endPoint;
        private Point thirdPoint;

        public override Point EndPoint
        {
            get => endPoint;
            set => endPoint = value;
        }

        public Point ThirdPoint
        {
            get => thirdPoint;
            set => thirdPoint = value;
        }

        // Calculated bounding box
        public override int Width
        {
            get
            {
                int maxX = Math.Max(Location.X, Math.Max(endPoint.X, thirdPoint.X));
                int minX = Math.Min(Location.X, Math.Min(endPoint.X, thirdPoint.X));
                return maxX - minX;
            }
            set { }
        }

        public override int Height
        {
            get
            {
                int maxY = Math.Max(Location.Y, Math.Max(endPoint.Y, thirdPoint.Y));
                int minY = Math.Min(Location.Y, Math.Min(endPoint.Y, thirdPoint.Y));
                return maxY - minY;
            }
            set { }
        }

        public Triangle(Point p1, Point p2, Point p3, Color color) : base(p1, color)
        {
            endPoint = p2;
            thirdPoint = p3;
        }

        public override void MoveTo(Point newLocation)
        {
            int dx = newLocation.X - Location.X;
            int dy = newLocation.Y - Location.Y;

            Location = newLocation;
            endPoint = new Point(endPoint.X + dx, endPoint.Y + dy);
            thirdPoint = new Point(thirdPoint.X + dx, thirdPoint.Y + dy);
        }

        public override void Resize(int width, int height)
        {
            int safeWidth = Math.Max(0, width);
            int safeHeight = Math.Max(0, height);

            endPoint = new Point(Location.X + safeWidth, Location.Y + safeHeight);
            thirdPoint = new Point(Location.X + safeWidth / 2, Location.Y);
        }
    }
}