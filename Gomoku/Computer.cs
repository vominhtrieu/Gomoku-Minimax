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

        public int GetScore(int r, int c, bool isMax, int depth = 1)
        {
            int attackScore = game.EvaluateAttack(r, c);
            if (attackScore < 0)
                attackScore = 0;
            int defenseScore = game.EvaluateDefense(r, c);
            if (defenseScore < 0)
                defenseScore = 0;
            int score = attackScore + defenseScore;
            if (game.CheckDraw() || score == 0)
                return 0;
            if (depth == maxDepth)
                return (isMax?-1:1)*score / depth;
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
                            maxValue = Math.Max(maxValue, GetScore(i, j, false, depth + 1));
                            game.RemoveMove(i, j);
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
                            minValue = Math.Min(minValue, GetScore(i, j, true, depth + 1));
                            game.RemoveMove(i, j);
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
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    if(game.IsEmpty(i,j))
                    {
                        game.NextMove(i, j);
                        int value = GetScore(i, j, false);
                        buttonBoard[i, j].Content = value.ToString();
                        game.RemoveMove(i, j);
                        if(value>max)
                        {
                            max = value;
                            a[0] = i;
                            a[1] = j;
                        }
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
