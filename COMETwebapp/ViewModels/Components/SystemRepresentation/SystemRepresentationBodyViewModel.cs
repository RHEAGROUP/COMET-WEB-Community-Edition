﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="SystemRepresentationBodyViewModel.cs" company="RHEA System S.A.">
//     Copyright (c) 2023 RHEA System S.A.
// 
//     Authors: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Théate Antoine, Nabil Abbar
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

namespace COMETwebapp.ViewModels.Components.SystemRepresentation
{
    using CDP4Common.EngineeringModelData;

    using CDP4Dal;

    using COMET.Web.Common.Extensions;
    using COMET.Web.Common.Services.SessionManagement;
    using COMET.Web.Common.ViewModels.Components;
    using COMET.Web.Common.ViewModels.Components.Selectors;

    using COMETwebapp.Model;

    using Microsoft.AspNetCore.Components;
    
    using ReactiveUI;

    /// <summary>
    /// View Model that handle the logic for the System Representation application
    /// </summary>
    public class SystemRepresentationBodyViewModel : SingleIterationApplicationBaseViewModel, ISystemRepresentationBodyViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleIterationApplicationBaseViewModel" /> class.
        /// </summary>
        /// <param name="sessionService">The <see cref="ISessionService" /></param>
        public SystemRepresentationBodyViewModel(ISessionService sessionService) : base(sessionService)
        {
            this.SystemTreeViewModel = new SystemTreeViewModel
            {
                OnClick = new EventCallbackFactory().Create<SystemNode>(this, this.SelectElement)
            };

            this.Disposables.Add(this.WhenAnyValue(x => x.OptionSelector.SelectedOption).SubscribeAsync(_ => this.ApplyFilters()));
        }

        /// <summary>
        /// Gets the <see cref="IOptionSelectorViewModel" />
        /// </summary>
        public IOptionSelectorViewModel OptionSelector { get; private set; } = new OptionSelectorViewModel(false);

        /// <summary>
        /// Represents the RootNode of the tree
        /// </summary>
        public SystemNode RootNode { get; set; }

        /// <summary>
        /// The <see cref="ISystemTreeViewModel" />
        /// </summary>
        public ISystemTreeViewModel SystemTreeViewModel { get; }

        /// <summary>
        /// The <see cref="IElementDefinitionDetailsViewModel" />
        /// </summary>
        public IElementDefinitionDetailsViewModel ElementDefinitionDetailsViewModel { get; } = new ElementDefinitionDetailsViewModel();

        /// <summary>
        /// All <see cref="ElementBase" /> of the iteration
        /// </summary>
        public List<ElementBase> Elements { get; set; } = new();

        /// <summary>
        /// Updates Elements list when a filter for option is selected
        /// </summary>
        /// <param name="selectedOption">the selected <see cref="Option"/></param>
        public void OnOptionFilterChange(Option selectedOption)
        {
            this.Elements.Clear();
        
            var nestedElements = this.CurrentIteration.QueryNestedElements(selectedOption).ToList();

            var associatedElements = new List<ElementUsage>();
            associatedElements.AddRange(nestedElements.SelectMany(x => x.ElementUsage));
            associatedElements = associatedElements.Distinct().ToList();

            var elementsToRemove = new List<ElementBase>();

            this.Elements.ForEach(e =>
            {
                if (e.GetType() == typeof(ElementUsage) && !associatedElements.Contains(e))
                {
                    elementsToRemove.Add(e);
                }
            });

            this.Elements.RemoveAll(e => elementsToRemove.Contains(e));

            this.InitializeElements();
            this.CreateElementUsages(this.Elements);
            this.SystemTreeViewModel.SystemNodes = new List<SystemNode> { this.RootNode };
        }

        /// <summary>
        /// Update this view model properties
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        protected override async Task OnIterationChanged()
        {
            this.Elements.Clear();
            await base.OnIterationChanged();
            this.OptionSelector.CurrentIteration = this.CurrentIteration;
            this.InitializeElements();
            await this.ApplyFilters();
            this.IsLoading = false;
        }

        /// <summary>
        /// Handles the change of <see cref="DomainOfExpertise" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        protected override async Task OnDomainChanged()
        {
            await base.OnDomainChanged();

            if (this.CurrentDomain != null)
            {
                this.IsLoading = true;
                await Task.Delay(1);
               
                await this.ApplyFilters();
                
                this.IsLoading = false;
            }
        }

        /// <summary>
        /// Handles the refresh of the current <see cref="ISession" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        protected override Task OnSessionRefreshed()
        {
            return this.OnIterationChanged();
        }

        /// <summary>
        /// Initialize <see cref="ElementBase" /> list
        /// </summary>
        private void InitializeElements()
        {
            if (this.CurrentIteration.TopElement != null)
            {
                this.Elements.Add(this.CurrentIteration.TopElement);
            }

            this.CurrentIteration.Element.ForEach(e => this.Elements.AddRange(e.ContainedElement));
        }

        /// <summary>
        /// Creates the <see cref="ElementUsage" /> used for the system tree nodes
        /// </summary>
        /// <param name="elements">the elements of the current <see cref="Iteration" /></param>
        private void CreateElementUsages(IEnumerable<ElementBase> elements)
        {
            var topElement = elements.First();
            this.RootNode = new SystemNode(topElement.Name);
            this.CreateTreeRecursively(topElement, this.RootNode, null);
        }

        /// <summary>
        /// Creates the tree in a recursive way
        /// </summary>
        /// <param name="elementBase"></param>
        /// <param name="current"></param>
        /// <param name="parent"></param>
        private void CreateTreeRecursively(ElementBase elementBase, SystemNode current, SystemNode parent)
        {
            var currentDomain = this.SessionService.GetDomainOfExpertise(this.CurrentIteration);
            var childsOfElementBase = elementBase switch
            {
                ElementDefinition elementDefinition => currentDomain != null ? elementDefinition.ContainedElement.Where(e => e.Owner == currentDomain).ToList() : elementDefinition.ContainedElement,
                ElementUsage elementUsage => currentDomain != null ? elementUsage.ElementDefinition.ContainedElement.Where(e => e.Owner == currentDomain).ToList() : elementUsage.ElementDefinition.ContainedElement,
                _ => null
            };

            if (childsOfElementBase is null)
            {
                return;
            }

            parent?.AddChild(current);

            foreach (var child in childsOfElementBase)
            {
                this.CreateTreeRecursively(child, new SystemNode(child.Name), current);
            }
        }

        /// <summary>
        /// set the selected <see cref="SystemNode" />
        /// </summary>
        /// <param name="selectedNode">The selected <see cref="SystemNode" /></param>
        /// <returns>A <see cref="Task" /></returns>
        private void SelectElement(SystemNode selectedNode)
        {
            // It is preferable to have a selection based on the Iid of the Thing
            this.ElementDefinitionDetailsViewModel.SelectedSystemNode = this.Elements.FirstOrDefault(e => e.Name.Equals(selectedNode.Title));
        }

        /// <summary>
        /// Apply all the filters on the <see cref="ISystemTreeViewModel" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ApplyFilters()
        {
            if (this.CurrentIteration != null)
            {
                this.IsLoading = true;
                await Task.Delay(1);

                this.OnOptionFilterChange(this.OptionSelector.SelectedOption);

                this.IsLoading = false;
            }
        }
    }
}
