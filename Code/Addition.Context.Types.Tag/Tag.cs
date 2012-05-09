using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Events;

using System.Web.UI;
using System.Collections;

using Sitecore.Text;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Addition.Context.Types.Tag
{
    public class Tag : Sitecore.Web.UI.HtmlControls.Control
    {
        Database db = Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;

        #region Varibles

        private string fieldName = string.Empty;
        private string itemID = string.Empty;
        private string source = string.Empty;
        private bool isEvent = true;
        private string AddNewVlue = string.Empty;
        private string EnterTagValue = string.Empty;
        private string OnBlureTextValue = string.Empty;
        private string itemLanguage=string.Empty;

        #endregion Variables
        

        #region Properties

        public string FieldName
        {
            get
            {
                return fieldName;
            }
            set
            {
                fieldName = value;
            }
        }

        public string ItemID
        {
            get
            {
                return itemID;
            }
            set
            {
                itemID = value;
            }
        }

        public string Source
        {
            get
            {
                return StringUtil.GetString(source);
            }
            set
            {

                if (value.IndexOf('&') > -1)
                {
                    source = value.Substring(0, value.IndexOf('&'));
                    if (value.ToLower().IndexOf("separator", value.IndexOf('&')) > -1)
                    {
                        string[] parameters = value.Split('&');
                        for (int i = 1; i < parameters.Length; i++)
                        {
                            if (parameters[i].ToLower().IndexOf("separator") > -1)
                            {
                                Separator = parameters[i].Substring((parameters[i].IndexOf("=") > -1) ? parameters[i].IndexOf("=") + 1 : 0);
                            }
                        }
                    }
                }
                else
                {
                    source = value;
                }
            }
        }

        public string Separator
        {
            get
            {
                if (ViewState[this.ClientID + "_separator"] != null)
                {
                    return ViewState[this.ClientID + "_separator"].ToString();
                }
                else
                {
                    return ", ";
                }
            }
            set
            {
                ViewState[this.ClientID + "_separator"] = value;
            }
        }

        public bool TrackModified
        {
            get
            {
                return base.GetViewStateBool("TrackModified", true);
            }
            set
            {
                base.SetViewStateBool("TrackModified", value, true);
            }
        }

        public string ItemLanguage
        {
            get
            {
                return base.GetViewStateString("ItemLanguage");
            }
            set
            {
                base.SetViewStateString("ItemLanguage", value);
            }
        }

        #endregion Properties

    #region Constructor

      public Tag()
      {
         this.Class = "scContentControl";
         base.Activation = true;
        
      
     
      }

      #endregion Constructor
      #region Overrides
    protected override void OnLoad(EventArgs args)
        {
            AddNewVlue = GetFieldvalue(Constants.AddNewPath, "text");
            EnterTagValue = GetFieldvalue(Constants.EnterNewPath, "text");
            OnBlureTextValue = GetFieldvalue(Constants.OnBlurPath, "text");
            try
            {
                //Sitecore.Diagnostics.Error.LogError("TAG in " + this.Value+" "+this.ID);

                if (!Sitecore.Context.ClientPage.IsEvent)
                {
                    
                    
                    base.ViewState["ItemLanguage"] = this.ItemLanguage;
                    isEvent = false;
                    Sitecore.Shell.Applications.ContentEditor.Checklist list = new Sitecore.Shell.Applications.ContentEditor.Checklist();
                    this.Controls.Add(list);
                    string sourceIds = string.Empty;
                    int i = 1;
                    foreach (String s in this.Value.Split('|'))
                    {
                        if (i < this.Value.Split('|').Length)
                        {
                            sourceIds += "@@id='" + s + "' or ";
                        }
                        else
                        {
                            sourceIds += "@@id='" +s+ "'";
                        }
                        i++;
                    }
                    list.ID = this.ID.ToLower() + "check_list";
                    list.Source = "query:" + this.Source + "/*["+sourceIds+"]";
                    list.ItemID = ItemID;
                    list.FieldName = FieldName;
                    list.TrackModified = TrackModified;
                    list.Disabled = this.Disabled;
                    list.Value = this.Value;
                    list.Class = "scContentControlChecklist hidebr";
                    Sitecore.Shell.Applications.ContentEditor.Text text = new Sitecore.Shell.Applications.ContentEditor.Text();
                    this.Controls.AddAt(0, text);
                    text.ID = this.ID.ToLower()+"text_tags";
                    text.ReadOnly = true;
                    text.Disabled = this.Disabled;
                    text.Attributes.Add("style", "display:none");
                    this.Controls.Add(new LiteralControl(Sitecore.Resources.Images.GetSpacer(0x18, 16)));
                    System.Web.UI.HtmlControls.HtmlGenericControl title = new System.Web.UI.HtmlControls.HtmlGenericControl();
                    Sitecore.Shell.Applications.ContentEditor.Text textVar = new Sitecore.Shell.Applications.ContentEditor.Text();
                    System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl();
                    Sitecore.Shell.Applications.ContentEditor.Text divhide = new Sitecore.Shell.Applications.ContentEditor.Text();
                    System.Web.UI.HtmlControls.HtmlInputButton addButton = new System.Web.UI.HtmlControls.HtmlInputButton();
                    Sitecore.Shell.Applications.ContentEditor.Text languageHidden = new Sitecore.Shell.Applications.ContentEditor.Text();

                    addButton.Value= AddNewVlue!=string.Empty ? AddNewVlue :"Add new";
                    divhide.Attributes.Add("style", "display:none");
                    divhide.ID = this.ID.ToLower() + "newtags";
                    divhide.Attributes.Add("id", this.ID.ToLower() + "newtags");

                    title.InnerHtml =EnterTagValue!=string.Empty?EnterTagValue: "Enter new tag";
                    title.Attributes.Add("class", "tag_text");
                    
                    div.Attributes.Add("style", "display:none");
                    div.Attributes.Add("class", "autocomplete_completionListElement");
                    div.Attributes.Add("id", this.ID.ToLower() + "tags");
                                      
                    this.BackColor = System.Drawing.Color.Red;

                    textVar.Attributes.Add("id", this.ID.ToLower() + "txtHint");
                    textVar.Attributes.Add("style","width:80%");
                    textVar.ID = this.ID.ToLower() + "txtHint";
                    textVar.Attributes.Add("onblur", "if (this.value == '') this.value = '" + (OnBlureTextValue != string.Empty ? OnBlureTextValue : "Write tag here") + "';");
                    textVar.Attributes.Add("onfocus", "if ('" + (OnBlureTextValue != string.Empty ? OnBlureTextValue : "Write tag here") + "' == this.value) this.value ='';");
                    textVar.Value = OnBlureTextValue!=string.Empty? OnBlureTextValue:"Write tag here";

                    
                    languageHidden.ID = this.ID.ToLower() + "languageHidden";
                    languageHidden.ReadOnly = true;
                    languageHidden.Disabled = this.Disabled;
                    languageHidden.Attributes.Add("style", "display:none");
                    languageHidden.Value = this.ItemLanguage;
                  //  Sitecore.Diagnostics.Error.LogError("TAG in " + this.ItemLanguage);

                    /*textVar.Attributes.Add("OnKeyDown","AddNewItem");
                    textVar.Attributes.Add("runat", "server");*/
                    try
                    {
                        Item contextItem = Sitecore.Context.ContentDatabase.Items[this.ItemID];
                        
                        foreach (TemplateFieldItem tfItem in contextItem.Template.OwnFields)
                        {
                            if (tfItem != null && tfItem.Source == this.Source && tfItem.InnerItem != null)
                            {
                               FieldName = tfItem.InnerItem.Key;
                            }
                        }

                        //Sitecore.Diagnostics.Error.LogError("TAG   "+this.FieldName);
                        addButton.Attributes.Add("onClick", "javascript:buttonClick('" + contextItem.ID.ToString() + "','" + this.ID.ToLower() + "','" + this.FieldName + "')");
                        textVar.Attributes.Add("onkeyup", "checkEvent(event,this.value,'" + contextItem.ID.ToString() + "','" + this.ID.ToLower() + "','" + this.FieldName + "')");
                    }
                    catch (Exception eeee) { Sitecore.Diagnostics.Error.LogError("TAG "+eeee.Message+eeee.Source+eeee.StackTrace); }

                  

                    this.Controls.AddAt(1, title);
                    this.Controls.AddAt(2, new LiteralControl("<br>"));
                    this.Controls.AddAt(3, textVar);
                    this.Controls.AddAt(4, addButton);
                    this.Controls.AddAt(5, divhide);
                    this.Controls.AddAt(6, new LiteralControl("<br>"));
                    this.Controls.AddAt(7, div);
                    this.Controls.AddAt(8, languageHidden);
                    this.ID = GetID("autocompleteTag");

                    
                }
                else
                {
                    
                    this.ItemLanguage = this.ViewState["ItemLanguage"] as string;
                    Sitecore.Shell.Applications.ContentEditor.Checklist list = FindControl(this.ID.ToLower() + "check_list") as Sitecore.Shell.Applications.ContentEditor.Checklist;
                    Sitecore.Shell.Applications.ContentEditor.Text newtags = FindControl(this.ID.ToLower() + "newtags") as Sitecore.Shell.Applications.ContentEditor.Text;
                    Sitecore.Shell.Applications.ContentEditor.Text languageHidden = FindControl(this.ID.ToLower() + "languageHidden") as Sitecore.Shell.Applications.ContentEditor.Text;
                    languageHidden.Value = this.ItemLanguage;
                   if (list != null)
                    {
                        ListString valueList = new ListString();
                       // Sitecore.Diagnostics.Error.LogError("TAG list!=null" + list.Items.Length + "  " + (newtags != null) + newtags.Value + "  " + this.Source + "/" + newtags.Value+"  this:"+this.Source+" s "+this.source+"  list "+list.Source);
                        if (newtags != null && newtags.Value !=null && newtags.Value != "")
                        {
                            foreach(string s in newtags.Value.Split('|'))
                            {
                               // Sitecore.Diagnostics.Error.LogError("TAG  foreach" + s);
                                if (s != "")
                                {
                                    valueList.Add(s);
                                }
                            }
                            //Sitecore.Diagnostics.Error.LogError("TAG iffffffffaaaaaaaaaaaaa|" + valueList);
                        }
                        foreach (DataChecklistItem item in list.Items)
                        {
                           // Sitecore.Diagnostics.Error.LogError("TAG1 for IF ELSE " + item.ItemID+"  "+item.Value);
                            if (item.Checked)
                            {
                                valueList.Add(item.ItemID);
                            }
                        }
                        if (this.Value != valueList.ToString())
                        {
                            this.TrackModified = list.TrackModified;
                            this.SetModified();
                        }
                        this.Value = valueList.ToString();
                        list.Class += " hidebr";
                    }
                   // Sitecore.Diagnostics.Error.LogError("TAG1 IF ELSE " + this.Value);
                  
                }

               
                
            
                base.OnLoad(args);
             
            }
            catch(Exception e){ Sitecore.Diagnostics.Error.LogError(" CATCH TAG"+e.Message+e.Source+e.StackTrace); }
        }



    

    protected override void OnPreRender(EventArgs e)
      {
          Sitecore.Shell.Applications.ContentEditor.Checklist list = FindControl(this.ID+"check_list") as Sitecore.Shell.Applications.ContentEditor.Checklist;
          Sitecore.Shell.Applications.ContentEditor.Text text = FindControl(this.ID+"text_tags") as Sitecore.Shell.Applications.ContentEditor.Text;
          FillTextBox(text, list);

         if(!isEvent)
         {
            if(list != null)
            {
               for(int i = 0; i < list.Items.Length; i++ )
               {
                  list.Items[i].ServerProperties["Click"] = string.Format("{0}.ListItemClick", this.ID);
               }
               list.Class += " hidebr";
            }
         }

         base.OnPreRender (e);
      }

      public void ListItemClick()
      {
         Sitecore.Shell.Applications.ContentEditor.Checklist list = FindControl(this.ID+"check_list") as Sitecore.Shell.Applications.ContentEditor.Checklist;
         Sitecore.Context.ClientPage.ClientResponse.SetReturnValue(true);
      }


      
      public override void HandleMessage(Sitecore.Web.UI.Sheer.Message message)
      {

          if (message["id"] == this.ID.ToLower())
         {
            Sitecore.Shell.Applications.ContentEditor.Checklist list = FindControl(this.ID+"check_list") as Sitecore.Shell.Applications.ContentEditor.Checklist;
            Sitecore.Shell.Applications.ContentEditor.Text text = FindControl(this.ID+"text_tags") as Sitecore.Shell.Applications.ContentEditor.Text;
            if(list != null)
            {
               string messageText;
               if ((messageText = message.Name) == null)
               {
                  return;
               }

               if (messageText != "checklist:checkall")
               {
                  if (messageText == "checklist:uncheckall")
                  {
                     list.UncheckAll();
                  }
                  else if (messageText == "checklist:invert")
                  {
                     list.Invert();
                  }
               }
               else
               {
                  list.CheckAll();
               }
               list.Class += " hidebr";
            }
         }

         base.HandleMessage (message);
      }

      #endregion Overrides

  

      #region Protected Scope

      public virtual void FillTextBox(Sitecore.Shell.Applications.ContentEditor.Text text, Sitecore.Shell.Applications.ContentEditor.Checklist list)
      {
         if((text != null) && (list != null) && (list.Items != null))
         {
            text.Value = string.Empty;
            Hashtable textItems = new Hashtable();
            for(int i = 0; i < list.Items.Length; i++)
            {
               if(list.Items[i].Checked)
               {
                  string itemTitle = list.Items[i].Header;
                  if(!textItems.ContainsKey(itemTitle))
                  {
                     textItems.Add(itemTitle, (int)1);
                     text.Value = text.Value + (text.Value == string.Empty ? string.Empty : Separator) + itemTitle; 
                  }
                  else
                  {
                     int sameItemsCount = (int)textItems[itemTitle];
                     if(sameItemsCount == 1)
                     {
                        if(text.Value.IndexOf(Separator + itemTitle + Separator) > -1)
                        {
                           text.Value = text.Value.Replace(Separator + itemTitle + Separator, Separator + itemTitle + " (1)" + Separator);
                        }
                        else
                        {
                           if(text.Value.IndexOf(itemTitle + Separator) == 0)
                           {
                              text.Value = text.Value.Insert(itemTitle.Length, " (1)");
                           }
                           else
                           {
                              if((text.Value.IndexOf(Separator + itemTitle) == (text.Value.Length - (Separator.Length + itemTitle.Length))) || (text.Value == itemTitle))
                              {
                                 text.Value += " (1)";
                              }
                           }
                        }
                        text.Value.Replace(itemTitle, itemTitle + "(1)");
                     }
                     textItems[itemTitle] = ++sameItemsCount;
                     text.Value = string.Format("{0}{1}{2} ({3})", text.Value,  (text.Value == string.Empty ? string.Empty : Separator),  itemTitle,  sameItemsCount); 
                  }
               }
            }
            Sitecore.Shell.Applications.ContentEditor.Text newtags = FindControl(this.ID + "newtags") as Sitecore.Shell.Applications.ContentEditor.Text;

            if (newtags != null && newtags.Value != null && newtags.Value != "")
            {
                Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master");
                foreach (string s in newtags.Value.Split('|'))
                {

                    if (s != "" && master.GetItem(s) != null)
                    {
                        Item tagItem = master.GetItem(s);
                        textItems.Add(tagItem.Fields["value"].Value.ToString(), (int)1);
                        text.Value = text.Value + (text.Value == string.Empty ? string.Empty : Separator) + tagItem.Fields["value"].Value.ToString();
                    }
                }
            }
         }

         return;
      }

      protected virtual void SetModified()
      {
         if(this.TrackModified)    
         {
            Sitecore.Context.ClientPage.Modified = true;
         }
      }

        /// <summary>
        /// get field value from the item with path=path and current language; if field value is empty then read the value from default language
        /// /// </summary>
        /// <param name="path"></param>
        /// <param name="field"></param>
        /// <returns></returns>

      protected virtual string GetFieldvalue(string path, string field)
      {
           Sitecore.Globalization.Language language = Sitecore.Globalization.Language.Parse(this.ItemLanguage);
          if (this.itemLanguage != null && language!=null)
          {
              Item itm =db.GetItem(path, language);
              if (itm != null && itm.Fields[field].Value != string.Empty)
              {
                return itm.Fields[field].Value;
              }
              else
              {
                  itm = db.GetItem(path, Sitecore.Globalization.Language.DefaultLanguage);
                  if (itm != null && itm.Fields[field].Value != string.Empty)
                  {
                      return itm.Fields[field].Value;
                  }

              }
          }
          return string.Empty;
      }
      #endregion



	}
}
