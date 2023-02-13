﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BelongsToIterationSelector.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.Components.Shared.Selectors
{
	using CDP4Common.EngineeringModelData;

	using COMETwebapp.ViewModels.Components.Shared.Selectors;

	using Microsoft.AspNetCore.Components;

	using ReactiveUI;

	/// <summary>
	/// Base component for selector that belongs to an <see cref="Iteration" />
	/// </summary>
	/// <typeparam name="TViewModel">An <see cref="IBelongsToIterationSelectorViewModel" /></typeparam>
	public abstract partial class BelongsToIterationSelector<TViewModel>
	{
		/// <summary>
		/// The <see cref="TViewModel" />
		/// </summary>
		[Parameter]
		public TViewModel ViewModel { get; set; }

		/// <summary>
		/// Method invoked when the component has received parameters from its parent in
		/// the render tree, and the incoming values have been assigned to properties.
		/// </summary>
		protected override void OnParametersSet()
		{
			base.OnParametersSet();

			this.Disposables.Add(this.ViewModel);

			this.Disposables.Add(this.WhenAnyValue(x => x.ViewModel.CurrentIteration)
				.Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
		}
	}
}