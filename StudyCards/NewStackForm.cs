using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using StudyCards.Utilities;

namespace StudyCards
{
    public partial class NewStackForm : Form
    {
        private XmlDocument _currstackdoc = new XmlDocument();
        private XmlDocument _currlangdoc = new XmlDocument();

        public NewStackForm()
        {
            InitializeComponent();
        }

        private void NewStackForm_Load(object sender, EventArgs e)
        {
            List<String> languages = StackFuncs.GetLanguages();
            cboBackLanguage.Items.AddRange(languages);
            cboFrontLanguage.Items.AddRange(languages);
            cboFrontLanguage.SelectedIndex = 0;
            cboBackLanguage.SelectedIndex = 0;
            _currstackdoc = StackFuncs.InitStack();
        }

        private void NewStackForm_FormClosing(Object sender, EventArgs e)
        {
            this.Tag = "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Tag = "";
            this.Close();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            FileInfo fi = new FileInfo(txtDir.Text);
            if (String.IsNullOrEmpty(fi.FullName) || !Directory.Exists(fi.Directory.ToString()))
            {
                MessageBox.Show("Directory given not valid.", "Invalid Directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                UpdateXml();
                XmlTextWriter xmltw = new XmlTextWriter(txtDir.Text, Encoding.UTF32);
                _currstackdoc.Save(xmltw);
                xmltw.Close();
                UpdateLanguages();
                this.Tag = txtDir.Text;
                this.Close();
            }
        }

        private void btnGetFileUrl_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = false;
            sfd.AutoUpgradeEnabled = true;
            sfd.DefaultExt = ".scd";
            sfd.Title = "Set Stack Location";
            sfd.ShowDialog();
            txtDir.Text = sfd.FileName;
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            UpdateXml();
            foreach (Control ctrl in pFront.Controls)
            {
                if (typeof(TextBox) == ctrl.GetType())
                {
                    ctrl.Text = "";
                    pBack.Controls.Find(ctrl.Tag.ToString(), true)[0].Text = "";
                }
            }
        }

        private void UpdateXml()
        {
            _currstackdoc["Stack"]["Settings"]["TopCard"]["Language"].InnerText = cboFrontLanguage.SelectedItem.ToString();
            _currstackdoc["Stack"]["Settings"]["BottomCard"]["Language"].InnerText = cboBackLanguage.SelectedItem.ToString();
            foreach (Control ctrl in pFront.Controls)
            {
                if (typeof(TextBox) == ctrl.GetType())
                {
                    if (!String.IsNullOrEmpty(((TextBox)ctrl).Text))
                    {
                        XmlElement cardNode = _currstackdoc.CreateElement("Card");
                        XmlElement frontNode = _currstackdoc.CreateElement("Front");
                        XmlElement backNode = _currstackdoc.CreateElement("Back");
                        cardNode.SetAttribute("Stack", "unlearned");
                        frontNode.InnerText = ((TextBox)ctrl).Text;
                        backNode.InnerText = pBack.Controls.Find(ctrl.Tag.ToString(), true)[0].Text;
                        cardNode.AppendChild(frontNode);
                        cardNode.AppendChild(backNode);
                        _currstackdoc["Stack"]["Cards"].AppendChild(cardNode);
                    }
                }
            }
        }

        private void UpdateLanguages()
        {
            if (!cboFrontLanguage.Items.Contains(cboFrontLanguage.SelectedText))
            {
                XmlTextWriter xmltw = new XmlTextWriter("Languages.config", Encoding.UTF32);
                XmlElement newLang = _currlangdoc.CreateElement("Language");
                newLang.InnerText = cboFrontLanguage.SelectedItem.ToString();
                _currlangdoc["Languages"].AppendChild(newLang);
                _currlangdoc.Save(xmltw);
                xmltw.Close();
            }
            else if (!cboBackLanguage.Items.Contains(cboBackLanguage.SelectedText))
            {
                XmlTextWriter xmltw = new XmlTextWriter("Languages.config", Encoding.UTF32);
                XmlElement newLang = _currlangdoc.CreateElement("Language");
                newLang.InnerText = cboBackLanguage.SelectedItem.ToString();
                _currlangdoc["Languages"].AppendChild(newLang);
                _currlangdoc.Save(xmltw);
                xmltw.Close();
            }
        }
    }
}
