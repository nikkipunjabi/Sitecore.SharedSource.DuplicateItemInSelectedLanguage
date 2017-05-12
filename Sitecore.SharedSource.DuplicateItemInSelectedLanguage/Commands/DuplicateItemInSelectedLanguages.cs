using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Sitecore.SharedSource.DuplicateItemInSelectedLanguage.Commands
{
    public class DuplicateItemInSelectedLanguages : Command
    {

        //Display Name: Duplicate - Select Languages
        //Icon: Applications/32x32/navigate_close.png
        //Message: item:getchilditemcount(id=$Target,title=Do you want to specify Template ID?)
        public override void Execute(CommandContext context)
        {
            if (context.Items.Length == 1)
            {
                var currentItem = context.Items[0];

                NameValueCollection parameters = new NameValueCollection();
                parameters["id"] = currentItem.ID.ToString();
                parameters["name"] = currentItem.Name;
                parameters["database"] = currentItem.Database.Name;
                parameters["language"] = currentItem.Language.ToString();
                parameters["first"] = "True";

                Context.ClientPage.Start((object)this, "Run", parameters);
            }
        }

        /// <summary>
        /// Handles the Postback of the Sheer Dialogs
        /// </summary>
        protected void Run(ClientPipelineArgs args)
        {
            try
            {
                if (!args.IsPostBack)
                {
                    if (!args.HasResult && !string.IsNullOrEmpty(args.Parameters["name"]) && args.Parameters["first"] == "True")
                    {
                        args.Parameters["first"] = "False";
                        SheerResponse.Input("Enter a name for the new item:", "Copy of " + args.Parameters["name"], Settings.ItemNameValidation, Translate.Text("'$Input' is not a valid name."), Settings.MaxItemNameLength);
                        args.WaitForPostBack();
                    }
                }
                else if (args.HasResult && !(string.IsNullOrEmpty(args.Result)) && ID.IsID(args.Result))
                {
                    //This will make sure the Content Editor loads the latest version after dialog closes.
                    string load = String.Concat(new object[] { "item:load(id=", args.Result, ")" });
                    Context.ClientPage.SendMessage(this, load);
                    String refresh = String.Format("item:refreshchildren(id={0})", args.Result);
                    Context.ClientPage.ClientResponse.Timer(refresh, 2);
                }
                else if (args.HasResult && !(string.IsNullOrEmpty(args.Result)))
                {
                    //Check if valid item name
                    string itemName = ItemUtil.ProposeValidItemName(args.Result);
                    if (itemName == args.Result)
                    {
                        //Show the dialog to select the languages
                        Sitecore.Text.UrlString url = new Sitecore.Text.UrlString("/sitecore/admin/SharedSource/DuplicateItemSelectLanguages.aspx");
                        url.Append("id", args.Parameters["id"]);
                        url.Append("duplicateItemName", args.Result);
                        url.Append("currentItemName", args.Parameters["name"]);
                        url.Append("database", args.Parameters["database"]);

                        Context.ClientPage.ClientResponse.ShowModalDialog(url.ToString(), "420", "400", string.Empty, true, "", "", false);
                        args.WaitForPostBack(true);
                    }
                    else
                    {
                        //Item name is invalid.
                        //If Item Name empty
                        //Control will never come here.
                        SheerResponse.Input("Enter a name for the new item:", "Copy of " + args.Parameters["name"]);
                        args.WaitForPostBack();
                    }
                }
                else
                {
                    // Do Nothing
                }
            }
            catch (Exception ex)
            {
                Log.Error("Some Error Occurred in Duplicate Item In Selected Languages", ex, this);
            }
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (context.Items.Length == 1)
            {
                Item currentItem = context.Items[0];
                if (currentItem.ID == new ID("11111111-1111-1111-1111-111111111111"))
                {
                    return CommandState.Disabled;
                }
            }
            return base.QueryState(context);
        }
    }
}