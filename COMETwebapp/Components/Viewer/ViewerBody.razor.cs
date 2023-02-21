﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ViewerBody.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.Components.Viewer
{
    using CDP4Common.EngineeringModelData;
    
    using COMETwebapp.Model;
    using COMETwebapp.Components.Viewer.Canvas;
    using COMETwebapp.ViewModels.Components.Viewer.Canvas;

    using Microsoft.AspNetCore.Components;
    
    using ReactiveUI;

    /// <summary>
    /// Support class for the <see cref="ViewerBody"/> component
    /// </summary>
    public partial class ViewerBody
    {
        /// <summary>
        /// The reference to the <see cref="CanvasComponent"/> component
        /// </summary>
        public CanvasComponent CanvasComponent { get; private set; }

        /// <summary>
        /// The initial <see cref="Option" />
        /// </summary>
        [Parameter]
        public Option InitialOption { get; set; }
        
        /// <summary>
        /// Method invoked after each time the component has been rendered. Note that the component does
        /// not automatically re-render after the completion of any returned <see cref="Task"/>, because
        /// that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">
        /// Set to <c>true</c> if this is the first time <see cref="OnAfterRender(bool)"/> has been invoked
        /// on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        /// <remarks>
        /// The <see cref="OnAfterRender(bool)"/> and <see cref="OnAfterRenderAsync(bool)"/> lifecycle methods
        /// are useful for performing interop, or interacting with values received from <c>@ref</c>.
        /// Use the <paramref name="firstRender"/> parameter to ensure that initialization work is only performed
        /// once.
        /// </remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await base.OnInitializedAsync();
                this.ViewModel.InitializeViewModel();
                this.ViewModel.OptionSelector.SelectedOption = this.InitialOption;

                await this.CanvasComponent.ViewModel.InitCanvas(true);

                this.Disposables.Add(this.WhenAnyValue(x => x.ViewModel.OptionSelector.SelectedOption)
                    .Subscribe(_ => this.UpdateUrl()));

                this.Disposables.Add(this.WhenAnyValue(x => x.ViewModel.ProductTreeViewModel.RootViewModel).Subscribe(async _ =>
                {
                    await this.RepopulateScene(this.ViewModel.ProductTreeViewModel.RootViewModel);
                }));
            }
        }

        /// <summary> 
        /// Repopulates the scene starting from the passed <see cref="TreeNode"/>  
        /// </summary> 
        /// <param name="rootNode">the top node of the hierarchy that needs to be on scene</param> 
        /// <returns>an asynchronous operation</returns> 
        private async Task RepopulateScene(INodeComponentViewModel rootNode)
        {
            await this.CanvasComponent.ViewModel.ClearScene();

            var sceneObjects = rootNode.GetFlatListOfDescendants().Where(x => x.Node.SceneObject.Primitive is not null).Select(x => x.Node.SceneObject).ToList();

            foreach (var sceneObject in sceneObjects)
            {
                await this.CanvasComponent.ViewModel.AddSceneObject(sceneObject);
            }
        }

        /// <summary>
        /// Sets the url of the <see cref="NavigationManager" /> based on the current values
        /// </summary>
        private void UpdateUrl()
        {
            var additionalParameters = new Dictionary<string, string>();

            if (this.ViewModel.OptionSelector.SelectedOption != null)
            {
                additionalParameters["option"] = this.ViewModel.OptionSelector.SelectedOption.Iid.ToString();
            }

            this.UpdateUrlWithParameters(additionalParameters, nameof(Pages.Viewer.Viewer));
        }
    }
}
