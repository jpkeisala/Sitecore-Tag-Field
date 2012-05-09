using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore;
using System.Xml.XPath;
using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;
using Sitecore.Configuration;
namespace Addition.Context.Types.Tag.Xml.Xsl
{
 public class XsltHelper : Sitecore.Xml.Xsl.XslHelper
    {

       public string fieldValue(string fieldName,XPathNodeIterator iterator)
       {
           return this.field(fieldName,iterator, string.Empty);
       }

        public string fieldValue(string fieldName,string secondFieldName,XPathNodeIterator iterator)
        {
            if (!iterator.MoveNext())
            {
                return string.Empty;
            }
            Item item = this.GetItem(iterator);
            if (item == null)
            {
                return string.Empty;
            }
           /* using (new ContextItemSwitcher(item))
            {
                FieldRenderer renderer = new FieldRenderer();
                renderer.Item = item;
                renderer.FieldName = fieldName;
                renderer.Parameters = parameters;
               
                return "aaa"+renderer.Render();
            }*/
            string result = string.Empty;
            if (secondFieldName != null && this.GetFieldValue(fieldName, iterator, String.Empty).IndexOf("|") >= 0)
            {
                foreach (string s in this.GetFieldValue(fieldName, iterator, String.Empty).Split('|'))
                {
                    if (s != null && Sitecore.Context.Database.GetItem(s) != null && Sitecore.Context.Database.GetItem(s).Fields[secondFieldName] != null)
                    {
                        result += Sitecore.Context.Database.GetItem(s,Sitecore.Globalization.Language.Parse("en")).Fields[secondFieldName].Value + "|";
                    }
                }

            }
            else
            {
                //result = this.GetFieldValue(fieldName, iterator, string.Empty);
                result = Sitecore.Context.Database.GetItem(this.GetFieldValue(fieldName, iterator, string.Empty), Sitecore.Globalization.Language.Parse("en")).Fields[secondFieldName].Value;
            }
            return result;
        }

 

 


 

    }
}
