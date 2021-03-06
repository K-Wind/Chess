﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Forms;

namespace Chess
{
    /// <summary>
    /// Interaction logic for GUI.xaml
    /// </summary>
    public partial class GUI : Window
    {
        private Board game;
        private Move nextMove;
        private Logic logic;

        public GUI()
        {
            InitializeComponent();
            logic = new Logic();
            game = Board.Game;
            game.ResetGame();
            DrawBoard();
        }

        //Goes through the tiles in the board and puts the pieces in the correct places
        public void DrawBoard()
        {
            int[,] tiles = game.tiles;
            for (int h = 0; h < 8; h++)
            {
                for (int w = 0; w < 8; w++)
                {
                    int piece = tiles[h, w];
                    String pos = "c" + h + w;
                    TextBlock block = (TextBlock)this.FindName(pos);
                    if (piece != 0)
                    {
                        block.Text = GetUnicode(piece, block.Background);
                    }
                    else
                    {
                        block.Text = "";
                    }
                }
            }
        }

        //Gets the unicode string used for symbolizing any specific piece
        public String GetUnicode(int piece, Brush color)
        {
            String c = new BrushConverter().ConvertToString(color);
            String uni = null;
            int absPiece = Math.Abs(piece);
            if ((c.Equals("#FFFFFFFF") && piece > 0) || (c.Equals("#FF000000") && piece < 0))
            {
                if (absPiece == 1)
                {
                    uni = "\u2659";
                }
                else if (absPiece == 3)
                {
                    uni = "\u2658";
                }
                else if (absPiece == 4)
                {
                    uni = "\u2657";
                }
                else if (absPiece == 2)
                {
                    uni = "\u2656";
                }
                else if (absPiece == 5)
                {
                    uni = "\u2655";
                }
                else if (absPiece == 6)
                {
                    uni = "\u2654";
                }
            }
            else if ((c.Equals("#FF000000") && piece > 0) || (c.Equals("#FFFFFFFF") && piece < 0))
            {
                if (absPiece == 1)
                {
                    uni = "\u265F";
                }
                else if (absPiece == 3)
                {
                    uni = "\u265E";
                }
                else if (absPiece == 4)
                {
                    uni = "\u265D";
                }
                else if (absPiece == 2)
                {
                    uni = "\u265C";
                }
                else if (absPiece == 5)
                {
                    uni = "\u265B";
                }
                else if (absPiece == 6)
                {
                    uni = "\u265A";
                }
            }
            return uni.ToString();
        }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            TextBlock s = (TextBlock)sender;
            int y = Int32.Parse(s.Name.Substring(1, 1));
            int x = Int32.Parse(s.Name.Substring(2, 1));
            Console.WriteLine("Selected: " + y + ":" + x);
            int[] clicked = new int[] { y, x }; //game.GetSpecificTile(y, x);
            int tileClicked = game.GetSpecificTile(clicked);
            if (nextMove == null && tileClicked != 0)
            {
                Console.WriteLine("start new move");
                UIElement uie = s;
                uie.Effect = new BlurEffect
                {
                    //GlowColor = new Color {A = 255, R = 255, G = 255, B = 0},
                    //GlowSize = 320,
                };
                nextMove = new Move(clicked, tileClicked);
                
                /*
                foreach (Move m in game.GetLegalMovements(clicked))
                {
                    Console.WriteLine(m.Target[0] + "," + m.Target[1]);
                    
                    if (m.ToKill != null)
                    {
                       Console.WriteLine("kill is " + m.ToKill[0] + "," + m.ToKill[1]);
                    }
                    
                    uie = (UIElement)FindName("c" + m.Target[0] + m.Target[1]);

                    uie.Effect = new DropShadowEffect
                    {
                        Color = new Color { A = 255, R = 0, G = 0, B = 255 },
                        Direction = 320,
                        ShadowDepth = 0,
                        Opacity = 1
                    };
                    
                }*/
            }
            else if (nextMove != null && (clicked[0] == nextMove.Moving.Origin[0] && clicked[1] == nextMove.Moving.Origin[1]))
            {
                nextMove = null;
                UIElement uie = s;
                uie.Effect = null;
            }
            else if (nextMove != null)
            {
                //nextMove.Killing.Position = clicked;
                //nextMove.Killing.Piece = tileClicked;
                nextMove.Moving.Target = clicked;
                nextMove.Execute();

                UIElement uie = (UIElement)FindName("c" + nextMove.Moving.Origin[0] + nextMove.Moving.Origin[1]);
                uie.Effect = null;
                uie = s;
                uie.Effect = null;

                nextMove = null;
                DrawBoard();
                /*
                if (game.GetLegalMovements(nextMove.Org).Contains(clicked))
                {
                    Console.WriteLine("move is legal");
                    nextMove.Target = clicked;
                    //int[] org = game.MovePieceB(y - 1, x - 1);
                    //TextBlock o = (TextBlock)this.FindName("c" + org[0] + "" + org[1]);
                    nextMove.Execute();
                    UIElement uie = s;
                    uie.Effect = null;
                    DrawBoard();
                    nextMove = null;
                }
                else
                {
                    Console.WriteLine("move is illegal");
                }*/
            }
        }

        private void MenuItem_NewGame(object sender, RoutedEventArgs e)
        {
            logic.EndGame = false;
            game.ResetGame();
            DrawBoard();
        }

        private void MenuItem_Switch(object sender, RoutedEventArgs e)
        {
            logic.EndGame = false;
            Board.aiColor *= -1;
            game.ResetGame();
            DrawBoard();
        }

        private void MenuItem_ExitGame(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItem_DoMove(object sender, RoutedEventArgs e)
        {
            logic.GetBestMove();
            DrawBoard();
        }

        private void MenuItem_Evaluate(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("value of board " + Evaluation.Evaluate(Board.Game));
        }

        private void MenuItem_Depth(object sender, RoutedEventArgs e)
        {

            logic.setDepth(int.Parse(ShowDialog("Choose number of ply. \nGame default is 3", "Choose number of ply", logic.getDepth())));
        }

        public static string ShowDialog(string text, string caption, int textDefault)
    {
        Form prompt = new Form();
        prompt.Width = 290;
        prompt.Height = 150;
        prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
        prompt.Text = caption;
        prompt.StartPosition = FormStartPosition.CenterScreen;
        String deftext = textDefault.ToString();
        System.Windows.Forms.Label textLabel = new System.Windows.Forms.Label() { Left = 50, Top=20, Text=text, Width=200, Height=35};
        System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox() { Left = 50, Top = 55, Width = 180, Text = deftext };
        System.Windows.Forms.Button confirmation = new System.Windows.Forms.Button() { Text = "Ok", Left = 350, Width = 100, Top = 70 };
        confirmation.Click += (sender, e) => { prompt.Close(); };
        prompt.Controls.Add(textBox);
        prompt.Controls.Add(confirmation);
        prompt.Controls.Add(textLabel);
        prompt.AcceptButton = confirmation;
        prompt.ShowDialog();
        return textBox.Text;
    }

    }
}