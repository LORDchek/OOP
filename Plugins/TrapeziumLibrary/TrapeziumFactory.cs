using NewGraphicEditor.Controls;
using NewGraphicEditor.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace TrapeziumLibrary
{
    public class TrapeziumFactory : IDrawableShape
    {
        public void Draw(Canvas canvas, Shapes shape)
        {
            if (shape is Trapezium tr)
            {

                var points = new PointCollection();
                //чтобы существовала ета трапеция, нужно чтобы вектора AB и CD были коллинеарны (параллельны)
                //вычисляю вектор AB
                int ax = tr.point[2] - tr.point[0]; 
                int ay = tr.point[3] - tr.point[1];
                //вычисляю векторс CD
                int bx = tr.point[4] - tr.point[6];
                int by = tr.point[5] - tr.point[7];

                if (ax * by == ay * bx)
                {
                    for (int i = 0; i < tr.point.Length; i += 2)
                    {
                        points.Add(new Point(tr.point[i], tr.point[i + 1]));
                    }

                    var newTrapezium = new System.Windows.Shapes.Polygon()
                    {
                        Stroke = Brushes.Tan,
                        Fill = Brushes.Aqua
                    };

                    newTrapezium.Points = points;
                    canvas.Children.Add(newTrapezium);
                }
                else
                    MessageBox.Show("Трапеция с такими координатами существовать не может...");
               
            }
        }
    }
}
