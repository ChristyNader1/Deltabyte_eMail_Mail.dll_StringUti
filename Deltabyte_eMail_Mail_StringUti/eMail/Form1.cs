using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace eMail
{
    public partial class MailForm : Form
    {
        public MailForm()
        {
            InitializeComponent();
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            tbMail.Text = String.Empty;
            tbMail.Lines = new String[] { };
        }

        private void btSend_Click(object sender, EventArgs e)
        {
            using (Deltabyte.Mail.Data m = new Deltabyte.Mail.Data())
            {
                m.Server = "mail.hostedoffice.ag";
                m.User = "t.rebel@acquifer.de";
                m.Password = "18.April";
                m.From = "t.rebel@acquifer.de";
                m.To = "t.rebel@acquifer.de";
                //m.To = "hess@deltabyte.de";
                m.Subject = "MailClassTest";
                m.Body = tbMail.Text;
                //Boolean b = m.Send("Bildtest", null, null, @"C:\Users\rebel\Pictures\danger\bomb.png");
                Boolean b = m.Send(true);
                if (b == false) MessageBox.Show(m.Text);
            }
        }
        private void Substitute()
        {
            try
            {
                using (StreamReader sr = new StreamReader(@".\Acquifer_Mail_Template-html"))
                {
                    String s = sr.ReadToEnd();
                    String c = "We would like you to inform about all Placeholder which are available at your HIVE. To find out more, please see below.";
                    List<Tuple<String, String>> subs = new List<Tuple<string, string>>();
                    subs.Add(new Tuple<string, string>("#USER#", Environment.UserName));
                    subs.Add(new Tuple<string, string>("#HOSTNAME#", Environment.MachineName));
                    subs.Add(new Tuple<string, string>("#DATETIME#", DateTime.Now.ToString()));
                    subs.Add(new Tuple<string, string>("#ERRNO#", "0"));
                    subs.Add(new Tuple<string, string>("#IPADRESSV4NR1#", "192.168.2.18"));
                    subs.Add(new Tuple<string, string>("#IPADRESSV4NR2#", "na"));
                    subs.Add(new Tuple<string, string>("#IPADRESSV4NR3#", "na"));
                    subs.Add(new Tuple<string, string>("#IPADRESSV4NR4#", "na"));
                    subs.Add(new Tuple<string, string>("#IPADRESSV4NR5#", "na"));
                    subs.Add(new Tuple<string, string>("#IPADRESSV4NR6#", "na"));
                    subs.Add(new Tuple<string, string>("#IPADRESSV6NR1#", "fe80::7535:6a43: b77a:1000"));
                    subs.Add(new Tuple<string, string>("#IPADRESSV6NR2#", "na"));
                    subs.Add(new Tuple<string, string>("#IPADRESSV6NR3#", "na"));
                    subs.Add(new Tuple<string, string>("#IPADRESSV6NR4#", "na"));
                    subs.Add(new Tuple<string, string>("#IPADRESSV6NR5#", "na"));
                    subs.Add(new Tuple<string, string>("#IPADRESSV6NR6#", "na"));
                    subs.Add(new Tuple<string, string>("#TITLE#", "This is a test email"));
                    subs.Add(new Tuple<string, string>("#CONTENT#", c));
                    subs.Add(new Tuple<string, string>("#APPEND#", "Appendix"));
                    tbMail.Text = Deltabyte.Util.Text.Strings.Replace(s, subs);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void MailForm_Load(object sender, EventArgs e)
        {
            Substitute();
        }
    }
}
