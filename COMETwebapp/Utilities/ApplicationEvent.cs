﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationEvent.cs" company="RHEA System S.A.">
//    Copyright (c) 2022 RHEA System S.A.
//
//    Author: Justine Veirier d'aiguebonne, Sam Gerené, Alex Vorobiev, Alexander van Delft
//
//    This file is part of COMET WEB Community Edition
//    The COMET WEB Community Edition is the RHEA Web Application implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET WEB Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET WEB Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace COMETwebapp.Utilities
{
    /// <summary>
    /// Event when active application changes
    /// </summary>
    public class ApplicationEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationEvent"/> class.
        /// </summary>
        /// <param name="applicationName">Name of the active application</param>
        public ApplicationEvent(string? applicationName)
        {
            this.ApplicationName = applicationName;
        }

        /// <summary>
        /// Name of the active application
        /// </summary>
        public string? ApplicationName { get; set; }
    }
}
