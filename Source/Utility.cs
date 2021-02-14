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
    using System.Globalization;
    using System.IO;
    using System.Linq;

    #endregion

    /// <summary>
    /// The utility.
    /// </summary>
    public class Utility
    {
        #region Public Methods

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
                if (objDir.GetFiles().Any(objFile => objFile.Name.EndsWith("SiteMap.css")))
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
        /// Checks if the Directory contains a CSS to valid if its a Skin 
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
                if (objDir.GetFiles().Any(objFile => objFile.Name.EndsWith("SiteMapTree.css")))
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

        #endregion
    }
}