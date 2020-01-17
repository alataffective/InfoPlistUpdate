using System;
using System.IO;
using System.Xml;

namespace InfoPlistUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                ShowUsage();
                return;
            }

            var filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"The path {filePath} does not exist.");
                return;
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

            IncrementBundleVersion(xmlDocument);
            UpdateCopyright(xmlDocument);

            xmlDocument.Save(filePath);
        }

        static void IncrementBundleVersion(XmlDocument xmlDocument)
        {
            var valueNode = GetValueNode(xmlDocument, "CFBundleVersion");

            if (!int.TryParse(valueNode.InnerText, out var bundleVersionValue))
            {
                throw new XmlDocumentException($"Cannot parse {valueNode.InnerText} as an integer");
            }

            ++bundleVersionValue;

            valueNode.InnerText = bundleVersionValue.ToString();
        }

        static void UpdateCopyright(XmlDocument xmlDocument)
        {
            var valueNode = GetValueNode(xmlDocument, "NSHumanReadableCopyright");
            var year = DateTime.UtcNow.Year;
            var valueString = $"Copyright {year} Affective Ltd. All rights reserved.";
            valueNode.InnerText = valueString;
        }

        static XmlNode GetValueNode(XmlDocument xmlDocument, string key)
        {
            var keyNode = GetKeyNode(xmlDocument, key);
            var valueNode = keyNode.NextSibling;
            
            if (valueNode.Name != "string")
            {
                throw new XmlDocumentException($"string node expected following {key} key");
            }

            return valueNode;
        }

        static XmlNode GetKeyNode(XmlDocument xmlDocument, string key)
        {
            var keyNode = xmlDocument.SelectSingleNode($"/plist/dict/key[text()='{key}']");
            return keyNode;
        }

        static void ShowUsage()
        {
            Console.WriteLine("Usage: InfoPlistUpdate <Info.plist file path>");
        }
    }

    class XmlDocumentException : Exception
    {
        public XmlDocumentException(string s)
            : base(s)
        {
        }
    }
}
