using NewGraphicEditor.Data;
using System.Windows;


namespace TrapeziumLibrary
{
    public class Trapezium : Shapes
    {
        private int[] _point = new int[8];
        public override int[] point => _point;
        public override string nameShapes => "Trapezium";
        protected override int countPoint => 8;

        public Trapezium()
        {
            NameShape = "Трапеция";
            Info = "Поочередно вводить координаты точек: " +
                   "(xn, yn)";
        }

        public void SetPoints(int[] values)
        {
            if (values == null || values.Length != 8)
                return;
            System.Array.Copy(values, _point, 8);
        }
    }
}
