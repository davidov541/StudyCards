using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Utilities;
using StudyCards.Utilities;

namespace StudyCards
{
    public partial class frmMain : Form
    {
        private String _dir;
        private Dictionary<String, String> _unlearned = new Dictionary<string, string>();
        private Dictionary<String, String> _learned = new Dictionary<string, string>();
        private Dictionary<String, String> _mastered = new Dictionary<string, string>();
        private Dictionary<String, String> _allwords = new Dictionary<string,string>();
        private int _currcard = 0;
        private Boolean _top = true;
        private XmlDocument _currconfig = new XmlDocument();
        private XmlDocument _currdeck = new XmlDocument();
        private String _usedstack = null;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            GetDefaultDeck();
            if (_dir != null)
            {
                GetDeck();
                ShowCard();
            }
            else
            {
                lblWord.Text = "";
                lblStatus.Text = "";
            }
        }

        private void frmMain_FormClosing(Object sender, EventArgs e)
        {
            XmlTextWriter xmltw = new XmlTextWriter(_dir, Encoding.UTF32);
            _currdeck.Save(xmltw);
            xmltw.Close();
            Application.Exit();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            ChangeCard(true);
        }

        private void btnFlip_Click(object sender, EventArgs e)
        {
            if (_usedstack.Equals("unlearned"))
            {
                if (_top)
                {
                    lblWord.Text = _unlearned.Values.ToList<String>()[_currcard];
                    _top = false;
                }
                else
                {
                    lblWord.Text = _unlearned.Keys.ToList<String>()[_currcard];
                    _top = true;
                }
            }
            else if (_usedstack.Equals("learned"))
            {
                if (_top)
                {
                    lblWord.Text = _learned.Values.ToList<String>()[_currcard];
                    _top = false;
                }
                else
                {
                    lblWord.Text = _learned.Keys.ToList<String>()[_currcard];
                    _top = true;
                }
            }
            else if (_usedstack.Equals("mastered"))
            {
                if (_top)
                {
                    lblWord.Text = _mastered.Values.ToList<String>()[_currcard];
                    _top = false;
                }
                else
                {
                    lblWord.Text = _mastered.Keys.ToList<String>()[_currcard];
                    _top = true;
                }
            }
            else
            {
                if (_top)
                {
                    lblWord.Text = _allwords.Values.ToList<String>()[_currcard];
                    _top = false;
                }
                else
                {
                    lblWord.Text = _allwords.Keys.ToList<String>()[_currcard];
                    _top = true;
                }
            }
        }

        private void btnMastered_Click(object sender, EventArgs e)
        {
            if (btnMastered.Text.Contains("Learned"))
            {
                WordLearned();
            }
            else if (btnMastered.Text.Contains("Mastered"))
            {
                WordMastered();
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            ChangeCard(false);
        }

        private void tsmiNew_Click(object sender, EventArgs e)
        {
            NewStackForm nsf = new NewStackForm();
            nsf.ShowDialog();
            if (!(nsf == null) && !String.IsNullOrEmpty(nsf.Tag.ToString()))
            {
                _dir = nsf.Tag.ToString();
                _currcard = 0;
                ChangeDefaultDeck();
                GetDeck();
                ShowCard();
            }
        }

        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            OpenNewDeck();
        }

        private void tsmiEdit_Click(object sender, EventArgs e)
        {
            frmEdit editForm = new frmEdit();
            editForm.Tag = _dir;
            editForm.ShowDialog();
            if (!(editForm == null) && String.IsNullOrEmpty(editForm.Tag.ToString()))
            {
                _currcard = 0;
                GetDeck();
                ShowCard();
            }
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            XmlTextWriter xmltw = new XmlTextWriter(_dir, Encoding.UTF32);
            _currdeck.Save(xmltw);
            xmltw.Close();
            Application.Exit();
        }

        private void tsmiSettings_Click(object sender, EventArgs e)
        {
            Settings settingsForm = new Settings();
            settingsForm.ShowDialog();
            XmlTextReader xmltr = new XmlTextReader("AppConfig.config");
            _currconfig.Load(xmltr);
            String used = _currconfig["Configuration"]["UsedStack"].InnerText;
            if (!used.Equals(_usedstack))
            {
                this._currcard = 0;
                this._usedstack = used;
                this._top = true;
                ShowCard();
            }
            xmltr.Close();
        }

