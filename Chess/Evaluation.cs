﻿using System;

namespace Chess
{
    internal class Evaluation
    {
        private static int[,] _eBoard;
        private static int _aiPieces, _playerPieces;

        public static int Evaluate(Board toEvaluate
            /*, int depth, int lastAI, int lastPlayer, out int wPieces, out int bPieces*/)
        {
            _eBoard = toEvaluate.tiles;
            _aiPieces = _playerPieces = 0;
            var aiScore = EvaluateSide(Board.aiColor);
            var playerScore = EvaluateSide(-Board.aiColor);

            /*
            if (_aiPieces < lastAI)
            {
                aiScore -= 100 * depth; 
            }
            if (_playerPieces < lastPlayer)
            {
                aiScore += 100 * depth;
            }
            wPieces = _aiPieces;
            bPieces = _playerPieces;
            */
            aiScore += _aiPieces;
            playerScore += _playerPieces;
            var total = aiScore - playerScore;
            return total;
        }

        public static int EvaluateSide(int player)
        {
            ScoreTable sTable;
            if (player == Board.aiColor)
            {
                sTable = new ScoreAI();
            }
            else
            {
                sTable = new ScorePlayer();
            }
            var pieces = 0;
            var score = 0; 
            for (var row = 0; row < _eBoard.GetLength(0); row++)
            {
                for (var col = 0; col < _eBoard.GetLength(0); col++)
                {
                    var thisPiece = _eBoard[row, col];
                    //pieces += ScoreTable.PieceValue(thisPiece);
                    switch (thisPiece*player)
                    {
                        case 1:
                            score += sTable.Pawn[row, col];
                            pieces += 100;
                            break;
                        case 2:
                            score += sTable.Rook[row, col];
                            pieces += 500;
                            break;
                        case 3:
                            score += sTable.Knight[row, col];
                            pieces += 300;
                            break;
                        case 4:
                            score += sTable.Bishop[row, col];
                            pieces += 325;
                            break;
                        case 5:
                            score += sTable.Queen[row, col];
                            pieces += 900;
                            break;
                        case 6:
                            score += sTable.King[row, col];
                            score += 10000;
                            break;
                    }
                }
            }
            if (player == Board.aiColor)
            {
                _aiPieces = pieces;
            }
            else
            {
                _playerPieces = pieces;
            }
            return score;
        }

        public static int EvaluateBonus(IMove move, int depth)
        {
            if (!(move is Move)) return 0;
            var m = (Move) move;
            if (m.Killing.Piece != 0 && m.Killing.Position != null)
            {
                /*
                    Console.WriteLine("at depth " + depth + " applied bonus/penalty");
                    Console.WriteLine("for " + m.Moving.Piece + " taking " + m.Killing.Piece + "\n");
                     * */
                return ScoreTable.PieceValue(m.Killing.Piece)/10*depth;
            }
            return 0;
        }
    }

    internal abstract class ScoreTable
    {
        public abstract int[,] Pawn { get; }
        public abstract int[,] Rook { get; }
        public abstract int[,] Knight { get; }
        public abstract int[,] Bishop { get; }
        public abstract int[,] Queen { get; }
        public abstract int[,] King { get; }

        public static int PieceValue(int piece)
        {
            var p = Math.Abs(piece);
            if (p == 1)
            {
                return 100;
            }
            if (p == 2)
            {
                return 500;
            }
            if (p == 3)
            {
                return 300;
            }
            if (p == 4)
            {
                return 325;
            }
            if (p == 5)
            {
                return 900;
            }
            if (p == 6)
            {
                return 10000;
            }
            return 0;
        }
    }

    internal class ScoreAI : ScoreTable
    {
        private readonly int[,] _pawn =
        {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {7, 7, 13, 23, 26, 13, 7, 7},
            {-2, -2, 4, 12, 15, 4, -2, -2},
            {2, -3, 2, 9, 11, 2, -3, -3},
            {1, 2, 0, 6, 9, 0, -4, -4},
            {-4, -4, 2, 4, 7, 0, -4, -4},
            {-1, -1, 1, 5, 6, 1, -1, -1},
            {0, 0, 0, 0, 0, 0, 0, 0}
        };

        private readonly int[,] _rook =
        {
            {9, 9, 11, 10, 11, 9, 9, 9},
            {4, 6, 7, 9, 9, 7, 6, 4},
            {9, 10, 10, 11, 11, 10, 10, 9},
            {8, 8, 8, 9, 9, 8, 8, 8},
            {6, 6, 5, 6, 6, 5, 6, 6},
            {4, 5, 5, 5, 5, 5, 5, 4},
            {0, 3, 3, 5, 5, 3, 3, 0},
            {0, -2, 0, 5, 5, 3, 0, 0}
        };

        private readonly int[,] _knight =
        {
            {-2, 2, 7, 9, 9, 7, 2, -2},
            {1, 4, 12, 13, 13, 12, 4, 1},
            {5, 11, 18, 19, 19, 18, 11, 5},
            {3, 10, 14, 14, 14, 14, 10, 3},
            {0, 5, 8, 9, 9, 8, 5, 0},
            {-3, 1, 3, 4, 4, 3, 1, -3},
            {-5, -3, -1, 0, 0, -1, -3, -5},
            {-7, -5, -4, -2, -2, -4, -5, -7}
        };

