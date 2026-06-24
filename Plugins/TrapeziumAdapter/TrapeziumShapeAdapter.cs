using System;
using System.Drawing;
using OOP.Shapes;
using TrapeziumLibrary;

namespace TrapeziumAdapter
{
  /// <summary>
  /// Adapter: класс друга Trapezium (WPF) → IShape хоста (WinForms).
  /// </summary>
  public sealed class TrapeziumShapeAdapter : ShapeBase
  {
    private readonly Trapezium friendShape;

    public TrapeziumShapeAdapter(Trapezium source) : base(TrapeziumGeometry.GetTopLeft(source.point), Color.Tan)
    {
      friendShape = source ?? throw new ArgumentNullException(nameof(source));
    }

    public Trapezium FriendShape => friendShape;

    public int[] Points => friendShape.point;

    public override string Name => "Trapezium";

    public override int Width
    {
      get => TrapeziumGeometry.GetWidth(Points);
      set { }
    }

    public override int Height
    {
      get => TrapeziumGeometry.GetHeight(Points);
      set { }
    }

    public override Point EndPoint
    {
      get => new Point(Location.X + Width, Location.Y + Height);
      set { }
    }

    public override void MoveTo(Point newLocation)
    {
      int dx = newLocation.X - Location.X;
      int dy = newLocation.Y - Location.Y;
      var moved = new int[8];
      for (int i = 0; i < 8; i += 2)
      {
        moved[i] = Points[i] + dx;
        moved[i + 1] = Points[i + 1] + dy;
      }
      friendShape.SetPoints(moved);
      Location = newLocation;
    }

    public override void Resize(int width, int height)
    {
      var resized = TrapeziumGeometry.CreateFromBounds(Location.X, Location.Y, width, height);
      friendShape.SetPoints(resized);
    }
  }
}
