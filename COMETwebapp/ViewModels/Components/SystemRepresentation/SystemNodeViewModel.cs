﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="SystemNodeViewModel.cs" company="RHEA System S.A.">
//     Copyright (c) 2023 RHEA System S.A.
// 
//     Authors: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Théate Antoine
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

namespace COMETwebapp.ViewModels.Components.SystemRepresentation
{
    using COMETwebapp.ViewModels.Components.Shared;
    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// ViewModel for a node in the SystemRepresentation tree
    /// </summary>
    public class SystemNodeViewModel : BaseNodeViewModel<SystemNodeViewModel>
    {
        /// <summary>
        ///     The <see cref="EventCallback" /> to call on baseNode selection
        /// </summary>
        public EventCallback<SystemNodeViewModel> OnSelect { get; set; }

        /// <summary>
        /// Creates a new instance of type <see cref="SystemNodeViewModel"/>
        /// </summary>
        /// <param name="title">the title of the view model</param>
        public SystemNodeViewModel(string title) : base(title)
        {
        }

        /// <summary>
        /// Callback method for when a node is selected
        /// </summary>
        /// <param name="node">the selected node</param>
        public override async void TreeSelectionChanged(SystemNodeViewModel node)
        {
            this.GetRootNode().GetFlatListOfDescendants(true).ForEach(x => x.IsSelected = false);
            await this.OnSelect.InvokeAsync(node);
        }
    }
}
