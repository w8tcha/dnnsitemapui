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
            double dummy;
            string inputValue = Convert.ToString(valueToCheck);

            bool bNumeric = double.TryParse(inputValue, NumberStyles.Any, null, out dummy);

            return bNumeric;
        }

        /// <summary>
        /// Checks if the Directory contains a CSS to valid if its a Skin Folder
        /// </summary>
        /// <param name="sDirectory">
        /// Directory to check
        /// </param>
        /// <returns>
        /// Returns true or false
        /// </returns>
        public static bool IsSkinDirectory(string sDirectory)
        {
            DirectoryInfo objDir = new DirectoryInfo(sDirectory);

            bool bSkinDir = false;

            try
            {
                if (objDir.GetFiles().Any(objFile => objFile.Name.EndsWith("SiteMap.css")))
                {
                    bSkinDir = true;
                }

                /* foreach (FileInfo objFile in objDir.GetFiles().Where(objFile => objFile.Name.EndsWith("SiteMap.css")))
                 {
                     bSkinDir = true;
                 }*/
            }
            catch (Exception)
            {
                bSkinDir = false;
            }

            return bSkinDir;
        }

        /// <summary>
        /// Checks if the Directory contains a CSS to valid if its a Skin 
        ///   Folder (TreeView)
        /// </summary>
        /// <param name="sDirectory">
        /// Directory to check
        /// </param>
        /// <returns>
        /// Returns true or false
        /// </returns>
        public static bool IsSkinTreeDirectory(string sDirectory)
        {
            DirectoryInfo objDir = new DirectoryInfo(sDirectory);

            bool bSkinDir = false;

            try
            {
                if (objDir.GetFiles().Any(objFile => objFile.Name.EndsWith("SiteMapTree.css")))
                {
                    bSkinDir = true;
                }

                /*foreach (FileInfo objFile in
                    objDir.GetFiles().Where(objFile => objFile.Name.EndsWith("SiteMapTree.css")))
                {
                    bSkinDir = true;
                }*/
            }
            catch (Exception)
            {
                bSkinDir = false;
            }

            return bSkinDir;
        }

        #endregion
    }
}