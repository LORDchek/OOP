using System;
using System.Drawing;
using OOP.Shapes;

namespace OOP.Factories
{
    /// <summary>
    /// Factory for creating Rectangle shapes
    /// </summary>
    public class RectangleFactory : IShapeFactory
    {
        public string ShapeName => "Rectangle";

        public IShape CreateShape(Point startPoint, Point endPoint, Color color)
        {
            int width = Math.Abs(endPoint.X - startPoint.X);
            int height = Math.Abs(endPoint.Y - startPoint.Y);
            Point topLeft = new Point(
                Math.Min(startPoint.X, endPoint.X),
                Math.Min(startPoint.Y, endPoint.Y)
            );

            return new RectangleShape(topLeft, width, height, color);
        }

        public IShape CreateDefaultShape(Point location)
        {
            return new RectangleShape(location, 80, 60, Color.Blue);
        }
    }
}