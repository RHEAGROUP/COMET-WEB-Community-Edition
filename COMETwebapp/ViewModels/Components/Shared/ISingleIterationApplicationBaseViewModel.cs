﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ISingleIterationApplicationBaseViewModel.cs" company="RHEA System S.A.">
//     Copyright (c) 2023 RHEA System S.A.
// 
//     Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Théate Antoine, Nabil Abbar
// 
//     This file is part of COMET WEB Community Edition
//     The COMET WEB Community Edition is the RHEA Web Application implementation of ECSS-E-TM-10-25 Annex A and Annex C.
// 
//     The COMET WEB Community Edition is free software; you can redistribute it and/or
//     modify it under the terms of the GNU Affero General Public
//     License as published by the Free Software Foundation; either
//     version 3 of the License, or (at your option) any later version.
// 
//     The COMET WEB Community Edition is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace COMETwebapp.ViewModels.Components.Shared
{
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using COMETwebapp.Utilities.DisposableObject;

    /// <summary>
    /// Base view model for any application that will need only one <see cref="Iteration" />
    /// </summary>
    public interface ISingleIterationApplicationBaseViewModel: IDisposableObject
	{
		/// <summary>
		/// The current <see cref="Iteration" /> to work with
		/// </summary>
		Iteration CurrentIteration { get; set; }

        /// <summary>
        /// Value asserting that the view model has set initial values at least once
        /// </summary>
        bool HasSetInitialValuesOnce { get; set; }

        /// <summary>
        /// Gets the current <see cref="DomainOfExpertise" />
        /// </summary>
        DomainOfExpertise CurrentDomain { get; }
    }
}
