using Match3.Model;
using System.Windows;
using System.Windows.Controls;

namespace Match3
{
    /// <summary>
    /// Interaction logic for GameOver.xaml
    /// </summary>
    public partial class GameOver : UserControl
    {
        private readonly Settings _standartSettings;
        public int Points { get; }

        public GameOver(int points)
        {
            InitializeComponent();

            _standartSettings = new Settings(new BoardSize(8, 8), 1);
            Points = points;
            DataContext = this;
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu(_standartSettings));
        }
    }
}

//Класс GameOver— это пользовательский элемент управления, который отображается, когда игра заканчивается. Он отображает окончательный счет игрока и предоставляет кнопку, которая позволяет игроку вернуться в главное меню. Класс GameOverустанавливает себя как DataContext элемент управления, что позволяет легко привязывать его данные к элементам управления.
//При нажатии кнопки «ОК» управление переключается обратно в главное меню.