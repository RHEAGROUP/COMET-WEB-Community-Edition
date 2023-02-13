﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ParameterTypeSelectorViewModel.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.ViewModels.Components.Shared.Selectors
{
    using CDP4Common.SiteDirectoryData;

    using COMETwebapp.Extensions;

    using ReactiveUI;

    /// <summary>
    /// View Model that enables the user to select an <see cref="ParameterType" />
    /// </summary>
    public class ParameterTypeSelectorViewModel : BelongsToIterationSelectorViewModel, IParameterTypeSelectorViewModel
    {
        /// <summary>
        /// A collection of all available <see cref="ParameterType" />
        /// </summary>
        private IEnumerable<ParameterType> allAvailableParameterTypes = new List<ParameterType>();

        /// <summary>
        /// Backing field for <see cref="SelectedParameterType" />
        /// </summary>
        private ParameterType selectedParameterType;

        /// <summary>
        /// The currently selected <see cref="ParameterType" />
        /// </summary>
        public ParameterType SelectedParameterType
        {
            get => this.selectedParameterType;
            set => this.RaiseAndSetIfChanged(ref this.selectedParameterType, value);
        }

        /// <summary>
        /// A collection of available <see cref="ParameterType" />
        /// </summary>
        public IEnumerable<ParameterType> AvailableParameterTypes { get; private set; } = Enumerable.Empty<ParameterType>();

        /// <summary>
        /// Filter the collection of the <see cref="IParameterTypeSelectorViewModel.AvailableParameterTypes" /> with provided values
        /// </summary>
        /// <param name="parameterTypesId">A collection of <see cref="Guid" /> for <see cref="ParameterType" /></param>
        public void FilterAvailableParameterTypes(IEnumerable<Guid> parameterTypesId)
        {
            this.AvailableParameterTypes = this.allAvailableParameterTypes.Where(x => parameterTypesId.Any(p => p == x.Iid));
            this.SelectedParameterType = this.AvailableParameterTypes.FirstOrDefault(x => x.Iid == this.SelectedParameterType?.Iid);
        }

        /// <summary>
        /// Updates this view model properties
        /// </summary>
        protected override void UpdateProperties()
        {
            this.SelectedParameterType = null;
            this.allAvailableParameterTypes = this.CurrentIteration?.QueryUsedParameterTypes().OrderBy(x => x.Name) ?? Enumerable.Empty<ParameterType>();
            this.AvailableParameterTypes = new List<ParameterType>(this.allAvailableParameterTypes);
        }
    }
}