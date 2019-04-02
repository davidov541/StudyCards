using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StudyCards.Utilities;
using System.Xml;

namespace StudyCards
{
    public partial class frmEdit : Form
    {

        private Dictionary<String, String> _currdeck = new Dictionary<string, string>();
        private XmlDocument _currdoc = new XmlDocument();

        public frmEdit()
        {
            InitializeComponent();
        }

        private void frmEdit_Load(object sender, EventArgs e)
        {
            _currdoc.Load(this.Tag.ToString());
            _currdeck = StackFuncs.LoadStack(_currdoc)[3];
            LoadCurrentCards(0);
            List<String> languages = StackFuncs.GetLanguages();
            cboBackLanguage.Items.AddRange(languages);
            cboFrontLanguage.Items.AddRange(languages);
            cboFrontLanguage.SelectedIndex = 0;
            cboBackLanguage.SelectedIndex = 0;
        }

        private void LoadCurrentCards(int start)
        {
            int i = start;
            foreach (Control ctrl in pFront)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    ctrl.Text = _currdeck.Keys[i];
                    pBack.Controls.Find(ctrl.Tag.ToString(), true)[0].Text = _currdeck.Values[i];
                    i++;
                }
            }
        }
    }
}
