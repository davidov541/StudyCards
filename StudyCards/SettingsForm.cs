using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace StudyCards
{
    public partial class Settings : Form
    {

        private XmlDocument _currconfig = new XmlDocument();

        public Settings()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _currconfig["Configuration"]["UsedStack"].InnerText = cboStack.SelectedItem.ToString().Split(' ')[0].ToLower();
            XmlTextWriter xmltw = new XmlTextWriter("AppConfig.config", Encoding.UTF32);
            _currconfig.Save(xmltw);
            xmltw.Close();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            XmlTextReader xmltr = new XmlTextReader("AppConfig.config");
            _currconfig.Load(xmltr);
            xmltr.Close();
            String currStack = _currconfig["Configuration"]["UsedStack"].InnerText;
            if (currStack.Equals("unlearned"))
            {
                cboStack.SelectedIndex = 0;
            }
            else if (currStack.Equals("learned"))
            {
                cboStack.SelectedIndex = 1;
            }
            else if (currStack.Equals("mastered"))
            {
                cboStack.SelectedIndex = 2;
            }
            else
            {
                cboStack.SelectedIndex = 3;
            }
        }
    }
}
