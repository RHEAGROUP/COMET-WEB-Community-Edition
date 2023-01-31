﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="PropertiesComponentViewModel.cs" company="RHEA System S.A.">
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
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Types;
    
    using CDP4Dal;
    
    using COMETwebapp.Components.Viewer.Canvas;
    using COMETwebapp.Components.Viewer.PropertiesPanel;
    using COMETwebapp.IterationServices;
    using COMETwebapp.Model;
    using COMETwebapp.Services.IterationServices;
    using COMETwebapp.SessionManagement;
    using COMETwebapp.Utilities;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    
    using Newtonsoft.Json;
    
    using ReactiveUI;

    /// <summary>
    /// View Model for the <see cref="PropertiesComponent"/>
    /// </summary>
    public class PropertiesComponentViewModel : ReactiveObject, IPropertiesComponentViewModel
    {
        /// <summary>
        /// Injected property to get access to <see cref="IIterationService"/>
        /// </summary>
        [Inject]
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// Injected property to get access to <see cref="ISessionAnchor"/>
        /// </summary>
        [Inject]
        public ISessionAnchor SessionAnchor { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISelectionMediator"/>
        /// </summary>
        [Inject]
        public ISelectionMediator SelectionMediator { get; set; }

        /// <summary>
        /// Gets or sets the property used for the Interoperability
        /// </summary>
        [Inject]
        public IJSRuntime JsInterop { get; set; }

        /// <summary>
        /// The collection of <see cref="ParameterBase"/> and <see cref="IValueSet"/> of the selected <see cref="SceneObject"/>
        /// </summary>
        public Dictionary<ParameterBase, IValueSet> ParameterValueSetRelations { get; set; }

        /// <summary>
        /// Backing field for the <see cref="SelectedParameter"/>
        /// </summary>
        private ParameterBase selectedParameter;

        /// <summary>
        /// Gets or sets the selected <see cref="ParameterBase"/> to fill the details
        /// </summary>
        public ParameterBase SelectedParameter
        {
            get => this.selectedParameter;
            set => this.RaiseAndSetIfChanged(ref this.selectedParameter, value);
        }

        /// <summary>
        /// Backing field for the <see cref="ParametersInUse"/>
        /// </summary>
        private List<ParameterBase> parametersInUse;

        /// <summary>
        /// The list of parameters that the selected <see cref="SceneObject"/> uses
        /// </summary>
        public List<ParameterBase> ParametersInUse
        {
            get => this.parametersInUse;
            set => this.RaiseAndSetIfChanged(ref this.parametersInUse, value);
        }

        /// <summary>
        /// Backing field for the <see cref="ParameterHaveChanges"/>
        /// </summary>
        private bool parameterHaveChanges;

        /// <summary>
        /// Gets or sets if the parameters have changes
        /// </summary>
        public bool ParameterHaveChanges
        {
            get => this.parameterHaveChanges;
            set => this.RaiseAndSetIfChanged(ref this.parameterHaveChanges, value);
        }

        /// <summary>
        /// Backing field for the <see cref="IsVisible"/>
        /// </summary>
        private bool isVisible;

        /// <summary> 
        /// Gets or sets if this component is visible 
        /// </summary> 
        public bool IsVisible
        {
            get => this.isVisible;
            set => this.RaiseAndSetIfChanged(ref this.isVisible, value);
        }

        /// <summary>
        /// Event callback for when a <see cref="IValueSet"/> asociated to a <see cref="ParameterBase"/> has changed
        /// </summary>
        public EventCallback<Dictionary<ParameterBase,IValueSet>> OnParameterValueSetChanged { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IValueSet"/> asociated to a <see cref="ParameterBase"/> that have changed;
        /// </summary>
        public Dictionary<ParameterBase, IValueSet> ChangedParameterValueSetRelations { get; set; }

        /// <summary>
        /// Creates a new instance of type <see cref="PropertiesComponentViewModel"/>
        /// </summary>
        /// <param name="jsRuntime">the <see cref="IJSRuntime"/></param>
        /// <param name="iterationService">the <see cref="IIterationService"/></param>
        /// <param name="sessionAnchor">the <see cref="ISessionAnchor"/></param>
        /// <param name="selectionMediator">the <see cref="ISelectionMediator"/></param>
        public PropertiesComponentViewModel(IJSRuntime jsRuntime, IIterationService iterationService, ISessionAnchor sessionAnchor, ISelectionMediator selectionMediator)
        {
            this.JsInterop = jsRuntime;
            this.IterationService = iterationService;
            this.SessionAnchor = sessionAnchor;
            this.SelectionMediator = selectionMediator;

            this.OnParameterValueSetChanged = new EventCallbackFactory().Create(this, (Dictionary<ParameterBase, IValueSet> parameterValueSetRelations) => 
            { 
                this.OnParameterValueChanged(parameterValueSetRelations); 
            });

            this.SelectionMediator.OnTreeSelectionChanged += (sender, node) =>
            {
                this.OnSelectionChanged(node.SceneObject);
            };

            this.SelectionMediator.OnModelSelectionChanged += (sender, sceneObject) =>
            {
                this.OnSelectionChanged(sceneObject);
            };
        }              

        /// <summary> 
        /// Called when the selection of a <see cref="SceneObject"/> has changed 
        /// </summary> 
        /// <param name="sceneObject">the changed object</param> 
        /// <returns>an asynchronous operation</returns> 
        private void OnSelectionChanged(SceneObject sceneObject)
        {
            this.IsVisible = sceneObject is not null;
            
            if (this.SelectionMediator.SelectedSceneObjectClone is not null)
            {
                this.ParameterValueSetRelations = this.SelectionMediator.SelectedSceneObjectClone.GetParameterValueSetRelations();
                this.ParametersInUse = this.SelectionMediator.SelectedSceneObjectClone.ParametersAsociated.OrderBy(x => x.ParameterType.ShortName).ToList();
                this.SelectedParameter = this.ParametersInUse.First();
            }
        }

        /// <summary>
        /// When the button for submit changes is clicked
        /// </summary>
        public void OnSubmit()
        {
            this.SelectionMediator.SceneObjectHasChanges = false;
            this.ParameterHaveChanges = false;
            this.IterationService.NewUpdates.Clear();

            foreach (var keyValue in this.ChangedParameterValueSetRelations)
            {
                var valueSet = keyValue.Value;

                if (valueSet is ParameterValueSetBase parameterValueSetBase && !this.IterationService.NewUpdates.Contains(parameterValueSetBase.Iid))
                {
                    this.IterationService.NewUpdates.Add(parameterValueSetBase.Iid);
                    CDPMessageBus.Current.SendMessage(new NewUpdateEvent(parameterValueSetBase.Iid));

                    var clonedParameterValueSet = parameterValueSetBase.Clone(false);
                    var valueSetNewValue = valueSet.ActualValue;
                    clonedParameterValueSet.Manual = valueSetNewValue;
                    this.SessionAnchor.UpdateThings(new List<Thing>() { clonedParameterValueSet });
                }
            }

            this.ParameterHaveChanges = false;
        }

        /// <summary>
        /// Gets the values that have changed for two ParameterValueSetRelation
        /// </summary>
        /// <returns>the values changed</returns>
        public async void OnParameterValueChanged(Dictionary<ParameterBase,IValueSet> changedParameterValueSetRelations)
        {
            var relation1 = this.SelectionMediator.SelectedSceneObject?.GetParameterValueSetRelations();

            if (relation1 != null && changedParameterValueSetRelations != null)
            {
                this.ChangedParameterValueSetRelations = relation1.GetChangesOnParameters(changedParameterValueSetRelations);
                this.SelectionMediator.SceneObjectHasChanges = this.ChangedParameterValueSetRelations.Any();
                this.ParameterHaveChanges = this.ChangedParameterValueSetRelations.Any();

                foreach(var valueSet in this.ChangedParameterValueSetRelations.Values)
                {
                    await this.ParameterValueSetChanged(valueSet);
                }
            }
        }

        /// <summary>
        /// Event for when a <see cref="IValueSet"/> asociated to a <see cref="ParameterBase"/> has changed.
        /// </summary>
        /// <param name="valueSet"></param>
        public async Task ParameterValueSetChanged(IValueSet valueSet)
        {
            var newValueArray = new ValueArray<string>(valueSet.ActualValue);

            if (valueSet is ParameterValueSetBase parameterValueSetBase)
            {
                var clonedValueSetBase = parameterValueSetBase.Clone(false);
                clonedValueSetBase.Manual = newValueArray;
                this.ParameterValueSetRelations[this.SelectedParameter] = clonedValueSetBase;

                this.SelectionMediator?.SelectedSceneObjectClone?.UpdateParameter(this.SelectedParameter, clonedValueSetBase);

                if (this.SelectedParameter.ParameterType.ShortName == SceneSettings.ShapeKindShortName)
                {
                    if (this.SelectionMediator?.SelectedSceneObjectClone?.Primitive is not null)
                    {
                        this.SelectionMediator.SelectedSceneObjectClone.Primitive.HasHalo = true;
                    }
                    
                    var parameters = this.ParameterValueSetRelations.Keys.Where(x => x.ParameterType.ShortName != SceneSettings.ShapeKindShortName);
                    
                    foreach (var parameter in parameters)
                    {
                        this.SelectionMediator?.SelectedSceneObjectClone?.UpdateParameter(parameter, this.ParameterValueSetRelations[parameter]);
                    }
                }

                var jsonSceneObject = JsonConvert.SerializeObject(this.SelectionMediator?.SelectedSceneObjectClone, Formatting.Indented);
                await this.JsInterop.InvokeVoidAsync("RegenMesh", jsonSceneObject);
            }
        }

        /// <summary> 
        /// Event for when a parameter item has been clicked 
        /// </summary> 
        /// <param name="parameterBase">the parameter clicked</param> 
        public void OnParameterClick(ParameterBase parameterBase)
        {
            this.SelectedParameter = parameterBase;
        }
    }
}
