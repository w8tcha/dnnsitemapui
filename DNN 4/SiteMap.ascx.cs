/*  **********************************************************
*                                                            *
*   WatchersNET.SiteMap - A Modern SiteMap / TreeView        *
*   Copyright(c) Ingo Herbote                                *
*   All rights reserved.                                     *
*   Ingo Herbote (thewatcher@watchersnet.de)                 *
*   Internet: http://www.watchersnet.de/SiteMap              *
*                                                            *
*************************************************************/

namespace WatchersNET.DNN.Modules
{
    #region

    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using DotNetNuke.Common;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Framework;
    using DotNetNuke.Security;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.FileSystem;
    using DotNetNuke.Services.Localization;

    using FileInfo = DotNetNuke.Services.FileSystem.FileInfo;

    #endregion

    /// <summary>
    /// SiteMap Module
    /// </summary>
    public partial class SiteMap : PortalModuleBase
    {
        #region Constants and Fields

        /// <summary>
        /// The b demo mode.
        /// </summary>
        private bool bDemoMode;

        /// <summary>
        /// The b human urls.
        /// </summary>
        private bool bHumanUrls;

        /// <summary>
        /// The b is skin changed.
        /// </summary>
        private bool bIsSkinChanged;

        /// <summary>
        /// The b render name.
        /// </summary>
        private bool bRenderName;

        /// <summary>
        /// The b show hidden.
        /// </summary>
        private bool bShowHidden;

        /// <summary>
        /// The b show info.
        /// </summary>
        private bool bShowInfo;

        /// <summary>
        /// The b show tab icons.
        /// </summary>
        private bool bShowTabIcons;

        /// <summary>
        /// The exclusion tabs.
        /// </summary>
        private string[] exclusionTabs;

        /// <summary>
        /// The i max level.
        /// </summary>
        private int iMaxLevel;

        /// <summary>
        /// The items skins.
        /// </summary>
        private ListItemCollection itemsSkins;

        /// <summary>
        /// The items tree view.
        /// </summary>
        private ListItemCollection itemsTreeView;

        /// <summary>
        /// The s animated.
        /// </summary>
        private string sAnimated;

        /// <summary>
        /// The s collapsed.
        /// </summary>
        private string sCollapsed;

        /// <summary>
        /// The s default icon.
        /// </summary>
        private string sDefaultIcon;

        /// <summary>
        /// The s persist.
        /// </summary>
        private string sPersist;

        /// <summary>
        /// The s render mode.
        /// </summary>
        private string sRenderMode;

        /// <summary>
        /// The s root level.
        /// </summary>
        private string sRootLevel;

        /// <summary>
        /// The s root tab.
        /// </summary>
        private string sRootTab;

        /// <summary>
        /// The s skin name.
        /// </summary>
        private string sSkinName;

        /// <summary>
        /// The s unique.
        /// </summary>
        private string sUnique;

        #endregion

        #region Methods

        /// <summary>
        /// Load Page
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Event Args e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.LoadSettings();

            if (this.bShowInfo)
            {
                this.ShowInfo();
            }
            else
            {
                this.lblInfo.Visible = false;
            }

            if (this.bIsSkinChanged/* || this.IsPostBack*/)
            {
                return;
            }

            if (!this.IsPostBack)
            {
                this.LoadAllSkins();
            }

            if (this.bDemoMode)
            {
                this.RenderDemoControls();
            }

            this.RenderSiteMap();
        }

        /// <summary>
        /// Reloads the Skin list based on Selected Render Mode
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void RBlRenderSelectedIndexChanged(object sender, EventArgs e)
        {
            this.sRenderMode = this.rBlRender.SelectedValue;

            this.FillSkinList(this.sRenderMode);

            this.sSkinName = this.dDlSkins.SelectedItem != null
                                 ? this.dDlSkins.SelectedItem.Text
                                 : this.dDlSkins.Items[0].Text;

            this.bIsSkinChanged = true;
            this.RenderSiteMap();
        }

