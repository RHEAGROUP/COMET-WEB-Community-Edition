﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ProductTreeViewModel.cs" company="RHEA System S.A.">
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
namespace COMETwebapp.ViewModels.Components.Viewer.Canvas
{
    using System.Linq;

    using COMETwebapp.Enumerations;
    using COMETwebapp.Model;
    
    using ReactiveUI;

    /// <summary>
    /// View Model for the <see cref="COMETwebapp.Components.Viewer.Canvas.ProductTree"/>
    /// </summary>
    public class ProductTreeViewModel : ReactiveObject, IProductTreeViewModel
    {
        /// <summary>
        /// Gets or sets the filter options for the tree
        /// </summary>
        public IReadOnlyList<TreeFilter> TreeFilters { get; private set; }

        /// <summary>
        /// Backing field for the <see cref="SelectedFilter"/>
        /// </summary>
        private TreeFilter treeFilter;

        /// <summary>
        /// Gets or sets the selected filter option
        /// </summary>
        public TreeFilter SelectedFilter
        {
            get => this.treeFilter;
            set => this.RaiseAndSetIfChanged(ref this.treeFilter, value);
        }

        /// <summary>
        /// Gets or sets the root of the <see cref="COMETwebapp.Components.Viewer.Canvas.ProductTree"/>
        /// </summary>
        public TreeNode RootNode { get; set; }

        /// <summary>
        /// Backing field for the <see cref="SearchText"/>
        /// </summary>
        private string searchText;

        /// <summary>
        /// Gets or sets the search text used for filtering the tree
        /// </summary>
        public string SearchText
        {
            get => this.searchText;
            set => this.RaiseAndSetIfChanged(ref this.searchText, value);
        }

        /// <summary>
        /// Creates a new instance of type <see cref="ProductTreeViewModel"/>
        /// </summary>
        public ProductTreeViewModel()
        {
            var enumValues = Enum.GetValues(typeof(TreeFilter)).Cast<TreeFilter>();
            this.TreeFilters = enumValues.ToList();
            this.SelectedFilter = TreeFilter.ShowFullTree;

            this.WhenAnyValue(x => x.SearchText).Subscribe(_ => this.OnSearchFilterChange());
            this.WhenAnyValue(x => x.SelectedFilter).Subscribe(_ => this.OnFilterChanged());
        }

        /// <summary>
        /// Event for when the filter on the tree changes
        /// </summary>
        /// <param name="showNodeWithGeometry">if the tree should show a complete tree or just nodes with geometry</param>
        public void OnFilterChanged()
        {
            var fullTree = this.RootNode?.GetFlatListOfDescendants(true);

            if(fullTree is not null)
            {
                if(this.SelectedFilter == TreeFilter.ShowNodesWithGeometry)
                {
                    foreach (var node in fullTree)
                    {
                        node.IsDrawn = node.SceneObject.Primitive != null;
                    }
                }
                else
                {
                    fullTree?.ForEach(x => x.IsDrawn = true);
                }
            }
        }

        /// <summary>
        /// Event for when the text of the search filter is changing
        /// </summary>
        /// <param name="e">the args of the event</param>
        public void OnSearchFilterChange()
        {
            var fullTree = this.RootNode?.GetFlatListOfDescendants(true);

            if (this.SearchText == string.Empty)
            {
                fullTree?.ForEach(x => x.IsDrawn = true);
            }
            else
            {
                fullTree?.ForEach(x =>
                {
                    if (!x.Title.Contains(this.SearchText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        x.IsDrawn = false;
                    }
                    else
                    {
                        x.IsDrawn = true;
                    }
                });
            }
        }
    }
}
