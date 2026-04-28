using System;
using System.Drawing;

namespace OOP.Shapes
{
    /// <summary>
    /// Base abstract class for all shapes
    /// </summary>
    public abstract class ShapeBase : IShape
    {
        public abstract string Name { get; }
        public Point Location { get; set; }
        public Color Color { get; set; }
        public bool IsSelected { get; set; }

        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public virtual Point EndPoint { get; set; }

        protected ShapeBase(Point location, Color color)
        {
            Location = location;
            Color = color;
            IsSelected = false;
        }

        public virtual void MoveTo(Point newLocation)
        {
            Location = newLocation;
        }

        public virtual void Resize(int width, int height)
        {
            Width = Math.Max(0, width);
            Height = Math.Max(0, height);
        }
    }
}