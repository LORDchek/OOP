using System;
using System.Drawing;
using System.Xml.Linq;
using OOP;
using OOP.Shapes;
using TrapeziumLibrary;

namespace TrapeziumAdapter
{
  public sealed class TrapeziumSerializerAdapter : IShapeSerializer
  {
    public string TypeKey => "Trapezium";

    public XElement Serialize(IShape shape)
    {
      var adapter = (TrapeziumShapeAdapter)shape;
      var p = adapter.Points;
      return new XElement("Shape",
        new XAttribute("Type", TypeKey),
        new XAttribute("P0X", p[0]), new XAttribute("P0Y", p[1]),
        new XAttribute("P1X", p[2]), new XAttribute("P1Y", p[3]),
        new XAttribute("P2X", p[4]), new XAttribute("P2Y", p[5]),
        new XAttribute("P3X", p[6]), new XAttribute("P3Y", p[7]),
        new XAttribute("ArgbColor", adapter.Color.ToArgb()));
    }

    public IShape Deserialize(XElement element)
    {
      var points = new[]
      {
        ReadInt(element, "P0X"), ReadInt(element, "P0Y"),
        ReadInt(element, "P1X"), ReadInt(element, "P1Y"),
        ReadInt(element, "P2X"), ReadInt(element, "P2Y"),
        ReadInt(element, "P3X"), ReadInt(element, "P3Y")
      };

      var trapezium = new Trapezium();
      trapezium.SetPoints(points);
      var adapter = new TrapeziumShapeAdapter(trapezium)
      {
        Color = Color.FromArgb(ReadInt(element, "ArgbColor"))
      };
      return adapter;
    }

    private static int ReadInt(XElement element, string name)
    {
      return Convert.ToInt32(element.Attribute(name).Value);
    }
  }
}
