using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using OOP.Shapes;

namespace OOP
{
  public class ShapeXmlService
  {
    private readonly ShapeSerializerRegistry serializerRegistry;
    private readonly DataProcessingPipeline dataPipeline;

    public ShapeXmlService(ShapeSerializerRegistry serializerRegistry, DataProcessingPipeline dataPipeline)
    {
      this.serializerRegistry = serializerRegistry ?? throw new ArgumentNullException(nameof(serializerRegistry));
      this.dataPipeline = dataPipeline ?? throw new ArgumentNullException(nameof(dataPipeline));
    }

    public void Save(string filePath, IEnumerable<IShape> shapes)
    {
      var xml = SerializeToXml(shapes);
      var processed = dataPipeline.ProcessBeforeSave(xml);
      File.WriteAllText(filePath, processed, Encoding.UTF8);
    }

    public List<IShape> Load(string filePath)
    {
      var raw = File.ReadAllText(filePath, Encoding.UTF8);
      var processed = dataPipeline.ProcessAfterLoad(raw);
      return DeserializeFromXml(processed);
    }

    private string SerializeToXml(IEnumerable<IShape> shapes)
    {
      var root = new XElement("Shapes");
      foreach (var shape in shapes)
      {
        var serializer = serializerRegistry.Get(shape.Name);
        if (serializer == null) continue;
        root.Add(serializer.Serialize(shape));
      }

      return new XDocument(root).ToString();
    }

    private List<IShape> DeserializeFromXml(string xml)
    {
      var result = new List<IShape>();
      var doc = XDocument.Parse(xml);
      var root = doc.Root;
      if (root == null) return result;

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
