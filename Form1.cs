using BlockGo_ControlPanel;
using System.Collections;

namespace BlockTerritory
{
    public partial class Form1 : Form
    {
        public int MCTS_First = 500;
        public int MCTS_Second = 500;
        public int MCTS_Enemy = 1000;

        delegate void update1(string str);
        delegate void upadte2(int[,] Board);
        delegate void upadte3(bool en);

        static setting setting = new setting();
        static int BoardSizeXY = setting.BoardSizeXY;
        Hashtable ButtonTable = new Hashtable();
        Rule Rule = new Rule();
        MCTS MCTS = new MCTS();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Enabled = false;//先鎖定
            EnablePlayerInput(false);
            EnableSelectGameMode(false);

            int startX = 10;
            int startY = 10;
            int gap = 1;
            int sizeX = 45, sizeY = 45;

            for (int r = 0; r < BoardSizeXY; r++)
            {
                for (int c = 0; c < BoardSizeXY; c++)
                {
                    int x = startX + c * (sizeX + gap);
                    int y = startY + r * (sizeY + gap);

                    var btn = new Button();
                    btn.BackColor = Color.FromArgb(164, 89, 28);
                    btn.Text = ((c + 1) + (r * BoardSizeXY)).ToString();
                    btn.Size = new Size(sizeX, sizeY);
                    btn.Location = new Point(x, y);
                    Controls.Add(btn);
                    ButtonTable.Add(btn.Text, btn);
                    btn.Click += BoardBtn_Click;// 加入 Click 事件
                }
            }

            EnableSelectGameMode(true);
            this.Enabled = true;
        }

        //玩家按下按鈕(棋盤)
        private void BoardBtn_Click(object sender, EventArgs e)
        {
            if (btnConfirm.Enabled == false) return;
            Button btn = (Button)sender;
            if (btn.BackColor.R != 164 || btn.BackColor.G != 89 || btn.BackColor.B != 28)
            {
                ReNewMsg("*此位置無法下棋*");
                return;
            }

            btn.BackColor = Color.FromArgb(168, 168, 168);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            /*
            0. MCTS VS MCTS
            1. 玩家 VS MCTS
            */

            if (GameModeSelect.SelectedIndex == 0)
            {
                ReNewMsg("*模式 : " + GameModeSelect.SelectedItem + "*");
                ReNewMsg("******\nMCTS互相對戰模式，MCTS次數請從內部程式修改\n******");
                Task.Factory.StartNew(AItoAI);
            }
            else if (GameModeSelect.SelectedIndex == 1)
            {
                Form2 Form2 = new Form2();
                if (Form2.ShowDialog() == DialogResult.OK)
                {
                    ReNewMsg("*模式 : " + GameModeSelect.SelectedItem + "*");

                    if (Form2.ResultMessage == "FirstPlayer")
                    {
                        ReNewMsg("*玩家'先'手*");
                    }
                    else if (Form2.ResultMessage == "SecondPlayer")
                    {
                        ReNewMsg("*玩家'後'手*");
                    }

                    Task.Factory.StartNew(PlayerToAI, Form2.ResultMessage.ToString());
                }
                else
                {
                    ReNewMsg("**玩家關閉視窗**");
                }
            }
            else
            {
                ReNewMsg("**請先選擇遊戲模式**");
            }
        }

        int[,] TempBoard = new int[BoardSizeXY, BoardSizeXY];
        int TempPlayer = 1;//1先手,2後手

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            int InputCount = 0;
            int[] InputPosition = new int[2] { 0, 0 };

            for (int i = 1; i <= BoardSizeXY * BoardSizeXY; i++)
            {
                Button btn = (Button)ButtonTable[i.ToString()];
                if (btn.BackColor.R == 168 && btn.BackColor.G == 168 && btn.BackColor.B == 168)
                {
                    if (InputCount == 2)
                    {
                        ReNewMsg("超出落子數量");
                        return;
                    }

                    //玩家欲下的位置
                    InputPosition[InputCount] = i;
                    InputCount++;
                }
            }

