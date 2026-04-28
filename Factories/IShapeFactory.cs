using System;
using System.Drawing;
using OOP.Shapes;

namespace OOP.Factories
{
    /// <summary>
    /// Interface for shape factories
    /// Enables adding new shapes without modifying existing code
    /// </summary>
    public interface IShapeFactory
    {
        string ShapeName { get; }
        IShape CreateShape(Point startPoint, Point endPoint, Color color);
        IShape CreateDefaultShape(Point location);
    }
}