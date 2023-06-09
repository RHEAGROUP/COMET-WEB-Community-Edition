﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ViewerNodeViewModel.cs" company="RHEA System S.A.">
//     Copyright (c) 2023 RHEA System S.A.
// 
//     Authors: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Théate Antoine
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

namespace COMETwebapp.ViewModels.Components.Viewer
{
    using COMETwebapp.Model;
    using COMETwebapp.Utilities;
    using COMETwebapp.ViewModels.Components.Shared;

    using ReactiveUI;

    /// <summary>
    /// ViewModel for a node in the Viewer tree
    /// </summary>
    public class ViewerNodeViewModel : BaseNodeViewModel<ViewerNodeViewModel>
    {
        /// <summary>
        /// The <see cref="SceneObject"/> that this <see cref="ViewerNodeViewModel"/> represents
        /// </summary>
        public SceneObject SceneObject { get; }

        /// <summary>
        /// Gets or set the <see cref="ISelectionMediator"/>
        /// </summary>
        public ISelectionMediator SelectionMediator { get; set; }

        /// <summary>
        /// Backing field for the <see cref="IsSceneObjectVisible"/>
        /// </summary>
        private bool isSceneObjectVisible = true;

        /// <summary>
        /// Gets or sets if the <see cref="SceneObject"/> asociated to the <see cref="ViewerNodeViewModel"/> is visible
        /// </summary>
        public bool IsSceneObjectVisible
        {
            get => this.isSceneObjectVisible;
            set => this.RaiseAndSetIfChanged(ref this.isSceneObjectVisible, value);
        }

        /// <summary> 
        /// Gets or sets if the propagation of the click event should be stopped  
        /// </summary> 
        private bool StopClickPropagation { get; set; }

        /// <summary>
        /// Creates a new instance of type <see cref="ViewerNodeViewModel"/>
        /// </summary>
        /// <param name="sceneObject">the <see cref="SceneObject"/></param>
        public ViewerNodeViewModel(SceneObject sceneObject) : base(sceneObject?.ElementBase?.Name)
        {
            this.SceneObject = sceneObject;
        }

        /// <summary>
        /// Callback method for when a node is selected
        /// </summary>
        /// <param name="node">the selected node</param>
        public override void TreeSelectionChanged(ViewerNodeViewModel node)
        {
            this.GetRootNode().GetFlatListOfDescendants(true).ForEach(x => x.IsSelected = false);

            if (!this.StopClickPropagation)
            {
                this.SelectionMediator.RaiseOnTreeSelectionChanged(node);
                this.IsSelected = true;
            }

            this.StopClickPropagation = false;
        }

        /// <summary>
        /// Callback method for when the node visibility changed
        /// </summary>
        /// <param name="node"></param>
        public void TreeNodeVisibilityChanged(ViewerNodeViewModel node)
        {
            this.StopClickPropagation = true;
            this.SelectionMediator.RaiseOnTreeVisibilityChanged(node);
        }

        /// <summary>
        /// Overrides the equals method for equality checking
        /// </summary>
        /// <param name="obj">the object to check for equality</param>
        /// <returns>true if the objects are the same, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj is not ViewerNodeViewModel treeNode)
            {
                return false;
            }

            return this.SceneObject.ID == treeNode.SceneObject.ID;
        }

        /// <summary>
        /// Gets the hashcode of this <see cref="ViewerNodeViewModel"/>
        /// </summary>
        /// <returns>the hashcode</returns>
        public override int GetHashCode()
        {
            return this.SceneObject.ID.GetHashCode();
        }
    }
}