            //------------檢測落子數量
            if (InputCount != 2)
            {
                ReNewMsg("落子數量必須為2");
                return;
            }

            //------------檢測連氣(檢測四個方向)
            if (InputPosition[0] - BoardSizeXY != InputPosition[1]
               && InputPosition[0] + BoardSizeXY != InputPosition[1]
                && InputPosition[0] - 1 != InputPosition[1]
                && InputPosition[0] + 1 != InputPosition[1])
            {
                ReNewMsg("棋子必須連在一起");
                return;
            }

            int[,] ToBoard = new int[2, 2];
            for (int i = 0; i < InputCount; i++)
            {
                int XY = InputPosition[i];
                int Y = (XY - 1) / BoardSizeXY;
                int X = (XY - 1) % BoardSizeXY;
                TempBoard[X, Y] = TempPlayer;
                ToBoard[i, 0] = X;
                ToBoard[i, 1] = Y;
            }
            Rule.ReverseBoard(ToBoard, TempBoard);
            ReNewBoard(TempBoard);

            ReNewMsg("落子完畢");
            EnablePlayerInput(false);
            TaskWaitControl.Set();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ReNewMsg("落子取消");
            ReNewBoard(TempBoard);
        }

        //-------------------------跨執行緒更新用函式-------------------------
        private void ReNewMsg(string str)
        {
            txtMsg.Text += "\n" + str;
            txtMsg.SelectionStart = txtMsg.TextLength; // 把插入點移到最後
            txtMsg.ScrollToCaret();                    // 自動捲到最底
        }

        private void ReNewBoard(int[,] Board)
        {
            for (int j = 0; j < BoardSizeXY; j++)
            {
                for (int i = 0; i < BoardSizeXY; i++)
                {
                    //先從X開始走
                    if (Board[i, j] == 0)
                    {
                        Button btn = (Button)ButtonTable[((i + 1) + (j * BoardSizeXY)).ToString()];
                        btn.BackColor = Color.FromArgb(164, 89, 28);
                    }
                    else if (Board[i, j] == 1)
                    {
                        Button btn = (Button)ButtonTable[((i + 1) + (j * BoardSizeXY)).ToString()];
                        btn.BackColor = Color.FromArgb(42, 42, 42);
                    }
                    else if (Board[i, j] == 2)
                    {
                        Button btn = (Button)ButtonTable[((i + 1) + (j * BoardSizeXY)).ToString()];
                        btn.BackColor = Color.FromArgb(255, 255, 255);
                    }

                }
            }
        }

        private void EnablePlayerInput(bool en)
        {
            btnConfirm.Enabled = en;
            btnCancel.Enabled = en;
        }

        private void EnableSelectGameMode(bool en)
        {
            btnOK.Enabled = en;
            GameModeSelect.Enabled = en;
        }

        //-------------------------非UI執行緒工作區-------------------------

        private readonly AutoResetEvent TaskWaitControl = new(initialState: false);

        private void AItoAI()
        {
            update1 updateMsg = ReNewMsg;
            upadte2 updateBoard = ReNewBoard;
            upadte3 updateBtn = EnableSelectGameMode;

            int[,] Board_SingleMode = new int[BoardSizeXY, BoardSizeXY];//目前實際狀況的棋盤

            Invoke(updateBtn, false);//鎖住選擇遊戲模式與開始按鈕
            Invoke(updateBoard, Board_SingleMode);//先清空畫面的棋盤
            Invoke(updateMsg, "***遊戲開始***");


            while (Rule.IsGameOver(Board_SingleMode) == false)
            {
                //先手(黑子)
                MCTS.select(MCTS_First, 1, Board_SingleMode);
                Invoke(updateMsg, "\a黑子思考完畢");

                Invoke(ReNewBoard, Board_SingleMode);

                //後手(白子)
                MCTS.select(MCTS_Second, 2, Board_SingleMode);
                Invoke(updateMsg, "\a白子思考完畢");

                Invoke(ReNewBoard, Board_SingleMode);
            }

            //檢查是誰勝利
            int[] Score = Rule.BlackWhiteCount(Board_SingleMode);

            if (Score[0] == Score[1])
            {
                Invoke(updateMsg, "遊戲結束，平手" + "\n黑:" + Score[0].ToString() + "\n白:" + Score[1].ToString());
            }
            else if (Score[0] > Score[1])
            {
                Invoke(updateMsg, "遊戲結束，黑子贏" + "\n黑:" + Score[0].ToString() + "\n白:" + Score[1].ToString());
            }
            else if (Score[0] < Score[1])
            {
                Invoke(updateMsg, "遊戲結束，白子贏" + "\n黑:" + Score[0].ToString() + "\n白:" + Score[1].ToString());
            }

            Invoke(updateBtn, true);//解除鎖定遊戲模式與開始按鈕
        }


        private void PlayerToAI(object WhoFirst)
        {
            update1 updateMsg = ReNewMsg;
            upadte2 updateBoard = ReNewBoard;
            upadte3 updateBtn = EnablePlayerInput;
            upadte3 updateBtn2 = EnableSelectGameMode;

            int[,] Board_SingleMode = new int[BoardSizeXY, BoardSizeXY];//目前實際狀況的棋盤

            Invoke(updateBtn2, false);//鎖住選擇遊戲模式與開始按鈕
            Invoke(ReNewBoard, Board_SingleMode);//先清空畫面的棋盤
            Invoke(updateMsg, "***遊戲開始***");

            //比10回合
            while (Rule.IsGameOver(Board_SingleMode) == false)
            {
                if (WhoFirst.ToString() == "FirstPlayer")
                {
                    //==玩家先手==

                    //後手(白子)
                    MCTS.select(Convert.ToInt32(MCTS_Enemy), 2, Board_SingleMode);
                    Invoke(updateMsg, "\a白子思考完畢");
                    Invoke(ReNewBoard, Board_SingleMode);

                    //先手(黑子)
                    Array.Copy(Board_SingleMode, TempBoard, BoardSizeXY * BoardSizeXY);
                    TempPlayer = 1;
                    Invoke(updateMsg, "輪到黑子(玩家)，請落子…");
                    Invoke(updateBtn, true);
                    TaskWaitControl.WaitOne();  //這裡暫停，等UI事件TaskWaitControl.Set()
                    Array.Copy(TempBoard, Board_SingleMode, BoardSizeXY * BoardSizeXY);
                }
                else
                {
                    //==玩家後手==

                    //後手(白子)
                    Array.Copy(Board_SingleMode, TempBoard, BoardSizeXY * BoardSizeXY);
                    TempPlayer = 2;
                    Invoke(updateMsg, "輪到白子(玩家)，請落子…");
                    Invoke(updateBtn, true);
                    TaskWaitControl.WaitOne();  //這裡暫停，等UI事件TaskWaitControl.Set()
                    Array.Copy(TempBoard, Board_SingleMode, BoardSizeXY * BoardSizeXY);

                    //先手(黑子)
                    MCTS.select(Convert.ToInt32(MCTS_Enemy), 1, Board_SingleMode);
                    Invoke(updateMsg, "\a黑子思考完畢");
                    Invoke(ReNewBoard, Board_SingleMode);
                }


            }

            //檢查是誰勝利
            int[] Score = Rule.BlackWhiteCount(Board_SingleMode);

            if (Score[0] == Score[1])
            {
                Invoke(updateMsg, "遊戲結束，平手" + "\n黑:" + Score[0].ToString() + "\n白:" + Score[1].ToString());
            }
            else if (Score[0] > Score[1])
            {
                Invoke(updateMsg, "遊戲結束，黑子贏" + "\n黑:" + Score[0].ToString() + "\n白:" + Score[1].ToString());
            }
            else if (Score[0] < Score[1])
            {
                Invoke(updateMsg, "遊戲結束，白子贏" + "\n黑:" + Score[0].ToString() + "\n白:" + Score[1].ToString());
            }

            Invoke(updateBtn2, true);//解除鎖定遊戲模式與開始按鈕
        }

        
    }
}
