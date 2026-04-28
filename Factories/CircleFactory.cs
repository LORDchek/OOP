using System;
using System.Drawing;
using OOP.Shapes;

namespace OOP.Factories
{
    /// <summary>
    /// Factory for creating Circle shapes
    /// </summary>
    public class CircleFactory : IShapeFactory
    {
        public string ShapeName => "Circle";

        public IShape CreateShape(Point startPoint, Point endPoint, Color color)
        {
            int radius = (int)Math.Sqrt(
                Math.Pow(endPoint.X - startPoint.X, 2) +
                Math.Pow(endPoint.Y - startPoint.Y, 2)
            ) / 2;

            return new Circle(startPoint, radius, color);
        }

        public IShape CreateDefaultShape(Point location)
        {
            return new Circle(location, 40, Color.Purple);
        }
    }
}