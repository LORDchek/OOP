using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using OOP.Shapes;

namespace OOP
{
    [Serializable]
    public class ShapeCollectionDto
    {
        public List<ShapeDto> Shapes { get; set; } = new List<ShapeDto>();
    }

    [Serializable]
    public class ShapeDto
    {
        public string Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }
        public int ThirdX { get; set; }
        public int ThirdY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ArgbColor { get; set; }
    }

    public class ShapeXmlService
    {
        public void Save(string filePath, IEnumerable<IShape> shapes)
        {
            var payload = new ShapeCollectionDto();
            foreach (var shape in shapes)
            {
                payload.Shapes.Add(ToDto(shape));
            }

            var serializer = new XmlSerializer(typeof(ShapeCollectionDto));
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, payload);
            }
        }

        public List<IShape> Load(string filePath)
        {
            var serializer = new XmlSerializer(typeof(ShapeCollectionDto));
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var payload = (ShapeCollectionDto)serializer.Deserialize(stream);
                var result = new List<IShape>();

                if (payload?.Shapes == null)
                {
                    return result;
                }

                foreach (var dto in payload.Shapes)
                {
                    var shape = FromDto(dto);
                    if (shape != null)
                    {
                        result.Add(shape);
                    }
                }

                return result;
            }
        }

        private static ShapeDto ToDto(IShape shape)
        {
            var dto = new ShapeDto
            {
                Type = shape.Name,
                X = shape.Location.X,
                Y = shape.Location.Y,
                EndX = shape.EndPoint.X,
                EndY = shape.EndPoint.Y,
                Width = shape.Width,
                Height = shape.Height,
                ArgbColor = shape.Color.ToArgb()
            };

            if (shape is Triangle triangle)
            {
                dto.ThirdX = triangle.ThirdPoint.X;
                dto.ThirdY = triangle.ThirdPoint.Y;
            }

            return dto;
        }

        private static IShape FromDto(ShapeDto dto)
        {
            var color = Color.FromArgb(dto.ArgbColor);
            var location = new Point(dto.X, dto.Y);

            switch (dto.Type)
            {
                case "Line":
                    return new Line(location, new Point(dto.EndX, dto.EndY), color);
                case "Rectangle":
                    return new RectangleShape(location, dto.Width, dto.Height, color);
                case "Ellipse":
                    return new Ellipse(location, dto.Width, dto.Height, color);
                case "Triangle":
                    return new Triangle(location, new Point(dto.EndX, dto.EndY), new Point(dto.ThirdX, dto.ThirdY), color);
                case "Square":
                    return new Square(location, dto.Width, color);
                case "Circle":
                    return new Circle(new Point(location.X + dto.Width / 2, location.Y + dto.Height / 2), dto.Width / 2, color);
                case "Point":
                    return new PointShape(location, color);
                default:
                    return null;
            }
        }
    }
}
