﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    /*
     * Board is based on a 2-dimensional array of ints, pieces are defined as:
     * 1 - Pawn
     * 2 - Rook
     * 3 - Knight
     * 4 - Bishop
     * 5 - Queen
     * 6 - King
     */
    public class Board
    {
        public static int aiColor = 1;
        private static Board board;
        public static Board Game
        {
            get
            {
                if (board == null)
                {
                    board = new Board();
                }
                return board;
            }
        }
        public int[] EnPassant;
        public Boolean aiLeftCastling, aiRightCastling, playerLeftCastling, playerRightCastling;
        public Boolean aiCheck, playerCheck;
        public int[,] tiles;

        public Board()
        {
            tiles = new int[8, 8];
            aiLeftCastling = aiRightCastling = playerLeftCastling = playerRightCastling = true;
            aiCheck = playerCheck = false;
        }

        //Used for resetting the pieces on the board
        public void ResetGame()
        {
            for (int h = 0; h < 8; h++)
            {
                for (int w = 0; w < 8; w++)
                {
                    tiles[h, w] = GetStartPiece(h, w);
                }
            }
        }


        //Used for getting which piece will be at the a specified tile at the start of a game
        public int GetStartPiece(int h, int w)
        {
            int piece = 0;

            //Gets which piece is supposed to be at what position
            if (h == 1 || h == 6)
            {
                piece = 1;
            }
            else if (w == 0 || w == 7)
            {
                piece = 2;
            }
            else if (w == 1 || w == 6)
            {
                piece = 3;
            }
            else if (w == 2 || w == 5)
            {
                piece = 4;
            }
            else if (w == 3)
            {
                piece = 5;
            }
            else if (w == 4)
            {
                piece = 6;
            }

            //Sets the correct color of the pieces
            if (h == 0 || h == 1)
            {
                piece = piece * -aiColor;
            }
            else if (h == 7 || h == 6)
            {
                piece = piece * aiColor;
            }
            else
            {
                piece = 0;
            }
            return piece;
        }

        public int GetSpecificTile(int[] tile)
        {
            return tiles[tile[0], tile[1]];
        }

        public Board CloneBoard()
        {
            Board newBoard = new Board();
            for (int h = 0; h < 8; h++)
            {
                for (int w = 0; w < 8; w++)
                {
                    newBoard.tiles[h, w] = this.tiles[h, w];
                }
            }
            newBoard.aiCheck = this.aiCheck;
            newBoard.aiLeftCastling = this.aiLeftCastling;
            newBoard.aiRightCastling = this.aiRightCastling;
            newBoard.playerCheck = this.playerCheck;
            newBoard.playerLeftCastling = this.playerLeftCastling;
            newBoard.playerRightCastling = this.playerRightCastling;
            return newBoard;
        }


        public static void CheckForCheck(Board board, int player)
        {
            int[] king = new int[2];
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if ((board.tiles[row, col] * player) == 6)
                    {
                        king = new int[] { row, col };
                    }
                }
            }

            MoveGenerator move = new MoveGenerator();
            List<IMove> check = move.GetAllMovesForPlayer(board, player * -1);

            foreach (Move element in check)
            {
                if (element.moving.Target[0] == king[0] && element.moving.Target[1] == king[1])
                {
                    if (player * Board.aiColor > 0)
                    {
                        board.aiCheck = true;
                        board.playerCheck = false;
                    }
                    else
                    {
                        board.aiCheck = false;
                        board.playerCheck = true;
                    }
                }
            }
        }

        public static void CheckForStuff(Board board, Move move)
        {
            int piece = move.moving.Piece;
            if (piece * Board.aiColor == 2)
            {
                if (board.aiLeftCastling && move.moving.Origin[1] == 0)
                {
                    board.aiLeftCastling = false;
                }
                else if (board.aiRightCastling && move.moving.Origin[1] == 7)
                {
                    board.aiRightCastling = false;
                }
            }
            else if (piece * Board.aiColor == -2)
            {
                if (board.playerLeftCastling && move.moving.Origin[1] == 0)
                {
                    board.playerLeftCastling = false;
                }
                else if (board.playerRightCastling && move.moving.Origin[1] == 7)
                {
                    board.playerRightCastling = false;
                }
            }
            else if (Math.Abs(piece) == 1)
            {
                if (move.moving.Target[0] == 0 || move.moving.Target[0] == 7)
                {
                    move.moving.Piece = 5 * piece;
                }
                else if (piece * Board.aiColor > 0 && move.moving.Origin[0] - move.moving.Origin[1] == 2)
                {
                    board.EnPassant = move.moving.Target;
                }
                else if (piece * Board.aiColor > 0 && move.moving.Origin[0] - move.moving.Origin[1] == -2)
                {
                    board.EnPassant = move.moving.Target;
                }
            }
            else if (piece * Board.aiColor == 6)
            {
                board.aiLeftCastling = false;
                board.aiRightCastling = false;
            }
            else if (piece * Board.aiColor == -6)
            {
                board.playerLeftCastling = false;
                board.playerRightCastling = false;
            }
            else
            {
                board.EnPassant = null;
            }
        }
    }
}
