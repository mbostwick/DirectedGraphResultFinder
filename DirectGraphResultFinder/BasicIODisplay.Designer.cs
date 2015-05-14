namespace DirectGraphResultFinder
{
    partial class BasicIODisplay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnTakeActionOnText = new System.Windows.Forms.Button();
            this.tcDataResults = new System.Windows.Forms.TabControl();
            this.tcInput = new System.Windows.Forms.TabPage();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.txbInputText = new System.Windows.Forms.RichTextBox();
            this.txbOutputText = new System.Windows.Forms.RichTextBox();
            this.pbRunningStatus = new System.Windows.Forms.ProgressBar();
            this.tcDataResults.SuspendLayout();
            this.tcInput.SuspendLayout();
            this.tpOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnTakeActionOnText
            // 
            this.btnTakeActionOnText.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnTakeActionOnText.Location = new System.Drawing.Point(0, 347);
            this.btnTakeActionOnText.Name = "btnTakeActionOnText";
            this.btnTakeActionOnText.Size = new System.Drawing.Size(409, 52);
            this.btnTakeActionOnText.TabIndex = 1;
            this.btnTakeActionOnText.Text = "Find Regions For Data";
            this.btnTakeActionOnText.UseVisualStyleBackColor = true;
            this.btnTakeActionOnText.Click += new System.EventHandler(this.btnTakeActionOnText_Click);
            // 
            // tcDataResults
            // 
            this.tcDataResults.Controls.Add(this.tcInput);
            this.tcDataResults.Controls.Add(this.tpOutput);
            this.tcDataResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcDataResults.Location = new System.Drawing.Point(0, 0);
            this.tcDataResults.Name = "tcDataResults";
            this.tcDataResults.SelectedIndex = 0;
            this.tcDataResults.Size = new System.Drawing.Size(409, 347);
            this.tcDataResults.TabIndex = 2;
            // 
            // tcInput
            // 
            this.tcInput.Controls.Add(this.txbInputText);
            this.tcInput.Location = new System.Drawing.Point(4, 22);
            this.tcInput.Name = "tcInput";
            this.tcInput.Padding = new System.Windows.Forms.Padding(3);
            this.tcInput.Size = new System.Drawing.Size(401, 321);
            this.tcInput.TabIndex = 0;
            this.tcInput.Text = "Input";
            this.tcInput.UseVisualStyleBackColor = true;
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.txbOutputText);
            this.tpOutput.Location = new System.Drawing.Point(4, 22);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tpOutput.Size = new System.Drawing.Size(335, 270);
            this.tpOutput.TabIndex = 1;
            this.tpOutput.Text = "Output";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // txbInputText
            // 
            this.txbInputText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbInputText.Location = new System.Drawing.Point(3, 3);
            this.txbInputText.Name = "txbInputText";
            this.txbInputText.Size = new System.Drawing.Size(395, 315);
            this.txbInputText.TabIndex = 0;
            this.txbInputText.Text = "";
            // 
            // txbOutputText
            // 
            this.txbOutputText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbOutputText.Location = new System.Drawing.Point(3, 3);
            this.txbOutputText.Name = "txbOutputText";
            this.txbOutputText.Size = new System.Drawing.Size(329, 264);
            this.txbOutputText.TabIndex = 0;
            this.txbOutputText.Text = "";
            // 
            // pbRunningStatus
            // 
            this.pbRunningStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pbRunningStatus.Location = new System.Drawing.Point(0, 324);
            this.pbRunningStatus.Name = "pbRunningStatus";
            this.pbRunningStatus.Size = new System.Drawing.Size(409, 23);
            this.pbRunningStatus.TabIndex = 1;
            // 
            // BasicIODisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 399);
            this.Controls.Add(this.tcDataResults);
            this.Controls.Add(this.pbRunningStatus);
            this.Controls.Add(this.btnTakeActionOnText);
            this.Name = "BasicIODisplay";
            this.Text = "Find Regions For Directed Graph";
            this.tcDataResults.ResumeLayout(false);
            this.tcInput.ResumeLayout(false);
            this.tpOutput.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTakeActionOnText;
        private System.Windows.Forms.TabControl tcDataResults;
        private System.Windows.Forms.TabPage tcInput;
        private System.Windows.Forms.TabPage tpOutput;
        private System.Windows.Forms.RichTextBox txbInputText;
        private System.Windows.Forms.RichTextBox txbOutputText;
        private System.Windows.Forms.ProgressBar pbRunningStatus;
    }
}

