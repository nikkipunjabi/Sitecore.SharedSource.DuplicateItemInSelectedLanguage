using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sitecore.SharedSource.DuplicateItemInSelectedLanguage.sitecore.admin.SharedSource
{
    public partial class DuplicateItem : System.Web.UI.Page
    {
        protected string CurrentItemName { get; set; }
        protected string DuplicateItemName { get; set; }
        Database CurrentDatabase { get; set; }
        Item CurrentItem { get; set; }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();

            string log = string.Format("User: {0} started RemoveItemLanguage Tool.", Sitecore.Context.User.Name);
            Log.Info(log, this);
            var duplicateItemID = string.Empty;
            if (CurrentItem != null && !string.IsNullOrWhiteSpace(DuplicateItemName) && CurrentDatabase != null)
            {
                var duplicatedItem = CurrentItem.Duplicate(DuplicateItemName);

                foreach (ListItem lang in chkBoxLanguages.Items)
                {
                    if (!lang.Selected)
                    {
                        RemoveLanguageVersion(duplicatedItem.Paths.FullPath, lang.Value, sb, CurrentDatabase, ref count);
                    }
                }
                duplicateItemID = duplicatedItem.ID.ToString();
            }
            Sitecore.Web.WebUtil.AddQueryString("", "duplicateItemID", duplicateItemID);
            //Below script will close the modal dialog
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Close", "window.top.returnValue = '" + duplicateItemID + "'; window.top.dialogClose();", true);
        }

        public void RemoveLanguageVersion(string rootItemPath, string languageCode, StringBuilder sb, Database db, ref int count)
        {
            string log = string.Empty;
            try
            {
                Language languageRemove = Sitecore.Globalization.Language.Parse(languageCode);
                Item rootItem = db.GetItem(rootItemPath, languageRemove);
                if (rootItem == null || rootItem.ID.ToString() == "{F344DBE2-BC34-49FB-8564-FD74048702D9}") { sb.Append(" Item not found: " + rootItemPath).Append("<br />"); return; }
                if (rootItem != null)
                {
                    using (new Sitecore.SecurityModel.SecurityDisabler())
                    {
                        if (rootItem.Versions.Count == 0)
                        {
                            sb.Append("Language version not found. Item: " + rootItemPath).Append(" Language: ").Append(languageCode).Append("<br />");
                        }
                        else
                        {
                            //Remove All Versions from Item
                            rootItem.Versions.RemoveAll(false);

                            sb.Append("Removed Item Language. Item ID:").Append(rootItem.ID).Append(" Language: ").Append(languageCode).Append("<br /> ");
                            log = string.Format("Removed Item Language. Item Path: {0}, Language Version Removed: {1} ", rootItem.Paths.FullPath, languageCode);
                            Log.Info(log, this);
                        }

                        if (removeVersionsFromChildren.Checked)
                        {
                            //Remove language version recursively from child items of duplicate item
                            foreach (Item child in rootItem.Axes.GetDescendants().Where(x => x.Language == languageRemove))
                            {
                                count++;
                                if (child.Versions.Count > 0)
                                {
                                    child.Versions.RemoveAll(false);
                                    sb.Append("Removed Child Item Language. Item ID:").Append(child.ID).Append(" Language: ").Append(languageCode).Append("<br />");
                                    log = string.Format("Removed Child Item Language. Item Path: {0}, Language Version Removed: {1} ", child.Paths.FullPath, languageCode);
                                    Log.Info(log, this);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                sb.Append("Error while processing. Item: ").Append(rootItemPath).Append(" Language: ").Append(languageCode).Append("<br />");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.ClientScript.GetPostBackEventReference(btnOK, string.Empty);

            var database = Sitecore.Web.WebUtil.GetQueryString("database");
            var itemID = Sitecore.Web.WebUtil.GetQueryString("id");
            CurrentItemName = Sitecore.Web.WebUtil.GetQueryString("currentItemName");
            DuplicateItemName = Sitecore.Web.WebUtil.GetQueryString("duplicateItemName");

            var db = Factory.GetDatabase(database);
            if (db != null)
            {
                CurrentDatabase = db;
                CurrentItem = db.GetItem(itemID);
                if (IsPostBack)
                {
                    return;
                }
                int versionLanguageCount = 0;
                if (CurrentItem != null && CurrentItem.Languages.Count() > 1)
                {
                    foreach (var itemLanguage in CurrentItem.Languages)
                    {
                        var item = CurrentItem.Database.GetItem(CurrentItem.ID, itemLanguage);
                        if (item.Versions.Count > 0)
                        {
                            versionLanguageCount = versionLanguageCount + 1;
                            ListItem lstItem = new ListItem(itemLanguage.Name);
                            lstItem.Selected = true;
                            chkBoxLanguages.Items.Add(lstItem);
                        }
                    }
                }

                if (versionLanguageCount <= 1)
                {
                    //If Item exist only in one or no language.
                    var duplicatedItem = CurrentItem.Duplicate(DuplicateItemName);
                    //Below script will close the modal dialog
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Close", "window.top.returnValue = '" + duplicatedItem.ID.ToString() + "'; window.top.dialogClose();", true);
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //Below script will close the modal dialog
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Close", "window.top.dialogClose();", true);
        }
    }
}