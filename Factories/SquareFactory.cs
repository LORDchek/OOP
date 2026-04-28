using System;
using System.Drawing;
using OOP.Shapes;

namespace OOP.Factories
{
    /// <summary>
    /// Factory for creating Square shapes
    /// </summary>
    public class SquareFactory : IShapeFactory
    {
        public string ShapeName => "Square";

        public IShape CreateShape(Point startPoint, Point endPoint, Color color)
        {
            int side = Math.Min(
                Math.Abs(endPoint.X - startPoint.X),
                Math.Abs(endPoint.Y - startPoint.Y)
            );
            Point topLeft = new Point(
                Math.Min(startPoint.X, endPoint.X),
                Math.Min(startPoint.Y, endPoint.Y)
            );

            return new Square(topLeft, side, color);
        }

        public IShape CreateDefaultShape(Point location)
        {
            return new Square(location, 70, Color.Orange);
        }
    }
}