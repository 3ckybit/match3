using System.Windows.Media;

namespace Match3.Model
{
    public class Tile
    {
        private int _top;
        private int _left;

        public Color Color { get; }

        public int Top
        {
            get => _top;
            set => _top = value;
        }

        public int Left
        {
            get => _left;
            set => _left = value;
        }

        public TileShape Shape { get; }

        public bool Selected { get; set; }

        public Tile(int top, int left, Color color)
        {
            Top = top;
            Left = left;
            Color = color;

            Shape = new TileShape
            {
                Fill = new SolidColorBrush(color),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2.0,
                Tag = this
            };
        }

        public void SwapCoordinates(ref Tile other)
        {
            Utility.SwapTiles(ref _top, ref other._top);
            Utility.SwapTiles(ref _left, ref other._left);
        }
    }
}

//Класс Tile представляет собой простую модель данных, представляющую плитку в игре. Он имеет свойства положения плитки на доске ( Top и Left), ее цвета ( Color) и ее графического представления ( Shape). У него также есть Selected свойство, указывающее, выбрана ли плитка в данный момент. Класс Tileпредоставляет SwapCoordinate s метод, который меняет местами свойства Top и Left со свойствами другого Tile объекта.