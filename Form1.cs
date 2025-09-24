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
            this.Enabled = false;//����w
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
                    btn.Click += BoardBtn_Click;// �[�J Click �ƥ�
                }
            }

            EnableSelectGameMode(true);
            this.Enabled = true;
        }

        //���a���U���s(�ѽL)
        private void BoardBtn_Click(object sender, EventArgs e)
        {
            if (btnConfirm.Enabled == false) return;
            Button btn = (Button)sender;
            if (btn.BackColor.R != 164 || btn.BackColor.G != 89 || btn.BackColor.B != 28)
            {
                ReNewMsg("*����m�L�k�U��*");
                return;
            }

            btn.BackColor = Color.FromArgb(168, 168, 168);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            /*
            0. MCTS VS MCTS
            1. ���a VS MCTS
            */

            if (GameModeSelect.SelectedIndex == 0)
            {
                ReNewMsg("*�Ҧ� : " + GameModeSelect.SelectedItem + "*");
                ReNewMsg("******\nMCTS���۹�ԼҦ��AMCTS���ƽбq�����{���ק�\n******");
                Task.Factory.StartNew(AItoAI);
            }
            else if (GameModeSelect.SelectedIndex == 1)
            {
                Form2 Form2 = new Form2();
                if (Form2.ShowDialog() == DialogResult.OK)
                {
                    ReNewMsg("*�Ҧ� : " + GameModeSelect.SelectedItem + "*");

                    if (Form2.ResultMessage == "FirstPlayer")
                    {
                        ReNewMsg("*���a'��'��*");
                    }
                    else if (Form2.ResultMessage == "SecondPlayer")
                    {
                        ReNewMsg("*���a'��'��*");
                    }

                    Task.Factory.StartNew(PlayerToAI, Form2.ResultMessage.ToString());
                }
                else
                {
                    ReNewMsg("**���a��������**");
                }
            }
            else
            {
                ReNewMsg("**�Х���ܹC���Ҧ�**");
            }
        }

        int[,] TempBoard = new int[BoardSizeXY, BoardSizeXY];
        int TempPlayer = 1;//1����,2���

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
                        ReNewMsg("�W�X���l�ƶq");
                        return;
                    }

                    //���a���U����m
                    InputPosition[InputCount] = i;
                    InputCount++;
                }
            }

            //------------�˴����l�ƶq
            if (InputCount != 2)
            {
                ReNewMsg("���l�ƶq������2");
                return;
            }

            //------------�˴��s��(�˴��|�Ӥ�V)
            if (InputPosition[0] - BoardSizeXY != InputPosition[1]
               && InputPosition[0] + BoardSizeXY != InputPosition[1]
                && InputPosition[0] - 1 != InputPosition[1]
                && InputPosition[0] + 1 != InputPosition[1])
            {
                ReNewMsg("�Ѥl�����s�b�@�_");
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

            ReNewMsg("���l����");
            EnablePlayerInput(false);
            TaskWaitControl.Set();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ReNewMsg("���l����");
            ReNewBoard(TempBoard);
        }

        //-------------------------��������s�Ψ禡-------------------------
        private void ReNewMsg(string str)
        {
            txtMsg.Text += "\n" + str;
            txtMsg.SelectionStart = txtMsg.TextLength; // �ⴡ�J�I����̫�
            txtMsg.ScrollToCaret();                    // �۰ʱ���̩�
        }

        private void ReNewBoard(int[,] Board)
        {
            for (int j = 0; j < BoardSizeXY; j++)
            {
                for (int i = 0; i < BoardSizeXY; i++)
                {
                    //���qX�}�l��
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

        //-------------------------�DUI������u�@��-------------------------

        private readonly AutoResetEvent TaskWaitControl = new(initialState: false);

        private void AItoAI()
        {
            update1 updateMsg = ReNewMsg;
            upadte2 updateBoard = ReNewBoard;
            upadte3 updateBtn = EnableSelectGameMode;

            int[,] Board_SingleMode = new int[BoardSizeXY, BoardSizeXY];//�ثe��ڪ��p���ѽL

            Invoke(updateBtn, false);//����ܹC���Ҧ��P�}�l���s
            Invoke(updateBoard, Board_SingleMode);//���M�ŵe�����ѽL
            Invoke(updateMsg, "***�C���}�l***");


            while (Rule.IsGameOver(Board_SingleMode) == false)
            {
                //����(�¤l)
                MCTS.select(MCTS_First, 1, Board_SingleMode);
                Invoke(updateMsg, "\a�¤l��ҧ���");

                Invoke(ReNewBoard, Board_SingleMode);

                //���(�դl)
                MCTS.select(MCTS_Second, 2, Board_SingleMode);
                Invoke(updateMsg, "\a�դl��ҧ���");

                Invoke(ReNewBoard, Board_SingleMode);
            }

            //�ˬd�O�ֳӧQ
            int[] Score = Rule.BlackWhiteCount(Board_SingleMode);

            if (Score[0] == Score[1])
            {
                Invoke(updateMsg, "�C�������A����" + "\n��:" + Score[0].ToString() + "\n��:" + Score[1].ToString());
            }
            else if (Score[0] > Score[1])
            {
                Invoke(updateMsg, "�C�������A�¤lĹ" + "\n��:" + Score[0].ToString() + "\n��:" + Score[1].ToString());
            }
            else if (Score[0] < Score[1])
            {
                Invoke(updateMsg, "�C�������A�դlĹ" + "\n��:" + Score[0].ToString() + "\n��:" + Score[1].ToString());
            }

            Invoke(updateBtn, true);//�Ѱ���w�C���Ҧ��P�}�l���s
        }


        private void PlayerToAI(object WhoFirst)
        {
            update1 updateMsg = ReNewMsg;
            upadte2 updateBoard = ReNewBoard;
            upadte3 updateBtn = EnablePlayerInput;
            upadte3 updateBtn2 = EnableSelectGameMode;

            int[,] Board_SingleMode = new int[BoardSizeXY, BoardSizeXY];//�ثe��ڪ��p���ѽL

            Invoke(updateBtn2, false);//����ܹC���Ҧ��P�}�l���s
            Invoke(ReNewBoard, Board_SingleMode);//���M�ŵe�����ѽL
            Invoke(updateMsg, "***�C���}�l***");

            //��10�^�X
            while (Rule.IsGameOver(Board_SingleMode) == false)
            {
                if (WhoFirst.ToString() == "FirstPlayer")
                {
                    //==���a����==

                    //���(�դl)
                    MCTS.select(Convert.ToInt32(MCTS_Enemy), 2, Board_SingleMode);
                    Invoke(updateMsg, "\a�դl��ҧ���");
                    Invoke(ReNewBoard, Board_SingleMode);

                    //����(�¤l)
                    Array.Copy(Board_SingleMode, TempBoard, BoardSizeXY * BoardSizeXY);
                    TempPlayer = 1;
                    Invoke(updateMsg, "����¤l(���a)�A�и��l�K");
                    Invoke(updateBtn, true);
                    TaskWaitControl.WaitOne();  //�o�̼Ȱ��A��UI�ƥ�TaskWaitControl.Set()
                    Array.Copy(TempBoard, Board_SingleMode, BoardSizeXY * BoardSizeXY);
                }
                else
                {
                    //==���a���==

                    //���(�դl)
                    Array.Copy(Board_SingleMode, TempBoard, BoardSizeXY * BoardSizeXY);
                    TempPlayer = 2;
                    Invoke(updateMsg, "����դl(���a)�A�и��l�K");
                    Invoke(updateBtn, true);
                    TaskWaitControl.WaitOne();  //�o�̼Ȱ��A��UI�ƥ�TaskWaitControl.Set()
                    Array.Copy(TempBoard, Board_SingleMode, BoardSizeXY * BoardSizeXY);

                    //����(�¤l)
                    MCTS.select(Convert.ToInt32(MCTS_Enemy), 1, Board_SingleMode);
                    Invoke(updateMsg, "\a�¤l��ҧ���");
                    Invoke(ReNewBoard, Board_SingleMode);
                }


            }

            //�ˬd�O�ֳӧQ
            int[] Score = Rule.BlackWhiteCount(Board_SingleMode);

            if (Score[0] == Score[1])
            {
                Invoke(updateMsg, "�C�������A����" + "\n��:" + Score[0].ToString() + "\n��:" + Score[1].ToString());
            }
            else if (Score[0] > Score[1])
            {
                Invoke(updateMsg, "�C�������A�¤lĹ" + "\n��:" + Score[0].ToString() + "\n��:" + Score[1].ToString());
            }
            else if (Score[0] < Score[1])
            {
                Invoke(updateMsg, "�C�������A�դlĹ" + "\n��:" + Score[0].ToString() + "\n��:" + Score[1].ToString());
            }

            Invoke(updateBtn2, true);//�Ѱ���w�C���Ҧ��P�}�l���s
        }

        
    }
}
