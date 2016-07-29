using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace MyTool
{
    internal class XmlHelper
    {
        private static readonly XmlHelper xmlHelper;
        public static readonly FindChangeSetInfoClass FindChangeSetInfo;
        public static readonly OpenIccIncidentInfoClass OpenIccIncidentInfo;
        public static readonly OpenEverythingInfoClass OpenEverythingInfo;
        public static readonly ClipboardHelperInfoClass ClipboardHelperInfo;

        static XmlHelper()
        {
            xmlHelper = new XmlHelper();
            FindChangeSetInfo = new FindChangeSetInfoClass(xmlHelper._root.SelectSingleNode("FindChangeSet"));
            OpenIccIncidentInfo = new OpenIccIncidentInfoClass(xmlHelper._root.SelectSingleNode("OpenIccIncident"));
            OpenEverythingInfo = new OpenEverythingInfoClass(xmlHelper._root.SelectSingleNode("OpenEverything"));
            ClipboardHelperInfo = new ClipboardHelperInfoClass(xmlHelper._root.SelectSingleNode("ClipboardHelper"));
        }

        public static void SaveMyToolInfo()
        {
            xmlHelper.SaveToFile();
        }

        private static bool ExplicitlyEqualsList<T>(List<T> list1, List<T> list2)
        {
            bool isEqual = list1 != null && list2 != null && list1.Count == list2.Count;

            for (int i = 0; i < list1.Count - 1; i++)
            {
                if (!isEqual) break;
                isEqual &= list1[i].Equals(list2[i]);
            }
            return isEqual;
        }

        private static bool ExplicitlyEqualsDictionary<T1, T2>(Dictionary<T1, T2> dic1, Dictionary<T1, T2> dic2)
        {
            bool isEqual = dic1 != null && dic2 != null && dic1.Count == dic2.Count;

            if (isEqual)
            {
                isEqual = ExplicitlyEqualsList<KeyValuePair<T1, T2>>(dic1.ToList(), dic2.ToList());
            }

            return isEqual;
        }

        private static List<T> GetNodeValues<T>(XmlNode sourceNode, string nodePath)
        {
            if (sourceNode == null || String.IsNullOrWhiteSpace(nodePath))
            {
                throw new Exception("Invalid Node or Node Path.");
            }

            var nodes = sourceNode.SelectNodes(nodePath);
            var resultList = nodes == null
                ? new List<string>()
                : nodes.Cast<XmlNode>().Select(p => p.InnerText.ToString());

            if (typeof (T) == typeof(int))
            {
                return resultList.Select(int.Parse).Cast<T>().ToList();
            }
            else //if (typeof(T) == typeof(string))
            {
                return resultList.Cast<T>().ToList();
            }
        }

        private static Dictionary<string, string> GetNodeDictionary(XmlNode sourceNode, string nodePath)
        {
            if (sourceNode == null || String.IsNullOrWhiteSpace(nodePath))
            {
                throw new Exception("Invalid Node or Node Path.");
            }

            var nodes = sourceNode.SelectNodes(nodePath);

            var result = new Dictionary<string, string>();
            if (nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    var key = (node.SelectSingleNode("Key")).InnerText;
                    var value = node.SelectSingleNode("Value").InnerText;
                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, value);
                    }
                }
            }
            return result;
        }

        private XmlDocument _xml = new XmlDocument();
        private XmlNode _root;
        private string _xmlPath = @".\MyToolInfo.xml";
        //private string _xmlPath_Out = @".\MyToolInfo_Out.xml";
        private XmlHelper()
        {
            _xml.Load(_xmlPath);
            _root = _xml.DocumentElement;
        }

        public void SaveToFile()
        {
            _xml.Save(_xmlPath);
        }

        public class FindChangeSetInfoClass
        {
            private readonly XmlDocument _xml;
            private readonly XmlNode _node = null;

            public FindChangeSetInfoClass( XmlNode node)
            {
                _node = node;
                _xml = _node.OwnerDocument;
            }

            public List<string> RecentlyUsedPaths
            {
                get { return GetNodeValues<string>(_node, @"SearchPath/RecentlyUsed/Path"); }
                set
                {
                    if (value == null || ExplicitlyEqualsList<string>(GetNodeValues<string>(_node, @"SearchPath/RecentlyUsed/Path"), value))
                        return;

                    var paths = _node.SelectSingleNode(@"SearchPath/RecentlyUsed");
                    if (paths == null)
                        throw new Exception("Cannot find node \"SearchPath/RecentlyUsed\"");

                    paths.RemoveAll();
                    foreach (var path in value)
                    {
                        var pathNode = _xml.CreateElement("Path");
                        pathNode.InnerText = path;
                        paths.AppendChild(pathNode);
                    }
                    SaveMyToolInfo();
                }
            }

            public List<string> TempletePaths
            {
                get
                {
                   return GetNodeValues<string>(_node, @"SearchPath/Templetes/Path");
                }
            }
        }

        public class OpenIccIncidentInfoClass
        {
            private readonly XmlDocument _xml;
            private readonly XmlNode _node = null;

            public OpenIccIncidentInfoClass(XmlNode node)
            {
                _node = node;
                _xml = _node.OwnerDocument;
            }

            public List<int> RecentIncidents
            {
                get { return GetNodeValues<int>(_node, @"RecentIncidents/Incident"); }
                set
                {
                    if (value == null || ExplicitlyEqualsList(GetNodeValues<int>(_node, @"RecentIncidents/Incident"), value))
                        return;

                    var incidents = _node.SelectSingleNode(@"RecentIncidents");
                    if (incidents == null)
                        throw new Exception("Cannot find node \"RecentIncidents\"");

                    incidents.RemoveAll();
                    foreach (var incident in value)
                    {
                        var incidentNode = _xml.CreateElement("Incident");
                        incidentNode.InnerText = incident.ToString();
                        incidents.AppendChild(incidentNode);
                    }
                    SaveMyToolInfo();
                }
            }
        }

        public class OpenEverythingInfoClass
        {
            private readonly XmlDocument _xml;
            private readonly XmlNode _node = null;

            public OpenEverythingInfoClass(XmlNode node)
            {
                _node = node;
                _xml = _node.OwnerDocument;
            }

            public Dictionary<string,string> Everythings
            {
                get { return GetNodeDictionary(_node, @"Everythings/Everything"); }
                set
                {
                    if (value == null ||
                         ExplicitlyEqualsDictionary<string, string>(GetNodeDictionary(_node, @"Everythings/Everything"), value))
                        return;

                    var everythings = _node.SelectSingleNode(@"Everythings");
                    if (everythings == null)
                        throw new Exception("Cannot find node \"Everythings\"");

                    everythings.RemoveAll();
                    foreach (var pair in value)
                    {
                        var everythingNode = _xml.CreateElement("Everything");
                        var keyNode = _xml.CreateElement("Key");
                        var valueNode = _xml.CreateElement("Value");
                        keyNode.InnerText = pair.Key;
                        valueNode.InnerText = pair.Value;
                        everythingNode.AppendChild(keyNode);
                        everythingNode.AppendChild(valueNode);
                        everythings.AppendChild(everythingNode);
                    }
                    SaveMyToolInfo();
                }
            }

        }

        public class ClipboardHelperInfoClass
        {
            private readonly XmlDocument _xml;
            private readonly XmlNode _node = null;

            public ClipboardHelperInfoClass(XmlNode node)
            {
                _node = node;
                _xml = _node.OwnerDocument;
            }

            public List<string> PredifinedCopies
            {
                get { return GetNodeValues<string>(_node, @"PredifinedCopies/CopyString"); }
            }
        }
    }
}
