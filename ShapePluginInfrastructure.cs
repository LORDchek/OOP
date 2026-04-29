using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using OOP.Shapes;

namespace OOP
{
    /// <summary>
    /// Draw handler for a particular shape type (keyed by shape.Name).
    /// Plugins can register additional renderers without editing host code.
    /// </summary>
    public interface IShapeRenderer
    {
        string TypeKey { get; }
        void Draw(Graphics g, IShape shape, Pen pen);
    }

    /// <summary>
    /// Persistence handler for a particular shape type (keyed by shape.Name).
    /// Plugins can register additional serializers without editing host code.
    /// </summary>
    public interface IShapeSerializer
    {
        string TypeKey { get; }
        XElement Serialize(IShape shape);
        IShape Deserialize(XElement element);
    }

    public class ShapeRendererRegistry
    {
        private readonly Dictionary<string, IShapeRenderer> renderers = new Dictionary<string, IShapeRenderer>(StringComparer.OrdinalIgnoreCase);

        public void Register(IShapeRenderer renderer)
        {
            if (renderer == null) return;
            if (string.IsNullOrWhiteSpace(renderer.TypeKey)) return;

            renderers[renderer.TypeKey] = renderer;
        }

        public IShapeRenderer Get(string typeKey)
        {
            if (typeKey == null) return null;
            renderers.TryGetValue(typeKey, out var renderer);
            return renderer;
        }
    }

    public class ShapeSerializerRegistry
    {
        private readonly Dictionary<string, IShapeSerializer> serializers = new Dictionary<string, IShapeSerializer>(StringComparer.OrdinalIgnoreCase);

        public void Register(IShapeSerializer serializer)
        {
            if (serializer == null) return;
            if (string.IsNullOrWhiteSpace(serializer.TypeKey)) return;

            serializers[serializer.TypeKey] = serializer;
        }

        public IShapeSerializer Get(string typeKey)
        {
            if (typeKey == null) return null;
            serializers.TryGetValue(typeKey, out var serializer);
            return serializer;
        }
    }

    /// <summary>
    /// Host-side built-in renderers/serializers.
    /// </summary>
    public static class ShapePluginInfrastructure
    {
        public static void RegisterBuiltIn(ShapeRendererRegistry rendererRegistry, ShapeSerializerRegistry serializerRegistry)
        {
            // Renderers
            rendererRegistry.Register(new LineRenderer());
            rendererRegistry.Register(new RectangleRenderer());
            rendererRegistry.Register(new EllipseRenderer());
            rendererRegistry.Register(new TriangleRenderer());
            rendererRegistry.Register(new SquareRenderer());
            rendererRegistry.Register(new CircleRenderer());
            rendererRegistry.Register(new PointRenderer());

            // Serializers
            serializerRegistry.Register(new LineSerializer());
            serializerRegistry.Register(new RectangleSerializer());
            serializerRegistry.Register(new EllipseSerializer());
            serializerRegistry.Register(new TriangleSerializer());
            serializerRegistry.Register(new SquareSerializer());
            serializerRegistry.Register(new CircleSerializer());
            serializerRegistry.Register(new PointSerializer());
        }

        private static int ReadInt(XElement element, string attrName)
        {
            // Host expects plugins to follow same attribute conventions.
            // If an attribute is missing, it's a malformed plugin payload / save file.
            return Convert.ToInt32(element.Attribute(attrName).Value);
        }

        private sealed class LineRenderer : IShapeRenderer
        {
            public string TypeKey => "Line";
            public void Draw(Graphics g, IShape shape, Pen pen)
            {
                var line = (Line)shape;
                g.DrawLine(pen, line.Location, line.EndPoint);
            }
        }

        private sealed class RectangleRenderer : IShapeRenderer
        {
            public string TypeKey => "Rectangle";
            public void Draw(Graphics g, IShape shape, Pen pen)
            {
                var rect = (RectangleShape)shape;
                g.DrawRectangle(pen, rect.Location.X, rect.Location.Y, rect.Width, rect.Height);
            }
        }

        private sealed class EllipseRenderer : IShapeRenderer
        {
            public string TypeKey => "Ellipse";
            public void Draw(Graphics g, IShape shape, Pen pen)
            {
                var ellipse = (Ellipse)shape;
                g.DrawEllipse(pen, ellipse.Location.X, ellipse.Location.Y, ellipse.Width, ellipse.Height);
            }
        }

