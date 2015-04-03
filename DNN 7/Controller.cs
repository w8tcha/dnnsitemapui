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
        /// <param name="moduleId">
        /// The Current Module Id
        /// </param>
        /// <returns>
        /// The export module.
        /// </returns>
        public string ExportModule(int moduleId)
        {
            try
            {
                var moduleController = new ModuleController();

                var moduleInfo = moduleController.GetModule(moduleId);
                var tabModuleSettings = moduleController.GetTabModuleSettings(moduleInfo.TabModuleID);

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
                string sFilterByTax = "false";
                string sTaxMode = "all";
                string sVocabularies = string.Empty;

                if (tabModuleSettings.ContainsKey("sSkin") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["sSkin"]))
                {
                    sSkin = (string)tabModuleSettings["sSkin"];
                }

                if (tabModuleSettings.ContainsKey("sRenderMode") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["sRenderMode"]))
                {
                    sRenderMode = (string)tabModuleSettings["sRenderMode"];
                }

                if (tabModuleSettings.ContainsKey("exclusTabsLst") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["exclusTabsLst"]))
                {
                    sExlTabLst = (string)tabModuleSettings["exclusTabsLst"];
                }

                if (tabModuleSettings.ContainsKey("bShowHidden") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["bShowHidden"]))
                {
                    sShowHidden = (string)tabModuleSettings["bShowHidden"];
                }

                if (tabModuleSettings.ContainsKey("bShowTabIcons") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["bShowTabIcons"]))
                {
                    sShowTabIcons = (string)tabModuleSettings["bShowTabIcons"];
                }

                if (tabModuleSettings.ContainsKey("sMaxLevel") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["sMaxLevel"]))
                {
                    sMaxLevel = (string)tabModuleSettings["sMaxLevel"];
                }

                if (tabModuleSettings.ContainsKey("bShowInfo") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["bShowInfo"]))
                {
                    sShowInfo = (string)tabModuleSettings["bShowInfo"];
                }

                if (tabModuleSettings.ContainsKey("bDemoMode") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["bDemoMode"]))
                {
                    sDemoMode = (string)tabModuleSettings["bDemoMode"];
                }

                if (tabModuleSettings.ContainsKey("sRootTab") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["sRootTab"]))
                {
                    sRootTab = (string)tabModuleSettings["sRootTab"];
                }

                if (tabModuleSettings.ContainsKey("sRootLevel") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["sRootLevel"]))
                {
                    sRootLevel = (string)tabModuleSettings["sRootLevel"];
                }

                if (tabModuleSettings.ContainsKey("sAnimated") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["sAnimated"]))
                {
                    sAnimated = (string)tabModuleSettings["sAnimated"];
                }

                if (tabModuleSettings.ContainsKey("bCollapsed") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["bCollapsed"]))
                {
                    sCollapsed = (string)tabModuleSettings["bCollapsed"];
                }

                if (tabModuleSettings.ContainsKey("bUnique") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["bUnique"]))
                {
                    sUnique = (string)tabModuleSettings["bUnique"];
                }

                if (tabModuleSettings.ContainsKey("sPersist") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["sPersist"]))
                {
                    sPersist = (string)tabModuleSettings["sPersist"];
                }

                if (tabModuleSettings.ContainsKey("bRenderName") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["bRenderName"]))
                {
                    sRenderName = (string)tabModuleSettings["bRenderName"];
                }

                if (tabModuleSettings.ContainsKey("bHumanUrls") &&
                    !string.IsNullOrEmpty((string)tabModuleSettings["bHumanUrls"]))
                {
                    sHumanUrls = (string)tabModuleSettings["bHumanUrls"];
                }

                if (!string.IsNullOrEmpty((string)tabModuleSettings["FilterByTax"]))
                {
                    sFilterByTax = (string)tabModuleSettings["FilterByTax"];
                }

                if (!string.IsNullOrEmpty((string)tabModuleSettings["TaxMode"]))
                {
                    sTaxMode = (string)tabModuleSettings["TaxMode"];
                }

                if (!string.IsNullOrEmpty((string)tabModuleSettings["TaxVocabularies"]))
                {
                    sVocabularies = (string)tabModuleSettings["TaxVocabularies"];
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
                xmlWrit.WriteElementString("FilterByTax", sFilterByTax);
                xmlWrit.WriteElementString("TaxMode", sTaxMode);
                xmlWrit.WriteElementString("TaxVocabularies", sVocabularies);

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
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "FilterByTax", xmlContent["FilterByTax"].InnerText);
                    objModules.UpdateTabModuleSetting(objModule.TabModuleID, "TaxMode", xmlContent["TaxMode"].InnerText);
                    objModules.UpdateTabModuleSetting(
                        objModule.TabModuleID, "TaxVocabularies", xmlContent["TaxVocabularies"].InnerText);
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