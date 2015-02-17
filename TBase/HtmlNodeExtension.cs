using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBase
{
    public static class HtmlNodeExtension
    {
        public static HtmlNode GetNodebyClassName(this HtmlNode doc, string className)
        {
            for (int i = 0; i < doc.ChildNodes.Count; i++)
            {
                var item = doc.ChildNodes[i];
                if (item.Attributes.Count != 0 && item.Attributes["class"] != null && item.Attributes["class"].Value == className)
                {
                    return item;
                }
                else
                {
                    var node = GetNodebyClassName(item, className);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        public static HtmlNode GetNodeById(this HtmlNode doc, string id)
        {
            for (int i = 0; i < doc.ChildNodes.Count; i++)
            {
                var item = doc.ChildNodes[i];
                if (item.Attributes.Count != 0 && item.Attributes["id"] != null && item.Attributes["id"].Value == id)
                {
                    return item;
                }
                else
                {
                    var node = GetNodeById(item, id);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }
            return null;
        }
    }

}