        /// <summary>
        /// Switch Skin to Selected One
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void SwitchSkin(object sender, EventArgs e)
        {
            this.sSkinName = this.dDlSkins.SelectedItem.Text;

            this.sRenderMode = this.rBlRender.SelectedValue;

            this.bIsSkinChanged = true;
            this.RenderSiteMap();
        }

        /// <summary>
        /// Checks if the Tab is in the Excluded List
        /// </summary>
        /// <param name="iCheckTabId">
        /// Tab to Check
        /// </param>
        /// <returns>
        /// true or false
        /// </returns>
        private bool ExcludeTabId(IEquatable<int> iCheckTabId)
        {
            bool bExclude = false;

            try
            {
                if (this.exclusionTabs.Length != 0)
                {
                    if (this.exclusionTabs.Any(sExTabValue => iCheckTabId.Equals(int.Parse(sExTabValue))))
                    {
                        bExclude = true;
                    }
                }
            }
            catch (Exception)
            {
                bExclude = false;
            }

            return bExclude;
        }

        /// <summary>
        /// Loads the List of available Skins.
        /// </summary>
        /// <param name="sRenderModus">
        /// The s Render Modus.
        /// </param>
        private void FillSkinList(IEquatable<string> sRenderModus)
        {
            if (this.itemsTreeView == null || this.itemsSkins == null)
            {
                this.LoadAllSkins();
            }

            this.dDlSkins.DataSource = sRenderModus.Equals("treeview") ? this.itemsTreeView : this.itemsSkins;

            this.dDlSkins.DataBind();
        }

        /// <summary>
        /// Check if Tab Reaches the Max. Level Setting
        /// </summary>
        /// <param name="checkTab">
        /// Tab to Check
        /// </param>
        /// <returns>
        /// The is max level.
        /// </returns>
        private bool IsMaxLevel(TabInfo checkTab)
        {
            bool bExclude;

            try
            {
                if (this.iMaxLevel.Equals(-1))
                {
                    bExclude = false;
                }
                else if (checkTab.Level > this.iMaxLevel)
                {
                    bExclude = true;
                }
                else
                {
                    bExclude = false;
                }
            }
            catch (Exception)
            {
                bExclude = false;
            }

            return bExclude;
        }

        /// <summary>
        /// Checks if Tab is a Invisible Tab
        /// </summary>
        /// <param name="checkTab">
        /// Tab to Check
        /// </param>
        /// <returns>
        /// The is not hidden.
        /// </returns>
        private bool IsNotHidden(TabInfo checkTab)
        {
            bool bNotHidden;

            // If Option Show Hidden Tabs is turned on always return true
            if (this.bShowHidden)
            {
                return true;
            }

            try
            {
                bNotHidden = checkTab.IsVisible;
            }
            catch (Exception)
            {
                bNotHidden = true;
            }

            return bNotHidden;
        }

        /// <summary>
        /// Load all Skins
        /// </summary>
        private void LoadAllSkins()
        {
            this.itemsSkins = new ListItemCollection();
            this.itemsTreeView = new ListItemCollection();

            try
            {
                DirectoryInfo objDir = new DirectoryInfo(this.MapPath(this.ResolveUrl("Skins")));

                foreach (DirectoryInfo objSubFolder in objDir.GetDirectories())
                {
                    ListItem skinItem = new ListItem { Text = objSubFolder.Name, Value = objSubFolder.Name };

                    if (Utility.IsSkinDirectory(objSubFolder.FullName))
                    {
                        this.itemsSkins.Add(skinItem);
                    }

                    if (Utility.IsSkinTreeDirectory(objSubFolder.FullName))
                    {
                        this.itemsTreeView.Add(skinItem);
                    }
                }
            }
            catch (Exception)
            {
                var skinItem = new ListItem
                                        {
                                            Text = Localization.GetString("None.Text", this.LocalResourceFile),
                                            Value = "None"
                                        };

                this.itemsSkins.Add(skinItem);
                this.itemsTreeView.Add(skinItem);
            }
        }

