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
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web.UI.WebControls;

    using DotNetNuke.Common;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Framework;
    using DotNetNuke.Security;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.UI.UserControls;

    #endregion

    /// <summary>
    /// Module Settings page
    /// </summary>
    public partial class Settings : ModuleSettingsBase
    {
        #region Properties

        /// <summary>
        ///  Gets or sets The ctl default icon.
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
        ///    Gets or sets The dsh tree opt.
        /// </summary>
        protected SectionHeadControl dshTreeOpt;

        /// <summary>
        ///    Gets or sets The dsh visl opt.
        /// </summary>
        protected SectionHeadControl dshVislOpt;

        /// <summary>
        ///    Gets or sets The pnl setting.
        /// </summary>
        protected Panel pnlSetting;

        #region Constants and Fields
        /// <summary>
        ///   The exlusion tabs.
        /// </summary>
        private string[] exlusionTabs;

        /// <summary>
        ///   The s skin.
        /// </summary>
        private string sSkin;

        #endregion

        /// <summary>
        ///   Gets DefaultIcon.
        /// </summary>
        private UrlControl DefaultIcon
        {
            get
            {
                return this.ctlDefaultIcon;
            }
        }
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
                this.Response.Redirect(Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Select All Tabs for Excluding
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void BtnSelectAllClick(object sender, EventArgs e)
        {
            foreach (ListItem item in this.cblExcludeLst.Items)
            {
                item.Selected = true;
            }
        }

        /// <summary>
        /// Deselect All Tabs from Excluding
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void BtnSelectNoneClick(object sender, EventArgs e)
        {
            foreach (ListItem item in this.cblExcludeLst.Items)
            {
                item.Selected = false;
            }
        }

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
        /// The Event Args e.
        /// </param>
        protected void DDlSkinsChanged(object sender, EventArgs e)
        {
            this.imgPreview.ImageUrl = string.Format(
                "{0}{1}/Preview.jpg", this.ResolveUrl("Skins/"), this.dDlSkins.SelectedItem.Text);
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
            foreach (ListItem excludeLstTab in from TabInfo objTab in this.PortalSettings.DesktopTabs
                                               where ((!objTab.IsDeleted && objTab.StartDate < DateTime.Now) && objTab.EndDate > DateTime.Now) && PortalSecurity.IsInRoles(objTab.AuthorizedRoles)
                                               select new ListItem { Text = objTab.TabName, Value = objTab.TabID.ToString() })
            {
                this.cblExcludeLst.Items.Add(excludeLstTab);
            }
        }

        /// <summary>
        /// Fills the RootLevel list.
        /// </summary>
        private void FillLevelList()
        {
            this.dDlRootLevel.Items.Add(
                new ListItem(
                    string.Format("< {0} >", Localization.GetString("Root.Text", this.LocalResourceFile)),
                    "root"));
            this.dDlRootLevel.Items.Add(
                new ListItem(
                    string.Format("< {0} >", Localization.GetString("Parent.Text", this.LocalResourceFile)),
                    "parent"));
            this.dDlRootLevel.Items.Add(
                new ListItem(
                    string.Format("< {0} >", Localization.GetString("Current.Text", this.LocalResourceFile)),
                    "current"));
            this.dDlRootLevel.Items.Add(
                new ListItem(
                    string.Format("< {0} >", Localization.GetString("Child.Text", this.LocalResourceFile)),
                    "children"));
            this.dDlRootLevel.Items.Add(
                new ListItem(Localization.GetString("Custom.Text", this.LocalResourceFile), "custom"));
        }

        /// <summary>
        /// Loads the Module Settings
        /// </summary>
        private void FillSettings()
        {
            this.FillLevelList();

            // Load Render Mode Setting
            string sRenderMode = string.Empty;

            try
            {
                sRenderMode = (string)this.TabModuleSettings["sRenderMode"];
            }
            catch (Exception)
            {
                sRenderMode = "normal";
            }
            finally
            {
                if (string.IsNullOrEmpty(sRenderMode))
                {
                    sRenderMode = "normal";
                }

                this.rBlRender.SelectedValue = sRenderMode;
            }

            // Load Skin List
            this.FillSkinList();

            this.ShowHideTreeOptions();

            // Load Skin Name Setting
            try
            {
                this.sSkin = (string)this.TabModuleSettings["sSkin"];
            }
            catch (Exception)
            {
                this.sSkin = "Default";
            }
            finally
            {
                if (string.IsNullOrEmpty(this.sSkin))
                {
                    this.sSkin = "Default";
                }

                // Load skin preview
                this.imgPreview.ImageUrl = string.Format("{0}{1}/Preview.jpg", this.ResolveUrl("Skins/"), this.sSkin);

                this.dDlSkins.SelectedValue = this.sSkin;
            }

            // Loads TreeView Options
            string sAnimated = string.Empty;
            try
            {
                sAnimated = (string)this.TabModuleSettings["sAnimated"];
            }
            catch (Exception)
            {
                sAnimated = "normal";
            }
            finally
            {
                if (string.IsNullOrEmpty(sAnimated))
                {
                    sAnimated = "normal";
                }

                foreach (ListItem item in
                    this.rBlAnimated.Items.Cast<ListItem>().Where(item => item.Value.Equals(sAnimated)))
                {
                    item.Selected = true;
                }
            }

            string sCollapsed = string.Empty;
            try
            {
                sCollapsed = (string)this.TabModuleSettings["bCollapsed"];
            }
            catch (Exception)
            {
                sCollapsed = "true";
            }
            finally
            {
                if (string.IsNullOrEmpty(sCollapsed))
                {
                    sCollapsed = "true";
                }

                foreach (ListItem item in
                    this.rBlCollapsed.Items.Cast<ListItem>().Where(item => item.Value.Equals(sCollapsed)))
                {
                    item.Selected = true;
                }
            }

            string sUnique = string.Empty;
            try
            {
                sUnique = (string)this.TabModuleSettings["bUnique"];
            }
            catch (Exception)
            {
                sUnique = "true";
            }
            finally
            {
                if (string.IsNullOrEmpty(sUnique))
                {
                    sUnique = "true";
                }

                foreach (ListItem item in
                    this.rBlUnique.Items.Cast<ListItem>().Where(item => item.Value.Equals(sUnique)))
                {
                    item.Selected = true;
                }
            }

            string sPersist = string.Empty;
            try
            {
                sPersist = (string)this.TabModuleSettings["sPersist"];
            }
            catch (Exception)
            {
                sPersist = "location";
            }
            finally
            {
                if (string.IsNullOrEmpty(sPersist))
                {
                    sPersist = "location";
                }

                foreach (ListItem item in
                    this.rBlPersist.Items.Cast<ListItem>().Where(item => item.Value.Equals(sPersist)))
                {
                    item.Selected = true;
                }
            }

            string sRenderName = string.Empty;
            try
            {
                sRenderName = (string)this.TabModuleSettings["bRenderName"];
            }
            catch (Exception)
            {
                sRenderName = "true";
            }
            finally
            {
                if (string.IsNullOrEmpty(sRenderName))
                {
                    sRenderName = "true";
                }

                foreach (ListItem item in
                    this.rBlRenderName.Items.Cast<ListItem>().Where(item => item.Value.Equals(sRenderName)))
                {
                    item.Selected = true;
                }
            }

            // Load Tabs
            this.FillTabList();
            this.FillExTabList();

            // Load Excluded Tabs
            string sExlTabLst;

            try
            {
                sExlTabLst = (string)this.TabModuleSettings["exclusTabsLst"];
            }
            catch (Exception)
            {
                sExlTabLst = null;
            }

            try
            {
                if (sExlTabLst != string.Empty)
                {
                    this.exlusionTabs = sExlTabLst.Split(',');

                    if (this.exlusionTabs.Length != 0)
                    {
                        foreach (ListItem item in from sExTabValue in this.exlusionTabs
                                                  from ListItem item in this.cblExcludeLst.Items
                                                  where item.Value.Equals(sExTabValue)
                                                  select item)
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                this.exlusionTabs = null;
            }

            // Load Max Render Level Setting
            try
            {
                string sMaxLevel = (string)this.TabModuleSettings["sMaxLevel"];
                this.tbMaxLevel.Text = sMaxLevel;
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
            string sRootLevel = string.Empty;
            try
            {
                sRootLevel = (string)this.TabModuleSettings["sRootLevel"];
            }
            catch (Exception)
            {
                sRootLevel = string.Empty;
            }
            finally
            {
                this.dDlRootLevel.SelectedValue = string.IsNullOrEmpty(sRootLevel) ? "root" : sRootLevel;

                this.dDlRootTab.Enabled = this.dDlRootLevel.SelectedValue.Equals("custom");
            }

            // Load Render Tab Setting
            string sRootTab = string.Empty;
            try
            {
                sRootTab = (string)this.TabModuleSettings["sRootTab"];
            }
            catch (Exception)
            {
                sRootTab = string.Empty;
            }
            finally
            {
                this.dDlRootTab.SelectedValue = string.IsNullOrEmpty(sRootTab) ? "-1" : sRootTab;
            }

            // Load Show Hidden Tabs Setting
            try
            {
                string sShowHidden = (string)this.TabModuleSettings["bShowHidden"];

                this.cBShowHidden.Checked = bool.Parse(sShowHidden);
            }
            catch (Exception)
            {
                this.cBShowHidden.Checked = false;
            }

            // Load Show Tab Icons Setting
            try
            {
                string sShowTabIcons = (string)this.TabModuleSettings["bShowTabIcons"];
                this.cBShowTabIcons.Checked = bool.Parse(sShowTabIcons);
            }
            catch (Exception)
            {
                this.cBShowTabIcons.Checked = false;
            }

            // Load Default Icon Setting
            try
            {
                string sDefaultIcon = (string)this.TabModuleSettings["sDefaultIcon"];

                this.DefaultIcon.Url = sDefaultIcon;
            }
            catch (Exception)
            {
                this.DefaultIcon.Url = null;
            }

            // Load Show Info Setting
            try
            {
                string sShowInfo = (string)this.TabModuleSettings["bShowInfo"];
                this.cBShowInfo.Checked = bool.Parse(sShowInfo);
            }
            catch (Exception)
            {
                this.cBShowInfo.Checked = true;
            }

            // Load Demo Mode Setting
            if (!string.IsNullOrEmpty((string)this.TabModuleSettings["bDemoMode"]))
            {
                bool bResult;
                if (bool.TryParse((string)this.TabModuleSettings["bDemoMode"], out bResult))
                {
                    this.cBDemoMode.Checked = bResult;
                }
            }
            else
            {
                this.cBDemoMode.Checked = false;
            }

            // Load Human Friendly Urls Setting
            if (!string.IsNullOrEmpty((string)this.TabModuleSettings["bHumanUrls"]))
            {
                bool bResult;
                if (bool.TryParse((string)this.TabModuleSettings["bHumanUrls"], out bResult))
                {
                    this.cBHumanUrls.Checked = bResult;
                }
            }
            else
            {
                this.cBHumanUrls.Checked = true;
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
                DirectoryInfo objDir = new DirectoryInfo(this.MapPath(this.ResolveUrl("Skins")));

                foreach (DirectoryInfo objSubFolder in objDir.GetDirectories())
                {
                    if (this.rBlRender.SelectedValue.Equals("normal"))
                    {
                        // Load Normal SiteMap Skins
                        if (Utility.IsSkinDirectory(objSubFolder.FullName))
                        {
                            ListItem skinItem = new ListItem { Text = objSubFolder.Name, Value = objSubFolder.Name };

                            this.dDlSkins.Items.Add(skinItem);
                        }
                    }
                    else if (this.rBlRender.SelectedValue.Equals("treeview"))
                    {
                        // Load TreeView Skins
                        if (Utility.IsSkinTreeDirectory(objSubFolder.FullName))
                        {
                            ListItem skinItem = new ListItem { Text = objSubFolder.Name, Value = objSubFolder.Name };

                            this.dDlSkins.Items.Add(skinItem);
                        }
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
            foreach (TabInfo objTab in this.PortalSettings.DesktopTabs)
            {
                if (objTab.IsDeleted || objTab.StartDate >= DateTime.Now || objTab.EndDate <= DateTime.Now ||
                    !PortalSecurity.IsInRoles(objTab.AuthorizedRoles))
                {
                    continue;
                }

                ListItem lstTab = new ListItem();

                if (objTab.Level.Equals(1))
                {
                    lstTab.Text = string.Format("|->{0}", objTab.TabName);
                }
                else if (objTab.Level.Equals(2))
                {
                    lstTab.Text = string.Format("|-->{0}", objTab.TabName);
                }
                else if (objTab.Level.Equals(3))
                {
                    lstTab.Text = string.Format("|--->{0}", objTab.TabName);
                }
                else if (objTab.Level.Equals(4))
                {
                    lstTab.Text = string.Format("|---->{0}", objTab.TabName);
                }
                else
                {
                    lstTab.Text = string.Format("|-{0}", objTab.TabName);
                }

                lstTab.Value = objTab.TabID.ToString();

                this.dDlRootTab.Items.Add(lstTab);
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
            string sExlTabLst = this.cblExcludeLst.Items.Cast<ListItem>().Where(item => item.Selected).Aggregate(string.Empty, (current, item) => current + string.Format("{0},", item.Value));

            ModuleController objModules = new ModuleController();

            if (sExlTabLst != string.Empty && sExlTabLst.EndsWith(","))
            {
                sExlTabLst = sExlTabLst.Remove(sExlTabLst.Length - 1, 1);
            }

            foreach (ListItem item in this.dDlSkins.Items.Cast<ListItem>().Where(item => item.Selected))
            {
                this.sSkin = item.Text;
            }

            // Save TreeView Options
            objModules.UpdateTabModuleSetting(this.TabModuleId, "sAnimated", this.rBlAnimated.SelectedValue);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "bCollapsed", this.rBlCollapsed.SelectedValue);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "bUnique", this.rBlUnique.SelectedValue);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "sPersist", this.rBlPersist.SelectedValue);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "bRenderName", this.rBlRenderName.SelectedValue);

            objModules.UpdateTabModuleSetting(this.TabModuleId, "sSkin", this.sSkin);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "exclusTabsLst", sExlTabLst);
            objModules.UpdateTabModuleSetting(this.TabModuleId, "bShowTabIcons", this.cBShowTabIcons.Checked.ToString());
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

                this.imgPreview.ImageUrl = string.Format(
                    "{0}{1}/Preview.jpg", this.ResolveUrl("Skins/"), this.dDlSkins.SelectedItem.Text);
            }
            else
            {
                this.rBlAnimated.Enabled = false;
                this.rBlCollapsed.Enabled = false;
                this.rBlUnique.Enabled = false;
                this.rBlPersist.Enabled = false;
                this.rBlRenderName.Enabled = false;

                this.imgPreview.ImageUrl = string.Format(
                    "{0}{1}/Preview.jpg", this.ResolveUrl("Skins/"), this.dDlSkins.SelectedItem.Text);
            }
        }

        #endregion
    }
}