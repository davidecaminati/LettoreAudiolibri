namespace WindowsFormsApplication1
{
    partial class Form1
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewFolder = new System.Windows.Forms.ListView();
            this.listViewMp3 = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewFolder);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listViewMp3);
            this.splitContainer1.Size = new System.Drawing.Size(706, 545);
            this.splitContainer1.SplitterDistance = 342;
            this.splitContainer1.TabIndex = 0;
            // 
            // listViewFolder
            // 
            this.listViewFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewFolder.Location = new System.Drawing.Point(0, 0);
            this.listViewFolder.Name = "listViewFolder";
            this.listViewFolder.Size = new System.Drawing.Size(342, 545);
            this.listViewFolder.TabIndex = 0;
            this.listViewFolder.UseCompatibleStateImageBehavior = false;
            this.listViewFolder.View = System.Windows.Forms.View.List;
            this.listViewFolder.SelectedIndexChanged += new System.EventHandler(this.listViewFolder_SelectedIndexChanged);
            this.listViewFolder.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewFolder_KeyUp);
            // 
            // listViewMp3
            // 
            this.listViewMp3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewMp3.Enabled = false;
            this.listViewMp3.Location = new System.Drawing.Point(0, 0);
            this.listViewMp3.Name = "listViewMp3";
            this.listViewMp3.Size = new System.Drawing.Size(360, 545);
            this.listViewMp3.TabIndex = 0;
            this.listViewMp3.UseCompatibleStateImageBehavior = false;
            this.listViewMp3.View = System.Windows.Forms.View.List;
            this.listViewMp3.SelectedIndexChanged += new System.EventHandler(this.listViewMp3_SelectedIndexChanged);
            this.listViewMp3.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewMp3_KeyUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 545);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Lettore Audiolibri";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listViewFolder;
        private System.Windows.Forms.ListView listViewMp3;
    }
}

