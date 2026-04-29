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

        public PluginHostContext(ShapeCreator shapeCreator, ShapeRendererRegistry rendererRegistry, ShapeSerializerRegistry serializerRegistry)
        {
            ShapeCreator = shapeCreator ?? throw new ArgumentNullException(nameof(shapeCreator));
            RendererRegistry = rendererRegistry ?? throw new ArgumentNullException(nameof(rendererRegistry));
            SerializerRegistry = serializerRegistry ?? throw new ArgumentNullException(nameof(serializerRegistry));
        }

        public void RegisterFactory(IShapeFactory factory) => ShapeCreator.RegisterFactory(factory);
        public void RegisterRenderer(IShapeRenderer renderer) => RendererRegistry.Register(renderer);
        public void RegisterSerializer(IShapeSerializer serializer) => SerializerRegistry.Register(serializer);
    }

    public sealed class PluginLoader
    {
        public int LoadFromFolder(string folderPath, PluginHostContext context)
        {
            if (!Directory.Exists(folderPath)) return 0;

            int loadedPlugins = 0;
            foreach (var dllPath in Directory.GetFiles(folderPath, "*.dll", SearchOption.TopDirectoryOnly))
            {
                loadedPlugins += LoadFromAssembly(dllPath, context);
            }

            return loadedPlugins;
        }

        public int LoadFromArguments(IEnumerable<string> modulePaths, PluginHostContext context)
        {
            if (modulePaths == null) return 0;

            int loadedPlugins = 0;
            foreach (var modulePath in modulePaths.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                var fullPath = Path.IsPathRooted(modulePath)
                    ? modulePath
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, modulePath);

                if (!File.Exists(fullPath)) continue;
                loadedPlugins += LoadFromAssembly(fullPath, context);
            }

            return loadedPlugins;
        }

        private int LoadFromAssembly(string assemblyPath, PluginHostContext context)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            int loaded = 0;

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface) continue;
                if (!typeof(IShapePlugin).IsAssignableFrom(type)) continue;

                if (!(Activator.CreateInstance(type) is IShapePlugin plugin)) continue;
                plugin.Register(context);
                loaded++;
            }

            return loaded;
        }
    }
}

