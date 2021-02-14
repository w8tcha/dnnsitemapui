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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using DotNetNuke.Abstractions;
    using DotNetNuke.Abstractions.Portals;
    using DotNetNuke.Collections;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Content;
    using DotNetNuke.Entities.Content.Common;
    using DotNetNuke.Entities.Content.Taxonomy;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Security;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.FileSystem;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Web.Client.ClientResourceManagement;

    using Microsoft.Extensions.DependencyInjection;

    #endregion

    /// <summary>
    /// SiteMap Module
    /// </summary>
    public partial class SiteMap : PortalModuleBase
    {
        #region Constants and Fields

        /// <summary>
        /// The navigation manager.
        /// </summary>
        private readonly INavigationManager navigationManager;

        /// <summary>
        /// The tax terms.
        /// </summary>
        private List<Term> taxTerms;

        /// <summary>
        /// The demo mode.
        /// </summary>
        private bool demoMode;

        /// <summary>
        /// The filter by tax.
        /// </summary>
        private bool filterByTax;

        /// <summary>
        /// The human URLs.
        /// </summary>
        private bool humanUrls;

        /// <summary>
        /// The is skin changed.
        /// </summary>
        private bool isSkinChanged;

        /// <summary>
        /// The render name.
        /// </summary>
        private bool renderName;

        /// <summary>
        /// The show hidden.
        /// </summary>
        private bool showHidden;

        /// <summary>
        /// The show info.
        /// </summary>
        private bool showInfo;

        /// <summary>
        /// The show tab icons.
        /// </summary>
        private bool showTabIcons;

        /// <summary>
        /// The exclusion tabs.
        /// </summary>
        private string[] exclusionTabs;

        /// <summary>
        /// The max level.
        /// </summary>
        private int maxLevel;

        /// <summary>
        /// The items skins.
        /// </summary>
        private ListItemCollection itemsSkins;

        /// <summary>
        /// The items tree view.
        /// </summary>
        private ListItemCollection itemsTreeView;

        /// <summary>
        /// The animated.
        /// </summary>
        private string animated;

        /// <summary>
        /// The collapsed.
        /// </summary>
        private string collapsed;

        /// <summary>
        /// The default icon.
        /// </summary>
        private string defaultIcon;

        /// <summary>
        /// The s persist.
        /// </summary>
        private string persist;

        /// <summary>
        /// The render mode.
        /// </summary>
        private string renderMode;

        /// <summary>
        /// The root level.
        /// </summary>
        private string rootLevel;

        /// <summary>
        /// The root tab.
        /// </summary>
        private string rootTab;

        /// <summary>
        /// The skin name.
        /// </summary>
        private string skinName;

        /// <summary>
        /// The tax mode.
        /// </summary>
        private string taxMode;

        /// <summary>
        /// The unique.
        /// </summary>
        private string unique;

        /// <summary>
        /// The tab permissions.
        /// </summary>
        private TabPermissionCollection tabPermissions;

        /// <summary>
        /// The tabs.
        /// </summary>
        private List<TabInfo> tabs;

        /// <summary>
        /// The vocabularies.
        /// </summary>
        private string[] vocabularies;

        /// <summary>
        /// The terms.
        /// </summary>
        private string[] termsList;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMap"/> class.
        /// </summary>
        protected SiteMap()
        {
            this.navigationManager = this.DependencyProvider.GetRequiredService<INavigationManager>();
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The  Event Arguments.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.LoadSettings();

            if (this.showInfo)
            {
                this.ShowInfo();
            }
            else
            {
                this.lblInfo.Visible = false;
            }

            if (this.isSkinChanged/* || this.IsPostBack*/)
            {
                return;
            }

            if (!this.IsPostBack)
            {
                this.LoadAllSkins();
            }

            if (this.demoMode)
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
            if (!this.IsPostBack)
            {
                return;
            }

            this.renderMode = this.rBlRender.SelectedValue;

            this.FillSkinList(this.renderMode);

            this.skinName = this.dDlSkins.SelectedItem != null
                                 ? this.dDlSkins.SelectedItem.Text
                                 : this.dDlSkins.Items[0].Text;

            this.isSkinChanged = true;
            this.RenderSiteMap();
        }

        /// <summary>
        /// Switch Skin to Selected One
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The  Event Arguments.
        /// </param>
        protected void SwitchSkin(object sender, EventArgs e)
        {
            this.skinName = this.dDlSkins.SelectedItem.Text;

            this.renderMode = this.rBlRender.SelectedValue;

            this.isSkinChanged = true;
            this.RenderSiteMap();
        }

        /// <summary>
        /// Checks if the Tab should be filtered by Tax
        /// </summary>
        /// <param name="checkTab">
        /// Tab to Check
        /// </param>
        /// <returns>
        /// true or false
        /// </returns>
        private bool ExcludeByTax(ContentItem checkTab)
        {
            var exclude = true;

            if (!this.filterByTax)
            {
                return this.filterByTax;
            }

            try
            {
                if (checkTab.Terms.Count == 0)
                {
                    return true;
                }

                if (checkTab.Terms.Any(term1 => this.taxTerms.Find(t => t.Name.Equals(term1.Name)) != null))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                exclude = false;
            }

            return exclude;
        }

        /// <summary>
        /// Checks if the Tab is in the Excluded List
        /// </summary>
        /// <param name="checkTabId">
        /// Tab to Check
        /// </param>
        /// <returns>
        /// true or false
        /// </returns>
        private bool ExcludeTabId(IEquatable<int> checkTabId)
        {
            var exclude = false;

            try
            {
                if (this.exclusionTabs.Length != 0)
                {
                    if (this.exclusionTabs.Any(sExTabValue => checkTabId.Equals(int.Parse(sExTabValue))))
                    {
                        exclude = true;
                    }
                }
            }
            catch (Exception)
            {
                exclude = false;
            }

            return exclude;
        }

        /// <summary>
        /// Loads the List of available Skins.
        /// </summary>
        /// <param name="renderModus">
        /// The Render Modus.
        /// </param>
        private void FillSkinList(IEquatable<string> renderModus)
        {
            if (this.itemsTreeView == null || this.itemsSkins == null)
            {
                this.LoadAllSkins();
            }

            this.dDlSkins.DataSource = renderModus.Equals("treeview") ? this.itemsTreeView : this.itemsSkins;

            this.dDlSkins.DataBind();
        }

        /// <summary>
        /// The fill tabs.
        /// </summary>
        /// <returns>
        /// Tab List
        /// </returns>
        private List<TabInfo> FillTabs()
        {
            var allTabs = new List<TabInfo>();

            // Add Portal Tabs
            TabController.GetTabsBySortOrder(this.PortalId).ForEach(
                objPortalTab =>
                {
                    if (Null.IsNull(objPortalTab.StartDate))
                    {
                        objPortalTab.StartDate = DateTime.MinValue;
                    }

                    if (Null.IsNull(objPortalTab.EndDate))
                    {
                        objPortalTab.EndDate = DateTime.MaxValue;
                    }

                    allTabs.Add(objPortalTab);
                });

            // Add Host Tabs
            TabController.GetTabsBySortOrder(Null.NullInteger).ForEach(
                objHostTab =>
                {
                    objHostTab.PortalID = this.PortalId;

                    objHostTab.StartDate = DateTime.MinValue;
                    objHostTab.EndDate = DateTime.MaxValue;

                    allTabs.Add(objHostTab);
                });

            return allTabs;
        }

        /// <summary>
        /// Get DotNetNuke Taxonomy Terms
        /// </summary>
        /// <returns>
        /// Tax. Terms List
        /// </returns>
        private IEnumerable<Term> GetTerms()
        {
            var terms = new List<Term>();

            switch (this.taxMode)
            {
                case "tab":
                    {
                        terms = this.PortalSettings.ActiveTab.Terms;
                    }

                    break;
                case "all":
                    {
                        var termRep = Util.GetTermController();

                        var vocabRep = Util.GetVocabularyController();

                        var vocabulariesAll = from v in vocabRep.GetVocabularies()
                                                    where
                                                        v.ScopeType.ScopeType == "Application" ||
                                                        v.ScopeType.ScopeType == "Portal" && v.ScopeId == this.PortalId
                                                    select v;

                        vocabulariesAll.AsEnumerable().ForEach(
                            v => termRep.GetTermsByVocabulary(v.VocabularyId).AsEnumerable().ForEach(
                                t =>
                                {
                                    if (v.Type == VocabularyType.Simple)
                                    {
                                        t.ParentTermId = -v.VocabularyId;
                                    }

                                    terms.Add(t);
                                }));
                    }

                    break;
                case "custom":
                    {
                        if (this.vocabularies != null)
                        {
                            var termRep = Util.GetTermController();

                            this.vocabularies.ForEach(id => terms.AddRange(termRep.GetTermsByVocabulary(int.Parse(id))));
                        }
                    }

                    break;
                case "terms":
                    {
                        if (this.termsList != null)
                        {
                            var termRep = Util.GetTermController();

                            terms.AddRange(this.termsList.Select(sTermId => termRep.GetTerm(int.Parse(sTermId))));
                        }
                    }

                    break;
            }

            return terms;
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
            bool exclude;

            try
            {
                if (this.maxLevel.Equals(-1))
                {
                    exclude = false;
                }
                else if (checkTab.Level > this.maxLevel)
                {
                    exclude = true;
                }
                else
                {
                    exclude = false;
                }
            }
            catch (Exception)
            {
                exclude = false;
            }

            return exclude;
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
            bool notHidden;

            // If Option Show Hidden Tabs is turned on always return true
            if (this.showHidden)
            {
                return true;
            }

            try
            {
                notHidden = checkTab.IsVisible;
            }
            catch (Exception)
            {
                notHidden = true;
            }

            return notHidden;
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
                var objDir = new DirectoryInfo(this.MapPath(this.ResolveUrl("Skins")));

                objDir.GetDirectories().ForEach(
                    objSubFolder =>
                    {
                        if (Utility.IsSkinDirectory(objSubFolder.FullName))
                        {
                            var skinItem = new ListItem { Text = objSubFolder.Name, Value = objSubFolder.Name };

                            this.itemsSkins.Add(skinItem);
                        }
                        else if (Utility.IsSkinTreeDirectory(objSubFolder.FullName))
                        {
                            var skinItem = new ListItem { Text = objSubFolder.Name, Value = objSubFolder.Name };

                            this.itemsTreeView.Add(skinItem);
                        }
                    });
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
            this.tabs = this.FillTabs();

            var tabModuleSettings = this.ModuleConfiguration.TabModuleSettings;

            try
            {
                this.maxLevel = int.Parse(tabModuleSettings["sMaxLevel"].ToString());
            }
            catch (Exception)
            {
                this.maxLevel = -1;
            }

            try
            {
                this.showInfo = bool.Parse(tabModuleSettings["bShowInfo"].ToString());
            }
            catch (Exception)
            {
                this.showInfo = true;
            }

            // Demo Mode Setting
            if (!string.IsNullOrEmpty((string)tabModuleSettings["bDemoMode"]))
            {
                if (bool.TryParse((string)tabModuleSettings["bDemoMode"], out var result))
                {
                    this.demoMode = result;
                }
            }
            else
            {
                this.demoMode = false;
            }

            try
            {
                this.showHidden = bool.Parse(tabModuleSettings["bShowHidden"].ToString());
            }
            catch (Exception)
            {
                this.showHidden = false;
            }

            try
            {
                this.showTabIcons = bool.Parse(tabModuleSettings["bShowTabIcons"].ToString());
            }
            catch (Exception)
            {
                this.showTabIcons = false;
            }

            // Load Default Icon Setting
            try
            {
                if (!string.IsNullOrEmpty((string)tabModuleSettings["sDefaultIcon"]))
                {
                    this.defaultIcon = (string)tabModuleSettings["sDefaultIcon"];

                    var fileId = int.Parse(this.defaultIcon.Substring(7));

                    var fileInfo = FileManager.Instance.GetFile(fileId);

                    this.defaultIcon = this.PortalSettings.HomeDirectory + fileInfo.Folder + fileInfo.FileName;
                }
            }
            catch (Exception)
            {
                this.defaultIcon = string.Empty;
            }

            string excludeTabList;

            try
            {
                excludeTabList = (string)tabModuleSettings["exclusTabsLst"];
            }
            catch (Exception)
            {
                excludeTabList = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(excludeTabList))
                {
                    this.exclusionTabs = excludeTabList.Split(',');
                }
            }
            catch (Exception)
            {
                this.exclusionTabs = null;
            }

            // Load RenderMode
            try
            {
                this.renderMode = (string)tabModuleSettings["sRenderMode"];
            }
            catch (Exception)
            {
                this.renderMode = "normal";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.renderMode))
                {
                    this.renderMode = "normal";
                }
            }

            // Load TreeView Options
            // if (sRenderMode.Equals("treeview"))
            // {
            try
            {
                this.animated = (string)tabModuleSettings["sAnimated"];
            }
            catch (Exception)
            {
                this.animated = "normal";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.animated))
                {
                    this.animated = "normal";
                }
            }

            try
            {
                this.collapsed = (string)tabModuleSettings["bCollapsed"];
            }
            catch (Exception)
            {
                this.collapsed = "true";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.collapsed))
                {
                    this.collapsed = "true";
                }
            }

            try
            {
                this.unique = (string)tabModuleSettings["bUnique"];
            }
            catch (Exception)
            {
                this.unique = "true";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.unique))
                {
                    this.unique = "true";
                }
            }

            try
            {
                this.persist = (string)tabModuleSettings["sPersist"];
            }
            catch (Exception)
            {
                this.persist = "location";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.persist))
                {
                    this.persist = "location";
                }
            }

            try
            {
                this.renderName = bool.Parse(tabModuleSettings["bRenderName"].ToString());
            }
            catch (Exception)
            {
                this.renderName = true;
            }

            // Load Skin
            try
            {
                this.skinName = (string)tabModuleSettings["sSkin"];
            }
            catch (Exception)
            {
                this.skinName = "Default";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.skinName))
                {
                    this.skinName = "Default";
                }
            }

            // Load Root Level
            try
            {
                this.rootLevel = (string)tabModuleSettings["sRootLevel"];
            }
            catch (Exception)
            {
                this.rootLevel = "root";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.rootLevel))
                {
                    this.rootLevel = "root";
                }
            }

            // Load Root Tab
            try
            {
                this.rootTab = (string)tabModuleSettings["sRootTab"];
            }
            catch (Exception)
            {
                this.rootTab = "-1";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.rootLevel))
                {
                    this.rootLevel = "-1";
                }
            }

            // Load Human Friendly Urls Setting
            if (!string.IsNullOrEmpty((string)tabModuleSettings["bHumanUrls"]))
            {
                if (bool.TryParse((string)tabModuleSettings["bHumanUrls"], out var result))
                {
                    this.humanUrls = result;
                }
            }
            else
            {
                this.humanUrls = true;
            }

            // Load Tax Settings
            if (!string.IsNullOrEmpty((string)tabModuleSettings["bHumanUrls"]))
            {
                if (bool.TryParse((string)tabModuleSettings["bHumanUrls"], out var result))
                {
                    this.humanUrls = result;
                }
            }
            else
            {
                this.humanUrls = true;
            }

            if (!string.IsNullOrEmpty((string)tabModuleSettings["FilterByTax"]))
            {
                if (bool.TryParse((string)tabModuleSettings["FilterByTax"], out var result))
                {
                    this.filterByTax = result;
                }
            }
            else
            {
                this.filterByTax = false;
            }

            if (!string.IsNullOrEmpty((string)tabModuleSettings["TaxMode"]))
            {
                try
                {
                    this.taxMode = (string)tabModuleSettings["TaxMode"];
                }
                catch (Exception)
                {
                    this.filterByTax = false;
                }
            }
            else
            {
                this.filterByTax = false;
            }

            // Setting TaxMode Vocabularies
            if (!string.IsNullOrEmpty((string)tabModuleSettings["TaxVocabularies"]))
            {
                this.vocabularies = tabModuleSettings["TaxVocabularies"].ToString().Split(';');
            }

            // Setting TaxMode Terms
            if (string.IsNullOrEmpty((string)tabModuleSettings["TaxTerms"]))
            {
                return;
            }

            this.termsList = tabModuleSettings["TaxTerms"].ToString().Split(';');
        }

        /// <summary>
        /// Adding Script Links to HTML Header
        /// </summary>
        private void PlaceScriptLink()
        {
            var type = this.GetType();

            // jQuery Cookie Plugin
            ScriptManager.RegisterClientScriptInclude(
                this, type, "jqueryCookieScript", this.ResolveUrl("js/jquery.cookie.js"));

            // jQuery TreeView Plugin
            ScriptManager.RegisterClientScriptInclude(
                this, type, "jqueryTreeview", this.ResolveUrl("js/jquery.treeview.js"));
        }

        /// <summary>
        /// Adding Script Tag to HTML Header
        /// </summary>
        private void PlaceScriptTag()
        {
            var type = this.GetType();

            var anim = string.Empty;
            var litPersist = string.Empty;

            var header =
                $"jQuery(document).ready(function(){{ jQuery(\".{this.skinName}\").treeview({{collapsed: {this.collapsed},unique: {this.unique}";

            if (this.animated != "false")
            {
                anim = $",animated: \"{this.animated}\"";
            }

            switch (this.persist)
            {
                case "location":
                    litPersist = $",persist: \"{this.persist}\"";
                    break;
                case "cookie":
                    litPersist = $",persist: \"{this.persist}\",cookieId: \"SiteMap{this.TabId}\"";
                    break;
            }

            var name = $"TreeViewScript{Guid.NewGuid()}";
            var script = $"{header}{anim}{litPersist}}}); }});";

            ScriptManager.RegisterStartupScript(this, type, name, script, true);
        }

        /// <summary>
        /// Adding Skin Template CSS Link to HTML Header
        /// </summary>
        private void PlaceSkinStyleLink()
        {
            switch (this.renderMode)
            {
                case "normal":
                    ClientResourceManager.RegisterStyleSheet(
                        this.Page,
                        this.ResolveUrl($"{this.ResolveUrl("Skins/")}{this.skinName}/SiteMap.css"));
                    break;
                case "treeview":
                    ClientResourceManager.RegisterStyleSheet(
                        this.Page,
                        this.ResolveUrl($"{this.ResolveUrl("Skins/")}{this.skinName}/SiteMapTree.css"));
                    break;
            }
        }

        /// <summary>
        /// Renders the Demo Controls (Skin Selection Drop Down)
        /// </summary>
        private void RenderDemoControls()
        {
            this.panDemoMode.Visible = true;

            if (this.IsPostBack || this.isSkinChanged)
            {
                return;
            }

            this.rBlRender.Items.FindByValue(this.renderMode).Selected = true;

            this.FillSkinList(this.renderMode);

            this.dDlSkins.Items.FindByText(this.skinName).Selected = true;
        }

        /// <summary>
        /// Renders the SiteMap
        /// </summary>
        /// <param name="ctlParentLi">
        /// Parent LI
        /// </param>
        /// <param name="parentTabId">
        /// Parent TabID
        /// </param>
        private void RenderLevel(Control ctlParentLi, int parentTabId)
        {
            HtmlGenericControl ctlUl = null;

            foreach (var objTab in this.tabs)
            {
                this.tabPermissions = TabPermissionController.GetTabPermissions(objTab.TabID, this.PortalId);

                if (!objTab.ParentId.Equals(parentTabId) || objTab.IsDeleted || !this.IsNotHidden(objTab) ||
                    objTab.IsDeleted || !PortalSecurity.IsInRoles(this.tabPermissions.ToString("VIEW")) ||
                    objTab.StartDate >= DateTime.Now || objTab.EndDate <= DateTime.Now || this.IsMaxLevel(objTab) ||
                    this.ExcludeTabId(objTab.TabID) || this.ExcludeByTax(objTab))
                {
                    continue;
                }

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
                        var ctlMainDiv = new HtmlGenericControl("div");

                        ctlMainDiv.Attributes["class"] = $"SiteMap-{this.skinName}";

                        this.siteMapPlaceHolder.Controls.Add(ctlMainDiv);

                        ctlUl.Attributes["class"] = this.skinName;

                        // siteMapPlaceHolder.Controls.Add(ctlUl);
                        ctlMainDiv.Controls.Add(ctlUl);
                    }
                }

                // Menu Tab Item (li)
                var ctlMenu = new HtmlGenericControl("li");

                if (objTab.Level.Equals(0))
                {
                    ctlMenu.Attributes["class"] = "SubLi";
                }

                ctlUl.Controls.Add(ctlMenu);

                // Menu Tab Item (link)
                var ctlAnchor = new HtmlAnchor();

                var decodedTabName = this.Server.HtmlDecode(objTab.LocalizedTabName);

                if (!objTab.DisableLink)
                {
                    ctlAnchor.HRef = objTab.FullUrl;

                    // Add Friendly URLS
                    if (this.humanUrls)
                    {
                        ctlAnchor.HRef = Globals.FriendlyUrl(
                            objTab, Globals.ApplicationURL(objTab.TabID), (IPortalSettings)this.PortalSettings);
                    }
                }

                ctlAnchor.Title = decodedTabName;

                ctlAnchor.InnerText = decodedTabName;

                // Menu Tab Icon
                if (!string.IsNullOrEmpty(objTab.IconFile) && this.showTabIcons)
                {
                    ctlAnchor.InnerHtml =
                        $"<img src=\"{this.SetIconPath(objTab)}\" class=\"tabIcon\" alt=\"{decodedTabName}\" width=\"16px\" height=\"16px\" />{ctlAnchor.InnerText}";

                    if (objTab.HasChildren)
                    {
                        ctlAnchor.Attributes["class"] = "SubItem";
                    }

                    ctlMenu.Attributes["class"] += " hasIcon";
                }
                else
                {
                    if (this.showTabIcons && !string.IsNullOrEmpty(this.defaultIcon))
                    {
                        ctlAnchor.InnerHtml =
                            $"<img src=\"{this.ResolveUrl(this.defaultIcon)}\" class=\"tabIcon\" alt=\"{decodedTabName}\" width=\"16px\" height=\"16px\" />{ctlAnchor.InnerText}";

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
            var ctlMainDiv = new HtmlGenericControl("div");

            ctlMainDiv.Attributes["class"] = "SiteMap-" + this.skinName;

            this.siteMapPlaceHolder.Controls.Add(ctlMainDiv);

            var ctlUl = new HtmlGenericControl("ul");

            ctlUl.Attributes["class"] = this.skinName;

            ctlMainDiv.Controls.Add(ctlUl);

            var ctlMainMenu = new HtmlGenericControl("li");

            ctlUl.Controls.Add(ctlMainMenu);

            var aliasInfo = this.PortalSettings.PortalAlias as IPortalAliasInfo;

            var ctlAnchorWeb = new HtmlAnchor
                {
                    Title = this.PortalSettings.PortalName, 
                    InnerHtml = $"<strong><em>{this.PortalSettings.PortalName}</em></strong>", 
                    HRef =
                        $"{Globals.GetPortalDomainName(aliasInfo.HttpAlias, null, true)}/{Globals.glbDefaultPage}"
                };

            ctlMainMenu.Controls.Add(ctlAnchorWeb);

            return ctlMainMenu;
        }

        /// <summary>
        /// Renders the SiteMap
        /// </summary>
        private void RenderSiteMap()
        {
            // Reset First
            this.siteMapPlaceHolder.Controls.Clear();

            this.PlaceSkinStyleLink();

            if (this.renderMode.Equals("treeview"))
            {
                this.PlaceScriptLink();

                this.PlaceScriptTag();
            }

            var tabActive = this.SetActiveTab();

            if (this.filterByTax)
            {
                this.taxTerms = this.GetTerms().ToList();
            }

            ////////////////////////////////////////////////
            HtmlGenericControl ctlMain = null;

            if (this.renderMode.Equals("treeview") && this.renderName)
            {
                ctlMain = this.RenderMainTree();
            }

            ////////////////////////////////////////////////

            // Render Menu
            try
            {
                switch (this.rootLevel)
                {
                    case "parent" when tabActive.ParentId.Equals(-1):
                        // Renders from Level : Root if No Parent
                        this.RenderLevel(ctlMain, -1);
                        break;
                    case "parent":
                        // Renders from Level : Parent
                        this.tabs.Where(objTestTab => objTestTab.TabID.Equals(tabActive.ParentId)).ForEach(
                            objTestTab => this.RenderLevel(ctlMain, objTestTab.ParentId));
                        break;
                    case "current":
                        // Renders from Level : Same (current)
                        this.RenderLevel(ctlMain, tabActive.ParentId);
                        break;
                    // Renders from Level : Children
                    case "children" when tabActive.HasChildren:
                        this.RenderLevel(ctlMain, tabActive.TabID);
                        break;
                    case "children":
                        // Renders from Level : Root if No Children
                        this.RenderLevel(ctlMain, -1);
                        break;
                    case "custom":
                        // Renders from Level : Custom
                        this.RenderLevel(ctlMain, tabActive.ParentId.Equals(-1) ? tabActive.TabID : tabActive.ParentId);
                        break;
                    default:
                        // Renders from Level : Root
                        // RenderLevel(null, -1);
                        this.RenderLevel(ctlMain, -1);
                        break;
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
            TabInfo activeTab;

            if (this.rootLevel.Equals("custom"))
            {
                try
                {
                    activeTab = this.tabs.FirstOrDefault(objTestTab => objTestTab.TabID.ToString().Equals(this.rootTab));
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

            return tab.IsSuperTab || tab.IsSecure
                       ? this.ResolveUrl($"{Globals.ApplicationPath}/images/{tab.IconFile}")
                       : this.ResolveUrl(Path.Combine(this.PortalSettings.HomeDirectory, tab.IconFile));
        }

        /// <summary>
        /// Shows Copyright Info on Footer
        /// </summary>
        private void ShowInfo()
        {
            var objDesktopModule =
                DesktopModuleController.GetDesktopModuleByModuleName("WatchersNET - SiteMap", this.PortalId);

            this.lblInfo.Text = string.Format(
                Localization.GetString("Copyright.Text", this.LocalResourceFile),
                objDesktopModule != null ? objDesktopModule.Version : string.Empty);
        }

        #endregion
    }
}