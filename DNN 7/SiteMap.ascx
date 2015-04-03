<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="SiteMap.ascx.cs" Inherits="WatchersNET.DNN.Modules.SiteMap" %>

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

<asp:PlaceHolder ID="siteMapPlaceHolder" runat="server"></asp:PlaceHolder>
<br class="SiteMapClear" />
<asp:Label runat="server" CssClass="SiteMapFooter" ID="lblInfo"></asp:Label>
<br class="SiteMapClear" />
<asp:Label runat="server" ID="lblError"></asp:Label>
