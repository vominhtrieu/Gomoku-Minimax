using System;
using System.Windows.Controls;

namespace Gomoku
{
    class Computer
    {
        //List<List<int>> matrix;
        GameLogic game;
        int nRows, nCols;
        int maxDepth = 1;

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
                return (isMax ? -1 : 1) * 100000000 - depth;
            }
            
            if (game.CheckDraw())
                return 0;
            if (depth == maxDepth)
            {
                int score = game.EvaluateAttack(r, c) + game.EvaluateDefense(r, c);
                if (score < 0)
                    score = 0;
                return (isMax ? -1 : 1) * score - depth;
            }
            if (isMax)
            {
                int maxValue = int.MinValue;
                for (int i = 0; i < nRows; i++)
                {
                    for (int j = 0; j < nCols; j++)
                    {
                        if (game.IsEmpty(i, j))
                        {
                            game.NextMove(i, j);
                            maxValue = Math.Max(maxValue, GetScore(i, j, false, alpha, beta, depth + 1));
                            game.RemoveMove(i, j);
                            alpha = Math.Max(alpha, maxValue);
                            if (beta <= alpha)
                                return maxValue;
                        }
                    }
                }
                return maxValue;
            }
            else
            {
                int minValue = int.MaxValue;
                for (int i = 0; i < nRows; i++)
                {
                    for (int j = 0; j < nCols; j++)
                    {
                        if (game.IsEmpty(i, j))
                        {
                            game.NextMove(i, j);
                            minValue = Math.Min(minValue, GetScore(i, j, true, alpha, beta, depth + 1));
                            game.RemoveMove(i, j);
                            beta = Math.Min(beta, minValue);
                            if (beta <= alpha)
                                return minValue;
                        }
                    }
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
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    if(game.IsEmpty(i,j))
                    {
                        game.NextMove(i, j);
                        int value = GetScore(i, j, false, alpha, beta) + random.Next(-2, 3);
                        buttonBoard[i, j].Content = value.ToString();
                        game.RemoveMove(i, j);
                        if(value>max)
                        {
                            max = value;
                            a[0] = i;
                            a[1] = j;
                        }
                        alpha = Math.Max(alpha, max);
                    }
                    else
                    {
                        buttonBoard[i, j].Content = "";
                    }
                }
            }
            return a;
        }
    }
}
