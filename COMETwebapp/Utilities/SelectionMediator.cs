﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionMediator.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.Utilities
{
    using COMETwebapp.Model;

    /// <summary>
    /// Class for controlling the selecetion of <see cref="SceneObject"/> in the Scene
    /// </summary>
    public class SelectionMediator : ISelectionMediator
    {
        /// <summary>
        /// Event for when the tree selection has changed
        /// </summary>
        public event EventHandler<TreeNode> OnTreeSelectionChanged;

        /// <summary>
        /// Event for when a node in the tree has changed his visibility
        /// </summary>
        public event EventHandler<TreeNode> OnTreeVisibilityChanged;

        /// <summary>
        /// Event for when the model selection has changed
        /// </summary>
        public event EventHandler<SceneObject> OnModelSelectionChanged;

        /// <summary>
        /// Raises the <see cref="OnTreeSelectionChanged"/> event
        /// </summary>
        /// <param name="node">the node that raised the event</param>
        public void RaiseOnTreeSelectionChanged(TreeNode node)
        {
            this.OnTreeSelectionChanged?.Invoke(this, node);
        }

        /// <summary>
        /// Raises the <see cref="OnTreeVisibilityChanged"/> event
        /// </summary>
        /// <param name="node"></param>
        public void RaiseOnTreeVisibilityChanged(TreeNode node)
        {
            this.OnTreeVisibilityChanged?.Invoke(this, node);
        }

        /// <summary>
        /// Raises the <see cref="OnModelSelectionChanged"/> event
        /// </summary>
        /// <param name="sceneObject"></param>
        public void RaiseOnModelSelectionChanged(SceneObject sceneObject)
        {
            this.OnModelSelectionChanged?.Invoke(this, sceneObject);
        }
    }
}