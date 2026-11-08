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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;

namespace DNN.Modules;

/// <summary>
/// The utility.
/// </summary>
public class Utility
{
    /// <summary>
    /// Checks if the Object is a Number
    /// </summary>
    /// <param name="valueToCheck">
    /// the Object to check
    /// </param>
    /// <returns>
    /// Returns true or false
    /// </returns>
    public static bool IsNumeric(object valueToCheck)
    {
        var inputValue = Convert.ToString(valueToCheck);

        return double.TryParse(inputValue, NumberStyles.Any, null, out _);
    }

    /// <summary>
    /// Checks if the Directory contains a CSS to valid if its a Skin Folder
    /// </summary>
    /// <param name="directory">
    /// Directory to check
    /// </param>
    /// <returns>
    /// Returns true or false
    /// </returns>
    public static bool IsSkinDirectory(string directory)
    {
        var objDir = new DirectoryInfo(directory);

        var isSkinDir = false;

        try
        {
            if (Array.Exists(objDir.GetFiles(), objFile => objFile.Name.EndsWith("SiteMap.css")))
            {
                isSkinDir = true;
            }
        }
        catch (Exception)
        {
            isSkinDir = false;
        }

        return isSkinDir;
    }

    /// <summary>
    /// Checks if the Directory contains a CSS to valid if it's a Skin
    ///   Folder (TreeView)
    /// </summary>
    /// <param name="directory">
    /// Directory to check
    /// </param>
    /// <returns>
    /// Returns true or false
    /// </returns>
    public static bool IsSkinTreeDirectory(string directory)
    {
        var objDir = new DirectoryInfo(directory);

        var isSkinDir = false;

        try
        {
            if (Array.Exists(objDir.GetFiles(), objFile => objFile.Name.EndsWith("SiteMapTree.css")))
            {
                isSkinDir = true;
            }
        }
        catch (Exception)
        {
            isSkinDir = false;
        }

        return isSkinDir;
    }

    /// <summary>
    /// Splits a list value into trimmed, non-empty entries.
    /// </summary>
    /// <param name="value">The list value (can be null).</param>
    /// <param name="separators">The separators.</param>
    /// <returns>The entries; empty when the value is empty.</returns>
    public static List<string> SplitList(string value, params char[] separators)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return value.Split(separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(entry => entry.Trim())
            .Where(entry => entry.Length > 0)
            .ToList();
    }

    /// <summary>
    /// Splits a list value into IDs, skipping entries that are not valid numbers.
    /// </summary>
    /// <param name="value">The list value (can be null).</param>
    /// <param name="separators">The separators.</param>
    /// <returns>The IDs; empty when the value contains no valid ID.</returns>
    public static List<int> ParseIdList(string value, params char[] separators)
    {
        var ids = new List<int>();

        foreach (var entry in SplitList(value, separators))
        {
            if (int.TryParse(entry, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                ids.Add(id);
            }
        }

        return ids;
    }

    /// <summary>
    /// Checks if the Tab is published right now (Start/End Date), treating empty dates as no limit.
    /// </summary>
    /// <param name="tab">The tab.</param>
    /// <returns>true if the tab is within its start and end date</returns>
    public static bool IsTabActive(TabInfo tab)
    {
        var now = DateTime.Now;

        return (Null.IsNull(tab.StartDate) || tab.StartDate < now) &&
               (Null.IsNull(tab.EndDate) || tab.EndDate > now);
    }

    /// <summary>
    /// Checks if the current user has view permission on the Tab.
    /// </summary>
    /// <param name="tab">The tab.</param>
    /// <returns>true if the current user can view the tab</returns>
    public static bool CanViewTab(TabInfo tab)
    {
        var permissions = TabPermissionController.GetTabPermissions(tab.TabID, tab.PortalID);

        return PortalSecurity.IsInRoles(permissions.ToString("VIEW"));
    }

    /// <summary>
    /// Builds the encoded inner HTML of a sitemap link with an icon.
    /// </summary>
    /// <param name="iconUrl">The icon URL.</param>
    /// <param name="text">The (decoded) link text.</param>
    /// <returns>The safe HTML markup.</returns>
    public static string BuildIconLinkHtml(string iconUrl, string text)
    {
        return
            $"<img src=\"{HttpUtility.HtmlAttributeEncode(iconUrl)}\" class=\"tabIcon\" alt=\"{HttpUtility.HtmlAttributeEncode(text)}\" width=\"16\" height=\"16\" />{HttpUtility.HtmlEncode(text)}";
    }

    /// <summary>
    /// Appends a CSS class to an existing class attribute value.
    /// </summary>
    /// <param name="classes">The existing classes (can be null).</param>
    /// <param name="cssClass">The class to add.</param>
    /// <returns>The combined class value.</returns>
    public static string AppendCssClass(string classes, string cssClass)
    {
        return string.IsNullOrWhiteSpace(classes) ? cssClass : $"{classes.Trim()} {cssClass}";
    }

    /// <summary>
    /// Selects all list items whose value is in the given values, ignoring values that are not in the list.
    /// </summary>
    /// <param name="list">The list control.</param>
    /// <param name="values">The values to select.</param>
    public static void SelectValues(ListControl list, IEnumerable<string> values)
    {
        if (values == null)
        {
            return;
        }

        foreach (var value in values)
        {
            var item = list.Items.FindByValue(value);

            if (item != null)
            {
                item.Selected = true;
            }
        }
    }
}
