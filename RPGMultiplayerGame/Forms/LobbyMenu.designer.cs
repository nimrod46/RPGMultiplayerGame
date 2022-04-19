namespace RPGMultiplayerGame.Forms
{
    partial class LobbyMenu
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
            this.LobbyList = new System.Windows.Forms.ListView();
            this.Connect = new System.Windows.Forms.Button();
            this.AddServer = new System.Windows.Forms.Button();
            this.Refresh = new System.Windows.Forms.Button();
            this.Remove = new System.Windows.Forms.Button();
            this.IpAdress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Ping = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.StartServer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LobbyList
            // 
            this.LobbyList.AccessibleRole = System.Windows.Forms.AccessibleRole.Grip;
            this.LobbyList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IpAdress,
            this.Status,
            this.Ping});
            this.LobbyList.FullRowSelect = true;
            this.LobbyList.GridLines = true;
            this.LobbyList.Location = new System.Drawing.Point(12, 12);
            this.LobbyList.Name = "LobbyList";
            this.LobbyList.Size = new System.Drawing.Size(417, 426);
            this.LobbyList.TabIndex = 0;
            this.LobbyList.UseCompatibleStateImageBehavior = false;
            this.LobbyList.View = System.Windows.Forms.View.Details;
            // 
            // Connect
            // 
            this.Connect.Location = new System.Drawing.Point(435, 12);
            this.Connect.Name = "Connect";
            this.Connect.Size = new System.Drawing.Size(75, 23);
            this.Connect.TabIndex = 1;
            this.Connect.Text = "Connect";
            this.Connect.UseVisualStyleBackColor = true;
            this.Connect.Click += new System.EventHandler(this.Connect_Click);
            // 
            // AddServer
            // 
            this.AddServer.Location = new System.Drawing.Point(435, 41);
            this.AddServer.Name = "AddServer";
            this.AddServer.Size = new System.Drawing.Size(75, 23);
            this.AddServer.TabIndex = 2;
            this.AddServer.Text = "Add server";
            this.AddServer.UseVisualStyleBackColor = true;
            this.AddServer.Click += new System.EventHandler(this.AddServer_Click);
            // 
            // Refresh
            // 
            this.Refresh.Location = new System.Drawing.Point(435, 70);
            this.Refresh.Name = "Refresh";
            this.Refresh.Size = new System.Drawing.Size(75, 23);
            this.Refresh.TabIndex = 3;
            this.Refresh.Text = "Refresh";
            this.Refresh.UseVisualStyleBackColor = true;
            this.Refresh.Click += new System.EventHandler(this.Refresh_Click);
            // 
            // Remove
            // 
            this.Remove.Location = new System.Drawing.Point(435, 99);
            this.Remove.Name = "Remove";
            this.Remove.Size = new System.Drawing.Size(75, 23);
            this.Remove.TabIndex = 4;
            this.Remove.Text = "Remove";
            this.Remove.UseVisualStyleBackColor = true;
            this.Remove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // IpAdress
            // 
            this.IpAdress.Text = "Ip adress";
            this.IpAdress.Width = 134;
            // 
            // Status
            // 
            this.Status.Text = "Status";
            this.Status.Width = 76;
            // 
            // Ping
            // 
            this.Ping.Text = "Ping";
            this.Ping.Width = 42;
            // 
            // StartServer
            // 
            this.StartServer.Location = new System.Drawing.Point(516, 12);
            this.StartServer.Name = "StartServer";
            this.StartServer.Size = new System.Drawing.Size(75, 23);
            this.StartServer.TabIndex = 5;
            this.StartServer.Text = "Start server";
            this.StartServer.UseVisualStyleBackColor = true;
            this.StartServer.Click += new System.EventHandler(this.StartServer_Click);
            // 
            // LobbyMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.StartServer);
            this.Controls.Add(this.Remove);
            this.Controls.Add(this.Refresh);
            this.Controls.Add(this.AddServer);
            this.Controls.Add(this.Connect);
            this.Controls.Add(this.LobbyList);
            this.Name = "LobbyMenu";
            this.Text = "Lobby";
            this.Shown += new System.EventHandler(this.LobbyMenu_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView LobbyList;
        private System.Windows.Forms.Button Connect;
        private System.Windows.Forms.Button AddServer;
        private new System.Windows.Forms.Button Refresh;
        private System.Windows.Forms.Button Remove;
        private System.Windows.Forms.ColumnHeader IpAdress;
        private System.Windows.Forms.ColumnHeader Status;
        private System.Windows.Forms.ColumnHeader Ping;
        private System.Windows.Forms.Button StartServer;
    }
}