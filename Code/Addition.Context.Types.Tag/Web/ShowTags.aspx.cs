using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Web.UI;
using Sitecore;
using Sitecore.Data.Items;

namespace Addition.Context.Types.Tag
{
 public class ShowTags : System.Web.UI.Page
	{
     string fieldName = string.Empty;
     string lang = string.Empty;
		private void Page_Load(object sender, System.EventArgs e)
		{
            //get the q parameter from URL
            string tags=string.Empty;
            string fieldId = string.Empty;
            try {  fieldName=Request.QueryString["fieldName"].ToString().ToLower();}
            catch { fieldName = "tags"; }
            try { fieldId = Request.QueryString["fieldId"].ToString().ToLower(); }
            catch { fieldId= ""; }
            try
            {
                string q = string.Empty;
                try
                {
                   q=Request.QueryString["q"].ToString()[0].ToString().ToUpper()+Request.QueryString["q"].ToString().ToLower().Substring(1);
                }
                catch { q = ""; }

                try
                {
                    lang = Request.QueryString["lang"].ToString();
                }
                catch { lang = "en"; }
                 // Sitecore.Diagnostics.Error.LogError("Tag aspx add" + (Request.QueryString["add"] != null) + Request.QueryString["add"].ToString());
                if (Request.QueryString["add"] != null && Request.QueryString["add"].ToString().Equals("1"))
                {
                    try
                    {
                        AddTag(q, Request.QueryString["sitecoreId"].ToString());
                    }
                    catch(Exception ex) { Sitecore.Diagnostics.Error.LogError("AddTag "+ex.Message+" "+ex.StackTrace+"  "+ex.Source); }
                }
                else
                {
                    Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master");
                    Item currentItem = master.GetItem(Request.QueryString["sitecoreId"].ToString());
                    Sitecore.Data.Fields.Field fld = currentItem.Fields[fieldName];
                   
                Item tagsParent = Sitecore.Context.Database.GetItem(fld.Source.ToLower());

              //  Sitecore.Diagnostics.Error.LogError("Tag aspx " + (tagsParent != null) + " field " + fieldId + " " + fld.Source);
                if (tagsParent != null)
                {
                    int i = 0;

                    foreach (Item item in tagsParent.Axes.GetDescendants())
                    {
                        //if (item != null && item.Fields["value"] != null)
                        //{
                        //    Sitecore.Diagnostics.Error.LogError(" TAG li " + q + " " + item.Fields["value"].Value + "  " + item.Fields["value"].Value.ToLower().StartsWith(q.ToLower()));
                        //}
                        if (item != null && item.Fields["value"] != null && item.Fields["value"].Value != null && item.Fields["value"].Value.ToLower().StartsWith(q.ToLower()))
                        {
                            i++;
                            
                            string id = fieldId+"list_" + i; // +"_" + item.ID.ToString().Substring(1, item.ID.ToString().Length - 3);
                            tags += "<li id=\"" + id + "\" class=\"autocomplete_listItem\" onclick=\"selectItem('" + id + "',this)\"   onMouseover=\"hoverItem('" + id + "',this)\" onMouseout=\"outhoverItem('" + id + "',this)\"   onkeypress=\"checkEvent(event,this.value,'" + id + "')\">" + item.Fields["value"].Value + "</li>";
                           }
                    }
                    if (tags != "")
                    {
                        Response.Write("<ul id=\""+fieldId+"autocomplete_list\" calss=\"autocomplete_list\">" + tags + "</ul>");
                    }
                }
                }
            }
            catch (Exception ex) { Sitecore.Diagnostics.Error.LogError(" ShowTags " + ex.Message + "  " + ex.Source); }
        }

