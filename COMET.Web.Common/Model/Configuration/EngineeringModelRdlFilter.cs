﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="EngineeringModelRdlFilter.cs" company="RHEA System S.A.">
//     Copyright (c) 2024 RHEA System S.A.
// 
//     Authors: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Théate Antoine
// 
//     This file is part of CDP4-COMET WEB Community Edition
//     The CDP4-COMET WEB Community Edition is the RHEA Web Application implementation of ECSS-E-TM-10-25 Annex A and Annex C.
// 
//     The CDP4-COMET WEB Community Edition is free software; you can redistribute it and/or
//     modify it under the terms of the GNU Affero General Public
//     License as published by the Free Software Foundation; either
//     version 3 of the License, or (at your option) any later version.
// 
//     The CDP4-COMET WEB Community Edition is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace COMET.Web.Common.Model.Configuration
{
    using CDP4Common.SiteDirectoryData;

    /// <summary>
    /// Class used to filter the <see cref="EngineeringModelSetup"/> with an specific RDL. The Filter uses the <see cref="EngineeringModelKind"/>
    /// and the Rdl ShortName to filter the models. 
    /// </summary>
    public class EngineeringModelRdlFilter
    {
        /// <summary>
        /// Gets or sets the types of <see cref="EngineeringModelKind"/> that pass the filter
        /// </summary>
        public IEnumerable<EngineeringModelKind> Kinds { get; set; }

        /// <summary>
        /// Gets or sets the shortnames of the RDLs that pass the filter
        /// </summary>
        public IEnumerable<string> RdlShortNames { get; set; }
    }
}
