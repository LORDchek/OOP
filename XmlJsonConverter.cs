using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace OOP
{
  /// <summary>
  /// Преобразование XML JSON для плагинов обработки данных.
  /// </summary>
  public static class XmlJsonConverter
  {
    public static string XmlToJson(string xml)
    {
      var document = XDocument.Parse(xml);
      if (document.Root == null)
        return "{}";

      var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
      return serializer.Serialize(ElementToObject(document.Root));
    }

    public static string JsonToXml(string json, string rootName = "Shapes")
    {
      var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
      var data = serializer.DeserializeObject(json);
      var root = ObjectToElement(rootName, data);
      return new XDocument(root).ToString();
    }

    private static object ElementToObject(XElement element)
    {
      var dict = new Dictionary<string, object>();

      foreach (var attribute in element.Attributes())
      {
        dict["@" + attribute.Name.LocalName] = attribute.Value;
      }

      var groupedChildren = element.Elements().GroupBy(e => e.Name.LocalName);
      foreach (var group in groupedChildren)
      {
        if (group.Count() == 1)
        {
          var child = group.First();
          dict[group.Key] = child.HasElements || child.Attributes().Any()
            ? ElementToObject(child)
            : child.Value;
        }
        else
        {
          dict[group.Key] = group.Select(ElementToObject).ToArray();
        }
      }

      if (!element.HasElements && !element.Attributes().Any())
        return element.Value;

      if (!element.HasElements && element.Attributes().Any() && string.IsNullOrEmpty(element.Value))
        return dict;

      if (!element.HasElements && !string.IsNullOrEmpty(element.Value))
      {
        if (dict.Count == 0)
          return element.Value;
        dict["#text"] = element.Value;
      }

      return dict;
    }

    private static XElement ObjectToElement(string name, object value)
    {
      if (value == null)
        return new XElement(name);

      if (value is string s)
        return new XElement(name, s);

      if (value is IDictionary<string, object> dict)
        return DictionaryToElement(name, dict);

      if (value is Dictionary<string, object> dict2)
        return DictionaryToElement(name, dict2);

      if (value is ArrayList list)
        return new XElement(name, list.Cast<object>().Select(item => ObjectToElement(name, item)));

      if (value is object[] array)
        return new XElement(name, array.Select(item => ObjectToElement(name, item)));

      return new XElement(name, Convert.ToString(value));
    }

    private static XElement DictionaryToElement(string name, IDictionary<string, object> dict)
    {
      var element = new XElement(name);

      foreach (var pair in dict)
      {
        if (pair.Key.StartsWith("@"))
        {
          element.SetAttributeValue(pair.Key.Substring(1), pair.Value);
          continue;
        }

        if (pair.Key == "#text")
        {
          element.Value = Convert.ToString(pair.Value);
          continue;
        }

        if (pair.Value is object[] array)
        {
          foreach (var item in array)
            element.Add(ObjectToElement(pair.Key, item));
        }
        else if (pair.Value is ArrayList list)
        {
          foreach (var item in list)
            element.Add(ObjectToElement(pair.Key, item));
        }
        else
        {
          element.Add(ObjectToElement(pair.Key, pair.Value));
        }
      }

      return element;
    }
  }
}
