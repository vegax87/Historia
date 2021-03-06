﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Historia
{
    public class HTMLWriter
    {
        public string docPath { get; private set; }
        private HtmlDocument doc;

        private static string packetsPath;
        private static string includesPath;
        private static readonly List<string> includes = new List<string> { "Template.css", "Template.html" };
        private static readonly string executableDir = AppDomain.CurrentDomain.BaseDirectory;

        public HTMLWriter() : this(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".html") {}

        public HTMLWriter(string filename)
        {
            try
            {
                Console.WriteLine("[HTMLWriter] Initializing output directories.");
                packetsPath = Path.Combine(executableDir, "packets");
                includesPath = Path.Combine(packetsPath, "includes");

                Directory.CreateDirectory(packetsPath);
                Directory.CreateDirectory(includesPath);
                foreach (var include in includes)
                    File.Copy(Path.Combine(executableDir, include), Path.Combine(includesPath, include), true);

                doc = new HtmlDocument();
                doc.Load(Path.Combine(includesPath, "Template.html"));
                this.docPath = Path.Combine(packetsPath, filename);
                doc.Save(this.docPath);
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Append(Op.Opcode opcode, byte[] raw, string direction, int size)
        {
            try
            {
                var header = opcode.header.ToString("X4");
                string hex = ByteHelper.ConvertToHex(raw);

                var ascii = Encoding.ASCII.GetString(raw);
                ascii = Regex.Replace(ascii, @"[^\u0020-\u007E]", ".");

                var nodeText = string.Format(@"<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>", opcode.name, direction, header, size, hex, ascii);
                var node = doc.DocumentNode.SelectSingleNode("//tbody");
                node.AppendChild(HtmlNode.CreateNode(nodeText));
                doc.Save(this.docPath);
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

    }
}
