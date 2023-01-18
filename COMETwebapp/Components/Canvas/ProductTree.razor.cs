﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductTree.cs" company="RHEA System S.A.">
//    Copyright (c) 2022 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar
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

namespace COMETwebapp.Components.Canvas
{
    using COMETwebapp.Model;
    using Microsoft.AspNetCore.Components;

    public partial class ProductTree
    {
        /// <summary>
        /// The root node of the tree
        /// </summary>
        [Parameter]
        public TreeNode? RootNode { get; set; }

        /// <summary>
        /// Event for when a node selection changed
        /// </summary>
        [Parameter]
        public EventCallback<TreeNode> OnTreeSelectionChanged { get; set; }

        /// <summary>
        /// Event for when a node visibility changed
        /// </summary>
        [Parameter]
        public EventCallback<TreeNode> OnTreeNodeVisibilityChanged { get; set; }

        /// <summary>
        /// Method for when a node is selected
        /// </summary>
        /// <param name="node">the selected <see cref="TreeNode"/></param>
        private void TreeSelectionChanged(TreeNode node)
        {
            this.OnTreeSelectionChanged.InvokeAsync(node);
        }

        /// <summary>
        /// Method for when a node visibility changed
        /// </summary>
        /// <param name="node">the selected <see cref="TreeNode"/></param>
        private void TreeNodeVisibilityChanged(TreeNode node)
        {
            this.OnTreeNodeVisibilityChanged.InvokeAsync(node);
        }

        /// <summary>
        /// Calls the StateHasChanged method to refresh this component
        /// </summary>
        public void Refresh()
        {
            this.StateHasChanged();
        }
    }
}
