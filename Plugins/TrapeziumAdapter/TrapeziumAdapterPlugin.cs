using OOP;

namespace TrapeziumAdapter
{
  /// <summary>
  /// Плагин-адаптер: подключает TrapeziumLibrary товарища к хосту OOP.
  /// Паттерн Adapter — новый API друга приводится к IShape / IShapeFactory / IShapeRenderer / IShapeSerializer.
  /// </summary>
  public sealed class TrapeziumAdapterPlugin : IShapePlugin
  {
    public string PluginName => "Trapezium adapter (friend's plugin)";

    public void Register(PluginHostContext context)
    {
      context.RegisterFactory(new TrapeziumFactoryAdapter());
      context.RegisterRenderer(new TrapeziumRendererAdapter());
      context.RegisterSerializer(new TrapeziumSerializerAdapter());
    }
  }
}
