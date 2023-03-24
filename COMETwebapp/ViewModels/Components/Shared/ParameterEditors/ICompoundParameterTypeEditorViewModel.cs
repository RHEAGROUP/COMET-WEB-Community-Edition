﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ICompoundParameterTypeEditorViewModel.cs" company="RHEA System S.A.">
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
//     Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace COMETwebapp.ViewModels.Components.Shared.ParameterEditors
{
    using CDP4Common.SiteDirectoryData;
    using COMETwebapp.ViewModels.Components.Viewer.PropertiesPanel;

    public interface ICompoundParameterTypeEditorViewModel
    {
        /// <summary>
        /// Indicates if confirmation popup is visible
        /// </summary>
        bool IsOnEditMode { get; set; }

        /// <summary>
        /// Event for when the edit button is clicked
        /// </summary>
        void OnComponentSelected();

        /// <summary>
        /// Creates a view model for the <see cref="COMETwebapp.Components.Viewer.PropertiesPanel.OrientationComponent" />
        /// </summary>
        /// <returns>The <see cref="IOrientationViewModel" /></returns>
        IOrientationViewModel CreateOrientationViewModel();

        /// <summary>
        /// Creates a view model for the corresponding editor
        /// </summary>
        /// <param name="parameterType">the parameter type</param>
        /// <param name="valueArrayIndex" the index of the <see cref="CompoundParameterType"/> in the <see cref="ParameterTypeComponent"/></param>
        /// <returns>the view model</returns>
        IParameterTypeEditorSelectorViewModel CreateParameterTypeEditorSelectorViewModel(ParameterType parameterType, int valueArrayIndex);
    }
}