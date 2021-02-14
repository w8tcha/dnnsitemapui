<%@ Control Language="c#" AutoEventWireup="True" Codebehind="Settings.ascx.cs" Inherits="WatchersNET.DNN.Modules.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<asp:panel id="pnlSettings" runat="server">
  <img id="SiteMapLogo" src="<%= this.ResolveUrl("SiteMap.gif")%>" alt="SiteMap Logo" title="SiteMap Logo" />
  <dnn:sectionhead id="dshVislOpt" runat="server" cssclass="Head" includerule="True" isExpanded="True" resourcekey="lVislOpt" section="tblVislOpt" />
    <table id="tblVislOpt" runat="server" cellspacing="0" cellpadding="0" style="margin-left:20px">
      <tr>
        <td>
          <div class="SiteMapSetting">
            <dnn:Label id="lblRender" runat="server" ResourceKey="lblRender" controlname="rBlRender" suffix=":" CssClass="SubHead"></dnn:Label>
            <asp:RadioButtonList AutoPostBack="true" OnSelectedIndexChanged="RBlRenderSelectedIndexChanged" ID="rBlRender" RepeatDirection="Horizontal" runat="server">
              <asp:ListItem Text="Normal" Selected="True" Value="normal"></asp:ListItem>
              <asp:ListItem Text="TreeView" Value="treeview"></asp:ListItem>
            </asp:RadioButtonList>
          </div>
          <div class="SiteMapSetting">
            <dnn:Label id="lblSkin" runat="server" ResourceKey="lblSkin" controlname="dDlSkins" suffix=":" CssClass="SubHead"></dnn:Label>
            <asp:DropDownList id="dDlSkins" OnSelectedIndexChanged="DDlSkinsChanged" AutoPostBack="true" Width="300px" runat="server"></asp:DropDownList>
          </div>
          <div style="clear:both" id="preview">
            <p><asp:Label ID="lPreview" runat="server" CssClass="SubHead"></asp:Label></p>
            <asp:Image ID="imgPreview" runat="server" />
          </div>
          <div class="SiteMapSetting">
            <dnn:Label id="lblShowInfo" runat="server" ResourceKey="lblShowInfo" controlname="cBShowInfo" suffix="?" CssClass="SubHead"></dnn:Label>
            <asp:CheckBox id="cBShowInfo" runat="server" />
          </div>
          <div class="SiteMapSetting">
            <dnn:Label id="lblDemoMode" runat="server" ResourceKey="lblDemoMode" controlname="cBDemoMode" suffix="?" CssClass="SubHead"></dnn:Label>
            <asp:CheckBox id="cBDemoMode" runat="server" />
          </div>
          <dnn:sectionhead id="dshTreeOpt" runat="server" cssclass="Head" includerule="True" isExpanded="False" resourcekey="lTreeOpt" section="tblTreeOpt" />
            <table id="tblTreeOpt" runat="server" cellspacing="0" cellpadding="0" style="margin-left:20px">
              <tr>
                <td>
                  <div class="SiteMapSetting">
                    <dnn:Label id="lblAnimated" runat="server" ResourceKey="lblAnimated" controlname="rBlAnimated" suffix=":" CssClass="SubHead"></dnn:Label>
                    <asp:RadioButtonList id="rBlAnimated" RepeatDirection="Horizontal" runat="server">
                      <asp:ListItem Text="Slow" Value="slow"></asp:ListItem>
                      <asp:ListItem Text="Normal" Value="normal"></asp:ListItem>
                      <asp:ListItem Text="Fast" Value="fast"></asp:ListItem>
                      <asp:ListItem Text="No" Value="false"></asp:ListItem>
                    </asp:RadioButtonList>
                  </div>
                  <div class="SiteMapSetting">
                    <dnn:Label id="lblCollapsed" runat="server" ResourceKey="lblCollapsed" controlname="rBlCollapsed" suffix=":" CssClass="SubHead"></dnn:Label>
                    <asp:RadioButtonList id="rBlCollapsed" RepeatDirection="Horizontal" runat="server">
                      <asp:ListItem Text="Yes" Value="true"></asp:ListItem>
                      <asp:ListItem Text="No" Value="false"></asp:ListItem>
                    </asp:RadioButtonList>
                  </div>
                  <div class="SiteMapSetting">
                    <dnn:Label id="lblUnique" runat="server" ResourceKey="lblUnique" controlname="rBlUnique" suffix=":" CssClass="SubHead"></dnn:Label>
                    <asp:RadioButtonList id="rBlUnique" RepeatDirection="Horizontal" runat="server">
                      <asp:ListItem Text="Yes" Value="true"></asp:ListItem>
                      <asp:ListItem Text="No" Value="false"></asp:ListItem>
                    </asp:RadioButtonList>
                  </div>
                  <div class="SiteMapSetting">
                    <dnn:Label id="lblPersist" runat="server" ResourceKey="lblPersist" controlname="rBlPersist" suffix=":" CssClass="SubHead"></dnn:Label>
                    <asp:RadioButtonList id="rBlPersist" RepeatDirection="Horizontal" runat="server">
                      <asp:ListItem Text="Location" Value="location"></asp:ListItem>
                      <asp:ListItem Text="Cookie" Value="cookie"></asp:ListItem>
                      <asp:ListItem Text="No" Value="false"></asp:ListItem>
                    </asp:RadioButtonList>
                  </div>
                  <div class="SiteMapSetting">
                    <dnn:Label id="lblRenderName" runat="server" ResourceKey="lblRenderName" controlname="rBlRenderName" suffix=":" CssClass="SubHead"></dnn:Label>
                    <asp:RadioButtonList id="rBlRenderName" RepeatDirection="Horizontal" runat="server">
                      <asp:ListItem Text="Yes" Value="true"></asp:ListItem>
                      <asp:ListItem Text="No" Value="false"></asp:ListItem>
                    </asp:RadioButtonList>
                  </div>
                </td>
              </tr>
            </table>
        </td>
      </tr>
    </table>    
  <dnn:sectionhead id="dshRenderOpt" runat="server" cssclass="Head" includerule="True" isExpanded="False" resourcekey="lRenderOpt" section="tblRenderOpt" />
    <table id="tblRenderOpt" runat="server" cellspacing="0" cellpadding="0" style="margin-left:20px">
      <tr>
        <td>
        <div class="SiteMapSetting">
          <dnn:Label id="lblRootLevel" runat="server" ResourceKey="lblRootLevel" controlname="dDlRootLevel" suffix=":" CssClass="SubHead"></dnn:Label>
          <asp:DropDownList ID="dDlRootLevel" Width="300px" AutoPostBack="true" OnSelectedIndexChanged="DDlRootLevelSelectedIndexChanged" runat="server"></asp:DropDownList>
        </div>
        <div class="SiteMapSetting">
          <dnn:Label id="lblRootTab" runat="server" ResourceKey="lblRootTab" controlname="dDlRootTab" suffix=":" CssClass="SubHead"></dnn:Label>
          <asp:DropDownList ID="dDlRootTab" AutoPostBack="false" Width="300px" runat="server"></asp:DropDownList>
        </div>
        <div class="SiteMapSetting">
          <dnn:Label id="lblMaxLevel" runat="server" ResourceKey="lblMaxLevel" controlname="tbMaxLevel" suffix=":" CssClass="SubHead"></dnn:Label>
          <asp:TextBox ID="tbMaxLevel" AutoPostBack="true" width="30px"  runat="server"></asp:TextBox>
          <asp:RangeValidator id="maxLevelValid" ControlToValidate="tbMaxLevel" Type="Integer" MinimumValue="-1" MaximumValue="5000" runat="server" ErrorMessage="RangeValidator"></asp:RangeValidator>
        </div>
        <div class="SiteMapSetting">
          <dnn:Label id="lblShowTabIcons" runat="server" ResourceKey="lblShowTabIcons" controlname="cBShowTabIcons" suffix="?" CssClass="SubHead"></dnn:Label>
          <asp:CheckBox id="cBShowTabIcons" runat="server" />
        </div>
        <div class="SiteMapSetting">
           <dnn:Label id="lblUseDefaultIcon" runat="server" ResourceKey="lblUseDefaultIcon" controlname="ctlDefaultIcon" suffix=":" CssClass="SubHead"></dnn:Label>
           <dnn:url id="ctlDefaultIcon" runat="server" width="300" showtabs="False" Required="False" showfiles="True" showUrls="False"
					urltype="F" showlog="False" shownewwindow="False" showtrack="False"></dnn:url>
        </div>
        <div class="SiteMapSetting">
          <dnn:Label id="lblShowHidden" runat="server" ResourceKey="lblShowHidden" controlname="cBShowHidden" suffix="?" CssClass="SubHead"></dnn:Label>
          <asp:CheckBox id="cBShowHidden" runat="server" />
        </div>
        <div class="SiteMapSetting">
          <dnn:Label id="lblHumanUrls" runat="server" ResourceKey="lblHumanUrls" controlname="cBHumanUrls" suffix="?" CssClass="SubHead"></dnn:Label>
          <asp:CheckBox id="cBHumanUrls" ResourceKey="lblHumanUrls" runat="server" />
        </div>
        </td>
      </tr>
    </table>
  <dnn:sectionhead id="dshExlOpt" runat="server" cssclass="Head" includerule="True" isExpanded="False" resourcekey="lExlOpt" section="tblExlOpt" />
    <table id="tblExlOpt" runat="server" cellspacing="0" cellpadding="0" style="margin-left:20px">
      <tr>
        <td>
          <div class="SiteMapSetting" id="ExludeDiv">
            <dnn:Label id="lblExcludeLst" runat="server" ResourceKey="lblExcludeLst" controlname="cblExcludeLst" suffix=":" CssClass="SubHead"></dnn:Label>
            <asp:Button id="btnSelectAll" runat="server" CssClass="CommandButton" />&nbsp;<asp:Button CssClass="CommandButton" id="btnSelectNone" runat="server" />
            <asp:CheckBoxList ID="cblExcludeLst" AutoPostBack="false" RepeatColumns="3" Height="200" runat="server"></asp:CheckBoxList>
          </div>
        </td>
      </tr>
    </table>
    <dnn:sectionhead id="dshTaxOpt" runat="server" cssclass="Head" includerule="True" isExpanded="False" resourcekey="lTaxOpt" section="tblTaxOpt" />
            <table id="tblTaxOpt" runat="server" cellspacing="0" cellpadding="0" style="margin-left:20px">
              <tr>
                <td>
                   <div class="SiteMapSetting">
                     <dnn:Label id="lblFilterByTax" runat="server"  ResourceKey="lblFilterByTax" controlname="cBFilterByTax" suffix="?" CssClass="SubHead"></dnn:Label>
                     <asp:CheckBox id="cBFilterByTax" runat="server" AutoPostBack="true" OnCheckedChanged="FilterByTaxChanged" />
                   </div>
                   <div class="SiteMapSetting">
                     <asp:DropDownList id="dDlTaxMode" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DDlTaxModeSelectedIndexChanged" Enabled="false">
                     </asp:DropDownList>
                   </div>
                   <div class="SiteMapSetting">
                     <dnn:label id="lblChooseVoc" runat="server"  ResourceKey="lblChooseVoc" controlname="cBlVocabularies" suffix=":" CssClass="SubHead"></dnn:label>
                     <asp:CheckBoxList ID="cBlVocabularies" runat="server" Enabled="false"></asp:CheckBoxList>
                   </div>
                   <div class="SiteMapSetting">
                     <dnn:label id="lblChooseTerms" runat="server"  ResourceKey="CustomTerms" controlname="cBlTerms" suffix=":" CssClass="SubHead"></dnn:label>
                     <asp:CheckBoxList ID="cBlTerms" runat="server" Enabled="false"></asp:CheckBoxList>
                   </div>
                 </td>
               </tr>
            </table>
  <asp:Label id="lblError" runat="server"></asp:Label>    
</asp:panel>
