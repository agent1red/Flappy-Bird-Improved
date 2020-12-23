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
        //Objects for the different SFX 
        private MediaPlayer player1 = new MediaPlayer();
        private MediaPlayer player2 = new MediaPlayer();
        private MediaPlayer player3 = new MediaPlayer();
        private MediaPlayer player4 = new MediaPlayer();
        private MediaPlayer player5 = new MediaPlayer();
        // Object for background music
        private SoundPlayer backGroundMusic = new SoundPlayer(); 
        // Prevent multiple SFX when pressing space bar 
        private bool isKeyRepeating = false;

        public MainWindow()
        {
            InitializeComponent();
            // Set game speed based on timer properties
            gameTimer.Tick += MainEventTimer;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            StartGame();
        }

        private void MainEventTimer(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score;
            // hit boundaries based on flappy bird height and width
            FlappyBirdHitBox = new Rect(Canvas.GetLeft(flappyBird), Canvas.GetTop(flappyBird), flappyBird.Width -25, flappyBird.Height -25);
            Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + gravity);

            // end game if hit 
            if (Canvas.GetTop(flappyBird) < -10 || Canvas.GetTop(flappyBird)> 458)
            {
               EndGame();
            }

            // loop through all image objects (x) in XAML form 
            foreach (var x in MyCanvas.Children.OfType<Image>())
            {
                if ((string)x.Tag == "obs1" || (string)x.Tag == "obs2" || (string)x.Tag == "obs3")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - 5);
                    if (Canvas.GetLeft(x) < -100)
                    {
                        // count score for each pipe total = 1. This is just in case 
                        // flappy bird hits a boundary before pipes pass the screen
                        Canvas.SetLeft(x, 800);
                        score += .5;
                        // Play score point sound after 5 pairs of pipes pass through.
                        // Marking a goal was reached
                        if(score % 5 == 0)
                        {
                            player5.Open(new Uri("../../Sound/sfx_point.wav", UriKind.RelativeOrAbsolute));
                            player5.Play();
                        }
                        
                    }
                    //hit boundaries based on pipes perspective height and width
                    Rect pipeHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (FlappyBirdHitBox.IntersectsWith(pipeHitBox))
                    {
                        EndGame();
                    }

                 
                }
                // move clouds slowly in background
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

        //spacebar handler 
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && gameOver == false)
            {
                // move flappy bird slightly canted up 
                flappyBird.RenderTransform = new RotateTransform(-20, flappyBird.Width / 2, flappyBird.Height / 2);
                gravity = -8;
                // handling repeated key pressing for SFX
                if ( !isKeyRepeating)
                {
                    player2.Open(new Uri("../../Sound/sfx_wing.wav", UriKind.RelativeOrAbsolute));
                    player2.Play();
                    isKeyRepeating = true;
                    e.Handled = true;  
                }
                
            }
            // handling R key press for game restart
            if (e.Key == Key.R && gameOver == true)
            {
                StartGame();
            }
        }

        // spacebar handler 
        private void KeyIsup(object sender, KeyEventArgs e)
        {
            // set repeating back to false 
            isKeyRepeating = false;
            // if game over is false move flappybird if true flappy bird does not move
            if (gameOver == false)
            {
                //move flappy bird slightly canted down
                flappyBird.RenderTransform = new RotateTransform(5, flappyBird.Width / 2, flappyBird.Height / 2);
                gravity = 8;
                player1.Open(new Uri("../../Sound/sfx_swooshing.wav", UriKind.RelativeOrAbsolute));
                player1.Play();
                
            }
            
        }

        // Start game method and set game objects and score 
        private void StartGame()
        {
            // make sure canvas is focused 
            MyCanvas.Focus();

            int temp = 300; // used for clouds seperation
            score = 0;
            gameOver = false;
            Canvas.SetTop(flappyBird, 190);
            backGroundMusic.SoundLocation = "../../Sound/arcade-music-loop.wav";
            // Using SoundPlayer because MediaPlayer lacks this helpfull method
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
