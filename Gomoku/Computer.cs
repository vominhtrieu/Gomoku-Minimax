using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Gomoku
{
    class Computer
    {
        //List<List<int>> matrix;
        GameLogic game;
        int nRows, nCols;
        int maxDepth = 8;
        int maxMoveSearch = 7;

        public Computer(GameLogic game, int nRows, int nCols)
        {
            this.game = game;
            this.nRows = nRows;
            this.nCols = nCols;
        }

        public int GetScore(int r, int c, bool isMax, int alpha, int beta, int depth = 1)
        {
            if (game.CheckWin(r, c))
            {
                return (isMax ? -1 : 1) * (1000000000 - 5000*depth);
            }
            
            if (game.CheckDraw())
                return 0;
            if (depth == maxDepth)
            {
                return game.EvaluateBoard() * (100 - 2*depth) / 100;
            }
            List<int[]> moveList = game.GetPossibleMoves(maxMoveSearch, game.isXMove ? 1 : 2);
            if (isMax)
            {
                int maxValue = int.MinValue;
                for (int i = 0; i < moveList.Count; i++)
                {
                    int row = moveList[i][0];
                    int col = moveList[i][1];
                    game.NextMove(row, col);
                    maxValue = Math.Max(maxValue, GetScore(row, col, false, alpha, beta, depth + 1));
                    game.RemoveMove(row, col);
                    alpha = Math.Max(alpha, maxValue);
                    if (beta <= alpha)
                        return maxValue;
                }
                return maxValue;
            }
            else
            {
                int minValue = int.MaxValue;
                for (int i = 0; i < moveList.Count; i++)
                {
                    int row = moveList[i][0];
                    int col = moveList[i][1];
                    game.NextMove(row, col);
                    minValue = Math.Min(minValue, GetScore(row, col, true, alpha, beta, depth + 1));
                    game.RemoveMove(row, col);
                    beta = Math.Min(beta, minValue);
                    if (beta <= alpha)
                        return minValue;
                }
                return minValue;
            }
        }

        public int[] NextMove(Button[,] buttonBoard)
        {
            int[] a = new int[2];
            int max = int.MinValue;
            int alpha = int.MinValue;
            int beta = int.MaxValue;
            Random random = new Random();

            List<int[]> moveList = game.GetPossibleMoves(maxMoveSearch, game.isXMove ? 1 : 2);
            for (int i = 0; i < moveList.Count; i++)
            {
                int row = moveList[i][0];
                int col = moveList[i][1];
                game.NextMove(row, col);
                int value = GetScore(row, col, false, alpha, beta) + random.Next(-1, 1);
                buttonBoard[row, col].Content = value.ToString();
                game.RemoveMove(row, col);
                if (value > max)
                {
                    max = value;
                    a[0] = row;
                    a[1] = col;
                }
                alpha = Math.Max(alpha, max);
            }
            return a;
        }
    }
}
