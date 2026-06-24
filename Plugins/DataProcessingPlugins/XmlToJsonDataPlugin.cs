using System;
using OOP;

namespace DataProcessingPlugins
{
  public sealed class XmlToJsonDataPlugin : IDataProcessingPlugin
  {
    public string Id => "xml-to-json";
    public string MenuText => "Трансформация XML → JSON";
    public int Order => 100;
    public bool IsEnabled { get; set; }

    public DataProcessResult ProcessBeforeSave(string content, string currentFormat)
    {
      if (!string.Equals(currentFormat, "xml", StringComparison.OrdinalIgnoreCase))
        return DataProcessResult.Unchanged(content);

      return DataProcessResult.WithFormat(XmlJsonConverter.XmlToJson(content), "json");
    }

    public DataProcessResult ProcessAfterLoad(string content, string currentFormat)
    {
      if (!string.Equals(currentFormat, "json", StringComparison.OrdinalIgnoreCase))
        return DataProcessResult.Unchanged(content);

      return DataProcessResult.WithFormat(XmlJsonConverter.JsonToXml(content), "xml");
    }
  }
}
