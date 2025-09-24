using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BlockTerritory
{
    public class ArrayTree
    {
        static int MaxTreeNum = 900000;

        public int TreeNum = 0;
        public int[] Tree = new int[MaxTreeNum];
        public int[] Win = new int[MaxTreeNum];
        public int[] Visit = new int[MaxTreeNum];
        public double[] UCB = new double[MaxTreeNum];
        public int[] Child = new int[MaxTreeNum];
        public int[] Father = new int[MaxTreeNum];
    }

    internal class MCTS
    {
        setting setting = new setting();
        ArrayTree ArrayTree = new ArrayTree();
        Dimension Dimension = new Dimension();
        Rule Rule = new Rule();
        Random Rd = new Random();

        public double CalculateUCB(float Win, int Visit, int AllVisit, bool UseCuriosity = true)
        {
            double WinningRate = Win / Visit;
            double Curiosity = 2 * Math.Sqrt(Math.Log10(AllVisit) / Visit);
            if (UseCuriosity == false) Curiosity = 0;
            double UCB = WinningRate + Curiosity;
            return UCB;
        }


        public void Input(int[,] Board, int num)
        {
            int XYSR = Convert.ToInt32(ArrayTree.Tree[num] % 1000000);
            int X = XYSR / 10000;
            int Y = (XYSR / 100) % 100;
            int SR = Convert.ToInt16(XYSR % 100);

            int Player = ArrayTree.Tree[num] / 1000000;

            int[,] ToBoard = new int[2, 2];
            switch (SR.ToString())
            {
                //2顆橫排落子
                case "0":
                    ToBoard[0, 0] = X;
                    ToBoard[0, 1] = Y;
                    ToBoard[1, 0] = X + 1;
                    ToBoard[1, 1] = Y;
                    break;

                //2顆直排落子
                case "1":
                    ToBoard[0, 0] = X;
                    ToBoard[0, 1] = Y;
                    ToBoard[1, 0] = X;
                    ToBoard[1, 1] = Y + 1;
                    break;
            }

            Board[ToBoard[0, 0], ToBoard[0, 1]] = Player;
            Board[ToBoard[1, 0], ToBoard[1, 1]] = Player;
            Rule.ReverseBoard(ToBoard,Board);
        }

        public void Input(int[] Tree, int[,] Board, int num)
        {
            int XYSR = Convert.ToInt32(Tree[num] % 1000000);
            int X = XYSR / 10000;
            int Y = (XYSR / 100) % 100;
            int SR = Convert.ToInt16(XYSR % 100);

            int Player = Tree[num] / 1000000;

            int[,] ToBoard = new int[2, 2];
            switch (SR.ToString())
            {
                //2顆橫排落子
                case "0":
                    ToBoard[0, 0] = X;
                    ToBoard[0, 1] = Y;
                    ToBoard[1, 0] = X + 1;
                    ToBoard[1, 1] = Y;
                    break;

                //2顆直排落子
                case "1":
                    ToBoard[0, 0] = X;
                    ToBoard[0, 1] = Y;
                    ToBoard[1, 0] = X;
                    ToBoard[1, 1] = Y + 1;
                    break;
            }

            Board[ToBoard[0, 0], ToBoard[0, 1]] = Player;
            Board[ToBoard[1, 0], ToBoard[1, 1]] = Player;
            Rule.ReverseBoard(ToBoard, Board);
        }


        public void select(int MCTS_TIME, int RealPlayer, int[,] RealBoard)
        {
            ArrayTree = new ArrayTree();
            
            for (int _ = 0; _ < MCTS_TIME; _++)
            {
                int SimPlayer = RealPlayer;
                int[,] SimBoard = new int[setting.BoardSizeXY, setting.BoardSizeXY];
                Array.Copy(RealBoard, SimBoard, setting.BoardSizeXY * setting.BoardSizeXY);

                if (_ == 0)
                {
                    //第一次直接進入擴展
                    Expansion(SimBoard, SimPlayer, 0);
                }
                else
                {
                    int Start = 1;
                    double MaxUCB = -10;
                    int MaxUCBnum = -1;
                    while (true)
                    {
                        int Win = ArrayTree.Win[Start];
                        int Visit = ArrayTree.Visit[Start];
                        double UCB = ArrayTree.UCB[Start];
                        int Father = ArrayTree.Father[Start];

                        if (Visit != 0)
                        {
                            //if (Father == 0) UCB = CalculateUCB(Win, Visit, _);
                            //else UCB = CalculateUCB(Win, Visit, Convert.ToInt32(Tree.Rows[Father][6].ToString()));
                            //Tree.Rows[Start][7] = UCB;

                            if (Father == 0) UCB = CalculateUCB(Win, Visit, _);
                            else UCB = CalculateUCB(Win, Visit, ArrayTree.Visit[Father]);
                            ArrayTree.UCB[Start] = UCB;
                        }

                        if (UCB > MaxUCB || (UCB == MaxUCB && Rd.Next(1, 101) >= 50))
                        {
                            MaxUCB = UCB;
                            MaxUCBnum = Start;
                        }

                        if (Start == ArrayTree.TreeNum)
                        {
                            //leaf node
                            Input(SimBoard, MaxUCBnum);

                            break;
                        }

                        if (Father != ArrayTree.Father[Start + 1])
                        {
                            //The last node of this layer
                            if (ArrayTree.Child[MaxUCBnum] == 0)
                            {
                                //leaf node
                                Input(SimBoard, MaxUCBnum);
                                if (SimPlayer == 1) SimPlayer = 2;
                                else SimPlayer = 1;

                                break;
                            }
                            else
                            {
                                //next layer
                                Input(SimBoard, MaxUCBnum);

                                Start = ArrayTree.Child[MaxUCBnum];
                                MaxUCB = -10;
                                MaxUCBnum = -1;
                                continue;
                            }
                        }

                        Start++;
                    }

                    SimPlayer = ArrayTree.Tree[MaxUCBnum] / 1000000;
                    if (SimPlayer == 1) SimPlayer = 2;
                    else SimPlayer = 1;

                    if (Rule.IsGameOver(SimBoard) == true)
                    {
                        //已經沒有著手 但MCTS尚未結束
                        //Back(ref Tree, SimBoard, MaxUCBnum);
                    }
                    else
                    {
                        Expansion(SimBoard, SimPlayer, MaxUCBnum);
                    }
                }
            }

            //find the winner of the first layer
            double MaxWinner = -10;
            int MaxWinnerNum = -1;
            for (int i = 1; i <= ArrayTree.TreeNum; i++)
            {
                //X,Y,S,R,Player,Win,Visit,UCB,Father,Child

                int Father = ArrayTree.Father[i];
                if (Father != 0) break;

                int Win = ArrayTree.Win[i];
                int Visit = ArrayTree.Visit[i];

                if (Visit != 0)
                {
                    double Winner = CalculateUCB(Win, Visit, MCTS_TIME, false);
                    if (Winner > MaxWinner || (Winner == MaxWinner && Rd.Next(1, 101) >= 50))
                    {
                        MaxWinner = Winner;
                        MaxWinnerNum = i;
                    }
                }
            }

            //change the real board state
            Input(RealBoard, MaxWinnerNum);
        }

        private void Expansion(int[,] Board, int Player, int Father)
        {
            int num = 0;

            for (int k = 0; k < setting.BoardSizeXY * setting.BoardSizeXY * 2; k++)
            {
                string Action = Dimension.ActionToBoard(k);
                int X = Convert.ToInt16(Action.Split(';')[0]);
                int Y = Convert.ToInt16(Action.Split(';')[1]);

                if (Rule.CanInput(Player, Board, k) == true)
                {
                    num++;

                    ArrayTree.TreeNum++;

                    //Player(1 or 2),X,Y,SR(2)
                    ArrayTree.Tree[ArrayTree.TreeNum] = Player * 1000000 + X * 10000 + Y * 100 + Convert.ToInt16(Action.Split(';')[2]);

                    //Win(擴展階段補0),Visit(擴展階段補0)
                    ArrayTree.Win[ArrayTree.TreeNum] = 0;
                    ArrayTree.Visit[ArrayTree.TreeNum] = 0;

                    //擴展階段UCB都設定成10
                    ArrayTree.UCB[ArrayTree.TreeNum] = 10;


                    ArrayTree.Father[ArrayTree.TreeNum] = Father;
                    ArrayTree.Child[ArrayTree.TreeNum] = 0;
                }
            }

            if (num > 0)
            {
                int head = (int)(ArrayTree.TreeNum + 1) - num;
                if (Father != 0) ArrayTree.Child[Father] = head;
                int UseNodeNum = head + Rd.Next(0, num);
                Rollout(Board, Player, UseNodeNum);
            }
            else
            {
                //Console.WriteLine("..MCTS Expansion Down..");
                return;
            }
        }

        private void Rollout(int[,] Board, int Player, int UseNodeNum)
        {
            Dimension Dimension = new Dimension();

            Input(Board, UseNodeNum);

            while (true)
            {
                if(Rule.IsGameOver(Board) == true) break;

                int TreeNum = 0;
                int[] RandomTree = new int[1800];

                if (Player == 1) Player = 2;
                else Player = 1;

                for (int k = 0; k < setting.BoardSizeXY * setting.BoardSizeXY * 2; k++)
                {
                    string Action = Dimension.ActionToBoard(k);
                    int X = Convert.ToInt16(Action.Split(';')[0]);
                    int Y = Convert.ToInt16(Action.Split(';')[1]);

                    if (Rule.CanInput(Player, Board, k) == true)
                    {
                        TreeNum++;

                        //Player(1 or 2),X,Y,SR(2)
                        RandomTree[TreeNum] = Player * 1000000 + X * 10000 + Y * 100 + Convert.ToInt16(Action.Split(';')[2]);
                    }

                }


                if (TreeNum > 0)
                {
                    int RandomNum = Rd.Next(1, TreeNum + 1);
                    Input(RandomTree, Board, RandomNum);
                }
                else
                {
                    Console.WriteLine("MCTS Rollout Down");
                    Console.WriteLine("Wait");
                    Console.ReadLine();
                }
            }

            Back(Board, UseNodeNum);
        }

        private void Back(int[,] Board, int UseNodeNum)
        {
            int[] Score = Rule.BlackWhiteCount(Board);

            while (true)
            {
                int Father = ArrayTree.Father[UseNodeNum];
                int Player = 0;

                Player = ArrayTree.Tree[UseNodeNum] / 1000000;

                ArrayTree.Visit[UseNodeNum]++;

                if (Player == 1 && Score[0] > Score[1]) ArrayTree.Win[UseNodeNum]++;
                else if (Player == 2 && Score[0] < Score[1]) ArrayTree.Win[UseNodeNum]++;
                //else if (Score[0] == Score[1]) Tree.Rows[UseNodeNum][5] = Convert.ToDouble(Tree.Rows[UseNodeNum][5]) + 0.2f;

                if (Father != 0) UseNodeNum = Father;
                else break;
            }
        }
    }
}
