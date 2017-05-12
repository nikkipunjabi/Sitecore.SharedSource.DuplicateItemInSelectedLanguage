<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DuplicateItemSelectLanguages.aspx.cs" Inherits="Sitecore.SharedSource.DuplicateItemInSelectedLanguage.sitecore.admin.SharedSource.DuplicateItem" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Duplicate Item - Select Languages</title>
    <style>
        .loader {
            left: 38%;
            top: 30%;
            position: fixed;
            z-index: 101;
            margin-left: -16px;
            margin-top: -16px;
            opacity: 0.5;
            background-position: center;
            background-repeat: no-repeat;
            border: 16px solid #f3f3f3; /* Light grey */
            border-top: 16px solid #2b2b2b;
            border-bottom: 16px solid #2b2b2b;
            border-radius: 50%;
            width: 100px;
            height: 100px;
            animation: spin 2s linear infinite;
        }
        .hide{
            display:none;
        }
        .show{
            display:block;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="loadingDiv hide" id="loadingDiv" runat="server">
            <div class="loader"></div>
        </div>
        <asp:PlaceHolder ID="phContent" runat="server">
            <h3>Item: "<%= CurrentItemName %>" exist in following languages.</h3>
            <h4>Select languages in which you want to duplicate:</h4>
            <asp:CheckBoxList ID="chkBoxLanguages" runat="server">
            </asp:CheckBoxList>
            <br />
            <asp:CheckBox ID="removeVersionsFromChildren" runat="server" Checked="true" Text="Do the same for child items " />
            <asp:Button ID="btnOK" Text="OK" CssClass="btnOK" OnClientClick="javascript:showLoading(); return true;" UseSubmitBehavior="true" OnClick="btnOK_Click" Width="60" runat="server" />&nbsp;
            <asp:Button ID="btnCancel" Text="Cancel" CssClass="btnCancel" OnClick="btnCancel_Click" Width="60" runat="server" />
        </asp:PlaceHolder>
    </form>
</body>
</html>

<script>
    function showLoading() {
        document.getElementById('btnOK').disabled = true;
        document.getElementById('btnCancel').disabled = true;
        var loadingDiv = document.querySelector(".loadingDiv");
        loadingDiv.classList.remove("hide");
        loadingDiv.classList.add("show");
         __doPostBack('<%= btnOK.ClientID %>', '');
    }
</script>