// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="KyonLeague">
//      Copyright (c) by Kyon 2016
// </copyright>
// <summary>
//      TODO Just the initiation of the program :roto2:
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UtilAIO
{
    using System;

    /// <summary>
    /// Just the initiation of the program
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///     Start up runtime for advanced utility options.
        /// </summary>
        /// <param name="args">Startup-args that are given by the operator</param>
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var utilitysuite = new UtilAio();
            utilitysuite.Init();

        }
    }
}
