using System.Windows.Controls;
using NewGraphicEditor.Data;

namespace NewGraphicEditor.Controls
{
  public interface IDrawableShape
  {
    void Draw(Canvas canvas, Shapes shape);
  }
}
