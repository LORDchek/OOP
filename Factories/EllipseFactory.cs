using System;
using System.Drawing;
using OOP.Shapes;

namespace OOP.Factories
{
    /// <summary>
    /// Factory for creating Ellipse shapes
    /// </summary>
    public class EllipseFactory : IShapeFactory
    {
        public string ShapeName => "Ellipse";

        public IShape CreateShape(Point startPoint, Point endPoint, Color color)
        {
            int width = Math.Abs(endPoint.X - startPoint.X);
            int height = Math.Abs(endPoint.Y - startPoint.Y);
            Point topLeft = new Point(
                Math.Min(startPoint.X, endPoint.X),
                Math.Min(startPoint.Y, endPoint.Y)
            );

            return new Ellipse(topLeft, width, height, color);
        }

        public IShape CreateDefaultShape(Point location)
        {
            return new Ellipse(location, 80, 60, Color.Green);
        }
    }
}