        private void ChangeCard(Boolean next)
        {
            if (_usedstack.Equals("unlearned"))
            {
                if (next)
                {
                    if (++_currcard >= _unlearned.Count)
                    {
                        _currcard = 0;
                    }
                }
                else
                {
                    if (--_currcard < 0)
                    {
                        _currcard = _unlearned.Count - 1;
                    }
                }
            }
            else if (_usedstack.Equals("learned"))
            {
                if (next)
                {
                    if (++_currcard >= _learned.Count)
                    {
                        _currcard = 0;
                    }
                }
                else
                {
                    if (--_currcard < 0)
                    {
                        _currcard = _learned.Count - 1;
                    }
                }
            }
            else if (_usedstack.Equals("mastered"))
            {
                if (next)
                {
                    if (++_currcard >= _mastered.Count)
                    {
                        _currcard = 0;
                    }
                }
                else
                {
                    if (--_currcard < 0)
                    {
                        _currcard = _mastered.Count - 1;
                    }
                }
            }
            else
            {
                if (next)
                {
                    if (++_currcard >= _allwords.Count)
                    {
                        _currcard = 0;
                    }
                }
                else
                {
                    if (--_currcard < 0)
                    {
                        _currcard = _allwords.Count - 1;
                    }
                }
            }
            _top = true;
            ShowCard();
        }

