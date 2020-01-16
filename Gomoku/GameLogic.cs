using System;
using System.Collections.Generic;

namespace Gomoku
{
    class GameLogic
    {
        public class Move
        {
            public int row, column;
            public Move(int r, int c)
            {
                row = r;
                column = c;
            }
        }
        int[,] matrix;
        int comChess;
        int nRows, nCols, turn;
        
        public bool isXMove { get; private set; }

        public GameLogic(int nRows, int nCols, int comChess)
        {
            matrix = new int[nRows, nCols];
            isXMove = true;
            this.nRows = nRows;
            this.nCols = nCols;
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    matrix[i, j] = 0;
                }
            }
            turn = 0;
            this.comChess = comChess;
        }

        public bool IsEmpty(int r, int c)
        {
            return matrix[r, c] == 0;
        }

        public void NextMove(int r, int c)
        {
            matrix[r, c] = isXMove ? 1 : 2;

            isXMove = !isXMove;
            turn++;
        }

        public void RemoveMove(int r, int c)
        {
            matrix[r, c] = 0;
            isXMove = !isXMove;
            turn--;
        }

        #region Winning Check
        bool CheckHorizontalWin(int r, int c)
        {
            int cur = c - 1;
            int count = 0;
            int blockedEnd = 0;
            while (cur >= 0 && matrix[r, cur] == matrix[r, c])
            {
                count++;
                cur--;
            }
            if (cur >= 0 && matrix[r, cur] != 0)
                blockedEnd++;
            cur = c + 1;
            while (cur < nCols && matrix[r, cur] == matrix[r, c])
            {
                count++;
                cur++;
            }
            if (cur < nCols && matrix[r, cur] != 0)
                blockedEnd++;

            return count == 4 && blockedEnd < 2;
        }

        bool CheckVerticalWin(int r, int c)
        {
            int cur = r - 1;
            int count = 0;
            int blockedEnd = 0;
            while (cur >= 0 && matrix[cur, c] == matrix[r, c])
            {
                count++;
                cur--;
            }

            if (cur >= 0 && matrix[cur, c] != 0)
                blockedEnd++;
            cur = r + 1;
            while (cur < nRows && matrix[cur, c] == matrix[r, c])
            {
                count++;
                cur++;
            }
            if (cur < nRows && matrix[cur, c] != 0)
                blockedEnd++;
            return count == 4 && blockedEnd < 2;
        }

        bool CheckMainDiagonal(int r, int c)
        {
            int i = r - 1, j = c - 1;
            int count = 0;
            int blockedEnd = 0;
            while (i >= 0 && j >= 0 && matrix[i, j] == matrix[r, c])
            {
                count++;
                i--;
                j--;
            }
            if (i >= 0 && j >= 0 && matrix[i, j] != 0)
                blockedEnd++;
            i = r + 1;
            j = c + 1;
            while (i < nRows && j < nCols && matrix[i, j] == matrix[r, c])
            {
                count++;
                i++;
                j++;
            }
            if (i < nRows && j < nCols && matrix[i, j] != 0)
                blockedEnd++;

            return count == 4 && blockedEnd < 2;
        }

        bool CheckSubDiagonal(int r, int c)
        {
            int i = r + 1, j = c - 1;
            int count = 0;
            int blockedEnd = 0;
            while (i < nRows && j >= 0 && matrix[i, j] == matrix[r, c])
            {
                count++;
                i++;
                j--;
            }
            if (i < nRows && j >= 0 && matrix[i, j] != 0)
                blockedEnd++;
            i = r - 1;
            j = c + 1;
            while (i >= 0 && j < nCols && matrix[i, j] == matrix[r, c])
            {
                count++;
                i--;
                j++;
            }
            if (i >= 0 && j < nCols && matrix[i, j] != 0)
                blockedEnd++;
            return count == 4 && blockedEnd < 2;
        }

        public bool CheckWin(int r, int c)
        {
            return CheckHorizontalWin(r, c) || CheckVerticalWin(r, c) || CheckMainDiagonal(r, c) || CheckSubDiagonal(r, c);
        }

        #endregion


        #region Evaluate
        static readonly int[] AttackScore = { 0, 10, 150, 2250, 33750, 506250, 10000000};
        static readonly int[] DefenseScore = { 0, 5, 75, 1125, 16875, 253125, 5000000};

        #region Evaluate Attack

        int GetAttackScore(int chessCount, int blockedEnd, int bonus1, int bonus2)
        {
            int bonus;
            if(bonus1 > bonus2)
            {
                if (chessCount + bonus1 <= 5)
                    bonus = bonus1;
                else
                    bonus = bonus2;
            }
            else
            {
                if (chessCount + bonus2 <= 5)
                    bonus = bonus2;
                else
                    bonus = bonus1;
            }
            chessCount += bonus;
            if (blockedEnd == 2 || chessCount > 5)
                return 0;
            return AttackScore[chessCount] - DefenseScore[blockedEnd * chessCount] - 5 * bonus * bonus;
        }

        int EvaluateAttackHorizontal(int r, int c)
        {
            int cur = c - 1;
            int chessCount = 1;
            int blockedEnd = 0;
            int bonus1 = 0, bonus2 = 0;
            while (cur >= 0 && matrix[r, cur] == matrix[r, c])
            {
                chessCount++;
                cur--;
            }

            if (cur >= 0 && matrix[r, cur] != 0)
                blockedEnd++;
            else if(cur > 0 && matrix[r, cur] == 0)
            {
                while (cur >= 0 && matrix[r, cur] == matrix[r, c])
                {
                    bonus1++;
                    cur--;
                }
            }
            cur = c + 1;
            while (cur < nCols && matrix[r, cur] == matrix[r, c])
            {
                chessCount++;
                cur++;
            }

            if (cur < nCols && matrix[r, cur] != 0)
                blockedEnd++;
            else if (cur < nCols - 1 && matrix[r, cur] == 0)
            {
                while (cur < nCols && matrix[r, cur] == matrix[r, c])
                {
                    bonus2++;
                    cur++;
                }
            }
            return GetAttackScore(chessCount, blockedEnd, bonus1, bonus2);
        }

        int EvaluateAttackVertical(int r, int c)
        {
            int cur = r - 1;
            int chessCount = 1;
            int blockedEnd = 0;
            int bonus1 = 0, bonus2 = 0;
            while (cur >= 0 && matrix[cur, c] == matrix[r, c])
            {
                chessCount++;
                cur--;
            }
            if (cur >= 0 && matrix[cur, c] != 0)
                blockedEnd++;
            else if (cur > 0 && matrix[cur, c] == 0)
            {
                while (cur >= 0 && matrix[cur, c] == matrix[r, c])
                {
                    bonus1++;
                    cur--;
                }
            }
            cur = r + 1;
            while (cur < nRows && matrix[cur, c] == matrix[r, c])
            {
                chessCount++;
                cur++;
            }
            if (cur < nRows && matrix[cur, c] != 0)
                blockedEnd++;
            else if (cur < nRows - 1 && matrix[cur, c] == 0)
            {
                while (cur < nRows && matrix[cur, c] == matrix[r, c])
                {
                    bonus2++;
                    cur++;
                }
            }
            return GetAttackScore(chessCount, blockedEnd, bonus1, bonus2);
        }

        int EvaluateAttackMainDiagonal(int r, int c)
        {
            int i = r - 1, j = c - 1;
            int chessCount = 1;
            int blockedEnd = 0;
            int bonus1 = 0, bonus2 = 0;
            while (i >= 0 && j >= 0 && matrix[i, j] == matrix[r, c])
            {
                chessCount++;
                i--;
                j--;
            }
            if (i >= 0 && j >= 0 && matrix[i, j] != 0)
                blockedEnd++;
            else if(i > 0 && j > 0 && matrix[i, j] == 0)
            {
                while (i >= 0 && j >= 0 && matrix[i, j] == matrix[r, c])
                {
                    bonus1++;
                    i--;
                    j--;
                }
            }
            i = r + 1;
            j = c + 1;
            while (i < nRows && j < nCols && matrix[i, j] == matrix[r, c])
            {
                chessCount++;
                i++;
                j++;
            }
            if (i < nRows && j < nCols && matrix[i, j] != 0)
                blockedEnd++;
            else if(i < nRows - 1 && j < nCols - 1 && matrix[i, j] == 0)
            {
                while (i < nRows && j < nCols && matrix[i, j] == matrix[r, c])
                {
                    bonus2++;
                    i++;
                    j++;
                }
            }
            return GetAttackScore(chessCount, blockedEnd, bonus1, bonus2);
        }

        int EvaluateAttackSubDiagonal(int r, int c)
        {
            int i = r + 1, j = c - 1;
            int chessCount = 1;
            int blockedEnd = 0;
            int bonus1 = 0, bonus2 = 0;
            while (i < nRows && j >= 0 && matrix[i, j] == matrix[r, c])
            {
                chessCount++;
                i++;
                j--;
            }
            if (i < nRows && j >= 0 && matrix[i, j] != 0)
                blockedEnd++;
            else if (i< nRows-1&&j >0&&matrix[i,j] ==0)
            {
                while (i < nRows && j >= 0 && matrix[i, j] == matrix[r, c])
                {
                    bonus1++;
                    i++;
                    j--;
                }
            }
            i = r - 1;
            j = c + 1;
            while (i >= 0 && j < nCols && matrix[i, j] == matrix[r, c])
            {
                chessCount++;
                i--;
                j++;
            }
            if (i >= 0 && j < nCols && matrix[i, j] != 0)
                blockedEnd++;
            else if(i >0 && j < nCols - 1&& matrix[i,j]==0)
            {
                while (i >= 0 && j < nCols && matrix[i, j] == matrix[r, c])
                {
                    bonus2++;
                    i--;
                    j++;
                }
            }
            return GetAttackScore(chessCount, blockedEnd, bonus1, bonus2);
        }

        public int EvaluateAttack(int r, int c)
        {
            return EvaluateAttackHorizontal(r, c) + EvaluateAttackVertical(r, c) + EvaluateAttackMainDiagonal(r, c) + EvaluateAttackSubDiagonal(r, c);
        }
        #endregion

        #region Evaluate Defense
        int GetDefenseScore(int opponentChessCount, int allyChessCount, int bonus1, int bonus2)
        {
            int bonus;
            if (bonus1 > bonus2)
            {
                if (opponentChessCount + bonus1 <= 5)
                    bonus = bonus1;
                else
                    bonus = bonus2;
            }
            else
            {
                if (opponentChessCount + bonus2 <= 5)
                    bonus = bonus2;
                else
                    bonus = bonus1;
            }
            opponentChessCount += bonus;
            if (allyChessCount == 3 || opponentChessCount >= 5)
                return 0;
            return DefenseScore[opponentChessCount + 1] - AttackScore[allyChessCount];
        }
        int EvaluateDefenseHorizontal(int r, int c)
        {
            int cur = c - 1;
            int opponentChessCount = 0;
            int allyChessCount = 1;
            int opponentChess = matrix[r, c] == 1 ? 2 : 1;
            int bonus1 = 0, bonus2 = 0;
            while (cur >= 0 && matrix[r, cur] == opponentChess)
            {
                opponentChessCount++;
                cur--;
            }

            if (cur >= 0 && matrix[r, cur] != 0)
                allyChessCount++;
            else if (cur > 0 && matrix[r, cur] == 0)
            {
                while (cur >= 0 && matrix[r, cur] == opponentChess)
                {
                    bonus1++;
                    cur--;
                }
            }
            cur = c + 1;
            while (cur < nCols && matrix[r, cur] == opponentChess)
            {
                opponentChessCount++;
                cur++;
            }

            if (cur < nCols && matrix[r, cur] != 0)
                allyChessCount++;
            else if (cur < nCols - 1 && matrix[r, cur] == 0)
            {
                while (cur < nCols && matrix[r, cur] == opponentChess)
                {
                    bonus2++;
                    cur++;
                }
            }

            return GetDefenseScore(opponentChessCount, allyChessCount, bonus1, bonus1);
        }

        int EvaluateDefenseVertical(int r, int c)
        {
            int cur = r - 1;
            int opponentChessCount = 0;
            int allyChessCount = 1;
            int opponentChess = matrix[r, c] == 1 ? 2 : 1;
            int bonus1 = 0, bonus2 = 0;
            while (cur >= 0 && matrix[cur, c] == opponentChess)
            {
                opponentChessCount++;
                cur--;
            }
            if (cur >= 0 && matrix[cur, c] != 0)
                allyChessCount++;
            else if (cur > 0 && matrix[cur, c] == 0)
            {
                while (cur >= 0 && matrix[cur, c] == opponentChess)
                {
                    bonus1++;
                    cur--;
                }
            }
            cur = r + 1;
            while (cur < nRows && matrix[cur, c] == opponentChess)
            {
                opponentChessCount++;
                cur++;
            }
            if (cur < nRows && matrix[cur, c] != 0)
                allyChessCount++;
            else if (cur < nRows - 1 && matrix[cur, c] == 0)
            {
                while (cur < nRows && matrix[cur, c] == opponentChess)
                {
                    bonus2++;
                    cur++;
                }
            }

            return GetDefenseScore(opponentChessCount, allyChessCount, bonus1, bonus1);
        }

        int EvaluateDefenseMainDiagonal(int r, int c)
        {
            int i = r - 1, j = c - 1;
            int opponentChessCount = 0;
            int allyChessCount = 1;
            int opponentChess = matrix[r, c] == 1 ? 2 : 1;
            int bonus1 = 0, bonus2 = 0;
            while (i >= 0 && j >= 0 && matrix[i, j] == opponentChess)
            {
                opponentChessCount++;
                i--;
                j--;
            }
            if (i >= 0 && j >= 0 && matrix[i, j] != 0)
                allyChessCount++;
            else if (i > 0 && j > 0 && matrix[i, j] == 0)
            {
                while (i >= 0 && j >= 0 && matrix[i, j] == opponentChess)
                {
                    bonus1++;
                    i--;
                    j--;
                }
            }
            i = r + 1;
            j = c + 1;
            while (i < nRows && j < nCols && matrix[i, j] == opponentChess)
            {
                opponentChessCount++;
                i++;
                j++;
            }
            if (i < nRows && j < nCols && matrix[i, j] != 0)
                allyChessCount++;
            else if (i < nRows - 1 && j < nCols - 1 && matrix[i, j] == 0)
            {
                while (i < nRows && j < nCols && matrix[i, j] == opponentChess)
                {
                    bonus2++;
                    i++;
                    j++;
                }
            }
            return GetDefenseScore(opponentChessCount, allyChessCount, bonus1, bonus1);
        }

        int EvaluateDefenseSubDiagonal(int r, int c)
        {
            int i = r + 1, j = c - 1;
            int opponentChessCount = 0;
            int allyChessCount = 1;
            int opponentChess = matrix[r, c] == 1 ? 2 : 1;
            int bonus1 = 0, bonus2 = 0;
            while (i < nRows && j >= 0 && matrix[i, j] == opponentChess)
            {
                opponentChessCount++;
                i++;
                j--;
            }
            if (i < nRows && j >= 0 && matrix[i, j] != 0)
                allyChessCount++;
            else if (i < nRows - 1 && j > 0 && matrix[i, j] == 0)
            {
                while (i < nRows && j >= 0 && matrix[i, j] == opponentChess)
                {
                    bonus1++;
                    i++;
                    j--;
                }
            }
            i = r - 1;
            j = c + 1;
            while (i >= 0 && j < nCols && matrix[i, j] == opponentChess)
            {
                opponentChessCount++;
                i--;
                j++;
            }
            if (i >= 0 && j < nCols && matrix[i, j] != 0)
                allyChessCount++;
            else if (i > 0 && j < nCols - 1 && matrix[i, j] == 0)
            {
                while (i >= 0 && j < nCols && matrix[i, j] == opponentChess)
                {
                    bonus2++;
                    i--;
                    j++;
                }
            }
            return GetDefenseScore(opponentChessCount, allyChessCount, bonus1, bonus1);
        }

        public int EvaluateDefense(int r, int c)
        {
            return EvaluateDefenseHorizontal(r, c) + EvaluateDefenseVertical(r, c) + EvaluateDefenseMainDiagonal(r, c) + EvaluateDefenseSubDiagonal(r, c);
        }
        #endregion

        public int EvaluateBoard()
        {
            int maxComScore = int.MinValue;
            for(int i = 0; i < nRows; i++)
            {
                for(int j = 0; j < nCols; j++)
                {
                    if(matrix[i, j] == 0)
                    {
                        matrix[i, j] = comChess;
                        maxComScore = Math.Max(maxComScore, EvaluateAttack(i, j));
                        matrix[i, j] = 0;
                    }
                }
            }

            int playerChess = comChess == 1 ? 2 : 1;
            int maxPlayerScore = int.MinValue;
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        matrix[i, j] = playerChess;
                        maxPlayerScore = Math.Max(maxPlayerScore, EvaluateAttack(i, j));
                        matrix[i, j] = 0;
                    }
                }
            }

            return maxComScore - maxPlayerScore;
        }
        #endregion

        public List<Move> GetPossibleMoves(int number, int chess)
        {
            int[,] scoreMatrix = new int[nRows, nCols];
            for(int i = 0; i < nRows; i ++)
            {
                for(int j = 0; j < nCols; j++)
                {
                    if (matrix[i, j] != 0)
                        scoreMatrix[i, j] = 0;
                    else
                    {
                        matrix[i, j] = chess;
                        scoreMatrix[i, j] = EvaluateAttack(i, j) + EvaluateDefense(i, j);
                        matrix[i, j] = 0;
                    }
                }
            }
            if (turn < 5)
                number = 7;
            List<Move> result = new List<Move>(nRows * nCols);
            
            for(int i = 0; i < number; i++)
            {
                int max = int.MinValue;
                int r = 0, c = 0;
                for(int j = 0; j < nRows; j++)
                {
                    for(int k = 0; k < nCols; k++)
                    {
                        if(max < scoreMatrix[j, k])
                        {
                            r = j;
                            c = k;
                            max = scoreMatrix[j, k];
                        }
                    }
                }
                result.Add(new Move(r, c));
                scoreMatrix[r, c] = 0;
            }
            return result;
        }

        public bool CheckDraw()
        {
            return turn == (nRows * nCols);
        }
    }
}