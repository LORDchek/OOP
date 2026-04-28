using System;
using System.Drawing;

namespace OOP.Shapes
{
    /// <summary>
    /// Represents a square shape (inherits from Rectangle)
    /// </summary>
    public class Square : RectangleShape
    {
        public override string Name => "Square";

        public int Side
        {
            get => Width;
            set
            {
                Width = value;
                Height = value;
            }
        }

        public Square(Point location, int side, Color color) : base(location, side, side, color)
        {
        }

        // Override to ensure square maintains equal sides
        public override int Width
        {
            get => base.Width;
            set
            {
                base.Width = value;
                base.Height = value; // Keep square shape
            }
        }

        public override int Height
        {
            get => base.Height;
            set
            {
                base.Height = value;
                base.Width = value; // Keep square shape
            }
        }
    }
}