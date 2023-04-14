﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IParameterSwitchKindSelectorViewModel.cs" company="RHEA System S.A.">
//    Copyright (c) 2023 RHEA System S.A.
// 
//    Authors: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Théate Antoine, Nabil Abbar
// 
//    This file is part of COMET WEB Community Edition
//    The COMET WEB Community Edition is the RHEA Web Application implementation of ECSS-E-TM-10-25
//    Annex A and Annex C.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace COMET.Web.Common.ViewModels.Components.Selectors
{
    using CDP4Common.EngineeringModelData;

    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// View Model to sets the current <see cref="ParameterSwitchKind" /> value
    /// </summary>
    public interface IParameterSwitchKindSelectorViewModel : IHaveReadOnlyStateViewModel
    {
        /// <summary>
        /// Get or set the <see cref="ParameterSwitchKind" />
        /// </summary>
        ParameterSwitchKind SwitchValue { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventCallback" /> to be called to perfom an update
        /// </summary>
        EventCallback OnUpdate { get; set; }

        /// <summary>
        /// The initial <see cref="ParameterSwitchKind" />
        /// </summary>
        ParameterSwitchKind InitialSwitchValue { get; }

        /// <summary>
        /// Updates this view model properties
        /// </summary>
        /// <param name="parameterSwitchKind">The <see cref="ParameterSwitchKind" /></param>
        /// <param name="readOnly">The readonly state</param>
        void UpdateProperties(ParameterSwitchKind parameterSwitchKind, bool readOnly);
    }
}
