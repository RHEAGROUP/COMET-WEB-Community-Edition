﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryHierarchyDiagram.razor.cs" company="RHEA System S.A.">
//    Copyright (c) 2023 RHEA System S.A.
//
//    Authors: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Nabil Abbar
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

namespace COMETwebapp.Components.ReferenceData
{
    using COMETwebapp.ViewModels.Components.ReferenceData;

    using Blazor.Diagrams.Core;
    using Blazor.Diagrams.Core.Models;

    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Support class for the <see cref="ParameterTypeTable"/>
    /// </summary>
    public partial class CategoryHierarchyDiagram
    {
        /// <summary>
        ///     The <see cref="ICategoryHierarchyDiagramViewModel" /> for the component
        /// </summary>
        [Parameter]
        public ICategoryHierarchyDiagramViewModel ViewModel { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var options = new DiagramOptions
            {
                DefaultNodeComponent = null, // Default component for nodes
                AllowMultiSelection = false,
                Links = new DiagramLinkOptions
                {
                    Factory = (diagram, sourcePort) =>
                    {
                        return new LinkModel(sourcePort, null)
                        {
                            Router = Routers.Orthogonal,
                            PathGenerator = PathGenerators.Straight,
                        };
                    }
                },
                Zoom = new DiagramZoomOptions
                {
                    Enabled = false,
                },
            };
            this.ViewModel.Diagram = new Diagram(options);
            this.ViewModel.Diagram.RegisterModelComponent<CategoryNode, CategoryNodeComponent>();
        }
    }
}
