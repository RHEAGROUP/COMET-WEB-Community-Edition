﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="SwitchTooltip.cs" company="RHEA System S.A.">
//     Copyright (c) 2023 RHEA System S.A.
// 
//     Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Théate Antoine
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

namespace COMETwebapp.Components.ParameterEditor
{
    using CDP4Common.EngineeringModelData;
    using COMETwebapp.ViewModels.Components.ParameterEditor;

    using Microsoft.AspNetCore.Components;
    using ReactiveUI;

    /// <summary>
    /// Class for the component <see cref="SwitchTooltip"/>
    /// </summary>
    public partial class SwitchTooltip
    {
        /// <summary>
        /// Gets or sets the <see cref="ISwitchTooltipViewModel"/>
        /// </summary>
        [Inject]
        public ISwitchTooltipViewModel ViewModel { get; set; }

        /// <summary>
        /// RenderFragment that contains the tooltip
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Iid of the associated ParametervalueSet
        /// </summary>
        [Parameter]
        public Guid ParameterValueSetIid { get; set; }

        /// <summary>
        /// The switch mode of the associated ParameterValueSet
        /// </summary>
        [Parameter]
        public ParameterSwitchKind ParameterValueSetSwitchMode { get; set; }

        /// <summary>
        /// Sets computed button active
        /// </summary>
        [Parameter]
        public ParameterSwitchKind SwitchValue { get; set; }

        /// <summary>
        /// Sets if the switch can be change in the ISession
        /// </summary>
        [Parameter]
        public bool IsEditable { get; set; }

        /// <summary>
        /// Method invoked after each time the component has been rendered. Note that the component does
        /// not automatically re-render after the completion of any returned <see cref="Task"/>, because
        /// that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">
        /// Set to <c>true</c> if this is the first time <see cref="OnAfterRenderAsync(bool)"/> has been invoked
        /// on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        /// <remarks>
        /// The <see cref="OnAfterRenderAsync(bool)"/> lifecycle methods
        /// are useful for performing interop, or interacting with values received from <c>@ref</c>.
        /// Use the <paramref name="firstRender"/> parameter to ensure that initialization work is only performed
        /// once.
        /// </remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                this.ViewModel.IsEditable = this.IsEditable;
                this.ViewModel.SwitchValue = this.SwitchValue;
                this.ViewModel.ParameterValueSetIid = this.ParameterValueSetIid;
                this.ViewModel.ParameterValueSetSwitchMode = this.ParameterValueSetSwitchMode;
            }
        }
    }
}
