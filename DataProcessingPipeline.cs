using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP
{
  public sealed class DataProcessResult
  {
    public string Content { get; }
    /// <summary>null — формат не менялся</summary>
    public string Format { get; }

    public DataProcessResult(string content, string format)
    {
      Content = content ?? throw new ArgumentNullException(nameof(content));
      Format = format;
    }

    public static DataProcessResult Unchanged(string content) => new DataProcessResult(content, null);
    public static DataProcessResult WithFormat(string content, string format) => new DataProcessResult(content, format);
  }

  public interface IDataProcessingPlugin
  {
    string Id { get; }
    string MenuText { get; }
    /// <summary>Меньшее значение — раньше при сохранении.</summary>
    int Order { get; }
    bool IsEnabled { get; set; }
    DataProcessResult ProcessBeforeSave(string content, string currentFormat);
    DataProcessResult ProcessAfterLoad(string content, string currentFormat);
  }

  public sealed class DataProcessingPipeline
  {
    private readonly List<IDataProcessingPlugin> plugins = new List<IDataProcessingPlugin>();

    public IReadOnlyList<IDataProcessingPlugin> Plugins => plugins;

    public string OutputFormat { get; private set; } = "xml";

    public void Register(IDataProcessingPlugin plugin)
    {
      if (plugin == null) return;
      if (plugins.Any(p => string.Equals(p.Id, plugin.Id, StringComparison.OrdinalIgnoreCase))) return;
      plugins.Add(plugin);
    }

    public void Clear()
    {
      plugins.Clear();
      OutputFormat = "xml";
    }

    public string ProcessBeforeSave(string content)
    {
      string current = content;
      string format = "xml";

      foreach (var plugin in plugins.Where(p => p.IsEnabled).OrderBy(p => p.Order))
      {
        var result = plugin.ProcessBeforeSave(current, format);
        current = result.Content;
        if (!string.IsNullOrEmpty(result.Format))
          format = result.Format;
      }

      OutputFormat = format;
      return current;
    }

    public string ProcessAfterLoad(string content)
    {
      string format = DetectFormat(content);

      // Обратный порядок относительно сохранения
      foreach (var plugin in plugins.Where(p => p.IsEnabled).OrderByDescending(p => p.Order))
      {
        var result = plugin.ProcessAfterLoad(content, format);
        content = result.Content;
        if (!string.IsNullOrEmpty(result.Format))
          format = result.Format;
      }

      OutputFormat = format;
      return content;
    }

    public string GetSaveFileFilter()
    {
      if (string.Equals(OutputFormat, "json", StringComparison.OrdinalIgnoreCase))
        return "JSON files (*.json)|*.json|XML files (*.xml)|*.xml|All files (*.*)|*.*";

      return "XML files (*.xml)|*.xml|JSON files (*.json)|*.json|All files (*.*)|*.*";
    }

    public string GetLoadFileFilter()
    {
      return "XML/JSON (*.xml;*.json)|*.xml;*.json|XML files (*.xml)|*.xml|JSON files (*.json)|*.json|All files (*.*)|*.*";
    }

    public string GetDefaultExtension()
    {
      return string.Equals(OutputFormat, "json", StringComparison.OrdinalIgnoreCase) ? "json" : "xml";
    }

    private static string DetectFormat(string content)
    {
      if (string.IsNullOrWhiteSpace(content)) return "xml";
      var trimmed = content.TrimStart();
      if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
        return "json";
      return "xml";
    }
  }
}
