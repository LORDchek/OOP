using System;
using System.Drawing;
using OOP.Shapes;

namespace OOP.Factories
{
    /// <summary>
    /// Factory for creating Triangle shapes
    /// </summary>
    public class TriangleFactory : IShapeFactory
    {
        public string ShapeName => "Triangle";

        public IShape CreateShape(Point startPoint, Point endPoint, Color color)
        {
            // Create an isosceles triangle with base from start to end
            int midX = (startPoint.X + endPoint.X) / 2;
            Point thirdPoint = new Point(midX, startPoint.Y - 50);

            return new Triangle(startPoint, endPoint, thirdPoint, color);
        }

        public IShape CreateDefaultShape(Point location)
        {
            Point p1 = location;
            Point p2 = new Point(location.X + 80, location.Y);
            Point p3 = new Point(location.X + 40, location.Y - 60);

            return new Triangle(p1, p2, p3, Color.Red);
        }
    }
}