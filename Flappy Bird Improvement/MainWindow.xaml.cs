using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;  // for the timer

namespace Flappy_Bird_Improvement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        DispatcherTimer gameTimer = new DispatcherTimer();
        double score;
        int gravity = 8;
        bool gameOver;
        Rect FlappyBirdHitBox;
        private MediaPlayer player1 = new MediaPlayer();
        private MediaPlayer player2 = new MediaPlayer();
        private MediaPlayer player3 = new MediaPlayer();
        private MediaPlayer player4 = new MediaPlayer();
        private MediaPlayer player5 = new MediaPlayer();
        private SoundPlayer backGroundMusic = new SoundPlayer();
        private bool isKeyRepeating = false;

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += MainEventTimer;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            StartGame();
        }

        private void MainEventTimer(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score;

            FlappyBirdHitBox = new Rect(Canvas.GetLeft(flappyBird), Canvas.GetTop(flappyBird), flappyBird.Width -25, flappyBird.Height -25);
            Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + gravity);

            if (Canvas.GetTop(flappyBird) < -10 || Canvas.GetTop(flappyBird)> 458)
            {
                
               EndGame();
            }

            foreach (var x in MyCanvas.Children.OfType<Image>())
            {
                if ((string)x.Tag == "obs1" || (string)x.Tag == "obs2" || (string)x.Tag == "obs3")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - 5);
                    if (Canvas.GetLeft(x) < -100)
                    {
                        Canvas.SetLeft(x, 800);
                        score += .5;
                        if(score % 5 == 0)
                        {
                            player5.Open(new Uri("../../Sound/sfx_point.wav", UriKind.RelativeOrAbsolute));
                            player5.Play();
                        }
                        
                    }

                    Rect pipeHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (FlappyBirdHitBox.IntersectsWith(pipeHitBox))
                    {
                        
                        EndGame();
                    }

                 
                }

                if ((string)x.Tag == "clouds")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - 2);

                    if (Canvas.GetLeft(x) < -250)
                    {
                        Canvas.SetLeft(x, 550);
                    }
                }
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && gameOver == false)
            {
              
                flappyBird.RenderTransform = new RotateTransform(-20, flappyBird.Width / 2, flappyBird.Height / 2);
                gravity = -8;
                
               
                if ( !isKeyRepeating)
                {
                    player2.Open(new Uri("../../Sound/sfx_wing.wav", UriKind.RelativeOrAbsolute));
                    player2.Play();
                    isKeyRepeating = true;
                    e.Handled = true;  
                }
                
            }

            if (e.Key == Key.R && gameOver == true)
            {
                StartGame();
            }
        }

        private void KeyIsup(object sender, KeyEventArgs e)
        {
            isKeyRepeating = false;
            if (gameOver == false)
            {
                flappyBird.RenderTransform = new RotateTransform(5, flappyBird.Width / 2, flappyBird.Height / 2);
                gravity = 8;
                player1.Open(new Uri("../../Sound/sfx_swooshing.wav", UriKind.RelativeOrAbsolute));
                player1.Play();
                
            }
            
        }

     
           

        private void StartGame()
        {
            MyCanvas.Focus();

            int temp = 300;
            score = 0;
            gameOver = false;
            Canvas.SetTop(flappyBird, 190);
            backGroundMusic.SoundLocation = "../../Sound/arcade-music-loop.wav";
            

          
            backGroundMusic.PlayLooping();


            foreach (var x in MyCanvas.Children.OfType<Image>())
            {
                if ((string)x.Tag == "obs1")
                {
                    Canvas.SetLeft(x, 500);
                }
                if ((string)x.Tag == "obs2")
                {
                    Canvas.SetLeft(x, 800);
                }
                if ((string)x.Tag == "obs3")
                {
                    Canvas.SetLeft(x, 1100);
                }

                if ((string)x.Tag == "clouds")
                {
                    Canvas.SetLeft(x, 300 + temp);
                    temp = 800;
                }
            }

            gameTimer.Start();
        }

        private void EndGame()
        {
           
            backGroundMusic.Stop();
            player4.Open(new Uri("../../Sound/sfx_hit.wav", UriKind.RelativeOrAbsolute));
            player4.Play();
            Thread.Sleep(1000);
            player3.Open(new Uri("../../Sound/sfx_die.wav", UriKind.RelativeOrAbsolute));
            player3.Play();
            gameTimer.Stop();
            gameOver = true;
            txtScore.Content += " Game Over !! Press R to try again";
        }
    }
}