        private void AddTag(string tagName, string id)
        {
           // Sitecore.Diagnostics.Error.LogError(" ADD TAG");
            string tagPath = string.Empty;
            Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master");
            Sitecore.Globalization.Language language = Sitecore.Globalization.Language.Parse(lang);
            Item currentItem = master.GetItem(id,language);
            if (currentItem != null)
            {
                //Sitecore.Diagnostics.Error.LogError(" ADD TAG 1");
                Sitecore.Data.Fields.Field fld = currentItem.Fields[fieldName];
                if (fld != null)
                {
                   // Sitecore.Diagnostics.Error.LogError(" ADD TAG 2");
                    if (fld.Source.Equals("")) { tagPath = Constants.TagPath +"/Default"; }
                    else
                    {
                       // Sitecore.Diagnostics.Error.LogError(" ADD TAG 3" + (master.GetItem(fld.Source.ToLower())==null)+fld.Source.ToLower());
                        string folderName = "";
                        try { folderName = fld.Source[0].ToString().ToUpper() + fld.Source.Substring(1).ToLower(); }
                        catch { folderName = fld.Source.ToLower(); }
                        fld.Source.ToLower();
                        if (master.GetItem(folderName) == null)
                        {  //add new tag group under global
                            Item tagsHome = master.GetItem(Constants.TagPath);
                            Sitecore.Data.Items.TemplateItem template = master.Templates["common/folder"];
                            string newtagfolder = folderName.Replace(Constants.TagPath + "/", "");
                          //  Sitecore.Diagnostics.Error.LogError(" TAG Path " + newtagfolder);
                            Item myItem = tagsHome.Add(newtagfolder, template);
                            PublishItem(myItem);
                        }
                        tagPath = fld.Source;
                    }
                }

                Item tagItem = master.GetItem(tagPath+"/"+tagName);
                string tagId = string.Empty;
                if (tagItem == null)
                {
                    //Sitecore.Diagnostics.Error.LogError(" ADD TAG 4" +tagPath);
                    Sitecore.Data.Items.TemplateItem template = master.Templates["tag"];
                    Item newTag = (master.GetItem(tagPath)).Add(tagName, template);
                    using (new Sitecore.SecurityModel.SecurityDisabler())
                    {
                        using (new Sitecore.Data.Items.EditContext(newTag))
                        {
                            newTag.Fields["value"].Value = tagName;
                        }
                    }
                    
                    tagId = newTag.ID.ToString();
                }
                else
                {
                   // Sitecore.Diagnostics.Error.LogError(" ADD TAG 5");
                    tagId = tagItem.ID.ToString();
                }
                tagItem=master.GetItem(tagId);
                //add to field
                using (new Sitecore.SecurityModel.SecurityDisabler()) 
                { 
                    using (new Sitecore.Data.Items.EditContext(currentItem))
                    {
                        //(Addition.Context.Types.Tag.Tag) currentItem.Fields["tags"]
                        Sitecore.Data.Fields.MultilistField multiselectField = currentItem.Fields[fieldName];

                        if (currentItem.Fields[fieldName].Value.IndexOf(tagId) < 0)
                        {
                            currentItem.Editing.BeginEdit();
                            multiselectField.Add(tagId);
                            currentItem.Editing.EndEdit();
                            // Addition.Context.Types.Tag.Tag mfld=new Addition.Context.Types.Tag.Tag();
                            // mfld.ReLoad();
                            /*Sitecore.Shell.Applications.ContentEditor.Checklist list = FindControl("check_list") as Sitecore.Shell.Applications.ContentEditor.Checklist;
                            Sitecore.Shell.Applications.ContentEditor.Text text = FindControl("text_tags") as Sitecore.Shell.Applications.ContentEditor.Text;
                            mfld.ListItemClick();
                            mfld.FillTextBox(text, list);
                            mfld.ReLoad();
                            Sitecore.Diagnostics.Error.LogError(" ADD TAG 6"+fld.Value+"   "+mfld.Value);
                       
                            //fld.SetValue(fld.Value + tagId + "|",true);
                       
                            Sitecore.Diagnostics.Error.LogError(" ADD TAG 6.2" + fld.Value + "   " + mfld.Value);

                           /* 
                            Sitecore.Web.UI.HtmlControls.ChecklistItem itml=new Sitecore.Web.UI.HtmlControls.ChecklistItem();
                            itml.Value=tagName;*/

                            Sitecore.Shell.Applications.ContentEditor.Checklist list = FindControl("check_list") as Sitecore.Shell.Applications.ContentEditor.Checklist;
                            if (list != null)
                            {
                                // Sitecore.Diagnostics.Error.LogError("ADD TAG 6" + multiselectField.Value);
                                list.Value = multiselectField.Value;
                                // Sitecore.Diagnostics.Error.LogError("ADD TAG 6" + list.Value);
                            }
                            //Sitecore.Diagnostics.Error.LogError("ADD TAG 6" + multiselectField.Value + "  " + (FindControl("check_list") == null));
                            Response.Write(tagName + "|" + tagId);
                        }
                        else 
                        {
                            Response.Write(tagName);
                        }
                        
                    } 
                }
                PublishItem(tagItem);

            }
        
        }


        private void PublishItem(Item itm)
        {
            Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master");
            DateTime publishDate = DateTime.Now;
            Sitecore.Data.Items.Item targets = master.GetItem("/sitecore/system/publishing targets"); 
            foreach (Sitecore.Data.Items.Item target in targets.Children) 
            {
              string targetDBName = target["target database"];
              //Sitecore.Diagnostics.Error.LogError(" TAG TARGET "+targetDBName);
              Sitecore.Data.Database targetDB = Sitecore.Configuration.Factory.GetDatabase(targetDBName);
              foreach (Sitecore.Globalization.Language language in master.Languages)
              {
                  //Sitecore.Diagnostics.Error.LogError(" TAG Lag " + language);
                  Sitecore.Publishing.PublishOptions publishOptions = new Sitecore.Publishing.PublishOptions(master, targetDB, Sitecore.Publishing.PublishMode.SingleItem, language, publishDate);
                  publishOptions.RootItem = itm;
                  publishOptions.Deep = false;
                  Sitecore.Publishing.Publisher publisher = new Sitecore.Publishing.Publisher(publishOptions);
                  publisher.Publish();
              }
          
            }
            //Sitecore.Diagnostics.Error.LogError(" TAG PUBLISH END");
        }
    }
 }


