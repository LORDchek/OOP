using System.Drawing;
using System.Drawing.Drawing2D;
using OOP;
using OOP.Shapes;

namespace TrapeziumAdapter
{
  /// <summary>
  /// Adapter: отрисовка через GDI+ по логике TrapeziumFactory друга (WPF Canvas → Graphics).
  /// </summary>
  public sealed class TrapeziumRendererAdapter : IShapeRenderer
  {
    public string TypeKey => "Trapezium";

    public void Draw(Graphics g, IShape shape, Pen pen)
    {
      var adapter = shape as TrapeziumShapeAdapter;
      if (adapter == null)
        return;

      var points = adapter.Points;
      if (!TrapeziumGeometry.IsValidTrapezium(points))
        return;

      var polygon = TrapeziumGeometry.ToPolygonPoints(points);
      using (var brush = new SolidBrush(Color.FromArgb(80, adapter.Color)))
      {
        g.FillPolygon(brush, polygon);
      }
      g.DrawPolygon(pen, polygon);
    }
  }
}
