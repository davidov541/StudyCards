using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using System.Xml;

namespace StudyCards.Utilities
{
    public static class StackFuncs
    {
        public static List<Dictionary<String, String>> LoadStack(XmlDocument currdeck)
        {
            Dictionary<string, String> unlearned = new Dictionary<string, string>();
            Dictionary<string, string> learned = new Dictionary<string, string>();
            Dictionary<String, string> mastered = new Dictionary<string, string>();
            Dictionary<string, String> allwords = new Dictionary<string, string>();
            List<Dictionary<String, string>> result = new List<Dictionary<string, string>>();
            foreach (XmlNode currNode in currdeck["Stack"]["Cards"].ChildNodes)
            {
                if (currNode.Attributes["Stack"].Value.Equals("unlearned"))
                {
                    unlearned.Add(currNode["Front"].InnerText, currNode["Back"].InnerText);
                }
                else if (currNode.Attributes["Stack"].Value.Equals("learned"))
                {
                    learned.Add(currNode["Front"].InnerText, currNode["Back"].InnerText);
                }
                else
                {
                    mastered.Add(currNode["Front"].InnerText, currNode["Back"].InnerText);
                }
                allwords.Add(currNode["Front"].InnerText, currNode["Back"].InnerText);
            }
            unlearned = DictUtil.ShuffleDict(unlearned);
            learned = DictUtil.ShuffleDict(learned);
            mastered = DictUtil.ShuffleDict(mastered);
            allwords = DictUtil.ShuffleDict(allwords);
            result.Add(unlearned);
            result.Add(learned);
            result.Add(mastered);
            result.Add(allwords);
            return result;
        }

        public static XmlDocument InitStack()
        {
            XmlDocument currstackdoc = new XmlDocument();
            currstackdoc.CreateXmlDeclaration("1.0", "utf-32", "no");
            XmlElement stackNode = currstackdoc.CreateElement("Stack");
            XmlElement settingsNode = currstackdoc.CreateElement("Settings");
            XmlElement cardsNode = currstackdoc.CreateElement("Cards");
            XmlElement frontCardElement = currstackdoc.CreateElement("TopCard");
            XmlElement backCardElement = currstackdoc.CreateElement("BottomCard");
            XmlElement frontLanguage = currstackdoc.CreateElement("Language");
            XmlElement backLanguage = currstackdoc.CreateElement("Language");
            stackNode.SetAttribute("xmlns", "http://tempuri.org/CardData.xsd");
            frontLanguage.InnerText = cboFrontLanguage.SelectedText;
            backLanguage.InnerText = cboBackLanguage.SelectedText;
            frontCardElement.AppendChild(frontLanguage);
            backCardElement.AppendChild(backLanguage);
            settingsNode.AppendChild(frontCardElement);
            settingsNode.AppendChild(backCardElement);
            currstackdoc.AppendChild(stackNode);
            currstackdoc["Stack"].AppendChild(settingsNode);
            currstackdoc["Stack"].AppendChild(cardsNode);
            return currstackdoc;
        }

        public static List<String> GetLanguages()
        {
            List<String> result = new List<string>();
            XmlTextReader xmltr = new XmlTextReader("Languages.config");
            XMLDocument currlangdoc = new XmlDocument();
            currlangdoc.Load(xmltr);
            foreach (XmlNode currNode in currlangdoc["Languages"].ChildNodes)
            {
                result.Add(currNode.InnerText);
            }
            xmltr.Close();
        }
    }
}
