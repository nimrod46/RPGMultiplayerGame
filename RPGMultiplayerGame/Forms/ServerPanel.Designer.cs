namespace RPGMultiplayerGame
{
    partial class ServerPanel
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
            this.LoadMap = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.LoadSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LoadMap
            // 
            this.LoadMap.Location = new System.Drawing.Point(12, 12);
            this.LoadMap.Name = "LoadMap";
            this.LoadMap.Size = new System.Drawing.Size(75, 23);
            this.LoadMap.TabIndex = 0;
            this.LoadMap.Text = "Load map";
            this.LoadMap.UseVisualStyleBackColor = true;
            this.LoadMap.Click += new System.EventHandler(this.LoadMap_Click);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(12, 41);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 1;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // LoadSave
            // 
            this.LoadSave.Location = new System.Drawing.Point(12, 70);
            this.LoadSave.Name = "LoadSave";
            this.LoadSave.Size = new System.Drawing.Size(75, 23);
            this.LoadSave.TabIndex = 2;
            this.LoadSave.Text = "Load";
            this.LoadSave.UseVisualStyleBackColor = true;
            this.LoadSave.Click += new System.EventHandler(this.LoadSave_Click);
            // 
            // ServerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 373);
            this.Controls.Add(this.LoadSave);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.LoadMap);
            this.Name = "ServerPanel";
            this.Text = "ServerPanel";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LoadMap;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button LoadSave;
    }
}