        private void OpenNewDeck()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            ofd.Title = "Open Deck";
            FileInfo fi = new FileInfo(_dir);
            ofd.InitialDirectory = fi.DirectoryName;
            ofd.Filter = "Study Card Stacks|*.scd";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _dir = ofd.FileName;
                GetDeck();
                _currcard = 0;
            }
            ChangeDefaultDeck();
            ShowCard();
        }

        private void GetDefaultDeck()
        {
            XmlTextReader xmltr = new XmlTextReader("AppConfig.config");
            _currconfig.Load(xmltr);
            _dir = _currconfig["Configuration"]["DefaultDeck"].InnerText;
            _usedstack = _currconfig["Configuration"]["UsedStack"].InnerText;
            xmltr.Close();
        }

        private void ChangeDefaultDeck()
        {
            _currconfig["Configuration"]["DefaultDeck"].InnerText = _dir;
            XmlTextWriter xmltw = new XmlTextWriter("AppConfig.config", Encoding.UTF32);
            _currconfig.Save(xmltw);
            xmltw.Close();
        }

        private void GetDeck()
        {
            _unlearned.Clear();
            _mastered.Clear();
            _allwords.Clear();
            _currdeck.Load(_dir);
            List<Dictionary<String, String>> decks = StackFuncs.LoadStack(_currdeck);
            _unlearned = decks[0];
            _learned = decks[1];
            _mastered = decks[2];
            _allwords = decks[3];
        }

        private void WordLearned()
        {
            String selectedKey = null;
            String selectedVal = null;
            if (_usedstack.Equals("all"))
            {
                selectedKey = _allwords.Keys.ToList<String>()[_currcard];
                selectedVal = _allwords.Values.ToList<String>()[_currcard];
            }
            else
            {
                this._top = true;
                selectedKey = _unlearned.Keys.ToList<String>()[_currcard];
                selectedVal = _unlearned.Values.ToList<String>()[_currcard];
            }
            _learned.Add(selectedKey, selectedVal);
            _unlearned.Remove(selectedKey);
            if (_currcard >= _unlearned.Count)
            {
                _currcard = 0;
            }
            foreach (XmlElement currElement in _currdeck["Stack"]["Cards"].ChildNodes)
            {
                if (currElement["Front"].InnerText.Equals(selectedKey))
                {
                    currElement.SetAttribute("Stack", "learned");
                    continue;
                }
            }
            ShowCard();
        }

        private void WordMastered()
        {
            String selectedKey = null;
            String selectedVal = null;
            if (_usedstack.Equals("all"))
            {
                selectedKey = _allwords.Keys.ToList<String>()[_currcard];
                selectedVal = _allwords.Values.ToList<String>()[_currcard];
            }
            else
            {
                this._top = true;
                selectedKey = _learned.Keys.ToList<String>()[_currcard];
                selectedVal = _learned.Values.ToList<String>()[_currcard];
            }
            _mastered.Add(selectedKey, selectedVal);
            _learned.Remove(selectedKey);
            if (_currcard >= _learned.Count)
            {
                _currcard = 0;
            }
            foreach (XmlElement currElement in _currdeck["Stack"]["Cards"].ChildNodes)
            {
                if (currElement["Front"].InnerText.Equals(selectedKey))
                {
                    currElement.SetAttribute("Stack", "mastered");
                    continue;
                }
            }
            ShowCard();
        }

        private void ShowCard()
        {
            if (!_top)
            {
                if (_usedstack.Equals("unlearned"))
                {
                    btnMastered.Text = "Card Learned";
                    btnMastered.Enabled = true;
                    if (_unlearned.Count == 0)
                    {
                        lblWord.Text = "";
                        lblStatus.Text = "No unlearned words in deck.";
                    }
                    else
                    {
                        lblWord.Text = _unlearned.Values.ToList<String>()[_currcard];
                        lblWord.ForeColor = Color.Red;
                        lblStatus.Text = "Word " + (_currcard + 1) + "/" + _unlearned.Count;
                    }
                }
                else if (_usedstack.Equals("learned"))
                {
                    btnMastered.Text = "Card Mastered";
                    btnMastered.Enabled = true;
                    if (_learned.Count == 0)
                    {
                        lblWord.Text = "";
                        lblStatus.Text = "No learned words in deck.";
                    }
                    else
                    {
                        lblWord.Text = _learned.Values.ToList<String>()[_currcard];
                        lblWord.ForeColor = Color.Orange;
                        lblStatus.Text = "Word " + (_currcard + 1) + "/" + _learned.Count;
                    }
                }
                else if (_usedstack.Equals("mastered"))
                {
                    btnMastered.Text = "";
                    btnMastered.Enabled = false;
                    if (_mastered.Count == 0)
                    {
                        lblWord.Text = "";
                        lblStatus.Text = "No mastered words in deck.";
                    }
                    else
                    {
                        lblWord.Text = _mastered.Values.ToList<String>()[_currcard];
                        lblWord.ForeColor = Color.Blue;
                        lblStatus.Text = "Word " + (_currcard + 1) + "/" + _mastered.Count;
                    }
                }
                else
                {
                    lblWord.Text = _allwords.Values.ToList<String>()[_currcard];
                    if (_unlearned.Count != 0 && _unlearned.ContainsValue(lblWord.Text))
                    {
                        btnMastered.Text = "Card Learned";
                        btnMastered.Enabled = true;
                        lblWord.ForeColor = Color.Red;
                    }
                    else if (_learned.Count != 0 && _learned.ContainsValue(lblWord.Text))
                    {
                        btnMastered.Text = "Card Mastered";
                        btnMastered.Enabled = true;
                        lblWord.ForeColor = Color.Orange;
                    }
                    else if (_mastered.Count != 0 && _mastered.ContainsValue(lblWord.Text))
                    {
                        btnMastered.Text = "";
                        btnMastered.Enabled = false;
                        lblWord.ForeColor = Color.Blue;
                    }
                    lblStatus.Text = "Word " + (_currcard + 1) + "/" + _allwords.Count;
                }
                _top = false;
            }
            else
            {
                if (_usedstack.Equals("unlearned"))
                {
                    btnMastered.Text = "Card Learned";
                    btnMastered.Enabled = true;
                    if (_unlearned.Count == 0)
                    {
                        lblWord.Text = "";
                        lblStatus.Text = "No unlearned words in deck.";
                    }
                    else
                    {
                        lblWord.Text = _unlearned.Keys.ToList<String>()[_currcard];
                        lblWord.ForeColor = Color.Red;
                        lblStatus.Text = "Word " + (_currcard + 1) + "/" + _unlearned.Count;
                    }
                }
                else if (_usedstack.Equals("learned"))
                {
                    btnMastered.Text = "Card Mastered";
                    btnMastered.Enabled = true;
                    if (_learned.Count == 0)
                    {
                        lblWord.Text = "";
                        lblStatus.Text = "No learned words in deck.";
                    }
                    else
                    {
                        lblWord.Text = _learned.Keys.ToList<String>()[_currcard];
                        lblWord.ForeColor = Color.Orange;
                        lblStatus.Text = "Word " + (_currcard + 1) + "/" + _learned.Count;
                    }
                }
                else if (_usedstack.Equals("mastered"))
                {
                    btnMastered.Text = "";
                    btnMastered.Enabled = false;
                    if (_mastered.Count == 0)
                    {
                        lblWord.Text = "";
                        lblStatus.Text = "No mastered words in deck.";
                    }
                    else
                    {
                        lblWord.Text = _mastered.Keys.ToList<String>()[_currcard];
                        lblWord.ForeColor = Color.Blue;
                        lblStatus.Text = "Word " + (_currcard + 1) + "/" + _mastered.Count;
                    }
                }
                else
                {
                    lblWord.Text = _allwords.Keys.ToList<string>()[_currcard];
                    if (_unlearned.Count != 0 && _unlearned.ContainsKey(lblWord.Text))
                    {
                        btnMastered.Text = "Card Learned";
                        btnMastered.Enabled = true;
                        lblWord.ForeColor = Color.Red;
                    }
                    else if (_learned.Count != 0 && _learned.ContainsKey(lblWord.Text))
                    {
                        btnMastered.Text = "Card Mastered";
                        btnMastered.Enabled = true;
                        lblWord.ForeColor = Color.Orange;
                    }
                    else if (_mastered.Count != 0 && _mastered.ContainsKey(lblWord.Text))
                    {
                        btnMastered.Text = "";
                        btnMastered.Enabled = false;
                        lblWord.ForeColor = Color.Blue;
                    }
                    lblStatus.Text = "Word " + (_currcard + 1) + "/" + _allwords.Count;
                }
                _top = true;
            }
        }
    }
}
