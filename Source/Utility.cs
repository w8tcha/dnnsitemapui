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
using System.Globalization;
using System.IO;

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
                if (Array.Exists(objDir.GetFiles(),objFile => objFile.Name.EndsWith("SiteMapTree.css")))
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
    }