        /// <summary>
        /// Loads the Module Settings
        /// </summary>
        private void LoadSettings()
        {
            ModuleController objModuleController = new ModuleController();

            Hashtable tabModuleSettings = objModuleController.GetTabModuleSettings(this.TabModuleId);

            try
            {
                string sMaxLevel = (string)tabModuleSettings["sMaxLevel"];
                this.iMaxLevel = int.Parse(sMaxLevel);
            }
            catch (Exception)
            {
                this.iMaxLevel = -1;
            }

            try
            {
                string sShowInfo = (string)tabModuleSettings["bShowInfo"];
                this.bShowInfo = bool.Parse(sShowInfo);
            }
            catch (Exception)
            {
                this.bShowInfo = true;
            }

            // Demo Mode Setting
            if (!string.IsNullOrEmpty((string)tabModuleSettings["bDemoMode"]))
            {
                bool bResult;
                if (bool.TryParse((string)tabModuleSettings["bDemoMode"], out bResult))
                {
                    this.bDemoMode = bResult;
                }
            }
            else
            {
                this.bDemoMode = false;
            }

            try
            {
                string sShowHidden = (string)tabModuleSettings["bShowHidden"];
                this.bShowHidden = bool.Parse(sShowHidden);
            }
            catch (Exception)
            {
                this.bShowHidden = false;
            }

            try
            {
                string sShowTabIcons = (string)tabModuleSettings["bShowTabIcons"];
                this.bShowTabIcons = bool.Parse(sShowTabIcons);
            }
            catch (Exception)
            {
                this.bShowTabIcons = false;
            }

            // Load Default Icon Setting
            try
            {
                if (!string.IsNullOrEmpty((string)tabModuleSettings["sDefaultIcon"]))
                {
                    this.sDefaultIcon = (string)tabModuleSettings["sDefaultIcon"];

                    int iFileId = int.Parse(this.sDefaultIcon.Substring(7));

                    FileController objFileController = new FileController();

                    FileInfo objFileInfo = objFileController.GetFileById(iFileId, this.PortalSettings.PortalId);

                    this.sDefaultIcon = this.PortalSettings.HomeDirectory + objFileInfo.Folder + objFileInfo.FileName;
                }
            }
            catch (Exception)
            {
                this.sDefaultIcon = string.Empty;
            }

            string sExlTabLst;

            try
            {
                sExlTabLst = (string)tabModuleSettings["exclusTabsLst"];
            }
            catch (Exception)
            {
                sExlTabLst = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(sExlTabLst))
                {
                    this.exclusionTabs = sExlTabLst.Split(',');
                }
            }
            catch (Exception)
            {
                this.exclusionTabs = null;
            }

