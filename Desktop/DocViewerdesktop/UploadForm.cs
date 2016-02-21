using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocViewerdesktop.Configuration;
using DocViewerUploader;

namespace DocViewerdesktop
{
    public partial class UploadForm : Form
    {
        private static readonly UploadSection _uploadSection = new UploadSection();

        //readonly 
        public UploadForm()
        {
            InitializeComponent();
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            AddData();
        }

        public void AddData()
        {
            var fileName = Environment.CurrentDirectory + "\\TestFiles\\test.rtf";
            var nvc = new NameValueCollection {{"userId", Guid.NewGuid().ToString()},
                                               {"userName", "admin"}};
            FormUploader _formUploader = new FormUploader("http://192.168.1.29:2001/api/Document/Add");
            var result = _formUploader.Upload(fileName, nvc);
            
            ListViewItem[] p = new ListViewItem[1];
            p[0] = new ListViewItem(new string[] { "上传", fileName });
        
            this.UploadListview.Items.AddRange(p);
        }
    }
}
