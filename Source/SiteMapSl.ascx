<%@ Control Language="c#" AutoEventWireup="True" Codebehind="SiteMapSl.ascx.cs" Inherits="WatchersNET.DNN.Modules.SiteMapSl" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>

<asp:Panel id="panDemoMode" runat="server"  CssClass="SiteMapSkinSelector" Visible="false">
  <asp:Label id="lblSkin" runat="server" ResourceKey="lblSkin"></asp:Label>:&nbsp;<asp:DropDownList id="dDlSkins" Width="300px" runat="server"></asp:DropDownList>&nbsp;&nbsp;
  <asp:Button id="btnSwitch" runat="server" CssClass="SiteMapButton" OnClick="SwitchSkin" Text="Switch Skin" />
  <div class="SiteMapRenderMode">
   <asp:Label id="lblRenderMode" runat="server"  ResourceKey="lblRenderMode"></asp:Label>:&nbsp;
   <asp:RadioButtonList AutoPostBack="true" OnSelectedIndexChanged="RBlRenderSelectedIndexChanged" ID="rBlRender" RepeatDirection="Horizontal" runat="server">
      <asp:ListItem Text="Normal" Value="normal"></asp:ListItem>
      <asp:ListItem Text="TreeView" Value="treeview"></asp:ListItem>
    </asp:RadioButtonList>
  </div>
</asp:Panel>

<asp:placeholder id="siteMapPlaceHolder" runat="server"></asp:placeholder>