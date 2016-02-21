namespace DocViewerdesktop
{
    partial class UploadForm
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
            this.UploadListview = new System.Windows.Forms.ListView();
            this.Action = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Path = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UploadListview
            // 
            this.UploadListview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Action,
            this.Path});
            this.UploadListview.FullRowSelect = true;
            this.UploadListview.Location = new System.Drawing.Point(12, 12);
            this.UploadListview.Name = "UploadListview";
            this.UploadListview.Size = new System.Drawing.Size(516, 344);
            this.UploadListview.TabIndex = 0;
            this.UploadListview.UseCompatibleStateImageBehavior = false;
            this.UploadListview.View = System.Windows.Forms.View.Details;
            // 
            // Action
            // 
            this.Action.Text = "操作";
            // 
            // Path
            // 
            this.Path.Text = "路径";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(372, 376);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(453, 376);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // UploadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 411);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.UploadListview);
            this.Name = "UploadForm";
            this.Text = "文档管理系统";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView UploadListview;
        private System.Windows.Forms.ColumnHeader Action;
        private System.Windows.Forms.ColumnHeader Path;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}

