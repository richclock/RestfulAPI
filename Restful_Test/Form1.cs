using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



using System.ServiceModel.Web;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;


namespace Restful_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try {
                richTextBox1.Clear();
                DateTime start, end;
                TimeSpan timeSpan;
                ITRI.RestClient client = new ITRI.RestClient();
                client.Url = txtUrl.Text;


                var param = new JavaScriptSerializer().Serialize(new {
                    Action = "GetAllRecords",
                });
                param = txtParams.Text;
                start = DateTime.Now;
                var resultPost = client.HttpRequest(param);
                end = DateTime.Now;
                timeSpan = end - start;
                var ret = "Time:" + timeSpan.TotalSeconds.ToString() + " s\r\n";
                ret += "Content:" + resultPost;
                richTextBox1.Text = ret;
            }
            catch (Exception ex) {
                Console.WriteLine("Rest Client失敗：{0}\r\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try {

                ITRI.RestServer service = new ITRI.RestServer();
                WebServiceHost _serviceHost = new WebServiceHost(service, new Uri(txtServerUrl.Text));
                //WebServiceHost _serviceHost = new WebServiceHost(typeof(RestServer), new Uri("http://127.0.0.1:7788/"));
                _serviceHost.Open();

  
                //_serviceHost.Close();
            }
            catch (Exception ex) {
                MessageBox.Show("Rest Server開啟失敗");
                Console.WriteLine("Rest Server開啟失敗：{0}\r\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }


    }
}
