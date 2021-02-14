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
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using DotNetNuke.Abstractions.Portals;
    using DotNetNuke.Collections;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Content;
    using DotNetNuke.Entities.Content.Common;
    using DotNetNuke.Entities.Content.Taxonomy;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Security;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.UI.Skins;
    using DotNetNuke.Web.Client.ClientResourceManagement;

    #endregion

    /// <summary>
    /// SiteMap Skin Object
    /// </summary>
    public partial class SiteMapSl : SkinObjectBase
    {
        #region Constants and Fields

        /// <summary>
        /// The tax terms.
        /// </summary>
        private List<Term> taxTerms;

        /// <summary>
        /// The is skin changed.
        /// </summary>
        private bool isSkinChanged;

        /// <summary>
        /// The exclusion tabs.
        /// </summary>
        private IList<string> exclusionTabs;

        /// <summary>
        /// The Include Only Tabs.
        /// </summary>
        private IList<string> inclusionTabs;

        /// <summary>
        /// The items skins.
        /// </summary>
        private ListItemCollection itemsSkins;

        /// <summary>
        /// The items tree view.
        /// </summary>
        private ListItemCollection itemsTreeView;

        /// <summary>
        /// The tab permissions.
        /// </summary>
        private TabPermissionCollection tabPermissions;

        /// <summary>
        /// The tax vocabularies.
        /// </summary>
        private string[] taxVocabularies;

        /// <summary>
        /// The terms.
        /// </summary>
        private string[] terms;

        /// <summary>
        /// The tabs.
        /// </summary>
        private List<TabInfo> tabs;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Animated.
        /// </summary>
        public string Animated { get; set; } = "normal";

        /// <summary>
        /// Gets or sets Collapsed.
        /// </summary>
        public string Collapsed { get; set; } = "true";

        /// <summary>
        /// Gets or sets DefaultIcon.
        /// </summary>
        public string DefaultIcon { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether DemoMode.
        /// </summary>
        public bool DemoMode { get; set; }

        /// <summary>
        /// Gets or sets ExclusionTabs.
        /// </summary>
        public string ExclusionTabs { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets InclusionTabs.
        /// </summary>
        public string InclusionTabs { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether FilterByTax.
        /// </summary>
        public bool FilterByTax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Human URLs.
        /// </summary>
        public bool HumanUrls { get; set; }

        /// <summary>
        /// Gets or sets MaxLevel.
        /// </summary>
        public int MaxLevel { get; set; } = -1;

        /// <summary>
        /// Gets or sets Persist.
        /// </summary>
        public string Persist { get; set; } = "location";

        /// <summary>
        /// Gets or sets RenderMode.
        /// </summary>
        public string RenderMode { get; set; } = "normal";

        /// <summary>
        /// Gets or sets a value indicating whether RenderName.
        /// </summary>
        public bool RenderName { get; set; }

        /// <summary>
        /// Gets or sets RootLevel.
        /// </summary>
        public string RootLevel { get; set; } = "root";

        /// <summary>
        /// Gets or sets RootTab.
        /// </summary>
        public string RootTab { get; set; } = "-1";

        /// <summary>
        /// Gets or sets a value indicating whether ShowHidden.
        /// </summary>
        public bool ShowHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ShowTabIcons.
        /// </summary>
        public bool ShowTabIcons { get; set; }

        /// <summary>
        /// Gets or sets SkinName.
        /// </summary>
        public string SkinName { get; set; } = "Default";

        /// <summary>
        /// Gets or sets TaxMode.
        /// </summary>
        public string TaxMode { get; set; } = "all";

        /// <summary>
        /// Gets or sets TaxVocabularies.
        /// </summary>
        public string TaxVocabularies { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Tax Terms.
        /// </summary>
        public string TaxTerms { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Unique.
        /// </summary>
        public string Unique { get; set; } = "true";

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.ExclusionTabs))
                {
                    this.exclusionTabs = this.ExclusionTabs.Split(',');
                }
            }
            catch (Exception)
            {
                this.exclusionTabs = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(this.InclusionTabs))
                {
                    this.inclusionTabs = this.InclusionTabs.Split(',');
                }
            }
            catch (Exception)
            {
                this.inclusionTabs = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(this.TaxVocabularies))
                {
                    this.taxVocabularies = this.TaxVocabularies.Split(',');
                }
            }
            catch (Exception)
            {
                this.taxVocabularies = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(this.TaxTerms))
                {
                    this.terms = this.TaxTerms.Split(',');
                }
            }
            catch (Exception)
            {
                this.terms = null;
            }

            if (this.isSkinChanged/* || this.IsPostBack*/)
            {
                return;
            }

            if (!this.IsPostBack)
            {
                this.LoadAllSkins();
            }

            if (this.DemoMode)
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

            this.RenderMode = this.rBlRender.SelectedValue;

            this.FillSkinList(this.RenderMode);

            this.SkinName = this.dDlSkins.SelectedItem != null
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
        /// The Event Args e.
        /// </param>
        protected void SwitchSkin(object sender, EventArgs e)
        {
            this.SkinName = this.dDlSkins.SelectedItem.Text;

            this.RenderMode = this.rBlRender.SelectedValue;

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

            if (!this.FilterByTax)
            {
                return this.FilterByTax;
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
                if (this.exclusionTabs.Count > 0)
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
        /// Checks if the Tab is in the Include List
        /// </summary>
        /// <param name="checkTabId">
        /// Tab to Check
        /// </param>
        /// <returns>
        /// true or false
        /// </returns>
        private bool IncludeTabId(IEquatable<int> checkTabId)
        {
            var include = false;

            try
            {
                if (this.inclusionTabs.Count > 0)
                {
                    include = this.inclusionTabs.ToList().Find(sInTabValue => checkTabId.Equals(int.Parse(sInTabValue))) == null;
                }
            }
            catch (Exception)
            {
                include = false;
            }

            return include;
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
        /// Loads all Portal Tabs and Host Tabs
        /// </summary>
        /// <returns>
        /// All Tabs for the Portal
        /// </returns>
        private List<TabInfo> FillTabs()
        {
            var allTabs = new List<TabInfo>();

            // Add Portal Tabs
            TabController.GetTabsBySortOrder(this.PortalSettings.PortalId).ForEach(objPortalTab => 
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
                    objHostTab.PortalID = this.PortalSettings.PortalId;

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
        /// Tax Terms List
        /// </returns>
        private IEnumerable<Term> GetTerms()
        {
            var tabTerms = new List<Term>();

            switch (this.TaxMode)
            {
                case "tab":
                    {
                        tabTerms = this.PortalSettings.ActiveTab.Terms;
                    }

                    break;
                case "all":
                    {
                        var termRep = Util.GetTermController();

                        var vocabRep = Util.GetVocabularyController();

                        var vocabulariesAll = from v in vocabRep.GetVocabularies()
                                                    where
                                                        v.ScopeType.ScopeType == "Application" ||
                                                        v.ScopeType.ScopeType == "Portal" &&
                                                        v.ScopeId == this.PortalSettings.PortalId
                                                    select v;

                        vocabulariesAll.AsEnumerable().ForEach(
                            v => termRep.GetTermsByVocabulary(v.VocabularyId).AsEnumerable().ForEach(t =>
                            {
                                if (v.Type == VocabularyType.Simple)
                                {
                                    t.ParentTermId = -v.VocabularyId;
                                }

                                tabTerms.Add(t);
                            }));
                    }

                    break;
                case "custom":
                    {
                        if (this.taxVocabularies != null)
                        {
                            var termRep = Util.GetTermController();

                            this.taxVocabularies.ForEach(
                                id => tabTerms.AddRange(termRep.GetTermsByVocabulary(int.Parse(id))));
                        }
                    }

                    break;
                case "terms":
                    {
                        if (this.terms != null)
                        {
                            var vocabRep = Util.GetVocabularyController();

                            var vs = from v in vocabRep.GetVocabularies()
                                                        where
                                                            v.ScopeType.ScopeType == "Application" ||
                                                            v.ScopeType.ScopeType == "Portal" && v.ScopeId == this.PortalSettings.PortalId
                                                        select v;

                            var allTerms = new List<Term>();

                            vs.AsEnumerable().ForEach(v => allTerms.AddRange(v.Terms));

                            tabTerms.AddRange(this.terms.Select(term => allTerms.Find(t => t.Name.Equals(term))));
                        }
                    }

                    break;
            }

            return tabTerms;
        }

        /// <summary>
        /// The initialize component.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblRenderMode.Text = Localization.GetString(
                "lblRenderMode.Text", Localization.GetResourceFile(this, "SiteMapSl.ascx"));

            this.lblSkin.Text = Localization.GetString(
                "lblSkin.Text", Localization.GetResourceFile(this, "SiteMapSl.ascx"));
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
                if (this.MaxLevel.Equals(-1))
                {
                    exclude = false;
                }
                else if (checkTab.Level > this.MaxLevel)
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
            if (this.ShowHidden)
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

                objDir.GetDirectories().ForEach(objSubFolder =>
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
                                       Text =
                                           Localization.GetString(
                                               "None.Text",
                                               Localization.GetResourceFile(this, "SiteMapSl.ascx")),
                                       Value = "None"
                                   };

                this.itemsSkins.Add(skinItem);
                this.itemsTreeView.Add(skinItem);
            }
        }

        /// <summary>
        /// Adding Script Links to HTML Header
        /// </summary>
        private void PlaceScriptLink()
        {
            var type = this.GetType();

            // JQuery JS
            if (HttpContext.Current.Items["jquery_registered"] == null)
            {
                ScriptManager.RegisterClientScriptInclude(
                    this, type, "jquery", "http://ajax.googleapis.com/ajax/libs/jquery/1/jquery.min.js");

                HttpContext.Current.Items.Add("jquery_registered", "true");
            }

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
            var persist = string.Empty;

            var header =
                $"jQuery(document).ready(function(){{ jQuery(\".{this.SkinName}\").treeview({{collapsed: {this.Collapsed},unique: {this.Unique}";

            if (this.Animated != "false")
            {
                anim = $",animated: \"{this.Animated}\"";
            }

            switch (this.Persist)
            {
                case "location":
                    persist = $",persist: \"{this.Persist}\"";
                    break;
                case "cookie":
                    persist =
                        $",persist: \"{this.Persist}\",cookieId: \"SiteMap{this.PortalSettings.ActiveTab.TabID}\"";
                    break;
            }

            var name = $"TreeViewScript{Guid.NewGuid()}";

            var script = $"{header}{anim}{persist}}}); }});";

            ScriptManager.RegisterStartupScript(this, type, name, script, true);
        }

        /// <summary>
        /// Adding Skin Template CSS Link to HTML Header
        /// </summary>
        private void PlaceSkinStyleLink()
        {
            switch (this.RenderMode)
            {
                case "normal":
                    ClientResourceManager.RegisterStyleSheet(
                        this.Page,
                        this.ResolveUrl($"{this.ResolveUrl("Skins/")}{this.SkinName}/SiteMap.css"));
                    break;
                case "treeview":
                    ClientResourceManager.RegisterStyleSheet(
                        this.Page,
                        this.ResolveUrl($"{this.ResolveUrl("Skins/")}{this.SkinName}/SiteMapTree.css"));
                    break;
            }
        }

        /// <summary>
        /// Renders the Demo Controls (Skin Selection Drop Down)
        /// </summary>
        private void RenderDemoControls()
        {
            this.panDemoMode.Visible = true;

            if (this.IsPostBack)
            {
                return;
            }

            this.rBlRender.Items.FindByValue(this.RenderMode).Selected = true;

            this.FillSkinList(this.RenderMode);

            this.dDlSkins.Items.FindByText(this.SkinName).Selected = true;
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
                this.tabPermissions = TabPermissionController.GetTabPermissions(
                    objTab.TabID, this.PortalSettings.PortalId);

                if (!objTab.ParentId.Equals(parentTabId) || objTab.IsDeleted || objTab.IsDeleted ||
                    !this.IsNotHidden(objTab) || !PortalSecurity.IsInRoles(this.tabPermissions.ToString("VIEW")) ||
                    objTab.StartDate >= DateTime.Now || objTab.EndDate <= DateTime.Now || this.IsMaxLevel(objTab) ||
                    this.ExcludeTabId(objTab.TabID) || this.ExcludeByTax(objTab) || this.IncludeTabId(objTab.TabID))
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
                        // Add Skin Name to Main List
                        /*if (sRenderMode.Equals("treeview"))
                            {
                                ctlUl.Attributes["class"] = sSkinName;
                            }*/
                        var ctlMainDiv = new HtmlGenericControl("div");

                        ctlMainDiv.Attributes["class"] = $"SiteMap-{this.SkinName}";

                        this.siteMapPlaceHolder.Controls.Add(ctlMainDiv);

                        ctlUl.Attributes["class"] = this.SkinName;

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
                    if (this.HumanUrls)
                    {
                        ctlAnchor.HRef = Globals.FriendlyUrl(
                            objTab, Globals.ApplicationURL(objTab.TabID), (IPortalSettings)this.PortalSettings);
                    }
                }

                ctlAnchor.Title = decodedTabName;
                ctlAnchor.InnerText = decodedTabName;

                // Menu Tab Icon
                if (!string.IsNullOrEmpty(objTab.IconFile) && this.ShowTabIcons)
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
                    if (this.ShowTabIcons && !string.IsNullOrEmpty(this.DefaultIcon))
                    {
                        ctlAnchor.InnerHtml =
                            $"<img src=\"{this.ResolveUrl(Path.Combine(this.PortalSettings.HomeDirectory, this.DefaultIcon))}\" class=\"tabIcon\" alt=\"{decodedTabName}\" width=\"16px\" height=\"16px\" />{ctlAnchor.InnerText}";

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

            ctlMainDiv.Attributes["class"] = "SiteMap-" + this.SkinName;

            this.siteMapPlaceHolder.Controls.Add(ctlMainDiv);

            var ctlUl = new HtmlGenericControl("ul");

            ctlUl.Attributes["class"] = this.SkinName;

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

            this.tabs = this.FillTabs();

            this.PlaceSkinStyleLink();

            if (this.RenderMode.Equals("treeview"))
            {
                this.PlaceScriptLink();

                this.PlaceScriptTag();
            }

            var tabActive = this.SetActiveTab();

            if (this.FilterByTax)
            {
                this.taxTerms = this.GetTerms().ToList();
            }

            ////////////////////////////////////////////////
            HtmlGenericControl ctlMain = null;

            if (this.RenderMode.Equals("treeview") && this.RenderName)
            {
                ctlMain = this.RenderMainTree();
            }

            ////////////////////////////////////////////////

            // Render Menu
            try
            {
                switch (this.RootLevel)
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

                // ContainerControl.Visible = false;
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
            var activeTab = new TabInfo();

            if (this.RootLevel.Equals("custom"))
            {
                try
                {
                    activeTab = this.tabs.FirstOrDefault(objTestTab => objTestTab.TabID.ToString().Equals(this.RootTab));
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

        #endregion
    }
}