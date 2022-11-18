﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BabylonCanvas.razor.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.Components.Viewer
{
    using System;
    using System.Threading.Tasks;

    using CDP4Common.EngineeringModelData;

    using COMETwebapp.Primitives;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.JSInterop;

    /// <summary>
    /// Partial class for the <see cref="BabylonCanvas"/> component
    /// </summary>
    public partial class BabylonCanvas 
    {
        /// <summary>
        /// Reference to the HTML5 canvas
        /// </summary>
        public ElementReference CanvasReference { get; set; }

        /// <summary>
        /// Gets or sets if the mouse is down on the scene.
        /// </summary>
        public bool IsMouseDown { get; private set; } = false;

        /// <summary>
        /// Gets or sets if the scene is rotating.
        /// </summary>
        public bool IsRotating { get; private set; } = false;

        /// <summary>
        /// Invokable method from JS to get a GUID.
        /// </summary>
        /// <returns>the GUID in string format</returns>
        [JSInvokable]
        public static string GetGUID() => Guid.NewGuid().ToString();

        /// <summary>
        /// Shape factory for creating <see cref="Primitive"/> from a <see cref="ElementUsage"/>
        /// </summary>
        [Inject]
        public IShapeFactory? ShapeFactory { get; set; }

        /// <summary>
        /// The provider of the 3D Scene
        /// </summary>
        [Inject]
        public ISceneProvider SceneProvider { get; set; }

        /// <summary>
        /// Method invoked after each time the component has been rendered. Note that the component does
        /// not automatically re-render after the completion of any returned <see cref="Task"/>, because
        /// that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">
        /// Set to <c>true</c> if this is the first time <see cref="OnAfterRenderAsync(bool)"/> has been invoked
        /// on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        /// <remarks>
        /// The <see cref="OnAfterRenderAsync(bool)"/> lifecycle methods
        /// are useful for performing interop, or interacting with values received from <c>@ref</c>.
        /// Use the <paramref name="firstRender"/> parameter to ensure that initialization work is only performed
        /// once.
        /// </remarks>
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {               
                this.SceneProvider.InitCanvas(this.CanvasReference);
                this.SceneProvider.AddWorldAxes();
            }
        }
               
        /// <summary>
        /// Canvas on mouse down event
        /// </summary>
        /// <param name="e">the mouse args of the event</param>
        public void OnMouseDown(MouseEventArgs e)
        {
            this.IsMouseDown = true;
            //TODO: when the tools are ready here we are going to manage the different types of actions that a user can make.
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            this.IsRotating = this.IsMouseDown;
        }

        /// <summary>
        /// Canvas on mouse up event
        /// </summary>
        /// <param name="e">the mouse args of the event</param>
        public async void OnMouseUp(MouseEventArgs e)
        {
            this.IsMouseDown = false;

            if (!this.IsRotating && e.Button != 1)
            {
                this.SceneProvider.ClearTemporaryPrimitives();
            }

            //Left
            if (!this.IsRotating && e.Button == 0) 
            {
                var primitive = await this.SceneProvider.GetPrimitiveUnderMouseAsync();
                this.SceneProvider.SelectedPrimitive = null;

                if (primitive is not null)
                {
                    this.SceneProvider.SelectedPrimitive = primitive;
                }
                this.SceneProvider.RaiseSelectionChanged(primitive);
            }
        }

        /// <summary>
        /// Canvas on key down event
        /// </summary>
        /// <param name="e">the keyboard args of the event</param>
        public void OnKeyDown(KeyboardEventArgs e)
        {

        }

        /// <summary>
        /// Canvas on key up event
        /// </summary>
        /// <param name="e">the keyboard args of the event</param>
        public void OnKeyUp(KeyboardEventArgs e)
        {
            if(e.Key == "Escape")
            {
                this.SceneProvider.ClearTemporaryPrimitives();
            }
        }

        /// <summary>
        /// Clears the scene and populates again with the <see cref="ElementUsage"/> 
        /// </summary>
        /// <param name="elementUsages">the <see cref="ElementUsage"/> used for the population</param>
        /// <param name="selectedOption">the current <see cref="Option"/> selected</param>
        /// <param name="states">the <see cref="ActualFiniteState"/> that are going to be used to position the <see cref="Primitive"/></param>
        public void RepopulateScene(List<ElementUsage> elementUsages, Option selectedOption, List<ActualFiniteState> states)
        {
            this.SceneProvider.ClearPrimitives();

            foreach (var elementUsage in elementUsages)
            {
                var basicShape = this.ShapeFactory.CreatePrimitiveFromElementUsage(elementUsage, selectedOption, states);

                if (basicShape is not null)
                {
                    this.SceneProvider.AddPrimitive(basicShape);
                }
            }
        }
    }
}
