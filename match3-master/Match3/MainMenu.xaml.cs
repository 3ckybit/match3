using Match3.Model;
using System.Windows;
using System.Windows.Controls;

namespace Match3
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        private readonly Settings _settings;

        public MainMenu(Settings settings)
        {
            InitializeComponent();

            _settings = settings;
        }

        public void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new GameInterface(_settings));
        }

        public void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new SettingsWindow());
        }

        public void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        
    }
}



//Класс MainMenu представляет собой пользовательский элемент управления, предоставляющий главное меню игры. На нем есть кнопки, которые позволяют игроку начать игру, изменить настройки игры и выйти из игры.
//При нажатии кнопки «Начать игру» управление переключается на GameInterface управление пользователем.
//При нажатии кнопки «Настройки» элемент управления переключается на SettingsWindow пользовательский элемент управления.
//При нажатии кнопки «Выход» приложение закрывается.