            // Load RenderMode
            try
            {
                this.sRenderMode = (string)tabModuleSettings["sRenderMode"];
            }
            catch (Exception)
            {
                this.sRenderMode = "normal";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.sRenderMode))
                {
                    this.sRenderMode = "normal";
                }
            }

            // Load TreeView Options
            // if (sRenderMode.Equals("treeview"))
            // {
            try
            {
                this.sAnimated = (string)tabModuleSettings["sAnimated"];
            }
            catch (Exception)
            {
                this.sAnimated = "normal";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.sAnimated))
                {
                    this.sAnimated = "normal";
                }
            }

            try
            {
                this.sCollapsed = (string)tabModuleSettings["bCollapsed"];
            }
            catch (Exception)
            {
                this.sCollapsed = "true";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.sCollapsed))
                {
                    this.sCollapsed = "true";
                }
            }

            try
            {
                this.sUnique = (string)tabModuleSettings["bUnique"];
            }
            catch (Exception)
            {
                this.sUnique = "true";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.sUnique))
                {
                    this.sUnique = "true";
                }
            }

            try
            {
                this.sPersist = (string)tabModuleSettings["sPersist"];
            }
            catch (Exception)
            {
                this.sPersist = "location";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.sPersist))
                {
                    this.sPersist = "location";
                }
            }

            try
            {
                string sRenderName = (string)tabModuleSettings["bRenderName"];
                this.bRenderName = bool.Parse(sRenderName);
            }
            catch (Exception)
            {
                this.bRenderName = true;
            }

            // }

            // Load Skin
            try
            {
                this.sSkinName = (string)tabModuleSettings["sSkin"];
            }
            catch (Exception)
            {
                this.sSkinName = "Default";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.sSkinName))
                {
                    this.sSkinName = "Default";
                }
            }

            // Load Root Level
            try
            {
                this.sRootLevel = (string)tabModuleSettings["sRootLevel"];
            }
            catch (Exception)
            {
                this.sRootLevel = "root";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.sRootLevel))
                {
                    this.sRootLevel = "root";
                }
            }

            // Load Root Tab
            try
            {
                this.sRootTab = (string)tabModuleSettings["sRootTab"];
            }
            catch (Exception)
            {
                this.sRootTab = "-1";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.sRootLevel))
                {
                    this.sRootLevel = "-1";
                }
            }

            // Load Human Friendly Urls Setting
            if (!string.IsNullOrEmpty((string)tabModuleSettings["bHumanUrls"]))
            {
                bool bResult;
                if (bool.TryParse((string)tabModuleSettings["bHumanUrls"], out bResult))
                {
                    this.bHumanUrls = bResult;
                }
            }
            else
            {
                this.bHumanUrls = true;
            }
        }

        /// <summary>
        /// Adding Script Links to HTML Header
        /// </summary>
        private void PlaceScriptLink()
        {
            Type csType = this.GetType();

            // JQuery JS
            if (HttpContext.Current.Items["jquery_registered"] == null)
            {
                ScriptManager.RegisterClientScriptInclude(
                    this, csType, "jquery", "http://ajax.googleapis.com/ajax/libs/jquery/1/jquery.min.js");

                HttpContext.Current.Items.Add("jquery_registered", "true");
            }

            // jQuery Cookie Plugin
            ScriptManager.RegisterClientScriptInclude(
                this, csType, "jqueryCookieScript", this.ResolveUrl("js/jquery.cookie.js"));

            // jQuery Treeview Plugin
            ScriptManager.RegisterClientScriptInclude(
                this, csType, "jqueryTreeview", this.ResolveUrl("js/jquery.treeview.js"));
        }

        /// <summary>
        /// Adding Script Tag to HTML Header
        /// </summary>
        private void PlaceScriptTag()
        {
            ClientScriptManager cs = this.Page.ClientScript;
            Type csType = this.GetType();

            string sLitAnim = string.Empty;
            string sLitPersist = string.Empty;

            string sLitHeader =
                string.Format(
                    "jQuery(document).ready(function(){{ jQuery(\".{0}\").treeview({{collapsed: {1},unique: {2}", 
                    this.sSkinName, 
                    this.sCollapsed, 
                    this.sUnique);

            if (this.sAnimated != "false")
            {
                sLitAnim = string.Format(",animated: \"{0}\"", this.sAnimated);
            }

            if (this.sPersist.Equals("location"))
            {
                sLitPersist = string.Format(",persist: \"{0}\"", this.sPersist);
            }
            else if (this.sPersist.Equals("cookie"))
            {
                sLitPersist = string.Format(",persist: \"{0}\",cookieId: \"SiteMap{1}\"", this.sPersist, this.TabId);
            }

            string sCsName = string.Format("TreeViewScript{0}", Guid.NewGuid());

            if (cs.IsClientScriptBlockRegistered(csType, sCsName))
            {
                return;
            }

            string sScript = string.Format("{0}{1}{2}}}); }});", sLitHeader, sLitAnim, sLitPersist);

            ScriptManager.RegisterStartupScript(this, csType, sCsName, sScript, true);
        }

        /// <summary>
        /// Adding Skin Template CSS Link to HTML Header
        /// </summary>
        private void PlaceSkinStyleLink()
        {
            if (this.sRenderMode.Equals("normal"))
            {
                ((CDefault)this.Page).AddStyleSheet(
                    string.Format("SiteMapCss{0}", this.sSkinName), 
                    this.ResolveUrl(string.Format("{0}{1}/SiteMap.css", this.ResolveUrl("Skins/"), this.sSkinName)));
            }
            else if (this.sRenderMode.Equals("treeview"))
            {
                ((CDefault)this.Page).AddStyleSheet(
                    string.Format("SiteMapCss{0}", this.sSkinName), 
                    this.ResolveUrl(string.Format("{0}{1}/SiteMapTree.css", this.ResolveUrl("Skins/"), this.sSkinName)));
            }
        }

        /// <summary>
        /// Renders the Demo Controls (Skin Selection Drop Down)
        /// </summary>
        private void RenderDemoControls()
        {
            this.panDemoMode.Visible = true;

            if (this.IsPostBack || this.bIsSkinChanged)
            {
                return;
            }

            this.rBlRender.Items.FindByValue(this.sRenderMode).Selected = true;

            this.FillSkinList(this.sRenderMode);

            this.dDlSkins.Items.FindByText(this.sSkinName).Selected = true;
        }

        /// <summary>
        /// Renders the SiteMap
        /// </summary>
        /// <param name="ctlParentLi">
        /// Parent LI
        /// </param>
        /// <param name="iParentTabId">
        /// Parent TabID
        /// </param>
        private void RenderLevel(Control ctlParentLi, int iParentTabId)
        {
            HtmlGenericControl ctlUl = null;

            foreach (TabInfo objTab in
                this.PortalSettings.DesktopTabs.Cast<TabInfo>().Where(objTab => (((((((objTab.ParentId.Equals(iParentTabId) && !objTab.IsDeleted) && !objTab.IsDeleted) && this.IsNotHidden(objTab)) && PortalSecurity.IsInRoles(objTab.AuthorizedRoles)) && objTab.StartDate < DateTime.Now) && objTab.EndDate > DateTime.Now) && !this.IsMaxLevel(objTab)) && !this.ExcludeTabId(objTab.TabID)))
            {
                if (ctlUl == null)
                {
                    ctlUl = new HtmlGenericControl("ul");

                    if (ctlParentLi != null)
                    {
                        ctlUl.Attributes["class"] = "SubUl";

                        ctlParentLi.Controls.Add(ctlUl);
                    }
                    else
                    {
                        // Add Skin Name to Main List
                        /*if (sRenderMode.Equals("treeview"))
                                {
                                    ctlUl.Attributes["class"] = sSkinName;
                                }*/
                        HtmlGenericControl ctlMainDiv = new HtmlGenericControl("div");

                        ctlMainDiv.Attributes["class"] = string.Format("SiteMap-{0}", this.sSkinName);

                        this.siteMapPlaceHolder.Controls.Add(ctlMainDiv);

                        ctlUl.Attributes["class"] = this.sSkinName;

                        // siteMapPlaceHolder.Controls.Add(ctlUl);
                        ctlMainDiv.Controls.Add(ctlUl);
                    }
                }

                // Menu Tab Item (li)
                HtmlGenericControl ctlMenu = new HtmlGenericControl("li");

                if (objTab.Level.Equals(0))
                {
                    ctlMenu.Attributes["class"] = "SubLi";
                }

                ctlUl.Controls.Add(ctlMenu);

                // Menu Tab Item (link)
                HtmlAnchor ctlAnchor = new HtmlAnchor();

                var decodedTabName = Server.HtmlDecode(objTab.TabName);

                if (!objTab.DisableLink)
                {
                    ctlAnchor.HRef = objTab.FullUrl;

                    // Add Friendly URLS
                    if (this.bHumanUrls)
                    {
                        ctlAnchor.HRef = Globals.FriendlyUrl(
                            objTab, Globals.ApplicationURL(objTab.TabID), this.PortalSettings);
                    }
                }

                ctlAnchor.Title = decodedTabName;
                ctlAnchor.InnerText = decodedTabName;

                // Menu Tab Icon
                if (!string.IsNullOrEmpty(objTab.IconFile) && this.bShowTabIcons)
                {
                    ctlAnchor.InnerHtml =
                        string.Format(
                            "<img src=\"{0}\" class=\"tabIcon\" alt=\"{1}\" width=\"16px\" height=\"16px\" />{2}", 
                            this.SetIconPath(objTab),
                            decodedTabName, 
                            ctlAnchor.InnerText);

                    if (objTab.HasChildren)
                    {
                        ctlAnchor.Attributes["class"] = "SubItem";
                    }

                    ctlMenu.Attributes["class"] += " hasIcon";
                }
                else
                {
                    if (this.bShowTabIcons && !string.IsNullOrEmpty(this.sDefaultIcon))
                    {
                        ctlAnchor.InnerHtml =
                            string.Format(
                                "<img src=\"{0}\" class=\"tabIcon\" alt=\"{1}\" width=\"16px\" height=\"16px\" />{2}", 
                                this.ResolveUrl(this.sDefaultIcon),
                                decodedTabName,
                                ctlAnchor.InnerText);

                        ctlMenu.Attributes["class"] = " hasIcon";
                    }
                    else
                    {
                        if (objTab.HasChildren)
                        {
                            ctlAnchor.Attributes["class"] = "SubItem";
                        }
                        else
                        {
                            ctlAnchor.InnerText = decodedTabName;
                        }
                    }
                }

                ctlMenu.Controls.Add(ctlAnchor);

                this.RenderLevel(ctlMenu, objTab.TabID);
            }
        }

        /// <summary>
        /// Adds Website name as Root Element of the SiteMap Tree
        /// </summary>
        /// <returns>
        /// Main UL Control
        /// </returns>
        private HtmlGenericControl RenderMainTree()
        {
            HtmlGenericControl ctlMainDiv = new HtmlGenericControl("div");

            ctlMainDiv.Attributes["class"] = "SiteMap-" + this.sSkinName;

            this.siteMapPlaceHolder.Controls.Add(ctlMainDiv);

            HtmlGenericControl ctlUl = new HtmlGenericControl("ul");

            ctlUl.Attributes["class"] = this.sSkinName;

            ctlMainDiv.Controls.Add(ctlUl);

            HtmlGenericControl ctlMainMenu = new HtmlGenericControl("li");

            ctlUl.Controls.Add(ctlMainMenu);

            HtmlAnchor ctlAnchorWeb = new HtmlAnchor
                {
                    Title = this.PortalSettings.PortalName, 
                    InnerHtml = string.Format("<strong><em>{0}</em></strong>", this.PortalSettings.PortalName), 
                    HRef =
                        string.Format(
                            "{0}/{1}", 
                            Globals.GetPortalDomainName(this.PortalSettings.PortalAlias.HTTPAlias), 
                            Globals.glbDefaultPage)
                };

            ctlMainMenu.Controls.Add(ctlAnchorWeb);

            return ctlMainMenu;
        }

        /// <summary>
        /// Renders The SiteMap
        /// </summary>
        private void RenderSiteMap()
        {
            // Reset First
            this.siteMapPlaceHolder.Controls.Clear();

            this.PlaceSkinStyleLink();

            if (this.sRenderMode.Equals("treeview"))
            {
                this.PlaceScriptLink();

                this.PlaceScriptTag();
            }

            TabInfo tabActive = this.SetActiveTab();

            ////////////////////////////////////////////////
            HtmlGenericControl ctlMain = null;

            if (this.sRenderMode.Equals("treeview") && this.bRenderName)
            {
                ctlMain = this.RenderMainTree();
            }

            ////////////////////////////////////////////////

            // Render Menu
            try
            {
                if (this.sRootLevel.Equals("parent"))
                {
                    if (tabActive.ParentId.Equals(-1))
                    {
                        // Renders from Level : Root if No Parent
                        this.RenderLevel(ctlMain, -1);
                    }
                    else
                    {
                        // Renders from Level : Parent
                        foreach (TabInfo objTestTab in
                            this.PortalSettings.DesktopTabs.Cast<TabInfo>().Where(objTestTab => objTestTab.TabID.Equals(tabActive.ParentId)))
                        {
                            this.RenderLevel(ctlMain, objTestTab.ParentId);
                        }
                    }
                }
                else if (this.sRootLevel.Equals("current"))
                {
                    // Renders from Level : Same (current)
                    this.RenderLevel(ctlMain, tabActive.ParentId);
                }
                else if (this.sRootLevel.Equals("children"))
                {
                    // Renders from Level : Children
                    if (tabActive.HasChildren)
                    {
                        this.RenderLevel(ctlMain, tabActive.TabID);
                    }
                    else
                    {
                        // Renders from Level : Root if No Childs
                        this.RenderLevel(ctlMain, -1);
                    }
                }
                else if (this.sRootLevel.Equals("custom"))
                {
                    // Renders from Level : Custom
                    this.RenderLevel(ctlMain, tabActive.ParentId.Equals(-1) ? tabActive.TabID : tabActive.ParentId);
                }
                else
                {
                    // Renders from Level : Root
                    // RenderLevel(null, -1);
                    this.RenderLevel(ctlMain, -1);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);

                this.lblError.Text = Localization.GetString("lblError.Text", this.LocalResourceFile);
            }
        }

        /// <summary>
        /// Set the Active Tab
        /// </summary>
        /// <returns>
        /// Returns Active Tab
        /// </returns>
        private TabInfo SetActiveTab()
        {
            TabInfo activeTab = new TabInfo();

            if (this.sRootLevel.Equals("custom"))
            {
                try
                {
                    foreach (TabInfo objTestTab in
                        this.PortalSettings.DesktopTabs.Cast<TabInfo>().Where(objTestTab => objTestTab.TabID.ToString().Equals(this.sRootTab)))
                    {
                        activeTab = objTestTab;
                    }
                }
                catch (Exception)
                {
                    activeTab = this.PortalSettings.ActiveTab;
                }
            }
            else
            {
                activeTab = this.PortalSettings.ActiveTab;
            }

            return activeTab;
        }

        /// <summary>
        /// Set the Correct Image Icon Path
        /// </summary>
        /// <param name="tab">
        /// Tab to Check
        /// </param>
        /// <returns>
        /// The Final Resolved Image Path
        /// </returns>
        private string SetIconPath(TabInfo tab)
        {
            if (tab.IconFile.StartsWith("~"))
            {
                return this.ResolveUrl(tab.IconFile);
            }

            return tab.IsSuperTab || tab.IsAdminTab
                       ? this.ResolveUrl(string.Format("{0}/images/{1}", Globals.ApplicationPath, tab.IconFile))
                       : this.ResolveUrl(Path.Combine(this.PortalSettings.HomeDirectory, tab.IconFile));
        }

        /// <summary>
        /// Shows Copyright Info on Footer
        /// </summary>
        private void ShowInfo()
        {
            DesktopModuleController objDesktopModules = new DesktopModuleController();
            DesktopModuleInfo objDesktopModule = objDesktopModules.GetDesktopModuleByName("WatchersNET - SiteMap");

            this.lblInfo.Text = string.Format(
                Localization.GetString("Copyright.Text", this.LocalResourceFile),
                objDesktopModule != null ? objDesktopModule.Version : string.Empty);
        }

        #endregion
    }
}