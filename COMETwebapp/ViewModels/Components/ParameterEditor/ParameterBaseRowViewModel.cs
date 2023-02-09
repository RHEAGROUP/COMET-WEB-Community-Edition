﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ParameterBaseRowViewModel.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.ViewModels.Components.ParameterEditor
{
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using COMETwebapp.Extensions;
    using COMETwebapp.ViewModels.Components.Shared.ParameterEditors;

    /// <summary>
    /// ViewModel for the rows asociated to a <see cref="ParameterBase"/>
    /// </summary>
    public class ParameterBaseRowViewModel : IParameterBaseRowViewModel
    {
        /// <summary>
        /// Gets or sets the <see cref="ParameterBase"/> for this <see cref="ParameterBaseRowViewModel"/>
        /// </summary>
        public ParameterBase Parameter { get; }

        /// <summary>
        /// Gets or sets the <see cref="ParameterType"/> for this <see cref="ParameterBaseRowViewModel"/>
        /// </summary>
        public ParameterType ParameterType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ElementBase"/> used for grouping this <see cref="ParameterBaseRowViewModel"/>
        /// </summary>
        public string ElementBaseName { get; }

        /// <summary>
        /// Gets the <see cref="Parameter"/> type name
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Gets the <see cref="Parameter"/> owner name
        /// </summary>
        public string OwnerName { get; }

        /// <summary>
        /// Gets the switch for the published value
        /// </summary>
        public ParameterSwitchKind Switch { get; }

        /// <summary>
        /// Gets the <see cref="Parameter"/> model code
        /// </summary>
        public string ModelCode { get; }

        /// <summary>
        /// Gets the <see cref="Option"/> name this <see cref="Parameter"/> is dependant on
        /// </summary>
        public string Option { get; } = string.Empty;

        /// <summary>
        /// Gets the <see cref="ActualFiniteState"/> name this <see cref="Parameter"/> is dependant on
        /// </summary>
        public string State { get; } = string.Empty;

        /// <summary>
        /// Creates a new instance of type <see cref="ParameterBaseRowViewModel"/>
        /// </summary>
        /// <param name="parameterBase">the parameter of this row</param>
        /// /// <param name="valueSet">the valueSet of the parameter</param>
        public ParameterBaseRowViewModel(ParameterBase parameterBase, IValueSet valueSet)
        {
            this.Parameter = parameterBase ?? throw new ArgumentNullException(nameof(parameterBase));
            this.ParameterType = this.Parameter.ParameterType;
            this.ParameterName = this.Parameter.ParameterType.Name;
            this.OwnerName = this.Parameter.Owner.ShortName;
            this.ModelCode = this.Parameter.ModelCode();
            this.ElementBaseName = (parameterBase.Container as ElementBase)?.ShortName;
            
            this.Option = valueSet.ActualOption?.Name;
            this.State = valueSet.ActualState?.Name;
            this.Switch = valueSet.ValueSwitch;
        }

        /// <summary>
        /// Creates a <see cref="IParameterTypeEditorSelectorViewModel"/> based on the data of this <see cref="IParameterBaseRowViewModel"/>
        /// </summary>
        /// <returns></returns>
        public IParameterTypeEditorSelectorViewModel CreateParameterTypeEditorSelectorViewModel()
        {
            return new ParameterTypeEditorSelectorViewModel(this.ParameterType);
        }
    }
}
