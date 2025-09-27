namespace BlockTerritory
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnCancel = new Button();
            btnConfirm = new Button();
            GameModeSelect = new ComboBox();
            btnOK = new Button();
            txtMsg = new RichTextBox();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            btnCancel.Location = new Point(582, 200);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(133, 50);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "清除落子";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnConfirm
            // 
            btnConfirm.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            btnConfirm.Location = new Point(732, 200);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(133, 50);
            btnConfirm.TabIndex = 3;
            btnConfirm.Text = "確定落子";
            btnConfirm.UseVisualStyleBackColor = true;
            btnConfirm.Click += btnConfirm_Click;
            // 
            // GameModeSelect
            // 
            GameModeSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            GameModeSelect.Font = new Font("Microsoft JhengHei UI", 19.8000011F, FontStyle.Regular, GraphicsUnit.Point, 136);
            GameModeSelect.FormattingEnabled = true;
            GameModeSelect.Items.AddRange(new object[] { "MCTS VS MCTS", "玩家 VS MCTS" });
            GameModeSelect.Location = new Point(582, 144);
            GameModeSelect.Name = "GameModeSelect";
            GameModeSelect.Size = new Size(283, 50);
            GameModeSelect.TabIndex = 0;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(802, 90);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(63, 50);
            btnOK.TabIndex = 1;
            btnOK.Text = "GO";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // txtMsg
            // 
            txtMsg.Location = new Point(582, 12);
            txtMsg.Name = "txtMsg";
            txtMsg.ReadOnly = true;
            txtMsg.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtMsg.Size = new Size(214, 128);
            txtMsg.TabIndex = 2;
            txtMsg.Text = "*系統訊息*";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(882, 573);
            Controls.Add(txtMsg);
            Controls.Add(btnOK);
            Controls.Add(GameModeSelect);
            Controls.Add(btnConfirm);
            Controls.Add(btnCancel);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnCancel;
        private Button btnConfirm;
        private ComboBox GameModeSelect;
        private Button btnOK;
        private RichTextBox txtMsg;
    }
}
