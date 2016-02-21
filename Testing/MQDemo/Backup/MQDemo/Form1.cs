using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace MQDemoSubscriber
{
    public partial class Form1 : Form
    {
        private MQ m_mq;
        private IMessageConsumer m_consumer;
        private delegate void ShowNoteMsgDelegate(string msg);

        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalFunction.CheckControlInput(txtURI, "URI", 0, false)) return;

                m_mq = new MQ();

                m_mq.uri = txtURI.Text;
                m_mq.username = txtUserName.Text;
                m_mq.password = txtPassword.Text;

                m_mq.Start();
            }
            catch (System.Exception ex)
            {
                GlobalFunction.MsgBoxException(ex.Message, "btnConnect_Click");
            }
        }

        private void btnSubscribe_Click(object sender, EventArgs e)
        {
            try
            {
                if (!GlobalFunction.CheckControlInput(txtTopicName, "Topic Name", 0, false)) return;

                if (m_consumer != null)
                {
                    m_consumer.Close();
                }
                if (txtSelector.Text != "")
                {
                    m_consumer = m_mq.CreateConsumer(rdTopic.Checked, txtTopicName.Text, txtSelector.Text);
                }
                else
                {
                    m_consumer = m_mq.CreateConsumer(rdTopic.Checked, txtTopicName.Text);
                }

                m_consumer.Listener += new MessageListener(consumer_listener);
            }
            catch (System.Exception ex)
            {
                GlobalFunction.MsgBoxException(ex.Message, "btnSubscribe_Click");
            }
        }

        private void consumer_listener(IMessage message)
        {
            string strMsg;

            try
            {
                if (rdMsgTypeText.Checked)
                {
                    ITextMessage msg = (ITextMessage)message;
                    strMsg = msg.Text;
                }
                else
                {
                    IBytesMessage msg = (IBytesMessage)message;
                    strMsg = Encoding.Default.GetString(msg.Content);
                }

                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new ShowNoteMsgDelegate(ShowMessage), strMsg);
                }
                
            }
            catch (System.Exception ex)
            {
                m_consumer.Close();
                GlobalFunction.MsgBoxException(ex.Message, "consumer_listener");
            }
        }

        private void ShowMessage(string msg)
        {
            string strText = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]:" + msg + "\r\n" + rtxData.Text;
            if (strText.Length > 10000)
            {
                rtxData.Text = strText.Substring(0, 10000);
            }
            else
            {
                rtxData.Text = strText;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (m_consumer != null)
            {
                m_consumer.Close();
            }
        }
    }
}
