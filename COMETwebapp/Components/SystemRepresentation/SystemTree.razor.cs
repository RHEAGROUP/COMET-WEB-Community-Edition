﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemTree.razor.cs" company="RHEA System S.A.">
//    Copyright (c) 2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Nabil Abbar
//
//    This file is part of COMET WEB Community Edition
//    The COMET WEB Community Edition is the RHEA Web Application implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET WEB Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET WEB Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace COMETwebapp.Components.SystemRepresentation
{
    using AntDesign;

    using COMETwebapp.Model;
    using COMETwebapp.ViewModels.Components.SystemRepresentation;
    
    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Partial class for the component <see cref="SystemTree"/>
    /// </summary>
    public partial class SystemTree
    {
        /// <summary>
        ///     The <see cref="ISystemTreeViewModel" /> for the component
        /// </summary>
        [Parameter]
        public ISystemTreeViewModel ViewModel { get; set; }

        /// <summary>
        ///    The <see cref="Tree{T}" /> to display
        /// </summary>
        Tree<SystemNode> tree;
    }
}
