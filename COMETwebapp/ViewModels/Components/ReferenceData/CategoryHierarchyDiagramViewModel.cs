﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryHierarchyDiagramViewModel.cs" company="RHEA System S.A.">
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
namespace COMETwebapp.ViewModels.Components.ReferenceData
{
    using System.Linq;

    using Blazor.Diagrams.Core;
    using Blazor.Diagrams.Core.Models;
    using Blazor.Diagrams.Core.Geometry;

    using CDP4Common.SiteDirectoryData;

    using ReactiveUI;
    using COMETwebapp.Components.ReferenceData;

    /// <summary>
    ///     View model for the <see cref="CategoryHierarchyDiagram" /> component
    /// </summary>
    public class CategoryHierarchyDiagramViewModel : ReactiveObject, ICategoryHierarchyDiagramViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="SelectedCategory" />
        /// </summary>
        private Category selectedCategory;

        /// <summary>
        ///     The selected <see cref="Category"/>
        /// </summary>
        public Category SelectedCategory
        {
            get => this.selectedCategory;
            set => this.RaiseAndSetIfChanged(ref this.selectedCategory, value);
        }

        /// <summary>
        /// A collection of <see cref="Category" />
        /// </summary>
        public IEnumerable<Category> Rows { get; set; }

        /// <summary>
        /// A collection of <see cref="Category" />
        /// </summary>
        public IEnumerable<Category> SubCategories { get; set; }

        /// <summary>
        /// The categories hierarchy <see cref="Diagram" /> to display
        /// </summary>
        public Diagram Diagram { get; set; }

        /// <summary>
        /// The svg path of arrow
        /// </summary>
        const string svgArrowPath = "M -0.093 17.86 V -18.5 L 29.233 -0.32 L -0.093 17.86 z M 1.407 -15.806 v 30.9715 L 26.3865 -0.32 L 1.407 -15.806 z";

        /// <summary>
        /// Create diagram nodes and links
        /// </summary>
        public void SetupDiagram()
        {
            this.Diagram.Nodes.Clear();
            var position = new Point(50, 50);
            var nodeCaption = this.SelectedCategory.Name;
            var node12 = new CategoryNode(this.SelectedCategory, position);
            node12.AddPort(PortAlignment.Bottom);
            node12.AddPort(PortAlignment.Top);
            node12.Title = this.SelectedCategory.Name;
            Diagram.Nodes.Add(node12);
            var numberOfNodes = this.Rows.Count();

            foreach (var row in this.Rows)
            {
                int distanceBetweenNodes = 200;
                int currentIndex = this.Rows.ToList().IndexOf(row);
                int xOffset = (currentIndex - (numberOfNodes - 1) / 2) * distanceBetweenNodes;

                position = new Point(node12.Position.X - xOffset, 300);

                var node = new CategoryNode(row, position);
                node.Title = row.Name;
                node.AddPort(PortAlignment.Top);
                Diagram.Nodes.Add(node);
                Diagram.Links.Add(new LinkModel(node12.GetPort(PortAlignment.Bottom), node.GetPort(PortAlignment.Top))
                {
                    Router = Routers.Orthogonal,
                    PathGenerator = PathGenerators.Straight,
                    TargetMarker = new LinkMarker(svgArrowPath, 100)
                });

            }

            var numberOfSubNodes = this.SubCategories.Count();

            // add subcategories
            foreach (var subCategory in this.SubCategories)
            {
                int distanceBetweenNodes = 200; // You can adjust this value to control the spacing between nodes
                int currentIndex = this.SubCategories.ToList().IndexOf(subCategory);
                int xOffset = (currentIndex - (numberOfSubNodes - 1) / 2) * distanceBetweenNodes;

                var position2 = new Point(node12.Position.X + xOffset, -200);

                var node2 = new CategoryNode(subCategory, position2);
                node2.Title = subCategory.Name;
                node2.AddPort(PortAlignment.Bottom);
                Diagram.Nodes.Add(node2);
                Diagram.Links.Add(new LinkModel(node2.GetPort(PortAlignment.Bottom), node12.GetPort(PortAlignment.Top))
                {
                    Router = Routers.Orthogonal,
                    PathGenerator = PathGenerators.Straight,
                    TargetMarker = new LinkMarker(svgArrowPath, 100)
                });
            }
        }
    }
}
