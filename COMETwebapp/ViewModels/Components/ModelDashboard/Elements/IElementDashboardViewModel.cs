﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IElementDashboardViewModel.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.ViewModels.Components.ModelDashboard.Elements
{
	using CDP4Common.EngineeringModelData;
	using CDP4Common.SiteDirectoryData;

	using DynamicData;

	/// <summary>
	/// Interface definition for <see cref="ElementDashboardViewModel" />
	/// </summary>
	public interface IElementDashboardViewModel
	{
		/// <summary>
		/// The current <see cref="DomainOfExpertise" />
		/// </summary>
		DomainOfExpertise CurrentDomain { get; }

		/// <summary>
		/// A collection of <see cref="ElementDefinition" />
		/// </summary>
		SourceList<ElementDefinition> ElementDefinitions { get; }

		/// <summary>
		/// A collection of unused <see cref="ElementDefinition"/>
		/// </summary>
		IEnumerable<ElementDefinition> UnusedElements { get; }

		/// <summary>
		/// A collection of unreferenced <see cref="ElementDefinition"/>
		/// </summary>
		IEnumerable<ElementDefinition> UnreferencedElements { get; }

		/// <summary>
		/// Updates this view model properties
		/// </summary>
		/// <param name="iteration">The <see cref="Iteration" /></param>
		/// <param name="currentDomain">The current <see cref="DomainOfExpertise" /></param>
		void UpdateProperties(Iteration iteration, DomainOfExpertise currentDomain);
	}
}