        private readonly int[,] _bishop =
        {
            {2, 3, 4, 4, 4, 4, 3, 2},
            {4, 7, 7, 7, 7, 7, 7, 4},
            {3, 5, 6, 6, 6, 6, 5, 3},
            {3, 5, 7, 7, 7, 7, 5, 3},
            {4, 5, 6, 8, 8, 6, 5, 4},
            {4, 5, 5, -2, -2, 5, 5, 4},
            {5, 5, 5, 3, 3, 5, 5, 5},
            {0, 0, 0, 0, 0, 0, 0, 0}
        };

        private readonly int[,] _queen =
        {
            {2, 3, 4, 3, 4, 3, 3, 2},
            {2, 3, 4, 4, 4, 4, 3, 2},
            {3, 4, 4, 4, 4, 4, 4, 3},
            {3, 3, 4, 4, 4, 4, 3, 3},
            {2, 3, 3, 4, 4, 3, 3, 2},
            {2, 2, 2, 3, 3, 2, 2, 2},
            {2, 2, 2, 2, 2, 2, 2, 2},
            {0, 0, 0, 0, 0, 0, 0, 0}
        };

        private readonly int[,] _king =
        {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 15, 0}
        };

        public override int[,] Pawn
        {
            get { return _pawn; }
        }

        public override int[,] Rook
        {
            get { return _rook; }
        }

        public override int[,] Knight
        {
            get { return _knight; }
        }

        public override int[,] Bishop
        {
            get { return _bishop; }
        }

        public override int[,] Queen
        {
            get { return _queen; }
        }

        public override int[,] King
        {
            get { return _king; }
        }
    }

    internal class ScorePlayer : ScoreTable
    {
        private readonly int[,] _pawn =
        {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {-1, -1, 1, 5, 6, 1, -1, -1},
            {-4, -4, 0, 4, 6, 0, -4, -4},
            {-4, -4, 0, 6, 8, 0, -4, -4},
            {-3, -3, 2, 9, 11, 2, -3, -3},
            {-2, -2, 4, 12, 15, 4, -2, -2},
            {7, 7, 13, 23, 26, 13, 7, 7},
            {0, 0, 0, 0, 0, 0, 0, 0}
        };

        private readonly int[,] _rook =
        {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {3, 4, 4, 6, 6, 4, 4, 3},
            {4, 5, 5, 5, 5, 5, 5, 4},
            {6, 6, 5, 6, 6, 5, 6, 6},
            {8, 8, 8, 9, 9, 8, 8, 8},
            {9, 10, 10, 11, 11, 10, 10, 9},
            {4, 6, 7, 9, 9, 7, 6, 4},
            {9, 9, 11, 10, 11, 9, 9, 9}
        };
        
        private readonly int[,] _knight =
        {
            {-7, -5, -4, -2, -2, -4, -5, -7},
            {-5, -3, -1, 0, 0, -1, -3, -5},
            {-3, 1, 3, 4, 4, 3, 1, -3},
            {0, 5, 8, 9, 9, 8, 5, 0},
            {3, 10, 14, 14, 14, 14, 10, 3},
            {5, 11, 18, 19, 19, 18, 11, 5},
            {1, 4, 12, 13, 13, 12, 4, 1},
            {-2, 2, 7, 9, 9, 7, 2, -2}
        };

        private readonly int[,] _bishop =
        {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {5, 5, 5, 3, 3, 5, 5, 5},
            {4, 5, 5, -2, -2, 5, 5, 4},
            {4, 5, 6, 8, 8, 6, 5, 4},
            {3, 5, 7, 7, 7, 7, 5, 3},
            {3, 5, 6, 6, 6, 6, 5, 3},
            {4, 7, 7, 7, 7, 7, 7, 4},
            {2, 3, 4, 4, 4, 4, 3, 2}
        };

        private readonly int[,] _queen =
        {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {2, 2, 2, 2, 2, 2, 2, 2},
            {2, 2, 2, 3, 3, 2, 2, 2},
            {2, 3, 3, 4, 4, 3, 3, 2},
            {3, 3, 4, 4, 4, 4, 3, 3},
            {3, 4, 4, 4, 4, 4, 4, 3},
            {2, 3, 4, 4, 4, 4, 3, 2},
            {2, 3, 4, 3, 4, 3, 3, 2}
        };

        private readonly int[,] _king =
        {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0}
        };

        

        public override int[,] Pawn
        {
            get { return _pawn; }
        }

        public override int[,] Rook
        {
            get { return _rook; }
        }

        public override int[,] Knight
        {
            get { return _knight; }
        }

        public override int[,] Bishop
        {
            get { return _bishop; }
        }

        public override int[,] Queen
        {
            get { return _queen; }
        }

        public override int[,] King
        {
            get { return _king; }
        }
    }
}