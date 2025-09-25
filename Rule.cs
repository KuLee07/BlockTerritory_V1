using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockTerritory
{
    internal class Rule
    {
        setting setting = new setting();

        /// <summary>
        /// 是否為合法步
        /// </summary>
        /// <param name="Player">玩家；1先手；2後手</param>
        /// <param name="ReadBoard">落子前棋盤</param>
        /// <param name="ActionNum">一維代碼(XY座標 與 直排or橫排)</param>
        /// <returns></returns>
        public bool CanInput(int Player, int[,] ReadBoard, int ActionNum)
        {
            Dimension Dimension = new Dimension();

            string Action = Dimension.ActionToBoard(ActionNum);
            int X = Convert.ToInt16(Action.Split(';')[0]);
            int Y = Convert.ToInt16(Action.Split(';')[1]);

            int[,] ToBoard = new int[2, 2];
            switch (Action.Split(';')[2])
            {
                //2顆橫排落子
                case "0":
                    ToBoard[0, 0] = X;
                    ToBoard[0, 1] = Y;
                    ToBoard[1, 0] = X+1;
                    ToBoard[1, 1] = Y;
                    break;

                //2顆直排落子
                case "1":
                    ToBoard[0, 0] = X;
                    ToBoard[0, 1] = Y;
                    ToBoard[1, 0] = X;
                    ToBoard[1, 1] = Y+1;
                    break;
            }

            if (ToBoard[1, 0] >= setting.BoardSizeXY || ToBoard[1, 1] >= setting.BoardSizeXY) return false;
            if (ReadBoard[ToBoard[0, 0], ToBoard[0, 1]] != 0) return false;
            if (ReadBoard[ToBoard[1, 0], ToBoard[1, 1]] != 0) return false;

            return true;
        }

        /// <summary>
        /// 翻轉棋子(必須先落子)
        /// </summary>
        /// <param name="ToBoard">剛剛落子位置</param>
        /// <param name="Board">落子後但尚未翻轉之棋盤</param>
        public void ReverseBoard(int[,] ToBoard, int[,] Board)
        {
            //讀取落子顏色
            int X1 = ToBoard[0, 0], Y1 = ToBoard[0, 1];
            int X2 = ToBoard[1, 0], Y2 = ToBoard[1, 1];
            int player = Board[X1, Y1];

            int enemyPlayer = 1;
            if (player == 1) enemyPlayer = 2;

            //先分辨是直排還是橫排
            if (Y1 == Y2)
            {
                //判定橫排

                //往上看
                if (Y1 > 0 && Board[X1, Y1 - 1] == enemyPlayer && Board[X2, Y2 - 1] == enemyPlayer)
                {
                    Board[X1, Y1 - 1] = player;
                    Board[X2, Y2 - 1] = player;
                }

                //往下看
                if (Y1 < setting.BoardSizeXY - 1 && Board[X1, Y1 + 1] == enemyPlayer && Board[X2, Y2 + 1] == enemyPlayer)
                {
                    Board[X1, Y1 + 1] = player;
                    Board[X2, Y2 + 1] = player;
                }
            }
            else
            {
                //判定直排
                //往左看
                if (X1 > 0 && Board[X1 - 1, Y1] == enemyPlayer && Board[X2 - 1, Y2] == enemyPlayer)
                {
                    Board[X1 - 1, Y1] = player;
                    Board[X2 - 1, Y2] = player;
                }

                //往右看
                if (X1 < setting.BoardSizeXY - 1 && Board[X1 + 1, Y1] == enemyPlayer && Board[X2 + 1, Y2] == enemyPlayer)
                {
                    Board[X1 + 1, Y1] = player;
                    Board[X2 + 1, Y2] = player;
                }

            }

        }

        /// <summary>
        /// 確認遊戲是否結束
        /// </summary>
        /// <param name="ReadBoard">棋盤</param>
        /// <returns>true遊戲結束;false遊戲進行中</returns>
        public bool IsGameOver(int[,] ReadBoard)
        {
            for (int y = 0; y < setting.BoardSizeXY; y++)
            {
                for (int x = 0; x < setting.BoardSizeXY; x++)
                {
                    if (ReadBoard[x, y] == 0)
                    {
                        //因為是由左至右，由上至下的掃棋盤
                        //所以只要看右邊跟下面就好
                        if (x < (setting.BoardSizeXY - 1))
                        {
                            if (ReadBoard[x + 1, y] == 0) return false;
                        }

                        if (y < (setting.BoardSizeXY - 1))
                        {
                            if (ReadBoard[x, y + 1] == 0) return false;
                        }
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// 計算黑子白子分數
        /// </summary>
        /// <param name="ReadBoard">比賽完的棋盤</param>
        /// <returns>int[0]存放黑子；int[1]存放白子</returns>
        public int[] BlackWhiteCount(int[,] ReadBoard)
        {
            int[] Score = new int[2] { 0, 0 };

            for (int y = 0; y < setting.BoardSizeXY; y++)
            {
                for (int x = 0; x < setting.BoardSizeXY; x++)
                {
                    if (ReadBoard[x, y] == 1)
                    {
                        Score[0]++;
                    }
                    else if (ReadBoard[x, y] == 2)
                    {
                        Score[1]++;
                    }
                }
            }

            return Score;
        }

    }
}
