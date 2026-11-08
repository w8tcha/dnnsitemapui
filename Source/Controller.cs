/*  **********************************************************
*                                                            *
*   SiteMap - A Modern SiteMap / TreeView                    *
*   Copyright(c) Ingo Herbote                                *
*   All rights reserved.                                     *
*   Ingo Herbote                                             *
*   Internet: https://github.com/w8tcha/dnnsitemapui         *
*                                                            *
*************************************************************/

using System;
using System.IO;
using System.Xml;

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;

using Microsoft.Extensions.DependencyInjection;

namespace DNN.Modules;

/// <summary>
/// Controller for the SiteMap for IPortable and ISearchable
/// </summary>
public class SiteMapController : ModuleSettingsBase, IPortable
{
    /// <summary>
    /// The names of all settings that are exported and imported.
    /// </summary>
    private static readonly string[] SettingNames =
    [
        "sSkin", "sRenderMode", "bShowInfo", "bDemoMode", "exclusTabsLst", "bShowHidden", "bShowTabIcons",
        "sMaxLevel", "sRootTab", "sRootLevel", "sAnimated", "bCollapsed", "bUnique", "sPersist", "bRenderName",
        "bHumanUrls", "FilterByTax", "TaxMode", "TaxVocabularies", "TaxTerms"
    ];

    /// <summary>
    /// Export Settings to XML File
    /// </summary>
    /// <param name="moduleID">
    /// The Current Module Id
    /// </param>
    /// <returns>
    /// The export module.
    /// </returns>
    public string ExportModule(int moduleID)
    {
        try
        {
            var moduleController = new ModuleController(null, null, this.DependencyProvider.GetRequiredService<IHostSettings>());

            var moduleInfo = moduleController.GetModule(moduleID);
            var tabModuleSettings = moduleInfo.TabModuleSettings;

            var sSkin = "Default";
            var sExlTabLst = string.Empty;
            var sShowHidden = "False";
            var sShowTabIcons = "False";
            var sShowInfo = "True";
            var sDemoMode = "False";
            var sRenderMode = "normal";
            var sMaxLevel = "-1";
            var sRootTab = "-1";
            var sRootLevel = "root";
            var sAnimated = "normal";
            var sCollapsed = "true";
            var sUnique = "true";
            var sPersist = "location";
            var sRenderName = "true";
            var sHumanUrls = "true";
            var sFilterByTax = "false";
            var sTaxMode = "all";
            var sVocabularies = string.Empty;
            var sTerms = string.Empty;

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

            if (!string.IsNullOrEmpty((string)tabModuleSettings["TaxTerms"]))
            {
                sTerms = (string)tabModuleSettings["TaxTerms"];
            }

            var sw = new StringWriter();

            var xmlWrit = new XmlTextWriter(sw);

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
            xmlWrit.WriteElementString("TaxTerms", sTerms);

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
    /// <param name="moduleID">
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
    void IPortable.ImportModule(int moduleID, string content, string version, int userId)
    {
        try
        {
            var xmlTagCloud = Globals.GetContent(content, "SiteMap");

            var objModules = new ModuleController(null, null, this.DependencyProvider.GetRequiredService<IHostSettings>());

            // The controller has no page context here (TabId is -1), so update every
            // tab module instance of the module
            var tabModules = objModules.GetTabModulesByModule(moduleID);

            foreach (XmlNode xmlContent in xmlTagCloud.SelectNodes("Settings"))
            {
                foreach (var settingName in SettingNames)
                {
                    // Skip settings missing in the import file (e.g. older exports without TaxTerms)
                    var settingNode = xmlContent[settingName];

                    if (settingNode == null)
                    {
                        continue;
                    }

                    foreach (var tabModule in tabModules)
                    {
                        objModules.UpdateTabModuleSetting(tabModule.TabModuleID, settingName, settingNode.InnerText);
                    }
                }
            }
        }
        catch (Exception exc)
        {
            Exceptions.ProcessModuleLoadException(this, exc);
        }
    }
}