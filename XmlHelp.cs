using System;
using System.Xml.XPath;
using System.Xml;

namespace CcssToCsv
{
    static internal class XmlHelp
    {
        static public string? XPVal(this XPathNavigator? nav, string xpath)
        {
            if (nav == null) return null;
            var result = nav.Evaluate(xpath);
            var iter = result as XPathNodeIterator;
            if (iter != null)
            {
                if (iter.MoveNext())
                {
                    return iter.Current?.Value;
                }
                else
                {
                    return null;
                }
            }
            return result.ToString();
        }

        static public string? XPVal(this IXPathNavigable ele, string xpath)
        {
            return XPVal(ele.CreateNavigator(), xpath);
        }

        static public string? XPVal(this XmlElement node, string xpath)
        {
            var result = node.SelectSingleNode(xpath);
            return result?.InnerText;
        }
    }
}
