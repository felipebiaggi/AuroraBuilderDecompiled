using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Builder.Data;
using Builder.Data.Rules;


namespace Builder.Presentation.Views.Development
{
    public static class Xtensions
    {
        public static XmlNode GenerateElementNode(this ElementBase elementBase)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode xmlNode = xmlDocument.AppendChild(xmlDocument.CreateElement("element"));
            xmlNode.Attributes.Append(xmlDocument.CreateAttribute("name")).Value = elementBase.Name;
            xmlNode.Attributes.Append(xmlDocument.CreateAttribute("type")).Value = elementBase.Type;
            xmlNode.Attributes.Append(xmlDocument.CreateAttribute("source")).Value = elementBase.Source;
            xmlNode.Attributes.Append(xmlDocument.CreateAttribute("id")).Value = elementBase.Id;
            if (elementBase.HasSupports)
            {
                xmlNode.AppendChild(xmlDocument.CreateElement("supports")).InnerText = string.Join(",", elementBase.Supports);
            }
            if (elementBase.HasDescription)
            {
                xmlNode.AppendChild(xmlDocument.CreateElement("description")).InnerXml = elementBase.Description;
            }
            if ((elementBase.SheetDescription.DisplayOnSheet && elementBase.SheetDescription.Any()) || (elementBase.SheetDescription.DisplayOnSheet && elementBase.SheetDescription.HasAlternateName))
            {
                XmlNode xmlNode2 = xmlNode.AppendChild(xmlDocument.CreateElement("sheet"));
                if (elementBase.SheetDescription.HasAlternateName)
                {
                    xmlNode2.Attributes.Append(xmlDocument.CreateAttribute("alt")).Value = elementBase.SheetDescription.AlternateName;
                }
                if (!elementBase.SheetDescription.DisplayOnSheet)
                {
                    xmlNode2.Attributes.Append(xmlDocument.CreateAttribute("display")).Value = "false";
                }
                foreach (ElementSheetDescriptions.SheetDescription item in elementBase.SheetDescription)
                {
                    XmlNode xmlNode3 = xmlNode2.AppendChild(xmlDocument.CreateElement("description"));
                    xmlNode3.InnerText = item.Description;
                    if (item.Level > 1)
                    {
                        xmlNode3.Attributes.Append(xmlDocument.CreateAttribute("level")).Value = item.Level.ToString();
                    }
                }
            }
            if (elementBase.ElementSetters.Any())
            {
                XmlNode xmlNode4 = xmlNode.AppendChild(xmlDocument.CreateElement("setters"));
                foreach (ElementSetters.Setter elementSetter in elementBase.ElementSetters)
                {
                    XmlNode xmlNode5 = xmlNode4.AppendChild(xmlDocument.CreateElement("set"));
                    xmlNode5.Attributes.Append(xmlDocument.CreateAttribute("name")).Value = elementSetter.Name;
                    xmlNode5.InnerText = elementSetter.Value;
                    foreach (KeyValuePair<string, string> additionalAttribute in elementSetter.AdditionalAttributes)
                    {
                        xmlNode5.Attributes.Append(xmlDocument.CreateAttribute(additionalAttribute.Key)).Value = additionalAttribute.Value;
                    }
                }
            }
            if (elementBase.HasRules)
            {
                XmlNode xmlNode6 = xmlNode.AppendChild(xmlDocument.CreateElement("rules"));
                foreach (RuleBase rule in elementBase.Rules)
                {
                    XmlNode xmlNode7 = xmlNode6.AppendChild(xmlDocument.CreateElement(rule.RuleName));
                    if (rule.RuleName == "grant")
                    {
                        GrantRule grantRule = rule as GrantRule;
                        xmlNode7.Attributes.Append(xmlDocument.CreateAttribute("type")).Value = grantRule.Attributes.Type;
                        xmlNode7.Attributes.Append(xmlDocument.CreateAttribute("name")).Value = grantRule.Attributes.Name;
                        xmlNode7.Attributes.Append(xmlDocument.CreateAttribute("level")).Value = grantRule.Attributes.RequiredLevel.ToString();
                    }
                }
            }
            return xmlDocument.DocumentElement;
        }

        public static string GenerateCleanOutput(this XmlNode node, string indentChars = "\t")
        {
            XmlDocument xmlDocument = new XmlDocument
            {
                InnerXml = node.OuterXml
            };
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = indentChars,
                NewLineChars = Environment.NewLine,
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter w = XmlWriter.Create(stringBuilder, settings))
            {
                xmlDocument.Save(w);
            }
            string[] array = stringBuilder.ToString().Split('\n');
            return stringBuilder.ToString().Replace(array[0] + "\n", "");
        }
    }

}
