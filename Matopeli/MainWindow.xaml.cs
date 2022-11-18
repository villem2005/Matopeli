using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Formats.Asn1.AsnWriter;

namespace Matopeli
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly Brush SnakeColor = Brushes.BlueViolet;
        private readonly int HeadSize = (int)MadonKoko.Thick;
        private readonly List<Point> SnakePoints = new List<Point>();
        private int Lenght = 100;
        private Point CurrentPosition = new Point();
        private readonly Point StartingPoint = new Point(100, 100);
        private int Direction = 0;
        private readonly Random rnd = new Random();
        private readonly List<Point> bonusPoints = new List<Point>();
        private int Score = 0;
        private int previousDirection = 0;

        private enum MadonKoko
        {
            Thin = 4,
            Normal = 6,
            Thick = 8
        };
        private enum Suunta
        {
            Upwards = 8,
            Downwards = 2,
            Toleft = 4,
            Toright = 6
        };
        private enum Nopeus
        {
            Fast = 1,
            Moderate = 10000,
            Slow = 50000,
            VerySlow = 500000
        };
        


        
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);

            timer.Interval = new TimeSpan((int)Nopeus.Moderate);
            timer.Start();

            Mato(StartingPoint);
            CurrentPosition = StartingPoint;

            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            Mato(StartingPoint);
            CurrentPosition = StartingPoint;

            for (var n = 0; n < 10; n++)
            {
                Pisteet(n);
            }
           
        }


        //Tässä luodaan mato
        private void Mato(Point currentposition)
        {
            Ellipse newEllipse = new Ellipse
            {
                Fill = SnakeColor,
                Width = HeadSize,
                Height = HeadSize
            };

            Canvas.SetTop(newEllipse, currentposition.Y);
            Canvas.SetLeft(newEllipse, currentposition.X);

            int count = PaintCanvas.Children.Count;
            PaintCanvas.Children.Add(newEllipse);
            SnakePoints.Add(currentposition);

            if(count > Lenght)
            {
                PaintCanvas.Children.RemoveAt(count - Lenght + 9);
                SnakePoints.RemoveAt(count - Lenght);
            }

        }

        //Tässä luodaan pisteet
        private void Pisteet(int index)
        {
            Point bonusPoint = new Point(rnd.Next(5, 780), rnd.Next(5, 480));
            Ellipse newEllipse = new Ellipse
            {
                Fill = Brushes.Red,
                Width = HeadSize,
                Height = HeadSize
            };
            Canvas.SetTop(newEllipse, bonusPoint.Y);
            Canvas.SetLeft(newEllipse, bonusPoint.X);
            PaintCanvas.Children.Insert(index, newEllipse);
            bonusPoints.Insert(index, bonusPoint);
            
        }

        //Tässä tehdään timer
        private void timer_Tick(object sender, EventArgs e)
        {
            switch (Direction)
            {
                case (int)Suunta.Downwards:
                    CurrentPosition.Y += 1;
                    Mato(CurrentPosition);
                    break;
                case (int)Suunta.Upwards:
                    CurrentPosition.Y -= 1;
                    Mato(CurrentPosition);
                    break;
                case (int)Suunta.Toleft:
                    CurrentPosition.X -= 1;
                    Mato(CurrentPosition);
                    break;
                case (int)Suunta.Toright:
                    CurrentPosition.X += 1;
                    Mato(CurrentPosition);
                    break;
                   
            }

            
            if ((CurrentPosition.X < 5) || (CurrentPosition.X > 780) ||
                (CurrentPosition.Y < 5) || (CurrentPosition.Y > 480))
                GameOver();

            //Tässä saadaan pisteitä ja mato kasvamaan
            int n = 0;
            foreach (Point point in bonusPoints)
            {
                if ((Math.Abs(point.X - CurrentPosition.X) < HeadSize) &&
                    (Math.Abs(point.Y - CurrentPosition.Y) < HeadSize))
                {
                    Lenght += 10;
                    Score += 10;

                    bonusPoints.RemoveAt(n);
                    PaintCanvas.Children.RemoveAt(n);
                    Pisteet(n);
                    break;
                }
                n++;
            }

            for (int q = 0; q < (SnakePoints.Count - HeadSize * 2); q++)
            {
                Point point = new Point(SnakePoints[q].X, SnakePoints[q].Y);
                if ((Math.Abs(point.X - CurrentPosition.X) < (HeadSize))&&
                    (Math.Abs(point.Y - CurrentPosition.Y) < (HeadSize)))
                {
                    GameOver();
                    break;
                }
            }

        }

        //Tässä saadaan mato liikkumaan painamalla WASD
        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.S:
                    if (previousDirection != (int)Suunta.Upwards)
                        Direction = (int)Suunta.Downwards;
                    break;
                case Key.W:
                    if (previousDirection != (int)Suunta.Downwards)
                        Direction = (int)Suunta.Upwards;
                    break;
                case Key.A:
                    if (previousDirection != (int)Suunta.Toright)
                        Direction = (int)Suunta.Toleft;
                    break;
                case Key.D:
                    if (previousDirection != (int)Suunta.Toleft)
                        Direction = (int)Suunta.Toright;
                    break;
            }
            previousDirection = Direction;
        }

        //Kun häviää, tulee messagebox jossa näkyy pisteet
        private void GameOver()
        {
            MessageBox.Show($@"Hävisit! Pisteesi: {Score}", MessageBoxButton.OK.ToString());
            this.Close();
        }
        
    }
}

