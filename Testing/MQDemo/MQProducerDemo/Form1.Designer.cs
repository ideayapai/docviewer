namespace MQDemoProducer
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdQueue = new System.Windows.Forms.RadioButton();
            this.rdTopic = new System.Windows.Forms.RadioButton();
            this.btnCreateProd = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTopicName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtURI = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdMsgTypeByte = new System.Windows.Forms.RadioButton();
            this.rdMsgTypeText = new System.Windows.Forms.RadioButton();
            this.lv = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.btnRemoveProperty = new System.Windows.Forms.Button();
            this.btnAddProperty = new System.Windows.Forms.Button();
            this.txtPropertyValue = new System.Windows.Forms.TextBox();
            this.txtPropertyName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.rtxSendData = new System.Windows.Forms.RichTextBox();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnTimerSend = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdQueue);
            this.groupBox2.Controls.Add(this.rdTopic);
            this.groupBox2.Controls.Add(this.btnCreateProd);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtTopicName);
            this.groupBox2.Location = new System.Drawing.Point(12, 99);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(591, 53);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Topic/Queue";
            // 
            // rdQueue
            // 
            this.rdQueue.AutoSize = true;
            this.rdQueue.Location = new System.Drawing.Point(323, 22);
            this.rdQueue.Name = "rdQueue";
            this.rdQueue.Size = new System.Drawing.Size(53, 16);
            this.rdQueue.TabIndex = 2;
            this.rdQueue.Text = "Queue";
            this.rdQueue.UseVisualStyleBackColor = true;
            // 
            // rdTopic
            // 
            this.rdTopic.AutoSize = true;
            this.rdTopic.Checked = true;
            this.rdTopic.Location = new System.Drawing.Point(251, 22);
            this.rdTopic.Name = "rdTopic";
            this.rdTopic.Size = new System.Drawing.Size(53, 16);
            this.rdTopic.TabIndex = 1;
            this.rdTopic.TabStop = true;
            this.rdTopic.Text = "Topic";
            this.rdTopic.UseVisualStyleBackColor = true;
            // 
            // btnCreateProd
            // 
            this.btnCreateProd.Location = new System.Drawing.Point(457, 20);
            this.btnCreateProd.Name = "btnCreateProd";
            this.btnCreateProd.Size = new System.Drawing.Size(111, 23);
            this.btnCreateProd.TabIndex = 3;
            this.btnCreateProd.Text = "Create Producer";
            this.btnCreateProd.UseVisualStyleBackColor = true;
            this.btnCreateProd.Click += new System.EventHandler(this.btnCreateProd_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Topic Name";
            // 
            // txtTopicName
            // 
            this.txtTopicName.Location = new System.Drawing.Point(100, 20);
            this.txtTopicName.Name = "txtTopicName";
            this.txtTopicName.Size = new System.Drawing.Size(114, 21);
            this.txtTopicName.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtURI);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(591, 81);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MQ Connection";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(457, 46);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(111, 23);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(308, 47);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(131, 21);
            this.txtPassword.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "URI";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Username";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(249, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "Password";
            // 
            // txtURI
            // 
            this.txtURI.Location = new System.Drawing.Point(100, 21);
            this.txtURI.Name = "txtURI";
            this.txtURI.Size = new System.Drawing.Size(339, 21);
            this.txtURI.TabIndex = 0;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(100, 47);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(131, 21);
            this.txtUserName.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdMsgTypeByte);
            this.groupBox3.Controls.Add(this.rdMsgTypeText);
            this.groupBox3.Controls.Add(this.lv);
            this.groupBox3.Controls.Add(this.btnRemoveProperty);
            this.groupBox3.Controls.Add(this.btnAddProperty);
            this.groupBox3.Controls.Add(this.txtPropertyValue);
            this.groupBox3.Controls.Add(this.txtPropertyName);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.rtxSendData);
            this.groupBox3.Controls.Add(this.txtInterval);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.btnStop);
            this.groupBox3.Controls.Add(this.btnSend);
            this.groupBox3.Controls.Add(this.btnTimerSend);
            this.groupBox3.Location = new System.Drawing.Point(12, 158);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(591, 268);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Send Data";
            // 
            // rdMsgTypeByte
            // 
            this.rdMsgTypeByte.AutoSize = true;
            this.rdMsgTypeByte.Location = new System.Drawing.Point(529, 129);
            this.rdMsgTypeByte.Name = "rdMsgTypeByte";
            this.rdMsgTypeByte.Size = new System.Drawing.Size(47, 16);
            this.rdMsgTypeByte.TabIndex = 11;
            this.rdMsgTypeByte.Text = "Byte";
            this.rdMsgTypeByte.UseVisualStyleBackColor = true;
            // 
            // rdMsgTypeText
            // 
            this.rdMsgTypeText.AutoSize = true;
            this.rdMsgTypeText.Checked = true;
            this.rdMsgTypeText.Location = new System.Drawing.Point(457, 129);
            this.rdMsgTypeText.Name = "rdMsgTypeText";
            this.rdMsgTypeText.Size = new System.Drawing.Size(47, 16);
            this.rdMsgTypeText.TabIndex = 10;
            this.rdMsgTypeText.TabStop = true;
            this.rdMsgTypeText.Text = "Text";
            this.rdMsgTypeText.UseVisualStyleBackColor = true;
            // 
            // lv
            // 
            this.lv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lv.FullRowSelect = true;
            this.lv.GridLines = true;
            this.lv.Location = new System.Drawing.Point(100, 47);
            this.lv.Name = "lv";
            this.lv.Size = new System.Drawing.Size(339, 98);
            this.lv.TabIndex = 3;
            this.lv.UseCompatibleStateImageBehavior = false;
            this.lv.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Property Name";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Property Value";
            this.columnHeader2.Width = 150;
            // 
            // btnRemoveProperty
            // 
            this.btnRemoveProperty.Location = new System.Drawing.Point(457, 47);
            this.btnRemoveProperty.Name = "btnRemoveProperty";
            this.btnRemoveProperty.Size = new System.Drawing.Size(111, 23);
            this.btnRemoveProperty.TabIndex = 4;
            this.btnRemoveProperty.Text = "Remove Property";
            this.btnRemoveProperty.UseVisualStyleBackColor = true;
            this.btnRemoveProperty.Click += new System.EventHandler(this.btnRemoveProperty_Click);
            // 
            // btnAddProperty
            // 
            this.btnAddProperty.Location = new System.Drawing.Point(457, 19);
            this.btnAddProperty.Name = "btnAddProperty";
            this.btnAddProperty.Size = new System.Drawing.Size(111, 23);
            this.btnAddProperty.TabIndex = 2;
            this.btnAddProperty.Text = "Add Property";
            this.btnAddProperty.UseVisualStyleBackColor = true;
            this.btnAddProperty.Click += new System.EventHandler(this.btnAddProperty_Click);
            // 
            // txtPropertyValue
            // 
            this.txtPropertyValue.Location = new System.Drawing.Point(325, 21);
            this.txtPropertyValue.Name = "txtPropertyValue";
            this.txtPropertyValue.Size = new System.Drawing.Size(114, 21);
            this.txtPropertyValue.TabIndex = 1;
            // 
            // txtPropertyName
            // 
            this.txtPropertyName.Location = new System.Drawing.Point(100, 20);
            this.txtPropertyName.Name = "txtPropertyName";
            this.txtPropertyName.Size = new System.Drawing.Size(114, 21);
            this.txtPropertyName.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(230, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 12);
            this.label7.TabIndex = 8;
            this.label7.Text = "Property Value";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(35, 153);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 12);
            this.label10.TabIndex = 6;
            this.label10.Text = "Send Data";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(29, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 6;
            this.label9.Text = "Properties";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "Property Name";
            // 
            // rtxSendData
            // 
            this.rtxSendData.Location = new System.Drawing.Point(100, 153);
            this.rtxSendData.Name = "rtxSendData";
            this.rtxSendData.Size = new System.Drawing.Size(485, 80);
            this.rtxSendData.TabIndex = 5;
            this.rtxSendData.Text = "";
            // 
            // txtInterval
            // 
            this.txtInterval.Location = new System.Drawing.Point(299, 240);
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(55, 21);
            this.txtInterval.TabIndex = 7;
            this.txtInterval.Text = "1000";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(360, 244);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "ms";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(204, 244);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "Timer Interval";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(517, 239);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(68, 23);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(100, 240);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(88, 23);
            this.btnSend.TabIndex = 6;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnTimerSend
            // 
            this.btnTimerSend.Location = new System.Drawing.Point(387, 239);
            this.btnTimerSend.Name = "btnTimerSend";
            this.btnTimerSend.Size = new System.Drawing.Size(124, 23);
            this.btnTimerSend.TabIndex = 8;
            this.btnTimerSend.Text = "Periodically Send";
            this.btnTimerSend.UseVisualStyleBackColor = true;
            this.btnTimerSend.Click += new System.EventHandler(this.btnTimerSend_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 437);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(614, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 459);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MQDemoProducer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCreateProd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTopicName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtURI;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.RichTextBox rtxSendData;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnTimerSend;
        private System.Windows.Forms.ListView lv;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnRemoveProperty;
        private System.Windows.Forms.Button btnAddProperty;
        private System.Windows.Forms.TextBox txtPropertyValue;
        private System.Windows.Forms.TextBox txtPropertyName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rdQueue;
        private System.Windows.Forms.RadioButton rdTopic;
        private System.Windows.Forms.RadioButton rdMsgTypeByte;
        private System.Windows.Forms.RadioButton rdMsgTypeText;
    }
}

