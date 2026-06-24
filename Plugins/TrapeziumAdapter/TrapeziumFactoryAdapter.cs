using System;
using System.Drawing;
using OOP.Factories;
using OOP.Shapes;
using TrapeziumLibrary;

namespace TrapeziumAdapter
{
  /// <summary>
  /// Adapter: создание фигуры друга через фабрику хоста (IShapeFactory).
  /// </summary>
  public sealed class TrapeziumFactoryAdapter : IShapeFactory
  {
    public string ShapeName => "Trapezium";

    public IShape CreateShape(Point startPoint, Point endPoint, Color color)
    {
      int left = Math.Min(startPoint.X, endPoint.X);
      int top = Math.Min(startPoint.Y, endPoint.Y);
      int width = Math.Abs(endPoint.X - startPoint.X);
      int height = Math.Abs(endPoint.Y - startPoint.Y);

      var trapezium = new Trapezium();
      trapezium.SetPoints(TrapeziumGeometry.CreateFromBounds(left, top, width, height));

      var adapter = new TrapeziumShapeAdapter(trapezium) { Color = color };
      return adapter;
    }

    public IShape CreateDefaultShape(Point location)
    {
      return CreateShape(location, new Point(location.X + 120, location.Y + 80), Color.Tan);
    }
  }
}
