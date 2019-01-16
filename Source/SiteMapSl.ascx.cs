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

    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
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
        /// The demo mode.
        /// </summary>
        private bool bDemoMode;

        /// <summary>
        /// The filter by tax.
        /// </summary>
        private bool bFilterByTax;

        /// <summary>
        /// The human URLs.
        /// </summary>
        private bool bHumanUrls;

        /// <summary>
        /// The is skin changed.
        /// </summary>
        private bool bIsSkinChanged;

        /// <summary>
        /// The render name.
        /// </summary>
        private bool bRenderName;

        /// <summary>
        /// The show hidden.
        /// </summary>
        private bool bShowHidden;

        /// <summary>
        /// The show tab icons.
        /// </summary>
        private bool bShowTabIcons;

        /// <summary>
        /// The exclusion tabs.
        /// </summary>
        private IList<string> exclusionTabs;

        /// <summary>
        /// The Include Only Tabs.
        /// </summary>
        private IList<string> inclusionTabs;

        /// <summary>
        /// The i max level.
        /// </summary>
        private int iMaxLevel = -1;

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
        private string sAnimated = "normal";

        /// <summary>
        /// The s collapsed.
        /// </summary>
        private string sCollapsed = "true";

        /// <summary>
        /// The s default icon.
        /// </summary>
        private string sDefaultIcon = string.Empty;

        /// <summary>
        /// The s exclusion tabs.
        /// </summary>
        private string sExclusionTabs = string.Empty;

        /// <summary>
        /// The Inclusion tabs.
        /// </summary>
        private string sInclusionTabs = string.Empty;

        /// <summary>
        /// The s persist.
        /// </summary>
        private string sPersist = "location";

        /// <summary>
        /// The s render mode.
        /// </summary>
        private string sRenderMode = "normal";

        /// <summary>
        /// The s root level.
        /// </summary>
        private string sRootLevel = "root";

        /// <summary>
        /// The s root tab.
        /// </summary>
        private string sRootTab = "-1";

        /// <summary>
        /// The s skin name.
        /// </summary>
        private string sSkinName = "Default";

        /// <summary>
        /// The s tax mode.
        /// </summary>
        private string sTaxMode = "all";

        /// <summary>
        /// The s tax vocabularies.
        /// </summary>
        private string sTaxVocabularies = string.Empty;

        /// <summary>
        /// The s tax terms.
        /// </summary>
        private string sTaxTerms = string.Empty;

        /// <summary>
        /// The s unique.
        /// </summary>
        private string sUnique = "true";

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
        /// The ti tabs.
        /// </summary>
        private List<TabInfo> tiTabs;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Animated.
        /// </summary>
        public string Animated
        {
            get
            {
                return this.sAnimated;
            }

            set
            {
                this.sAnimated = value;
            }
        }

        /// <summary>
        /// Gets or sets Collapsed.
        /// </summary>
        public string Collapsed
        {
            get
            {
                return this.sCollapsed;
            }

            set
            {
                this.sCollapsed = value;
            }
        }

        /// <summary>
        /// Gets or sets DefaultIcon.
        /// </summary>
        public string DefaultIcon
        {
            get
            {
                return this.sDefaultIcon;
            }

            set
            {
                this.sDefaultIcon = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether DemoMode.
        /// </summary>
        public bool DemoMode
        {
            get
            {
                return this.bDemoMode;
            }

            set
            {
                this.bDemoMode = value;
            }
        }

        /// <summary>
        /// Gets or sets ExclusionTabs.
        /// </summary>
        public string ExclusionTabs
        {
            get
            {
                return this.sExclusionTabs;
            }

            set
            {
                this.sExclusionTabs = value;
            }
        }

        /// <summary>
        /// Gets or sets InclusionTabs.
        /// </summary>
        public string InclusionTabs
        {
            get
            {
                return this.sInclusionTabs;
            }

            set
            {
                this.sInclusionTabs = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether FilterByTax.
        /// </summary>
        public bool FilterByTax
        {
            get
            {
                return this.bFilterByTax;
            }

            set
            {
                this.bFilterByTax = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Human URLs.
        /// </summary>
        public bool HumanUrls
        {
            get
            {
                return this.bHumanUrls;
            }

            set
            {
                this.bHumanUrls = value;
            }
        }

        /// <summary>
        /// Gets or sets MaxLevel.
        /// </summary>
        public int MaxLevel
        {
            get
            {
                return this.iMaxLevel;
            }

            set
            {
                this.iMaxLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets Persist.
        /// </summary>
        public string Persist
        {
            get
            {
                return this.sPersist;
            }

            set
            {
                this.sPersist = value;
            }
        }

        /// <summary>
        /// Gets or sets RenderMode.
        /// </summary>
        public string RenderMode
        {
            get
            {
                return this.sRenderMode;
            }

            set
            {
                this.sRenderMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether RenderName.
        /// </summary>
        public bool RenderName
        {
            get
            {
                return this.bRenderName;
            }

            set
            {
                this.bRenderName = value;
            }
        }

        /// <summary>
        /// Gets or sets RootLevel.
        /// </summary>
        public string RootLevel
        {
            get
            {
                return this.sRootLevel;
            }

            set
            {
                this.sRootLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets RootTab.
        /// </summary>
        public string RootTab
        {
            get
            {
                return this.sRootTab;
            }

            set
            {
                this.sRootTab = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowHidden.
        /// </summary>
        public bool ShowHidden
        {
            get
            {
                return this.bShowHidden;
            }

            set
            {
                this.bShowHidden = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowTabIcons.
        /// </summary>
        public bool ShowTabIcons
        {
            get
            {
                return this.bShowTabIcons;
            }

            set
            {
                this.bShowTabIcons = value;
            }
        }

        /// <summary>
        /// Gets or sets SkinName.
        /// </summary>
        public string SkinName
        {
            get
            {
                return this.sSkinName;
            }

            set
            {
                this.sSkinName = value;
            }
        }

        /// <summary>
        /// Gets or sets TaxMode.
        /// </summary>
        public string TaxMode
        {
            get
            {
                return this.sTaxMode;
            }

            set
            {
                this.sTaxMode = value;
            }
        }

        /// <summary>
        /// Gets or sets TaxVocabularies.
        /// </summary>
        public string TaxVocabularies
        {
            get
            {
                return this.sTaxVocabularies;
            }

            set
            {
                this.sTaxVocabularies = value;
            }
        }

        /// <summary>
        /// Gets or sets Tax Terms.
        /// </summary>
        public string TaxTerms
        {
            get
            {
                return this.sTaxTerms;
            }

            set
            {
                this.sTaxTerms = value;
            }
        }

        /// <summary>
        /// Gets or sets Unique.
        /// </summary>
        public string Unique
        {
            get
            {
                return this.sUnique;
            }

            set
            {
                this.sUnique = value;
            }
        }

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
                if (!string.IsNullOrEmpty(this.sExclusionTabs))
                {
                    this.exclusionTabs = this.sExclusionTabs.Split(',');
                }
            }
            catch (Exception)
            {
                this.exclusionTabs = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(this.sInclusionTabs))
                {
                    this.inclusionTabs = this.sInclusionTabs.Split(',');
                }
            }
            catch (Exception)
            {
                this.inclusionTabs = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(this.sTaxVocabularies))
                {
                    this.taxVocabularies = this.sTaxVocabularies.Split(',');
                }
            }
            catch (Exception)
            {
                this.taxVocabularies = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(this.sTaxTerms))
                {
                    this.terms = this.sTaxTerms.Split(',');
                }
            }
            catch (Exception)
            {
                this.terms = null;
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
            if (!this.IsPostBack)
            {
                return;
            }

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
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Event Args e.
        /// </param>
        protected void SwitchSkin(object sender, EventArgs e)
        {
            this.sSkinName = this.dDlSkins.SelectedItem.Text;

            this.sRenderMode = this.rBlRender.SelectedValue;

            this.bIsSkinChanged = true;
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
        private bool ExcludeByTax(TabInfo checkTab)
        {
            var bExclude = true;

            if (!this.bFilterByTax)
            {
                return this.bFilterByTax;
            }

            try
            {
                if (checkTab.Terms.Count == 0)
                {
                    return true;
                }

                /*if (checkTab.Terms.Any(term => this.taxTerms.Where(tax => tax.Name.Equals(term.Name)).Count() == 0))
                {
                    return true;
                }*/

                if (checkTab.Terms.Any(term1 => this.taxTerms.Find(t => t.Name.Equals(term1.Name)) != null))
                {
                    return false;
                }

                // bExclude = this.taxTerms.Find(tax => objTab.LocalizedTabName.ToLower().Contains(tax.Name.ToLower()) || checkTab.Title.ToLower().Contains(tax.Name.ToLower())) == null;
            }
            catch (Exception)
            {
                bExclude = false;
            }

            return bExclude;
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
            var bExclude = false;

            try
            {
                if (this.exclusionTabs.Count > 0)
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
        /// Checks if the Tab is in the Include List
        /// </summary>
        /// <param name="iCheckTabId">
        /// Tab to Check
        /// </param>
        /// <returns>
        /// true or false
        /// </returns>
        private bool IncludeTabId(IEquatable<int> iCheckTabId)
        {
            var bInclude = false;

            try
            {
                if (this.inclusionTabs.Count > 0)
                {
                    bInclude = this.inclusionTabs.ToList().Find(sInTabValue => iCheckTabId.Equals(int.Parse(sInTabValue))) == null;
                }
            }
            catch (Exception)
            {
                bInclude = false;
            }

            return bInclude;
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
        /// Loads all Portal Tabs and Host Tabs
        /// </summary>
        /// <returns>
        /// All Tabs for the Portal
        /// </returns>
        private List<TabInfo> FillTabs()
        {
            var tiAllTabs = new List<TabInfo>();

            // Add Portal Tabs
            foreach (var objPortalTab in TabController.GetTabsBySortOrder(this.PortalSettings.PortalId).Select(objTab => objTab.Clone()))
            {
                if (Null.IsNull(objPortalTab.StartDate))
                {
                    objPortalTab.StartDate = DateTime.MinValue;
                }

                if (Null.IsNull(objPortalTab.EndDate))
                {
                    objPortalTab.EndDate = DateTime.MaxValue;
                }

                tiAllTabs.Add(objPortalTab);
            }

            // Add Host Tabs
            foreach (var objHostTab in TabController.GetTabsBySortOrder(Null.NullInteger).Select(objTab => objTab.Clone()))
            {
                objHostTab.PortalID = this.PortalSettings.PortalId;

                objHostTab.StartDate = DateTime.MinValue;
                objHostTab.EndDate = DateTime.MaxValue;

                tiAllTabs.Add(objHostTab);
            }

            return tiAllTabs;
        }

        /// <summary>
        /// Get DotNetNuke Taxonomy Terms
        /// </summary>
        /// <returns>
        /// Tax Terms List
        /// </returns>
        private IEnumerable<Term> GetTerms()
        {
            var termslist = new List<Term>();

            switch (this.sTaxMode)
            {
                case "tab":
                    {
                        termslist = this.PortalSettings.ActiveTab.Terms;
                    }

                    break;
                case "all":
                    {
                        var termRep = Util.GetTermController();

                        var vocabRep = Util.GetVocabularyController();

                        var vs = from v in vocabRep.GetVocabularies()
                                                    where
                                                        v.ScopeType.ScopeType == "Application" ||
                                                        (v.ScopeType.ScopeType == "Portal" &&
                                                         v.ScopeId == this.PortalSettings.PortalId)
                                                    select v;

                        foreach (var v in vs)
                        {
                            foreach (var t in termRep.GetTermsByVocabulary(v.VocabularyId))
                            {
                                if (v.Type == VocabularyType.Simple)
                                {
                                    t.ParentTermId = -v.VocabularyId;
                                }

                                termslist.Add(t);
                            }
                        }
                    }

                    break;
                case "custom":
                    {
                        if (this.taxVocabularies != null)
                        {
                            var termRep = Util.GetTermController();

                            foreach (var sVocabularyId in this.taxVocabularies)
                            {
                                termslist.AddRange(termRep.GetTermsByVocabulary(int.Parse(sVocabularyId)));
                            }
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
                                                            (v.ScopeType.ScopeType == "Portal" && v.ScopeId == this.PortalSettings.PortalId)
                                                        select v;

                            var allTerms = new List<Term>();

                            foreach (var v in vs)
                            {
                                 allTerms.AddRange(v.Terms);
                            }

                            termslist.AddRange(this.terms.Select(term => allTerms.Find(t => t.Name.Equals(term))));
                        }
                    }

                    break;
            }

            return termslist;
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
                var objDir = new DirectoryInfo(this.MapPath(this.ResolveUrl("Skins")));

                foreach (var objSubFolder in objDir.GetDirectories())
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
                }
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
            var csType = this.GetType();

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
            var csType = this.GetType();

            var sLitAnim = string.Empty;
            var sLitPersist = string.Empty;

            var sLitHeader =
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
                sLitPersist = string.Format(
                    ",persist: \"{0}\",cookieId: \"SiteMap{1}\"", this.sPersist, this.PortalSettings.ActiveTab.TabID);
            }

            var sCsName = string.Format("TreeViewScript{0}", Guid.NewGuid());

            var sScript = string.Format("{0}{1}{2}}}); }});", sLitHeader, sLitAnim, sLitPersist);

            ScriptManager.RegisterStartupScript(this, csType, sCsName, sScript, true);
        }

        /// <summary>
        /// Adding Skin Template CSS Link to HTML Header
        /// </summary>
        private void PlaceSkinStyleLink()
        {
            if (this.sRenderMode.Equals("normal"))
            {
                ClientResourceManager.RegisterStyleSheet(
                    this.Page,
                    this.ResolveUrl(string.Format("{0}{1}/SiteMap.css", this.ResolveUrl("Skins/"), this.sSkinName)));
            }
            else if (this.sRenderMode.Equals("treeview"))
            {
                ClientResourceManager.RegisterStyleSheet(
                    this.Page,
                    this.ResolveUrl(string.Format("{0}{1}/SiteMapTree.css", this.ResolveUrl("Skins/"), this.sSkinName)));
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

            foreach (var objTab in this.tiTabs)
            {
                this.tabPermissions = TabPermissionController.GetTabPermissions(
                    objTab.TabID, this.PortalSettings.PortalId);

                if (!objTab.ParentId.Equals(iParentTabId) || objTab.IsDeleted || objTab.IsDeleted ||
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

                        ctlMainDiv.Attributes["class"] = string.Format("SiteMap-{0}", this.sSkinName);

                        this.siteMapPlaceHolder.Controls.Add(ctlMainDiv);

                        ctlUl.Attributes["class"] = this.sSkinName;

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

                var decodedTabName = Server.HtmlDecode(objTab.LocalizedTabName);

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
                                this.ResolveUrl(Path.Combine(this.PortalSettings.HomeDirectory, this.sDefaultIcon)),
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
            var ctlMainDiv = new HtmlGenericControl("div");

            ctlMainDiv.Attributes["class"] = "SiteMap-" + this.sSkinName;

            this.siteMapPlaceHolder.Controls.Add(ctlMainDiv);

            var ctlUl = new HtmlGenericControl("ul");

            ctlUl.Attributes["class"] = this.sSkinName;

            ctlMainDiv.Controls.Add(ctlUl);

            var ctlMainMenu = new HtmlGenericControl("li");

            ctlUl.Controls.Add(ctlMainMenu);

            var ctlAnchorWeb = new HtmlAnchor
                {
                    Title = this.PortalSettings.PortalName, 
                    InnerHtml = string.Format("<strong><em>{0}</em></strong>", this.PortalSettings.PortalName), 
                    HRef =
                        string.Format(
                            "{0}/{1}", 
                            Globals.GetPortalDomainName(this.PortalSettings.PortalAlias.HTTPAlias, null, true), 
                            Globals.glbDefaultPage)
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

            this.tiTabs = this.FillTabs();

            this.PlaceSkinStyleLink();

            if (this.sRenderMode.Equals("treeview"))
            {
                this.PlaceScriptLink();

                this.PlaceScriptTag();
            }

            var tabActive = this.SetActiveTab();

            if (this.bFilterByTax)
            {
                this.taxTerms = this.GetTerms().ToList();
            }

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
                        foreach (var objTestTab in
                            this.tiTabs.Where(objTestTab => objTestTab.TabID.Equals(tabActive.ParentId)))
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

            if (this.sRootLevel.Equals("custom"))
            {
                try
                {
                    foreach (var objTestTab in
                        this.tiTabs.Where(objTestTab => objTestTab.TabID.ToString().Equals(this.sRootTab)))
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

            return tab.IsSuperTab || tab.IsSecure
                       ? this.ResolveUrl(string.Format("{0}/images/{1}", Globals.ApplicationPath, tab.IconFile))
                       : this.ResolveUrl(Path.Combine(this.PortalSettings.HomeDirectory, tab.IconFile));
        }

        #endregion
    }
}