        private sealed class TriangleRenderer : IShapeRenderer
        {
            public string TypeKey => "Triangle";
            public void Draw(Graphics g, IShape shape, Pen pen)
            {
                var triangle = (Triangle)shape;
                Point[] points = { triangle.Location, triangle.EndPoint, triangle.ThirdPoint };
                g.DrawPolygon(pen, points);
            }
        }

        private sealed class SquareRenderer : IShapeRenderer
        {
            public string TypeKey => "Square";
            public void Draw(Graphics g, IShape shape, Pen pen)
            {
                var square = (Square)shape;
                g.DrawRectangle(pen, square.Location.X, square.Location.Y, square.Width, square.Height);
            }
        }

        private sealed class CircleRenderer : IShapeRenderer
        {
            public string TypeKey => "Circle";
            public void Draw(Graphics g, IShape shape, Pen pen)
            {
                var circle = (Circle)shape;
                // Circle.Location in this project is top-left of bounding box.
                g.DrawEllipse(pen, circle.Location.X, circle.Location.Y, circle.Radius * 2, circle.Radius * 2);
            }
        }

        private sealed class PointRenderer : IShapeRenderer
        {
            public string TypeKey => "Point";
            public void Draw(Graphics g, IShape shape, Pen pen)
            {
                var point = (PointShape)shape;
                g.DrawEllipse(pen, point.Location.X - 2, point.Location.Y - 2, 4, 4);
                using (var brush = new SolidBrush(point.Color))
                {
                    g.FillEllipse(brush, point.Location.X - 2, point.Location.Y - 2, 4, 4);
                }
            }
        }

        private sealed class LineSerializer : IShapeSerializer
        {
            public string TypeKey => "Line";
            public XElement Serialize(IShape shape)
            {
                var line = (Line)shape;
                return new XElement("Shape",
                    new XAttribute("Type", TypeKey),
                    new XAttribute("X", line.Location.X),
                    new XAttribute("Y", line.Location.Y),
                    new XAttribute("EndX", line.EndPoint.X),
                    new XAttribute("EndY", line.EndPoint.Y),
                    new XAttribute("ArgbColor", line.Color.ToArgb())
                );
            }

            public IShape Deserialize(XElement element)
            {
                var color = Color.FromArgb(ReadInt(element, "ArgbColor"));
                var location = new Point(ReadInt(element, "X"), ReadInt(element, "Y"));
                var endPoint = new Point(ReadInt(element, "EndX"), ReadInt(element, "EndY"));
                return new Line(location, endPoint, color);
            }
        }

        private sealed class RectangleSerializer : IShapeSerializer
        {
            public string TypeKey => "Rectangle";
            public XElement Serialize(IShape shape)
            {
                var rect = (RectangleShape)shape;
                return new XElement("Shape",
                    new XAttribute("Type", TypeKey),
                    new XAttribute("X", rect.Location.X),
                    new XAttribute("Y", rect.Location.Y),
                    new XAttribute("Width", rect.Width),
                    new XAttribute("Height", rect.Height),
                    new XAttribute("ArgbColor", rect.Color.ToArgb())
                );
            }

            public IShape Deserialize(XElement element)
            {
                var color = Color.FromArgb(ReadInt(element, "ArgbColor"));
                var location = new Point(ReadInt(element, "X"), ReadInt(element, "Y"));
                int width = ReadInt(element, "Width");
                int height = ReadInt(element, "Height");
                return new RectangleShape(location, width, height, color);
            }
        }

        private sealed class EllipseSerializer : IShapeSerializer
        {
            public string TypeKey => "Ellipse";
            public XElement Serialize(IShape shape)
            {
                var ellipse = (Ellipse)shape;
                return new XElement("Shape",
                    new XAttribute("Type", TypeKey),
                    new XAttribute("X", ellipse.Location.X),
                    new XAttribute("Y", ellipse.Location.Y),
                    new XAttribute("Width", ellipse.Width),
                    new XAttribute("Height", ellipse.Height),
                    new XAttribute("ArgbColor", ellipse.Color.ToArgb())
                );
            }

