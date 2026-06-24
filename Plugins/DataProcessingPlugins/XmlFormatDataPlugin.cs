using System;
using System.Xml.Linq;
using OOP;

namespace DataProcessingPlugins
{
  public sealed class XmlFormatDataPlugin : IDataProcessingPlugin
  {
    public string Id => "xml-format";
    public string MenuText => "Форматирование XML";
    public int Order => 20;
    public bool IsEnabled { get; set; }

    public DataProcessResult ProcessBeforeSave(string content, string currentFormat)
    {
      if (!string.Equals(currentFormat, "xml", StringComparison.OrdinalIgnoreCase))
        return DataProcessResult.Unchanged(content);

      return DataProcessResult.Unchanged(FormatXml(content));
    }

    public DataProcessResult ProcessAfterLoad(string content, string currentFormat)
    {
      if (!string.Equals(currentFormat, "xml", StringComparison.OrdinalIgnoreCase))
        return DataProcessResult.Unchanged(content);

      return DataProcessResult.Unchanged(FormatXml(content));
    }

    private static string FormatXml(string xml)
    {
      return XDocument.Parse(xml).ToString();
    }
  }
}
