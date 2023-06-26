﻿using Match3.Model;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Match3
{
    /// <summary>
    /// Interaction logic for GameInterface.xaml
    /// </summary>
    public partial class GameInterface : UserControl
    {
        private Game _game;
        private readonly Settings _settings;

        public GameInterface(Settings settings)
        {
            InitializeComponent();
            _settings = settings;
            _game = new Game(RegisterTile, UnregisterTile, DropAnimation, _settings.GameTime);
            DataContext = _game;
        }

        private void RegisterTile(Tile tile)
        {
            tile.Shape.Height = GameCanvas.Height / _settings.BoardSize.ColumnCount;
            tile.Shape.Width = GameCanvas.Width / _settings.BoardSize.RowCount;
            tile.Shape.RenderTransform =
                new ScaleTransform(1.0, 1.0, tile.Shape.Height / 2, tile.Shape.Width / 2);
            GameCanvas.Children.Add(tile.Shape);
            Canvas.SetTop(tile.Shape, tile.Top * tile.Shape.Height);
            Canvas.SetLeft(tile.Shape, tile.Left * tile.Shape.Width);
        }

        private int _dropAnimationRegister;

        private void DropAnimation(Tile tile)
        {
            _dropAnimationRegister++;
            DoubleAnimation animTop = new DoubleAnimation
            {
                To = tile.Top * tile.Shape.Height,
                Duration = TimeSpan.FromMilliseconds(200),
            };
            animTop.Completed += delegate
            {
                _dropAnimationRegister--;
                if (_dropAnimationRegister != 0)
                {
                    return;
                }

                _game.FillBoard(RegisterTile);
                _game.RemoveMatches(DeleteAnimation);
                if (_isFirstStartSelection)
                {
                    _game.Points = 0;
                }
            };
            tile.Shape.BeginAnimation(Canvas.TopProperty, animTop);
        }

        private bool _isFirstStartSelection = true;

        private void StartSelectionAnimation(Tile tile)
        {
            DoubleAnimation anim = new DoubleAnimation
            {
                From = 1.0,
                To = 0.8,
                Duration = TimeSpan.FromMilliseconds(300),
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true,
            };
            tile.Shape.RenderTransform.BeginAnimation(
                ScaleTransform.ScaleXProperty, anim);
            tile.Shape.RenderTransform.BeginAnimation(
                ScaleTransform.ScaleYProperty, anim);
            _isFirstStartSelection = false;
        }

        private void StopSelectionAnimation(Tile tile)
        {
            tile.Shape.RenderTransform.BeginAnimation(
                ScaleTransform.ScaleXProperty, null);
            tile.Shape.RenderTransform.BeginAnimation(
                ScaleTransform.ScaleYProperty, null);
        }

        private int _deleteAnimationRegister;

        private void DeleteAnimation(Tile tile)
        {
            _deleteAnimationRegister += 2;
            DoubleAnimation anim = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(300),
            };
            anim.Completed += delegate
            {
                _deleteAnimationRegister--;
                if (_deleteAnimationRegister == 0)
                {
                    _game.DeleteAndDropTiles(
                        DropAnimation, RegisterTile, UnregisterTile);
                }
            };
            tile.Shape.RenderTransform.BeginAnimation(
                ScaleTransform.ScaleXProperty, anim);
            tile.Shape.RenderTransform.BeginAnimation(
                ScaleTransform.ScaleYProperty, anim);
        }

        private int _successAnimationRegister;

        private void OnSuccessAnimationComplete(object o, EventArgs e)
        {
            _successAnimationRegister--;
            if (_successAnimationRegister == 0)
            {
                _game.RemoveMatches(DeleteAnimation);
            }
        }

        private void SuccessAnimation(Tile first, Tile second)
        {
            _successAnimationRegister += 2;
            AnimateSwap(first, second, OnSuccessAnimationComplete);
        }

        private void AnimateSwap(Tile first, Tile second, Action<object, EventArgs> onCompleted)
        {
            int dt = Math.Sign(Math.Abs(first.Top - second.Top));
            int dl = Math.Sign(Math.Abs(first.Left - second.Left));
            DoubleAnimation animFirst = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(200),
            };
            DoubleAnimation animSecond = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(200),
            };
            animFirst.Completed += (o, ea) => onCompleted(o, ea);
            animSecond.Completed += (o, ea) => onCompleted(o, ea);
            if (dt == 1)
            {
                animSecond.To = second.Top * second.Shape.Height;
                animFirst.To = first.Top * first.Shape.Height;
                first.Shape.BeginAnimation(Canvas.TopProperty, animFirst);
                second.Shape.BeginAnimation(Canvas.TopProperty, animSecond);
            }
            else if (dl == 1)
            {
                animSecond.To = second.Left * second.Shape.Width;
                animFirst.To = first.Left * first.Shape.Width;
                first.Shape.BeginAnimation(Canvas.LeftProperty, animFirst);
                second.Shape.BeginAnimation(Canvas.LeftProperty, animSecond);
            }
            else
            {
                throw new InvalidOperationException("Движение по диагонали!");
            }
        }

        private int _failAnimationRegister;

        private void FailAnimation(Tile first, Tile second)
        {
            _failAnimationRegister += 2;
            first.SwapCoordinates(ref second);
            AnimateSwap(
                first, second, (o1, e1) =>
                {
                    _failAnimationRegister--;
                    if (_failAnimationRegister != 0)
                    {
                        return;
                    }

                    first.SwapCoordinates(ref second);
                    _failAnimationRegister += 2;
                    AnimateSwap(
                        first, second, (o2, e2) => { _failAnimationRegister--; });
                });
        }

        private void UnregisterTile(Tile tile)
        {
            GameCanvas.Children.Remove(tile.Shape);
        }

        private Tile _selected;

        private void GameCanvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_failAnimationRegister + _deleteAnimationRegister +
                _successAnimationRegister + _dropAnimationRegister > 0)
            {
                return;
            }

            if (!(e.OriginalSource is TileShape ts))
            {
                return;
            }

            Tile t = (Tile)ts.Tag;
            if (t.Selected)
            {
                t.Selected = false;
                StopSelectionAnimation(t);
                _selected = null;
            }
            else
            {
                if (_selected != null)
                {
                    Tile tempTile = _selected;
                    _selected.Selected = false;
                    StopSelectionAnimation(_selected);
                    _selected = null;
                    _game.TrySwapTiles(t, tempTile, SuccessAnimation, FailAnimation);
                }
                else
                {
                    t.Selected = true;
                    _selected = t;
                    StartSelectionAnimation(t);
                }
            }
        }

        private void ReturnToMenuButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu(_settings));
        }

        private void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _game = new Game(RegisterTile, UnregisterTile, DropAnimation, _settings.GameTime);
            DataContext = _game;
            Switcher.PageSwitcher.Navigate(new GameInterface(_settings));
        }
        public GameInterface()
        {
            InitializeComponent();

            _game = new Game(RegisterTile, UnregisterTile, DropAnimation, 10);
            DataContext = _game;
        }
    }
}


//Класс GameInterface представляет собой пользовательский элемент управления, предоставляющий графический интерфейс для игры.
//Он создает экземпляр Game класса и устанавливает его в качестве DataContext элемента управления, что позволяет легко связать игровые данные с элементами элемента управления.
//Класс GameInterface также предоставляет ряд методов обратного вызова анимации, которые используются Game классом для анимации различных действий, таких как замена плиток, удаление совпадающих плиток и добавление новых плиток на игровое поле.