            public IShape Deserialize(XElement element)
            {
                var color = Color.FromArgb(ReadInt(element, "ArgbColor"));
                var location = new Point(ReadInt(element, "X"), ReadInt(element, "Y"));
                int width = ReadInt(element, "Width");
                int height = ReadInt(element, "Height");
                return new Ellipse(location, width, height, color);
            }
        }

        private sealed class TriangleSerializer : IShapeSerializer
        {
            public string TypeKey => "Triangle";
            public XElement Serialize(IShape shape)
            {
                var triangle = (Triangle)shape;
                return new XElement("Shape",
                    new XAttribute("Type", TypeKey),
                    new XAttribute("X", triangle.Location.X),
                    new XAttribute("Y", triangle.Location.Y),
                    new XAttribute("EndX", triangle.EndPoint.X),
                    new XAttribute("EndY", triangle.EndPoint.Y),
                    new XAttribute("ThirdX", triangle.ThirdPoint.X),
                    new XAttribute("ThirdY", triangle.ThirdPoint.Y),
                    new XAttribute("ArgbColor", triangle.Color.ToArgb())
                );
            }

            public IShape Deserialize(XElement element)
            {
                var color = Color.FromArgb(ReadInt(element, "ArgbColor"));
                var p1 = new Point(ReadInt(element, "X"), ReadInt(element, "Y"));
                var p2 = new Point(ReadInt(element, "EndX"), ReadInt(element, "EndY"));
                var p3 = new Point(ReadInt(element, "ThirdX"), ReadInt(element, "ThirdY"));
                return new Triangle(p1, p2, p3, color);
            }
        }

        private sealed class SquareSerializer : IShapeSerializer
        {
            public string TypeKey => "Square";
            public XElement Serialize(IShape shape)
            {
                var square = (Square)shape;
                return new XElement("Shape",
                    new XAttribute("Type", TypeKey),
                    new XAttribute("X", square.Location.X),
                    new XAttribute("Y", square.Location.Y),
                    new XAttribute("Width", square.Width),
                    new XAttribute("Height", square.Height),
                    new XAttribute("ArgbColor", square.Color.ToArgb())
                );
            }

            public IShape Deserialize(XElement element)
            {
                var color = Color.FromArgb(ReadInt(element, "ArgbColor"));
                var location = new Point(ReadInt(element, "X"), ReadInt(element, "Y"));
                int side = ReadInt(element, "Width");
                return new Square(location, side, color);
            }
        }

        private sealed class CircleSerializer : IShapeSerializer
        {
            public string TypeKey => "Circle";
            public XElement Serialize(IShape shape)
            {
                var circle = (Circle)shape;
                int diameter = circle.Radius * 2;
                return new XElement("Shape",
                    new XAttribute("Type", TypeKey),
                    new XAttribute("X", circle.Location.X),
                    new XAttribute("Y", circle.Location.Y),
                    new XAttribute("Width", diameter),
                    new XAttribute("Height", diameter),
                    new XAttribute("ArgbColor", circle.Color.ToArgb())
                );
            }

            public IShape Deserialize(XElement element)
            {
                var color = Color.FromArgb(ReadInt(element, "ArgbColor"));
                var location = new Point(ReadInt(element, "X"), ReadInt(element, "Y"));
                int diameter = ReadInt(element, "Width");
                int radius = diameter / 2;
                // Stored X/Y is top-left of bounding box.
                var center = new Point(location.X + radius, location.Y + radius);
                return new Circle(center, radius, color);
            }
        }

        private sealed class PointSerializer : IShapeSerializer
        {
            public string TypeKey => "Point";
            public XElement Serialize(IShape shape)
            {
                var point = (PointShape)shape;
                return new XElement("Shape",
                    new XAttribute("Type", TypeKey),
                    new XAttribute("X", point.Location.X),
                    new XAttribute("Y", point.Location.Y),
                    new XAttribute("ArgbColor", point.Color.ToArgb())
                );
            }

            public IShape Deserialize(XElement element)
            {
                var color = Color.FromArgb(ReadInt(element, "ArgbColor"));
                var location = new Point(ReadInt(element, "X"), ReadInt(element, "Y"));
                return new PointShape(location, color);
            }
        }
    }
}

