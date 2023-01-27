﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IOrientationViewModel.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.ViewModels.Components.Viewer.PropertiesPanel
{
    using CDP4Common.SiteDirectoryData;
    using COMETwebapp.Enumerations;
    using COMETwebapp.Model;
    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// Interface for the <see cref="OrientationViewModel"/>
    /// </summary>
    public interface IOrientationViewModel
    {
        /// <summary>
        /// Gets or sets the angle format. 
        /// </summary>
        Enumerations.AngleFormat AngleFormat { get; set; } 

        /// <summary>
        /// Gets or sets the <see cref="Orientation"/>
        /// </summary>
        Orientation Orientation { get; set; }

        /// <summary>
        /// Event for when the euler angles changed
        /// </summary>
        /// <param name="sender">the sender of the event. Rx,Ry or Ry</param>
        /// <param name="e">the args of the event</param>
        void OnEulerAnglesChanged(string sender, ChangeEventArgs e);

        /// <summary> 
        /// Event for when the matrix values changed 
        /// </summary> 
        void OnMatrixValuesChanged(int index, ChangeEventArgs e);

        /// <summary> 
        /// Event for when the angle format has changed 
        /// </summary> 
        /// <param name="angleFormat"></param> 
        void OnAngleFormatChanged(AngleFormat angleFormat);
    }
}
