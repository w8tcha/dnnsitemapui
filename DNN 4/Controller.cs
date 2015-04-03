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
    using System.IO;
    using System.Xml;

    using DotNetNuke.Common;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Search;

    #endregion

    /// <summary>
    /// Controller for the SiteMap for IPortable and ISearchable
    /// </summary>
    public class SiteMapController : ModuleSettingsBase, IPortable, ISearchable
    {
        #region Implemented Interfaces

        #region IPortable

        /// <summary>
        /// Export Settings to XML File
        /// </summary>
        /// <param name="iModuleId">
        /// Current Module Id
        /// </param>
        /// <returns>
        /// The export module.
        /// </returns>
        string IPortable.ExportModule(int iModuleId)
        {
            // return XmlUtils.Serialize(TabModuleSettings);
            try
            {
                string sSkin = "Default";
                string sExlTabLst = string.Empty;
                string sShowHidden = "False";
                string sShowTabIcons = "False";
                string sShowInfo = "True";
                string sDemoMode = "False";
                string sRenderMode = "normal";
                string sMaxLevel = "-1";
                string sRootTab = "-1";
                string sRootLevel = "root";
                string sAnimated = "normal";
                string sCollapsed = "true";
                string sUnique = "true";
                string sPersist = "location";
                string sRenderName = "true";
                string sHumanUrls = "true";

                if (this.TabModuleSettings.ContainsKey("sSkin") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["sSkin"]))
                {
                    sSkin = (string)this.TabModuleSettings["sSkin"];
                }

                if (this.TabModuleSettings.ContainsKey("sRenderMode") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["sRenderMode"]))
                {
                    sRenderMode = (string)this.TabModuleSettings["sRenderMode"];
                }

                if (this.TabModuleSettings.ContainsKey("exclusTabsLst") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["exclusTabsLst"]))
                {
                    sExlTabLst = (string)this.TabModuleSettings["exclusTabsLst"];
                }

                if (this.TabModuleSettings.ContainsKey("bShowHidden") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["bShowHidden"]))
                {
                    sShowHidden = (string)this.TabModuleSettings["bShowHidden"];
                }

                if (this.TabModuleSettings.ContainsKey("bShowTabIcons") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["bShowTabIcons"]))
                {
                    sShowTabIcons = (string)this.TabModuleSettings["bShowTabIcons"];
                }

                if (this.TabModuleSettings.ContainsKey("sMaxLevel") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["sMaxLevel"]))
                {
                    sMaxLevel = (string)this.TabModuleSettings["sMaxLevel"];
                }

                if (this.TabModuleSettings.ContainsKey("bShowInfo") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["bShowInfo"]))
                {
                    sShowInfo = (string)this.TabModuleSettings["bShowInfo"];
                }

                if (this.TabModuleSettings.ContainsKey("bDemoMode") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["bDemoMode"]))
                {
                    sDemoMode = (string)this.TabModuleSettings["bDemoMode"];
                }

                if (this.TabModuleSettings.ContainsKey("sRootTab") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["sRootTab"]))
                {
                    sRootTab = (string)this.TabModuleSettings["sRootTab"];
                }

                if (this.TabModuleSettings.ContainsKey("sRootLevel") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["sRootLevel"]))
                {
                    sRootLevel = (string)this.TabModuleSettings["sRootLevel"];
                }

                if (this.TabModuleSettings.ContainsKey("sAnimated") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["sAnimated"]))
                {
                    sAnimated = (string)this.TabModuleSettings["sAnimated"];
                }

                if (this.TabModuleSettings.ContainsKey("bCollapsed") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["bCollapsed"]))
                {
                    sCollapsed = (string)this.TabModuleSettings["bCollapsed"];
                }

                if (this.TabModuleSettings.ContainsKey("bUnique") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["bUnique"]))
                {
                    sUnique = (string)this.TabModuleSettings["bUnique"];
                }

                if (this.TabModuleSettings.ContainsKey("sPersist") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["sPersist"]))
                {
                    sPersist = (string)this.TabModuleSettings["sPersist"];
                }

                if (this.TabModuleSettings.ContainsKey("bRenderName") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["bRenderName"]))
                {
                    sRenderName = (string)this.TabModuleSettings["bRenderName"];
                }

                if (this.TabModuleSettings.ContainsKey("bHumanUrls") &&
                    !string.IsNullOrEmpty((string)this.TabModuleSettings["bHumanUrls"]))
                {
                    sHumanUrls = (string)this.TabModuleSettings["bHumanUrls"];
                }

                StringWriter sw = new StringWriter();

                XmlTextWriter xmlWrit = new XmlTextWriter(sw);

                xmlWrit.WriteStartElement("SiteMap");
                xmlWrit.WriteStartElement("Settings");

                xmlWrit.WriteElementString("sSkin", sSkin);
                xmlWrit.WriteElementString("sRenderMode", sRenderMode);
                xmlWrit.WriteElementString("bShowInfo", sShowInfo);
                xmlWrit.WriteElementString("bDemoMode", sDemoMode);
                xmlWrit.WriteElementString("exclusTabsLst", sExlTabLst);
                xmlWrit.WriteElementString("bShowHidden", sShowHidden);
                xmlWrit.WriteElementString("bShowTabIcons", sShowTabIcons);
                xmlWrit.WriteElementString("sMaxLevel", sMaxLevel);
                xmlWrit.WriteElementString("sRootTab", sRootTab);
                xmlWrit.WriteElementString("sRootLevel", sRootLevel);
                xmlWrit.WriteElementString("sAnimated", sAnimated);
                xmlWrit.WriteElementString("bCollapsed", sCollapsed);
                xmlWrit.WriteElementString("bUnique", sUnique);
                xmlWrit.WriteElementString("sPersist", sPersist);
                xmlWrit.WriteElementString("bRenderName", sRenderName);
                xmlWrit.WriteElementString("bHumanUrls", sHumanUrls);

                xmlWrit.WriteEndElement();
                xmlWrit.WriteEndElement();
                xmlWrit.Flush();
                xmlWrit.Close();
                sw.Flush();
                return sw.ToString();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            return string.Empty;
        }

        /// <summary>
        /// Import Settings from XML File.
        /// </summary>
        /// <param name="moduleId">
        /// The module Id.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        void IPortable.ImportModule(int moduleId, string content, string version, int userId)
        {
            try
            {
                XmlNode xmlTagCloud = Globals.GetContent(content, "SiteMap");

                ModuleController objModules = new ModuleController();

                ModuleInfo objModule = objModules.GetModule(moduleId, this.TabId);

                objModules.GetTabModuleSettings(objModule.TabModuleID);

                foreach (XmlNode xmlContent in xmlTagCloud.SelectNodes("Settings"))
                {
                    objModules.UpdateTabModuleSetting(objModule.TabModuleID, "sSkin", xmlContent["sSkin"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "sRenderMode", xmlContent["sRenderMode"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "bShowInfo", xmlContent["bShowInfo"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "bDemoMode", xmlContent["bDemoMode"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "exclusTabsLst", xmlContent["exclusTabsLst"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "bShowHidden", xmlContent["bShowHidden"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "bShowTabIcons", xmlContent["bShowTabIcons"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "sMaxLevel", xmlContent["sMaxLevel"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "sRootTab", xmlContent["sRootTab"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "sRootLevel", xmlContent["sRootLevel"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "sAnimated", xmlContent["sAnimated"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "bCollapsed", xmlContent["bCollapsed"].InnerText);
                    objModules.UpdateTabModuleSetting(objModule.TabModuleID, "bUnique", xmlContent["bUnique"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "sPersist", xmlContent["sPersist"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "bRenderName", xmlContent["bRenderName"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "bHumanUrls", xmlContent["bHumanUrls"].InnerText);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region ISearchable

        /// <summary>
        /// included as a stub only so that the core knows this module Implements Entities.Modules.ISearchable
        /// </summary>
        /// <param name="modInfo">
        /// Current Module Info
        /// </param>
        /// <returns>
        /// The Search Items from the Module
        /// </returns>
        public SearchItemInfoCollection GetSearchItems(ModuleInfo modInfo)
        {
            return null;
        }

        #endregion

        #endregion
    }
}