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

            IncrementBundleVersion(xmlDocument, filePath);
        }

        static void IncrementBundleVersion(XmlDocument xmlDocument, string filePath)
        {
            var bundleVersionKeyNode = xmlDocument.SelectSingleNode("/plist/dict/key[text()='CFBundleVersion']");
            var bundleVersionValueNode = bundleVersionKeyNode.NextSibling;
            if (bundleVersionValueNode.Name != "string")
            {
                Console.WriteLine("string node expected following CFBundleVersion key");
            }

            if (!int.TryParse(bundleVersionValueNode.InnerText, out var bundleVersionValue))
            {
                Console.WriteLine($"Cannot parse {bundleVersionValueNode.InnerText} as an integer");
            }

            ++bundleVersionValue;

            bundleVersionValueNode.InnerText = bundleVersionValue.ToString();

            xmlDocument.Save(filePath);
        }

        static void ShowUsage()
        {
            Console.WriteLine("Usage: InfoPlistUpdate <Info.plist file path>");
        }
    }
}
