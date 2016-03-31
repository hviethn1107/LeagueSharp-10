// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilAIO.cs" company="KyonLeague">
//      Copyright (c) by Kyon 2016
// </copyright>
// <summary>
//   Defines the UtilAio type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UtilAIO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO The utility all in one.
    /// </summary>
    internal class UtilAio
    {
        /// <summary>
        /// TODO The setup of the menu, spells and other classes.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Init()
        {
            // TODO: Initialize everything ^^
            return Setup.Menu.Setup();
        }
    }
}
