using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using OOP.Shapes;

namespace OOP
{
    public class ShapeXmlService
    {
        private readonly ShapeSerializerRegistry serializerRegistry;

        public ShapeXmlService(ShapeSerializerRegistry serializerRegistry)
        {
            this.serializerRegistry = serializerRegistry ?? throw new ArgumentNullException(nameof(serializerRegistry));
        }

        public void Save(string filePath, IEnumerable<IShape> shapes)
        {
            var root = new XElement("Shapes");
            foreach (var shape in shapes)
            {
                var serializer = serializerRegistry.Get(shape.Name);
                if (serializer == null) continue;
                root.Add(serializer.Serialize(shape));
            }

            var doc = new XDocument(root);
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                doc.Save(stream);
            }
        }

        public List<IShape> Load(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var result = new List<IShape>();

            var root = doc.Root;
            if (root == null)
            {
                return result;
            }

            foreach (var element in root.Elements("Shape"))
            {
                var typeKey = (string)element.Attribute("Type");
                var serializer = serializerRegistry.Get(typeKey);
                if (serializer == null) continue;
                result.Add(serializer.Deserialize(element));
            }

            return result;
        }
    }
}
