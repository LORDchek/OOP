using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OOP.Factories;

namespace OOP
{
  public interface IShapePlugin
  {
    string PluginName { get; }
    void Register(PluginHostContext context);
  }

  public sealed class PluginHostContext
  {
    public ShapeCreator ShapeCreator { get; }
    public ShapeRendererRegistry RendererRegistry { get; }
    public ShapeSerializerRegistry SerializerRegistry { get; }
    public DataProcessingPipeline DataPipeline { get; }

    public PluginHostContext(
      ShapeCreator shapeCreator,
      ShapeRendererRegistry rendererRegistry,
      ShapeSerializerRegistry serializerRegistry,
      DataProcessingPipeline dataPipeline)
    {
      ShapeCreator = shapeCreator ?? throw new ArgumentNullException(nameof(shapeCreator));
      RendererRegistry = rendererRegistry ?? throw new ArgumentNullException(nameof(rendererRegistry));
      SerializerRegistry = serializerRegistry ?? throw new ArgumentNullException(nameof(serializerRegistry));
      DataPipeline = dataPipeline ?? throw new ArgumentNullException(nameof(dataPipeline));
    }

    public void RegisterFactory(IShapeFactory factory) => ShapeCreator.RegisterFactory(factory);
    public void RegisterRenderer(IShapeRenderer renderer) => RendererRegistry.Register(renderer);
    public void RegisterSerializer(IShapeSerializer serializer) => SerializerRegistry.Register(serializer);
    public void RegisterDataProcessor(IDataProcessingPlugin processor) => DataPipeline.Register(processor);
  }

  public sealed class PluginLoadResult
  {
    public int ShapePlugins { get; set; }
    public int DataPlugins { get; set; }
    public int Total => ShapePlugins + DataPlugins;
  }

  public sealed class PluginLoader
  {
    private readonly HashSet<string> loadedAssemblyPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public PluginLoadResult LoadFromFolder(string folderPath, PluginHostContext context)
    {
      var result = new PluginLoadResult();
      if (!Directory.Exists(folderPath)) return result;

      foreach (var dllPath in Directory.GetFiles(folderPath, "*.dll", SearchOption.TopDirectoryOnly))
      {
        var partial = LoadFromFile(dllPath, context);
        result.ShapePlugins += partial.ShapePlugins;
        result.DataPlugins += partial.DataPlugins;
      }

      return result;
    }

    public PluginLoadResult LoadFromArguments(IEnumerable<string> modulePaths, PluginHostContext context)
    {
      var result = new PluginLoadResult();
      if (modulePaths == null) return result;

      foreach (var modulePath in modulePaths.Where(p => !string.IsNullOrWhiteSpace(p)))
      {
        var fullPath = Path.IsPathRooted(modulePath)
          ? modulePath
          : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, modulePath);

        if (!File.Exists(fullPath)) continue;

        var partial = LoadFromFile(fullPath, context);
        result.ShapePlugins += partial.ShapePlugins;
        result.DataPlugins += partial.DataPlugins;
      }

      return result;
    }

    public PluginLoadResult LoadFromFile(string assemblyPath, PluginHostContext context)
    {
      var result = new PluginLoadResult();
      var fullPath = Path.GetFullPath(assemblyPath);
      if (!File.Exists(fullPath)) return result;
      if (loadedAssemblyPaths.Contains(fullPath)) return result;

      loadedAssemblyPaths.Add(fullPath);
      var assembly = Assembly.LoadFrom(fullPath);

      foreach (var type in assembly.GetTypes())
      {
        if (type.IsAbstract || type.IsInterface) continue;

        if (typeof(IShapePlugin).IsAssignableFrom(type))
        {
          if (Activator.CreateInstance(type) is IShapePlugin shapePlugin)
          {
            shapePlugin.Register(context);
            result.ShapePlugins++;
          }
        }

        if (typeof(IDataProcessingPlugin).IsAssignableFrom(type))
        {
          if (Activator.CreateInstance(type) is IDataProcessingPlugin dataPlugin)
          {
            context.DataPipeline.Register(dataPlugin);
            result.DataPlugins++;
          }
        }
      }

      return result;
    }

    public void ResetLoadedAssemblies()
    {
      loadedAssemblyPaths.Clear();
    }
  }
}
