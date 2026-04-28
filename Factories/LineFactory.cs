using System;
using System.Drawing;
using OOP.Shapes;

namespace OOP.Factories
{
    /// <summary>
    /// Factory for creating Line shapes
    /// </summary>
    public class LineFactory : IShapeFactory
    {
        public string ShapeName => "Line";

        public IShape CreateShape(Point startPoint, Point endPoint, Color color)
        {
            return new Line(startPoint, endPoint, color);
        }

        public IShape CreateDefaultShape(Point location)
        {
            // Create a default line with 50px length at 45 degrees
            Point endPoint = new Point(location.X + 50, location.Y + 50);
            return new Line(location, endPoint, Color.Black);
        }
    }
}