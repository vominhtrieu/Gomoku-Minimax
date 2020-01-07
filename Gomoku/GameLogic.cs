using System.Collections.Generic;

namespace Gomoku
{
    class GameLogic
    {
        int[,] matrix;
        int nRows, nCols, filledCount;
        
        public bool isXMove { get; private set; }

        public GameLogic(int nRows, int nCols)
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
            filledCount = 0;
        }

        public bool IsEmpty(int r, int c)
        {
            return matrix[r, c] == 0;
        }

        public void NextMove(int r, int c)
        {
            matrix[r, c] = isXMove ? 1 : 2;

            isXMove = !isXMove;
            filledCount++;
        }

        public void RemoveMove(int r, int c)
        {
            matrix[r, c] = 0;
            isXMove = !isXMove;
            filledCount--;
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
        static readonly int[] AttackScore = { 0, 2, 30, 450, 6750, 101250 };
        static readonly int[] DefenseScore = { 0, 1, 15, 225, 3375, 50625 };

        int EvaluateAttackHorizontal(int r, int c)
        {
            int cur = c - 1;
            int chessCount = 1;
            int blockedEnd = 0;

            while (cur >= 0 && matrix[r, cur] == matrix[r, c])
            {
                chessCount++;
                cur--;
            }

            if (cur >= 0 && matrix[r, cur] != 0)
                blockedEnd++;
            cur = c + 1;
            while (cur < nCols && matrix[r, cur] == matrix[r, c])
            {
                chessCount++;
                cur++;
            }

            if (cur < nCols && matrix[r, cur] != 0)
                blockedEnd++;
            if (blockedEnd == 2 || chessCount > 5)
                return 0;
            return AttackScore[chessCount] - DefenseScore[blockedEnd * chessCount];
        }

        int EvaluateAttackVertical(int r, int c)
        {
            int cur = r - 1;
            int chessCount = 1;
            int blockedEnd = 0;
            while (cur >= 0 && matrix[cur, c] == matrix[r, c])
            {
                chessCount++;
                cur--;
            }
            if (cur >= 0 && matrix[cur, c] != 0)
                blockedEnd++;
            cur = r + 1;
            while (cur < nRows && matrix[cur, c] == matrix[r, c])
            {
                chessCount++;
                cur++;
            }
            if (cur < nRows && matrix[cur, c] != 0)
                blockedEnd++;
            if (blockedEnd == 2 || chessCount > 5)
                return 0;
            return AttackScore[chessCount] - DefenseScore[blockedEnd * chessCount];
        }

        int EvaluateAttackMainDiagonal(int r, int c)
        {
            int i = r - 1, j = c - 1;
            int chessCount = 1;
            int blockedEnd = 0;
            while (i >= 0 && j >= 0 && matrix[i, j] == matrix[r, c])
            {
                chessCount++;
                i--;
                j--;
            }
            if (i >= 0 && j >= 0 && matrix[i, j] != 0)
                blockedEnd++;
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
            if (blockedEnd == 2 || chessCount > 5)
                return 0;
            return AttackScore[chessCount] - DefenseScore[blockedEnd];
        }

        int EvaluateAttackSubDiagonal(int r, int c)
        {
            int i = r + 1, j = c - 1;
            int chessCount = 1;
            int blockedEnd = 0;
            while (i < nRows && j >= 0 && matrix[i, j] == matrix[r, c])
            {
                chessCount++;
                i++;
                j--;
            }
            if (i < nRows && j >= 0 && matrix[i, j] != 0)
                blockedEnd++;
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
            if (blockedEnd == 2 || chessCount > 5)
                return 0;
            return AttackScore[chessCount] - DefenseScore[blockedEnd * chessCount];
        }

        public int EvaluateAttack(int r, int c)
        {
            return EvaluateAttackHorizontal(r, c) + EvaluateAttackVertical(r, c) + EvaluateAttackMainDiagonal(r, c) + EvaluateAttackSubDiagonal(r, c);
        }

        int EvaluateDefenseHorizontal(int r, int c)
        {
            int cur = c - 1;
            int opponentChessCount = 0;
            int allyChessCount = 1;
            int opponentChess = matrix[r, c] == 1 ? 2 : 1;
            while (cur >= 0 && matrix[r, cur] == opponentChess)
            {
                opponentChessCount++;
                cur--;
            }

            if (cur >= 0 && matrix[r, cur] != 0)
                allyChessCount++;
            cur = c + 1;
            while (cur < nCols && matrix[r, cur] == opponentChess)
            {
                opponentChessCount++;
                cur++;
            }

            if (cur < nCols && matrix[r, cur] != 0)
                allyChessCount++;
            if (allyChessCount == 3 || opponentChessCount >= 5)
                return 0;
            return DefenseScore[opponentChessCount + 1] - AttackScore[allyChessCount];
        }

        int EvaluateDefenseVertical(int r, int c)
        {
            int cur = r - 1;
            int opponentChessCount = 0;
            int allyChessCount = 1;
            int opponentChess = matrix[r, c] == 1 ? 2 : 1;
            while (cur >= 0 && matrix[cur, c] == opponentChess)
            {
                opponentChessCount++;
                cur--;
            }
            if (cur >= 0 && matrix[cur, c] != 0)
                allyChessCount++;
            cur = r + 1;
            while (cur < nRows && matrix[cur, c] == opponentChess)
            {
                opponentChessCount++;
                cur++;
            }
            if (cur < nRows && matrix[cur, c] != 0)
                allyChessCount++;
            if (allyChessCount == 3 || opponentChessCount > 5)
                return 0;
            return DefenseScore[opponentChessCount + 1] - AttackScore[allyChessCount];
        }

        int EvaluateDefenseMainDiagonal(int r, int c)
        {
            int i = r - 1, j = c - 1;
            int opponentChessCount = 0;
            int allyChessCount = 1;
            int opponentChess = matrix[r, c] == 1 ? 2 : 1;
            while (i >= 0 && j >= 0 && matrix[i, j] == opponentChess)
            {
                opponentChessCount++;
                i--;
                j--;
            }
            if (i >= 0 && j >= 0 && matrix[i, j] != 0)
                allyChessCount++;
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
            if (allyChessCount == 3 || opponentChessCount > 5)
                return 0;
            return DefenseScore[opponentChessCount + 1] - AttackScore[allyChessCount];
        }

        int EvaluateDefenseSubDiagonal(int r, int c)
        {
            int i = r + 1, j = c - 1;
            int opponentChessCount = 0;
            int allyChessCount = 1;
            int opponentChess = matrix[r, c] == 1 ? 2 : 1;
            while (i < nRows && j >= 0 && matrix[i, j] == opponentChess)
            {
                opponentChessCount++;
                i++;
                j--;
            }
            if (i < nRows && j >= 0 && matrix[i, j] != 0)
                allyChessCount++;
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
            if (allyChessCount == 3 || opponentChessCount > 5)
                return 0;
            return DefenseScore[opponentChessCount + 1] - AttackScore[allyChessCount];
        }

        public int EvaluateDefense(int r, int c)
        {
            return EvaluateDefenseHorizontal(r, c) + EvaluateDefenseVertical(r, c) + EvaluateDefenseMainDiagonal(r, c) + EvaluateDefenseSubDiagonal(r, c);
        }
        #endregion

        public bool CheckDraw()
        {
            return filledCount == (nRows * nCols);
        }
    }
}