using System;
using System.Drawing;
using System.Xml.Linq;
using OOP;
using OOP.Factories;
using OOP.Shapes;

namespace DiamondPlugin
{
    public sealed class DiamondShape : ShapeBase
    {
        public override string Name => "Diamond";

        public override Point EndPoint
        {
            get => new Point(Location.X + Width, Location.Y + Height);
            set
            {
                Width = Math.Max(0, value.X - Location.X);
                Height = Math.Max(0, value.Y - Location.Y);
            }
        }

        public DiamondShape(Point location, int width, int height, Color color)
            : base(location, color)
        {
            Width = Math.Max(0, width);
            Height = Math.Max(0, height);
        }
    }

    public sealed class DiamondFactory : IShapeFactory
    {
        public string ShapeName => "Diamond";

        public IShape CreateShape(Point startPoint, Point endPoint, Color color)
        {
            int x = Math.Min(startPoint.X, endPoint.X);
            int y = Math.Min(startPoint.Y, endPoint.Y);
            int width = Math.Abs(endPoint.X - startPoint.X);
            int height = Math.Abs(endPoint.Y - startPoint.Y);
            return new DiamondShape(new Point(x, y), width, height, color);
        }

        public IShape CreateDefaultShape(Point location)
        {
            return new DiamondShape(location, 80, 60, Color.DarkCyan);
        }
    }

    public sealed class DiamondRenderer : IShapeRenderer
    {
        public string TypeKey => "Diamond";

        public void Draw(Graphics g, IShape shape, Pen pen)
        {
            var diamond = (DiamondShape)shape;
            int centerX = diamond.Location.X + diamond.Width / 2;
            int centerY = diamond.Location.Y + diamond.Height / 2;

            var points = new[]
            {
                new Point(centerX, diamond.Location.Y),
                new Point(diamond.Location.X + diamond.Width, centerY),
                new Point(centerX, diamond.Location.Y + diamond.Height),
                new Point(diamond.Location.X, centerY)
            };

            g.DrawPolygon(pen, points);
        }
    }

    public sealed class DiamondSerializer : IShapeSerializer
    {
        public string TypeKey => "Diamond";

        public XElement Serialize(IShape shape)
        {
            var diamond = (DiamondShape)shape;
            return new XElement("Shape",
                new XAttribute("Type", TypeKey),
                new XAttribute("X", diamond.Location.X),
                new XAttribute("Y", diamond.Location.Y),
                new XAttribute("Width", diamond.Width),
                new XAttribute("Height", diamond.Height),
                new XAttribute("ArgbColor", diamond.Color.ToArgb()));
        }

        public IShape Deserialize(XElement element)
        {
            var location = new Point(
                Convert.ToInt32(element.Attribute("X").Value),
                Convert.ToInt32(element.Attribute("Y").Value));
            int width = Convert.ToInt32(element.Attribute("Width").Value);
            int height = Convert.ToInt32(element.Attribute("Height").Value);
            var color = Color.FromArgb(Convert.ToInt32(element.Attribute("ArgbColor").Value));
            return new DiamondShape(location, width, height, color);
        }
    }

    public sealed class DiamondShapePlugin : IShapePlugin
    {
        public string PluginName => "Diamond plugin";

        public void Register(PluginHostContext context)
        {
            context.RegisterFactory(new DiamondFactory());
            context.RegisterRenderer(new DiamondRenderer());
            context.RegisterSerializer(new DiamondSerializer());
        }
    }
}

