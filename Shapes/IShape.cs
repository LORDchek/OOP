using System;
using System.Drawing;

namespace OOP.Shapes
{
    /// <summary>
    /// Interface for all graphical shapes
    /// Contains only data properties, no drawing logic
    /// </summary>
    public interface IShape
    {
        string Name { get; }
        Point Location { get; set; }
        Color Color { get; set; }
        bool IsSelected { get; set; }

        // Properties for shape dimensions
        int Width { get; set; }
        int Height { get; set; }

        // For shapes that need additional points (like Line)
        Point EndPoint { get; set; }

        // Generic editing operations
        void MoveTo(Point newLocation);
        void Resize(int width, int height);
    }
}