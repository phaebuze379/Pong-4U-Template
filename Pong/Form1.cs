/*
 * Description: A basic PONG simulator
 * Author: Phaedra Buzek          
 * Date: Sept. 21, 2020           
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //random colours for ball
        Random colour = new Random();
        int red = 255;
        int green = 255;
        int blue = 255;

        //graphics objects for drawing
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush pinkBrush = new SolidBrush(Color.Pink);
        SolidBrush blueBrush = new SolidBrush(Color.LightSkyBlue);

        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.fanfare_x);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.boing2);
        SoundPlayer paddleSound = new SoundPlayer(Properties.Resources.collision);
        SoundPlayer clapSound = new SoundPlayer(Properties.Resources.applause3);


        //determines whether a key is being pressed or not
        Boolean wKeyDown, sKeyDown, uKeyDown, jKeyDown, aKeyDown, dKeyDown, hKeyDown, kKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions, speed, and rectangle
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        int BALL_SPEED = 3;
        Rectangle ball;

        //paddle speeds and rectangles
        const int PADDLE_SPEED = 8;
        Rectangle p1, p2;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 3;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = true;
                    break;
                case Keys.S:
                    sKeyDown = true;
                    break;
                case Keys.U:
                    uKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.D:
                    dKeyDown = true;
                    break;
                case Keys.H:
                    hKeyDown = true;
                    break;
                case Keys.K:
                    kKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = false;
                    break;
                case Keys.S:
                    sKeyDown = false;
                    break;
                case Keys.U:
                    uKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.D:
                    dKeyDown = false;
                    break;
                case Keys.H:
                    hKeyDown = false;
                    break;
                case Keys.K:
                    kKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }

            //set starting position for paddles on new game and point scored 
            const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            

            p1.Width = p2.Width = 10;    //height for both paddles set the same
            p1.Height = p2.Height = 40;  //width for both paddles set the same

            //p1 starting position
            p1.X = PADDLE_EDGE;
            p1.Y = this.Height / 2 - p1.Height / 2;

            //p2 starting position
            p2.X = this.Width - PADDLE_EDGE - p2.Width;
            p2.Y = this.Height / 2 - p2.Height / 2;

            //set Width and Height of ball
            ball.Width = 15;
            ball.Height = 15;
            //set starting X position for ball to middle of screen
            ball.X = this.Width / 2 - ball.Width / 2;
            //set starting Y position for ball to middle of screen
            ball.Y = this.Height / 2 - ball.Height / 2;

        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position

            // move ball either left or right based on ballMoveRight and using BALL_SPEED
            if (ballMoveRight == true)
            {
                ball.X = ball.X + BALL_SPEED;
            }
            else if (ballMoveRight == false)
            {
                ball.X = ball.X - BALL_SPEED;
            }
            // move ball either down or up based on ballMoveDown and using BALL_SPEED
            if (ballMoveDown == true)
            {
                ball.Y = ball.Y + BALL_SPEED;
            }
            else if (ballMoveDown == false)
            {
                ball.Y = ball.Y - BALL_SPEED;
            }
            #endregion

            #region update paddle positions

            //p1 paddle move up down left or right
            if (wKeyDown == true && p1.Y >= 0)
            {
                p1.Y = p1.Y - PADDLE_SPEED;
            }
            if (sKeyDown == true && p1.Y <= this.Height - p1.Height)
            {
                p1.Y = p1.Y + PADDLE_SPEED;
            }
            if (aKeyDown == true && p1.X >= 0)
            {
                p1.X = p1.X - PADDLE_SPEED;
            }
            if (dKeyDown == true && p1.X <= this.Width - p1.Width)
            {
                p1.X = p1.X + PADDLE_SPEED;
            }

            //p2 paddle move up down left or right
            if (uKeyDown == true && p2.Y >= 0)
            {
                p2.Y = p2.Y - PADDLE_SPEED;
            }
            if (jKeyDown == true && p2.Y <= this.Height - p2.Height)
            {
                p2.Y = p2.Y + PADDLE_SPEED;
            }
            if (hKeyDown == true && p2.X >= 0)
            {
                p2.X = p2.X - PADDLE_SPEED;
            }
            if (kKeyDown == true && p2.X <= this.Width - p2.Width)
            {
                p2.X = p2.X + PADDLE_SPEED;
            }

            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y <= 0) // if ball hits top line
            {
                collisionSound.Play();
                ballMoveDown = true;
            }

            else if (ball.Y >= this.Height - ball.Height) //if ball hits bottom line
            {
                collisionSound.Play();
                ballMoveDown = false;
            }


            #endregion

            #region ball collision with paddles

            if (ball.IntersectsWith(p1)) //if ball hits p1 paddle
            {
                paddleSound.Play(); //play sound
                ballMoveRight = true; //change ball direction
                BALL_SPEED++; //up speed
                //random colour
                red = colour.Next(0, 256);
                green = colour.Next(0, 256);
                blue = colour.Next(0, 256);
            }

            if (ball.IntersectsWith(p2)) //if ball hits p2 paddle
            {
                paddleSound.Play(); //play sound
                ballMoveRight = false; //change ball direction
                BALL_SPEED++; //up speed
                //random colour
                red = colour.Next(0, 256);
                green = colour.Next(0, 256);
                blue = colour.Next(0, 256);
            }

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall logic
            {
                // --- play score sound
                scoreSound.Play();
                player2Score++; //add pouint to score
                BALL_SPEED = 3; //reset speed
                //ball to wite
                red = 255;
                green = 255;
                blue = 255;
                SetParameters();
                if (player2Score == gameWinScore) //if winning score
                {
                    clapSound.Play(); //play sound
                    GameOver("player 2");
                }
            }

            if (ball.X >= this.Width - ball.Width) //if ball hits rught wall
            {
                scoreSound.Play(); //play score sound
                player1Score++; //add point
                BALL_SPEED = 3; //reset speed
                //ball to white 
                red = 255;
                green = 255;
                blue = 255;
                SetParameters();
                if (player1Score == gameWinScore) //if winning score
                {
                    clapSound.Play(); //play sound
                    GameOver("player 1");
                }
            }


            #endregion


            this.Refresh();
        }

        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
            newGameOk = true;
            gameUpdateLoop.Stop(); //stop the gameUpdateLoop
            startLabel.Visible = true;
            startLabel.Text = winner + " is the winner!"; //display the winner
            startLabel.Refresh();
            Thread.Sleep(2000);
            //ask the user if they want to play again
            startLabel.Text = "play again? press space" + "\n" + "Press N to Exit";

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            // draw paddles using FillRectangle
            e.Graphics.FillRectangle(pinkBrush, p1);
            e.Graphics.FillRectangle(blueBrush, p2);

            //draw ball in random colour
            Brush randBrush = new SolidBrush(Color.FromArgb(red, green, blue));
            e.Graphics.FillEllipse(randBrush, ball);


            //draw scores to the screen using DrawString
            e.Graphics.DrawString("player 1: " + player1Score, drawFont, pinkBrush, 20, 20);
            e.Graphics.DrawString("player 2: " + player2Score, drawFont, blueBrush, this.Width - 125, 20);
        }

    }
}
