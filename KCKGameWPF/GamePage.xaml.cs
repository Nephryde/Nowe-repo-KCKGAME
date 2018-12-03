﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KCKGameWPF
{
    public partial class GamePage : Page
    {
        static readonly int left = 4;
        static readonly int right = 6;
        static readonly int up = 8;
        static readonly int down = 2;

        private readonly int headSize = 6;
        private readonly Brush snake1Color = Brushes.Green;
        private readonly Brush snake2Color = Brushes.Red;
        private readonly Brush obstacleColor = Brushes.Yellow;

        static int firstPlayerScore = 0;
        static int firstPlayerDirection = right;
        private Point firstPlayerPosition = new Point();

        static int secondPlayerScore = 0;
        static int secondPlayerDirection = left;
        private Point secondPlayerPosition = new Point();

        private Point startingPoint;
        private Point startingPoint2;

        static bool[,] isUsed;

        int totalRoundNumber, roundNumber;

        //Do trybu speed, potem można usunąć
        private enum GameSpeed
        {
            Fast = 1,
            Moderate = 10000,
            Slow = 50000,
            DamnSlow = 500000
        };

        public GamePage()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(TimerTick);

            timer.Interval = new TimeSpan((int)GameSpeed.Moderate);
            timer.Start();

            Application.Current.MainWindow.KeyDown += new KeyEventHandler(ChangePlayerDirection);

            Game("obstacles");

            string level = "obstacles";

            totalRoundNumber = 10;
            roundNumber = 1;

            if (level == "obstacles")
                MakeObstacles(roundNumber);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            MovePlayers();

            bool firstPlayerLoses = DoesPlayerLose(firstPlayerPosition);
            bool secondPlayerLoses = DoesPlayerLose(secondPlayerPosition);

            if (firstPlayerLoses || secondPlayerLoses)
            {
                MessageBox.Show("Wynik");
                ResetGame("obstacles");

            }

            FillUsed(firstPlayerPosition, firstPlayerDirection);
            FillUsed(secondPlayerPosition, secondPlayerDirection);

            WriteOnPosition(firstPlayerPosition, snake1Color);
            WriteOnPosition(secondPlayerPosition, snake2Color);
        }

        private void Game(string level)
        {
            SetGameField();

            int totalRoundNumber = 10;
            int roundNumber = 1;

            if (level == "obstacles")
                MakeObstacles(roundNumber);

        }

        private void ResetGame(string level)
        {
            InvalidateVisual();
            PaintCanvas.Children.Clear();

            SetGameField();
            firstPlayerDirection = right;
            secondPlayerDirection = left;

            roundNumber++;

            if (level == "obstacles" && roundNumber < 10)
                MakeObstacles(roundNumber);

            MovePlayers();
        }


        ////
        // Nowe
        ////

        private void Navigate()
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("MainMenuPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void SetGameField()
        {
            isUsed = new bool[800, 500];

            startingPoint = new Point(4, 230);
            startingPoint2 = new Point(776, 230);

            WriteOnPosition(startingPoint, snake1Color);
            firstPlayerPosition = startingPoint;

            WriteOnPosition(startingPoint2, snake2Color);
            secondPlayerPosition = startingPoint2;

            isUsed[(int)firstPlayerPosition.X, (int)firstPlayerPosition.Y] = true;
            isUsed[(int)secondPlayerPosition.X, (int)secondPlayerPosition.Y] = true;
        }

        private void FillUsed(Point playerPosition, int playerDirection)
        {
            isUsed[(int)playerPosition.X, (int)playerPosition.Y] = true;

            if (playerDirection == left)
            {
                for (int i = -4; i <= 4; i++)
                {
                    isUsed[(int)playerPosition.X + 1, (int)playerPosition.Y + i] = true;
                }
            }
            else if (playerDirection == right)
            {
                for (int i = -4; i <= 4; i++)
                {
                    isUsed[(int)playerPosition.X - 1, (int)playerPosition.Y + i] = true;
                }
            }
            else if (playerDirection == down)
            {
                for (int i = -4; i <= 4; i++)
                {
                    isUsed[(int)playerPosition.X + i, (int)playerPosition.Y - 1] = true;
                }
            }
            else if (playerDirection == up)
            {
                for (int i = -4; i <= 4; i++)
                {
                    isUsed[(int)playerPosition.X + i, (int)playerPosition.Y + 1] = true;
                }
            }
        }

        private void FillUsedObstacles(Point obstaclePosition)
        {
            for (int i = -6; i < 13; i++)
            {
                for (int j = -6; j < 13; j++)
                {
                    isUsed[(int)obstaclePosition.X + i, (int)obstaclePosition.Y + j] = true;
                }
            }
        }

        private void MakeObstacles(int roundNumber)
        {
            Random random = new Random();

            for (int i = 1; i <= roundNumber * 2 + 5; i++)
            {
                Point bonusPoint = new Point(random.Next(100, 680), random.Next(70, 415));

                FillUsedObstacles(bonusPoint);
                WriteOnObstacle(bonusPoint, obstacleColor);
            }
        }

        private void ChangePlayerDirection(object sender, KeyEventArgs key)
        {
            if (key.Key == Key.W && firstPlayerDirection != down)
            {
                firstPlayerDirection = up;
            }
            if (key.Key == Key.A && firstPlayerDirection != right)
            {
                firstPlayerDirection = left;
            }
            if (key.Key == Key.D && firstPlayerDirection != left)
            {
                firstPlayerDirection = right;
            }
            if (key.Key == Key.S && firstPlayerDirection != up)
            {
                firstPlayerDirection = down;
            }

            if (key.Key == Key.Up && secondPlayerDirection != down)
            {
                secondPlayerDirection = up;
            }
            if (key.Key == Key.Left && secondPlayerDirection != right)
            {
                secondPlayerDirection = left;
            }
            if (key.Key == Key.Right && secondPlayerDirection != left)
            {
                secondPlayerDirection = right;
            }
            if (key.Key == Key.Down && secondPlayerDirection != up)
            {
                secondPlayerDirection = down;
            }
        }

        private void MovePlayers()
        {
            if (firstPlayerDirection == right)
            {
                firstPlayerPosition.X += 1;
            }
            if (firstPlayerDirection == left)
            {
                firstPlayerPosition.X -= 1;
            }
            if (firstPlayerDirection == up)
            {
                firstPlayerPosition.Y -= 1;
            }
            if (firstPlayerDirection == down)
            {
                firstPlayerPosition.Y += 1;
            }

            if (secondPlayerDirection == right)
            {
                secondPlayerPosition.X += 1;
            }
            if (secondPlayerDirection == left)
            {
                secondPlayerPosition.X -= 1;
            }
            if (secondPlayerDirection == up)
            {
                secondPlayerPosition.Y -= 1;
            }
            if (secondPlayerDirection == down)
            {
                secondPlayerPosition.Y += 1;
            }
        }

        private bool DoesPlayerLose(Point playerPosition)
        {
            if ((playerPosition.X < 4) || (playerPosition.X > 776) ||
                (playerPosition.Y < 4) || (playerPosition.Y > 452))
                return true;

            if (isUsed[(int)playerPosition.X, (int)playerPosition.Y])
                return true;

            return false;
        }

        private void WriteOnPosition(Point currentposition, Brush color)
        {
            Ellipse newEllipse = new Ellipse
            {
                Fill = color,
                Width = headSize,
                Height = headSize
            };

            Canvas.SetTop(newEllipse, currentposition.Y);
            Canvas.SetLeft(newEllipse, currentposition.X);

            PaintCanvas.Children.Add(newEllipse);
        }

        private void WriteOnObstacle(Point currentposition, Brush color)
        {
            Rectangle rectangle = new Rectangle
            {
                Fill = color,
                Width = 13,
                Height = 13,
                Stroke = Brushes.DarkOrange,
                StrokeThickness = 2
            };

            Canvas.SetTop(rectangle, currentposition.Y);
            Canvas.SetLeft(rectangle, currentposition.X);

            PaintCanvas.Children.Add(rectangle);
        }
    }
}
