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
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web.UI.WebControls;

    using DotNetNuke.Abstractions;
    using DotNetNuke.Collections;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Content.Common;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Framework;
    using DotNetNuke.Security;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.UI.UserControls;

    using Microsoft.Extensions.DependencyInjection;

    #endregion

    /// <summary>
    /// Module Settings page
    /// </summary>
    public partial class Settings : ModuleSettingsBase
    {
        #region Properties

        /// <summary>
        ///    Gets or sets The ctl default icon.
        /// </summary>
        protected UrlControl ctlDefaultIcon;

        /// <summary>
        ///    Gets or sets The dsh exl opt.
        /// </summary>
        protected SectionHeadControl dshExlOpt;

        /// <summary>
        ///    Gets or sets The dsh render opt.
        /// </summary>
        protected SectionHeadControl dshRenderOpt;

        /// <summary>
        ///    Gets or sets The dsh tax opt.
        /// </summary>
        protected SectionHeadControl dshTaxOpt;

        /// <summary>
        ///    Gets or sets The dsh tree opt.
        /// </summary>
        protected SectionHeadControl dshTreeOpt;

        /// <summary>
        ///    Gets or sets The dsh visl opt.
        /// </summary>
        protected SectionHeadControl dshVislOpt;

        /// <summary>
        /// The navigation manager.
        /// </summary>
        private readonly INavigationManager navigationManager;

        /// <summary>
        ///   The exclusion tabs.
        /// </summary>
        private string[] exclusionTabs;

        /// <summary>
        ///   The tab permissions.
        /// </summary>
        private TabPermissionCollection tabPermissions;

        /// <summary>
        ///   The tabs.
        /// </summary>
        private List<TabInfo> tabs;

        /// <summary>
        ///   The vocabularies.
        /// </summary>
        private string[] vocabularies;

        /// <summary>
        ///   The terms.
        /// </summary>
        private string[] terms;

        #region Constants and Fields

        /// <summary>
        ///   The skin.
        /// </summary>
        private string skin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        protected Settings()
        {
            this.navigationManager = this.DependencyProvider.GetRequiredService<INavigationManager>();
        }

        #endregion

        /// <summary>
        ///   Gets the DefaultIcon.
        /// </summary>
        private UrlControl DefaultIcon => this.ctlDefaultIcon;

        #endregion

        #region Public Methods

        /// <summary>
        /// Load all Settings
        /// </summary>
        public override void LoadSettings()
        {
            try
            {
                if (!this.Page.IsPostBack)
                {
                    this.FillSettings();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Save all Settings
        /// </summary>
        public override void UpdateSettings()
        {
            try
            {
                this.SaveChanges();
                this.Response.Redirect(this.navigationManager.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if Root Level is set to Custom
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Event Args e.
        /// </param>
        protected void DDlRootLevelSelectedIndexChanged(object sender, EventArgs e)
        {
            this.dDlRootTab.Enabled = this.dDlRootLevel.SelectedValue.Equals("custom");
        }

        /// <summary>
        /// The d dl skins changed.
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The  Event Arguments.
        /// </param>
        protected void DDlSkinsChanged(object sender, EventArgs e)
        {
            this.imgPreview.ImageUrl = $"{this.ResolveUrl("Skins/")}{this.dDlSkins.SelectedItem.Text}/Preview.jpg";
        }

        /// <summary>
        /// Show Vocabulary Selector
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Event Args e.
        /// </param>
        protected void DDlTaxModeSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.dDlTaxMode.SelectedValue)
            {
                case "custom":
                {
                    this.cBlVocabularies.Enabled = true;

                    this.cBlTerms.Enabled = false;

                    if (this.vocabularies == null)
                    {
                        return;
                    }

                    this.vocabularies.ForEach(
                        sVocabulary => this.cBlVocabularies.Items.FindByValue(sVocabulary).Selected = true);

                    break;
                }

                case "terms":
                {
                    this.cBlTerms.Enabled = true;

                    this.cBlVocabularies.Enabled = false;

                    this.terms.ForEach(term => this.cBlTerms.Items.FindByValue(term).Selected = true);

                    break;
                }

                default:
                    this.cBlTerms.Enabled = false;
                    this.cBlVocabularies.Enabled = false;
                    break;
            }
        }

        /// <summary>
        /// Show / Hide Tax Selectors
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Event Args e.
        /// </param>
        protected void FilterByTaxChanged(object sender, EventArgs e)
        {
            if (this.cBFilterByTax.Checked)
            {
                this.dDlTaxMode.Enabled = true;

                switch (this.dDlTaxMode.SelectedValue)
                {
                    case "custom":
                    {
                        this.cBlVocabularies.Enabled = true;

                        this.cBlTerms.Enabled = false;

                        if (this.vocabularies == null)
                        {
                            return;
                        }

                        foreach (var sVocabulary in this.vocabularies)
                        {
                            this.cBlVocabularies.Items.FindByValue(sVocabulary).Selected = true;
                        }

                        break;
                    }

                    case "terms":
                        this.cBlTerms.Enabled = true;

                        this.cBlVocabularies.Enabled = false;
                        break;
                    default:
                        this.cBlVocabularies.Enabled = false;
                        this.cBlTerms.Enabled = false;
                        break;
                }
            }
            else
            {
                this.dDlTaxMode.Enabled = false;
                this.cBlVocabularies.Enabled = false;
                this.cBlTerms.Enabled = false;
            }
        }

        /// <summary>
        /// CODEGEN: This call is required by the ASP.NET Web Form Designer.
        /// </summary>
        /// <param name="e">
        /// The Event Args e.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Reloads the Skin list based on Selected Render Mode
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Event Args e.
        /// </param>
        protected void RBlRenderSelectedIndexChanged(object sender, EventArgs e)
        {
            this.FillSkinList();

            this.ShowHideTreeOptions();
        }

        /// <summary>
        /// Loads the Portal Tabs
        /// </summary>
        private void FillExTabList()
        {
            foreach (var objTab in this.tabs)
            {
                this.tabPermissions = TabPermissionController.GetTabPermissions(objTab.TabID, this.PortalId);

                if (objTab.IsDeleted || objTab.StartDate >= DateTime.Now || objTab.EndDate <= DateTime.Now ||
                    !PortalSecurity.IsInRoles(this.tabPermissions.ToString("VIEW")))
                {
                    continue;
                }

                var excludeLstTab = new ListItem { Text = objTab.LocalizedTabName, Value = objTab.TabID.ToString() };

                this.cblExcludeLst.Items.Add(excludeLstTab);
            }
        }

        /// <summary>
        /// Fills the RootLevel list.
        /// </summary>
        private void FillLevelList()
        {
            this.dDlRootLevel.Items.Add(
                new ListItem($"< {Localization.GetString("Root.Text", this.LocalResourceFile)} >", "root"));
            this.dDlRootLevel.Items.Add(
                new ListItem($"< {Localization.GetString("Parent.Text", this.LocalResourceFile)} >", "parent"));
            this.dDlRootLevel.Items.Add(
                new ListItem($"< {Localization.GetString("Current.Text", this.LocalResourceFile)} >", "current"));
            this.dDlRootLevel.Items.Add(
                new ListItem($"< {Localization.GetString("Child.Text", this.LocalResourceFile)} >", "children"));
            this.dDlRootLevel.Items.Add(
                new ListItem(Localization.GetString("Custom.Text", this.LocalResourceFile), "custom"));
        }

        /// <summary>
        /// Loads the Module Settings
        /// </summary>
        private void FillSettings()
        {
            this.tabs = this.FillTabs();

            this.FillLevelList();

            // Load Render Mode Setting
            var renderMode = string.Empty;

            try
            {
                renderMode = (string)this.TabModuleSettings["sRenderMode"];
            }
            catch (Exception)
            {
                renderMode = "normal";
            }
            finally
            {
                if (string.IsNullOrEmpty(renderMode))
                {
                    renderMode = "normal";
                }

                this.rBlRender.SelectedValue = renderMode;
            }

            // Load Skin List
            this.FillSkinList();

            this.ShowHideTreeOptions();

            // Load Skin Name Setting
            try
            {
                this.skin = (string)this.TabModuleSettings["sSkin"];
            }
            catch (Exception)
            {
                this.skin = "Default";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.skin))
                {
                    this.skin = "Default";
                }

                // Load skin preview
                this.imgPreview.ImageUrl = $"{this.ResolveUrl("Skins/")}{this.skin}/Preview.jpg";

                this.dDlSkins.SelectedValue = this.skin;
            }

            // Loads TreeView Options
            var animated = string.Empty;

            try
            {
                animated = (string)this.TabModuleSettings["sAnimated"];
            }
            catch (Exception)
            {
                animated = "normal";
            }
            finally
            {
                if (string.IsNullOrEmpty(animated))
                {
                    animated = "normal";
                }

                foreach (var item in this.rBlAnimated.Items.Cast<ListItem>().Where(item => item.Value.Equals(animated)))
                {
                    item.Selected = true;
                }
            }

            var collapsed = string.Empty;
            try
            {
                collapsed = (string)this.TabModuleSettings["bCollapsed"];
            }
            catch (Exception)
            {
                collapsed = "true";
            }
            finally
            {
                if (string.IsNullOrEmpty(collapsed))
                {
                    collapsed = "true";
                }

                this.rBlCollapsed.Items.Cast<ListItem>().Where(item => item.Value.Equals(collapsed)).ForEach(
                    item => item.Selected = true);
            }

            var unique = string.Empty;
            try
            {
                unique = (string)this.TabModuleSettings["bUnique"];
            }
            catch (Exception)
            {
                unique = "true";
            }
            finally
            {
                if (string.IsNullOrEmpty(unique))
                {
                    unique = "true";
                }

                this.rBlUnique.Items.Cast<ListItem>().Where(item => item.Value.Equals(unique))
                    .ForEach(item => item.Selected = true);
            }

            var persist = string.Empty;
            try
            {
                persist = (string)this.TabModuleSettings["sPersist"];
            }
            catch (Exception)
            {
                persist = "location";
            }
            finally
            {
                if (string.IsNullOrEmpty(persist))
                {
                    persist = "location";
                }

                this.rBlPersist.Items.Cast<ListItem>().Where(item => item.Value.Equals(persist))
                    .ForEach(item => item.Selected = true);
            }

            var renderName = string.Empty;
            try
            {
                renderName = (string)this.TabModuleSettings["bRenderName"];
            }
            catch (Exception)
            {
                renderName = "true";
            }
            finally
            {
                if (string.IsNullOrEmpty(renderName))
                {
                    renderName = "true";
                }

                this.rBlRenderName.Items.Cast<ListItem>().Where(item => item.Value.Equals(renderName))
                    .ForEach(item => item.Selected = true);
            }

            // Load Tabs
            this.FillTabList();
            this.FillExTabList();

            // Load Excluded Tabs
            string exlTabLst;

            try
            {
                exlTabLst = (string)this.TabModuleSettings["exclusTabsLst"];
            }
            catch (Exception)
            {
                exlTabLst = null;
            }

            try
            {
                if (exlTabLst != string.Empty)
                {
                    this.exclusionTabs = exlTabLst.Split(',');

                    if (this.exclusionTabs.Length != 0)
                    {
                        (from sExTabValue in this.exclusionTabs
                            from ListItem item in this.cblExcludeLst.Items
                            where item.Value.Equals(sExTabValue)
                            select item).ForEach(item => item.Selected = true);
                    }
                }
            }
            catch (Exception)
            {
                this.exclusionTabs = null;
            }

            // Load Max Render Level Setting
            try
            {
                var maxLevel = (string)this.TabModuleSettings["sMaxLevel"];
                this.tbMaxLevel.Text = maxLevel;
            }
            catch (Exception)
            {
                this.tbMaxLevel.Text = "-1";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.tbMaxLevel.Text))
                {
                    this.tbMaxLevel.Text = "-1";
                }
            }

            // Load Render Level Setting
            var rootLevel = string.Empty;
            try
            {
                rootLevel = (string)this.TabModuleSettings["sRootLevel"];
            }
            catch (Exception)
            {
                rootLevel = string.Empty;
            }
            finally
            {
                this.dDlRootLevel.SelectedValue = string.IsNullOrEmpty(rootLevel) ? "root" : rootLevel;

                this.dDlRootTab.Enabled = this.dDlRootLevel.SelectedValue.Equals("custom");
            }

            // Load Render Tab Setting
            var rootTab = string.Empty;
            try
            {
                rootTab = (string)this.TabModuleSettings["sRootTab"];
            }
            catch (Exception)
            {
                rootTab = string.Empty;
            }
            finally
            {
                this.dDlRootTab.SelectedValue = string.IsNullOrEmpty(rootTab) ? "-1" : rootTab;
            }

            // Load Show Hidden Tabs Setting
            try
            {
                var showHidden = (string)this.TabModuleSettings["bShowHidden"];

                this.cBShowHidden.Checked = bool.Parse(showHidden);
            }
            catch (Exception)
            {
                this.cBShowHidden.Checked = false;
            }

            // Load Show Tab Icons Setting
            try
            {
                var showTabIcons = (string)this.TabModuleSettings["bShowTabIcons"];
                this.cBShowTabIcons.Checked = bool.Parse(showTabIcons);
            }
            catch (Exception)
            {
                this.cBShowTabIcons.Checked = false;
            }

            // Load Default Icon Setting
            try
            {
                var defaultIcon = (string)this.TabModuleSettings["sDefaultIcon"];

                this.DefaultIcon.Url = defaultIcon;
            }
            catch (Exception)
            {
                this.DefaultIcon.Url = null;
            }

            // Load Show Info Setting
            try
            {
                var showInfo = (string)this.TabModuleSettings["bShowInfo"];
                this.cBShowInfo.Checked = bool.Parse(showInfo);
            }
            catch (Exception)
            {
                this.cBShowInfo.Checked = true;
            }

            // Load Demo Mode Setting
            if (!string.IsNullOrEmpty((string)this.TabModuleSettings["bDemoMode"]))
            {
                if (bool.TryParse((string)this.TabModuleSettings["bDemoMode"], out var result))
                {
                    this.cBDemoMode.Checked = result;
                }
            }
            else
            {
                this.cBDemoMode.Checked = false;
            }

            // Load Human Friendly Urls Setting
            if (!string.IsNullOrEmpty((string)this.TabModuleSettings["bHumanUrls"]))
            {
                if (bool.TryParse((string)this.TabModuleSettings["bHumanUrls"], out var result))
                {
                    this.cBHumanUrls.Checked = result;
                }
            }
            else
            {
                this.cBHumanUrls.Checked = true;
            }

            // Setting TaxMode
            if (!string.IsNullOrEmpty((string)this.TabModuleSettings["TaxMode"]))
            {
                try
                {
                    this.dDlTaxMode.SelectedValue = (string)this.TabModuleSettings["TaxMode"];
                }
                catch (Exception)
                {
                    this.dDlTaxMode.SelectedValue = "tab";
                }
            }
            else
            {
                this.dDlTaxMode.SelectedValue = "tab";
            }

            if (!string.IsNullOrEmpty((string)this.TabModuleSettings["FilterByTax"]))
            {
                if (bool.TryParse((string)this.TabModuleSettings["FilterByTax"], out var result))
                {
                    this.cBFilterByTax.Checked = result;
                }
            }
            else
            {
                this.cBFilterByTax.Checked = false;
            }

            if (this.cBFilterByTax.Checked)
            {
                this.dDlTaxMode.Enabled = true;
            }

            switch (this.dDlTaxMode.SelectedValue)
            {
                case "custom" when this.cBFilterByTax.Checked:
                {
                    this.cBlVocabularies.Enabled = true;
                    this.cBlTerms.Enabled = false;

                    // Setting TaxMode Vocabularies
                    if (!string.IsNullOrEmpty((string)this.TabModuleSettings["TaxVocabularies"]))
                    {
                        var taxVocabularies = (string)this.TabModuleSettings["TaxVocabularies"];

                        this.vocabularies = taxVocabularies.Split(';');

                        this.vocabularies.ForEach(
                            sVocabulary => this.cBlVocabularies.Items.FindByValue(sVocabulary).Selected = true);
                    }

                    break;
                }

                case "terms" when this.cBFilterByTax.Checked:
                {
                    this.cBlVocabularies.Enabled = false;
                    this.cBlTerms.Enabled = true;

                    // Setting TaxMode Terms
                    if (!string.IsNullOrEmpty((string)this.TabModuleSettings["TaxTerms"]))
                    {
                        var taxTerms = (string)this.TabModuleSettings["TaxTerms"];

                        this.terms = taxTerms.Split(';');

                        this.terms.ForEach(sTerm => this.cBlTerms.Items.FindByValue(sTerm).Selected = true);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Loads the List of available Skins.
        /// </summary>
        private void FillSkinList()
        {
            this.dDlSkins.Items.Clear();

            try
            {
                var objDir = new DirectoryInfo(this.MapPath(this.ResolveUrl("Skins")));

                foreach (var objSubFolder in objDir.GetDirectories())
                {
                    switch (this.rBlRender.SelectedValue)
                    {
                        case "normal":
                        {
                            // Load Normal SiteMap Skins
                            if (Utility.IsSkinDirectory(objSubFolder.FullName))
                            {
                                var skinItem = new ListItem { Text = objSubFolder.Name, Value = objSubFolder.Name };

                                this.dDlSkins.Items.Add(skinItem);
                            }

                            break;
                        }

                        case "treeview":
                        {
                            // Load TreeView Skins
                            if (Utility.IsSkinTreeDirectory(objSubFolder.FullName))
                            {
                                var skinItem = new ListItem { Text = objSubFolder.Name, Value = objSubFolder.Name };

                                this.dDlSkins.Items.Add(skinItem);
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                var skinItem = new ListItem
                {
                    Text = Localization.GetString("None.Text", this.LocalResourceFile), Value = "None"
                };

                this.lblError.Text = Localization.GetString("lblError.Text", this.LocalResourceFile);
                this.lblError.ForeColor = Color.Red;

                this.lPreview.Visible = false;
                this.imgPreview.Visible = false;

                this.dDlSkins.Items.Add(skinItem);
            }
        }

        /// <summary>
        /// Fills the RootTab list with all Tabs
        /// </summary>
        private void FillTabList()
        {
            foreach (var objTab in this.tabs)
            {
                this.tabPermissions = TabPermissionController.GetTabPermissions(objTab.TabID, this.PortalId);

                if (objTab.IsDeleted || objTab.StartDate >= DateTime.Now || objTab.EndDate <= DateTime.Now ||
                    !PortalSecurity.IsInRoles(this.tabPermissions.ToString("VIEW")))
                {
                    continue;
                }

                var lstTab = new ListItem();

                switch (objTab.Level)
                {
                    case 1:
                        lstTab.Text = $"|->{objTab.LocalizedTabName}";
                        break;
                    case 2:
                        lstTab.Text = $"|-->{objTab.LocalizedTabName}";
                        break;
                    case 3:
                        lstTab.Text = $"|--->{objTab.LocalizedTabName}";
                        break;
                    case 4:
                        lstTab.Text = $"|---->{objTab.LocalizedTabName}";
                        break;
                    default:
                        lstTab.Text = $"|-{objTab.LocalizedTabName}";
                        break;
                }

                lstTab.Value = objTab.TabID.ToString();

                this.dDlRootTab.Items.Add(lstTab);
            }
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
        /// Fill Options for Tax Mode
        /// </summary>
        private void FillTaxOptions()
        {
            var itemTab = new ListItem
            {
                Text = Localization.GetString("TabTerms.Text", this.LocalResourceFile), Value = "tab"
            };

            this.dDlTaxMode.Items.Add(itemTab);

            var itemAll = new ListItem
            {
                Text = Localization.GetString("AllTerms.Text", this.LocalResourceFile), Value = "all"
            };

            this.dDlTaxMode.Items.Add(itemAll);

            var itemCustom = new ListItem
            {
                Text = Localization.GetString("CustomVocabulary.Text", this.LocalResourceFile), Value = "custom"
            };

            this.dDlTaxMode.Items.Add(itemCustom);

            var itemTerms = new ListItem
            {
                Text = Localization.GetString("CustomTerms.Text", this.LocalResourceFile), Value = "terms"
            };

            this.dDlTaxMode.Items.Add(itemTerms);
        }

        /// <summary>
        /// Fill Terms Vocabulary Selector
        /// </summary>
        private void FillVocabularies()
        {
            try
            {
                var vocabRep = Util.GetVocabularyController();

                var vs = from v in vocabRep.GetVocabularies()
                    where v.ScopeType.ScopeType == "Application" ||
                          v.ScopeType.ScopeType == "Portal" && v.ScopeId == this.PortalId
                    select v;

                foreach (var v in vs)
                {
                    this.cBlVocabularies.Items.Add(new ListItem(v.Name, v.VocabularyId.ToString()));

                    foreach (var term in v.Terms)
                    {
                        this.cBlTerms.Items.Add(new ListItem(term.Name, term.TermId.ToString()));
                    }
                }
            }
            catch (Exception)
            {
                this.dDlTaxMode.Items.RemoveAt(2);
            }
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SetLocalization();

            this.DefaultIcon.FileFilter = Globals.glbImageFileTypes;

            this.FillTaxOptions();

            this.FillVocabularies();

            this.RegPostBack();
        }

        /// <summary>
        /// Register Controls with AutoPostBack on AJAX
        ///   if Activated
        /// </summary>
        private void RegPostBack()
        {
            if (!AJAX.IsInstalled())
            {
                return;
            }

            AJAX.RegisterPostBackControl(this.rBlRender);
            AJAX.RegisterPostBackControl(this.dDlSkins);
            AJAX.RegisterPostBackControl(this.dDlRootLevel);
            AJAX.RegisterPostBackControl(this.tbMaxLevel);
            AJAX.RegisterPostBackControl(this.btnSelectAll);
            AJAX.RegisterPostBackControl(this.btnSelectNone);
        }

        /// <summary>
        /// Saves the Module Settings
        /// </summary>
        private void SaveChanges()
        {
            var exlTabLst = this.cblExcludeLst.Items.Cast<ListItem>().Where(item => item.Selected).Aggregate(
                string.Empty,
                (current, item) => current + $"{item.Value},");

            var objModules = new ModuleController();

            if (exlTabLst != string.Empty && exlTabLst.EndsWith(","))
            {
                exlTabLst = exlTabLst.Remove(exlTabLst.Length - 1, 1);
            }

            this.dDlSkins.Items.Cast<ListItem>().Where(item => item.Selected).ForEach(item => this.skin = item.Text);

            // Save TreeView Options
            objModules.UpdateTabModuleSetting(this.TabModuleId, "sAnimated", this.rBlAnimated.SelectedValue);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "bCollapsed", this.rBlCollapsed.SelectedValue);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "bUnique", this.rBlUnique.SelectedValue);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "sPersist", this.rBlPersist.SelectedValue);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "bRenderName", this.rBlRenderName.SelectedValue);

            objModules.UpdateTabModuleSetting(this.TabModuleId, "sSkin", this.skin);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "exclusTabsLst", exlTabLst);
            objModules.UpdateTabModuleSetting(
                this.TabModuleId,
                "bShowTabIcons",
                this.cBShowTabIcons.Checked.ToString());
            objModules.UpdateTabModuleSetting(this.TabModuleId, "bShowHidden", this.cBShowHidden.Checked.ToString());
            objModules.UpdateTabModuleSetting(this.TabModuleId, "bShowInfo", this.cBShowInfo.Checked.ToString());
            objModules.UpdateTabModuleSetting(this.TabModuleId, "bDemoMode", this.cBDemoMode.Checked.ToString());
            objModules.UpdateTabModuleSetting(this.TabModuleId, "bHumanUrls", this.cBHumanUrls.Checked.ToString());

            if (!string.IsNullOrEmpty(this.DefaultIcon.Url))
            {
                objModules.UpdateTabModuleSetting(this.TabModuleId, "sDefaultIcon", this.DefaultIcon.Url);
            }

            if (!Utility.IsNumeric(this.tbMaxLevel.Text))
            {
                this.tbMaxLevel.Text = "-1";
            }

            objModules.UpdateTabModuleSetting(this.TabModuleId, "sMaxLevel", this.tbMaxLevel.Text);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "sRenderMode", this.rBlRender.SelectedValue);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "sRootTab", this.dDlRootTab.SelectedValue);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "sRootLevel", this.dDlRootLevel.SelectedValue);

            // Setting TaxMode
            objModules.UpdateTabModuleSetting(this.TabModuleId, "FilterByTax", this.cBFilterByTax.Checked.ToString());

            if (!this.cBFilterByTax.Checked)
            {
                return;
            }

            objModules.UpdateTabModuleSetting(this.TabModuleId, "TaxMode", this.dDlTaxMode.SelectedValue);

            switch (this.dDlTaxMode.SelectedValue)
            {
                case "custom":
                {
                    // Setting TaxMode Vocabularies
                    var sVocabularies = string.Empty;

                    sVocabularies = this.cBlVocabularies.Items.Cast<ListItem>().Where(item => item.Selected).Aggregate(
                        sVocabularies,
                        (current, item) => current + string.Format("{0};", item.Value));

                    if (sVocabularies.EndsWith(";"))
                    {
                        sVocabularies = sVocabularies.Remove(sVocabularies.Length - 1);
                    }

                    objModules.UpdateTabModuleSetting(this.TabModuleId, "TaxVocabularies", sVocabularies);
                    break;
                }

                case "terms":
                {
                    var sTerms = string.Empty;

                    sTerms = this.cBlTerms.Items.Cast<ListItem>().Where(item => item.Selected).Aggregate(
                        sTerms,
                        (current, item) => current + string.Format("{0};", item.Value));

                    if (sTerms.EndsWith(";"))
                    {
                        sTerms = sTerms.Remove(sTerms.Length - 1);
                    }

                    // Setting TaxMode Terms
                    objModules.UpdateTabModuleSetting(this.TabModuleId, "TaxTerms", sTerms);
                    break;
                }
            }
        }

        /// <summary>
        /// Set Localization for Buttons and Error Messages.
        /// </summary>
        private void SetLocalization()
        {
            this.btnSelectAll.Click += this.BtnSelectAllClick;
            this.btnSelectNone.Click += this.BtnSelectNoneClick;

            this.btnSelectAll.Text = Localization.GetString("btnSelectAll.Text", this.LocalResourceFile);
            this.btnSelectNone.Text = Localization.GetString("btnSelectNone.Text", this.LocalResourceFile);

            this.maxLevelValid.Text = Localization.GetString("Error.Text", this.LocalResourceFile);

            this.cBShowInfo.Text = Localization.GetString("lblShowInfo.Text", this.LocalResourceFile);
            this.cBDemoMode.Text = Localization.GetString("lblDemoMode.Text", this.LocalResourceFile);
            this.cBShowTabIcons.Text = Localization.GetString("lblShowTabIcons.Text", this.LocalResourceFile);
            this.cBShowHidden.Text = Localization.GetString("lblShowHidden.Text", this.LocalResourceFile);
            this.cBFilterByTax.Text = Localization.GetString("lblFilterByTax.Text", this.LocalResourceFile);

            this.dshVislOpt.Text = Localization.GetString("lVislOpt.Text", this.LocalResourceFile);
            this.dshTreeOpt.Text = Localization.GetString("lTreeOpt.Text", this.LocalResourceFile);
            this.dshRenderOpt.Text = Localization.GetString("lRenderOpt.Text", this.LocalResourceFile);
            this.dshExlOpt.Text = Localization.GetString("lExlOpt.Text", this.LocalResourceFile);

            this.lPreview.Text = Localization.GetString("lPreview.Text", this.LocalResourceFile);

            this.rBlAnimated.Items[0].Text = Localization.GetString("Slow.Text", this.LocalResourceFile);
            this.rBlAnimated.Items[1].Text = Localization.GetString("Normal.Text", this.LocalResourceFile);
            this.rBlAnimated.Items[2].Text = Localization.GetString("Fast.Text", this.LocalResourceFile);
            this.rBlAnimated.Items[3].Text = Localization.GetString("None.Text", this.LocalResourceFile);

            this.rBlCollapsed.Items[0].Text = Localization.GetString("Yes.Text", this.LocalResourceFile);
            this.rBlCollapsed.Items[1].Text = Localization.GetString("No.Text", this.LocalResourceFile);

            this.rBlUnique.Items[0].Text = Localization.GetString("Yes.Text", this.LocalResourceFile);
            this.rBlUnique.Items[1].Text = Localization.GetString("No.Text", this.LocalResourceFile);

            this.rBlPersist.Items[0].Text = Localization.GetString("Location.Text", this.LocalResourceFile);
            this.rBlPersist.Items[1].Text = Localization.GetString("Cookie.Text", this.LocalResourceFile);
            this.rBlPersist.Items[2].Text = Localization.GetString("No.Text", this.LocalResourceFile);

            this.rBlRenderName.Items[0].Text = Localization.GetString("Yes.Text", this.LocalResourceFile);
            this.rBlRenderName.Items[1].Text = Localization.GetString("No.Text", this.LocalResourceFile);

            this.rBlRender.Items[0].Text = Localization.GetString("Normal.Text", this.LocalResourceFile);
            this.rBlRender.Items[1].Text = Localization.GetString("Treeview.Text", this.LocalResourceFile);
        }

        /// <summary>
        /// Show/Hide TreeView Options
        /// </summary>
        private void ShowHideTreeOptions()
        {
            if (this.rBlRender.SelectedValue.Equals("treeview"))
            {
                this.rBlAnimated.Enabled = true;
                this.rBlCollapsed.Enabled = true;
                this.rBlUnique.Enabled = true;
                this.rBlPersist.Enabled = true;
                this.rBlRenderName.Enabled = true;

                this.imgPreview.ImageUrl = $"{this.ResolveUrl("Skins/")}{this.dDlSkins.SelectedItem.Text}/Preview.jpg";
            }
            else
            {
                this.rBlAnimated.Enabled = false;
                this.rBlCollapsed.Enabled = false;
                this.rBlUnique.Enabled = false;
                this.rBlPersist.Enabled = false;
                this.rBlRenderName.Enabled = false;

                this.imgPreview.ImageUrl = $"{this.ResolveUrl("Skins/")}{this.dDlSkins.SelectedItem.Text}/Preview.jpg";
            }
        }

        /// <summary>
        /// Select All Tabs for Excluding
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Event Args e.
        /// </param>
        private void BtnSelectAllClick(object sender, EventArgs e)
        {
            foreach (ListItem item in this.cblExcludeLst.Items)
            {
                item.Selected = true;
            }
        }

        /// <summary>
        /// Deselect All Tabs from Excluding
        /// </summary>
        /// <param name="sender">
        /// The sender object.
        /// </param>
        /// <param name="e">
        /// The Event Args e.
        /// </param>
        private void BtnSelectNoneClick(object sender, EventArgs e)
        {
            foreach (ListItem item in this.cblExcludeLst.Items)
            {
                item.Selected = false;
            }
        }

        #endregion
    }
}