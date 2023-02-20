﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="OpenModelViewModel.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.ViewModels.Components.Shared
{
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using COMETwebapp.Model;
    using COMETwebapp.Services.SessionManagement;
    using COMETwebapp.Utilities.DisposableObject;

    using DynamicData.Binding;

    using ReactiveUI;

    /// <summary>
    /// View Model that enables a user to open an <see cref="EngineeringModel" />
    /// </summary>
    public class OpenModelViewModel : DisposableObject, IOpenModelViewModel
    {
        /// <summary>
        /// The <see cref="ISessionService" />
        /// </summary>
        private readonly ISessionService sessionService;

        /// <summary>
        /// Backing field for <see cref="IsOpeningSession" />
        /// </summary>
        private bool isOpeningSession;

        /// <summary>
        /// Backing field for <see cref="SelectedDomainOfExpertise" />
        /// </summary>
        private DomainOfExpertise selectedDomainOfExpertise;

        /// <summary>
        /// </summary>
        private EngineeringModelSetup selectedEngineeringModel;

        /// <summary>
        /// Backing field for <see cref="SelectedIterationSetup" />
        /// </summary>
        private IterationData selectedIterationSetup;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenModelViewModel" /> class.
        /// </summary>
        /// <param name="sessionService">The <see cref="ISessionService" /></param>
        public OpenModelViewModel(ISessionService sessionService)
        {
            this.sessionService = sessionService;

            this.Disposables.Add(this.WhenAnyPropertyChanged(nameof(this.SelectedEngineeringModel))
                .Subscribe(_ => this.ComputeAvailableCollections()));
        }

        /// <summary>
        /// The selected <see cref="EngineeringModelSetup" />
        /// </summary>
        public EngineeringModelSetup SelectedEngineeringModel
        {
            get => this.selectedEngineeringModel;
            set => this.RaiseAndSetIfChanged(ref this.selectedEngineeringModel, value);
        }

        /// <summary>
        /// The selected <see cref="IterationData" />
        /// </summary>
        public IterationData SelectedIterationSetup
        {
            get => this.selectedIterationSetup;
            set => this.RaiseAndSetIfChanged(ref this.selectedIterationSetup, value);
        }

        /// <summary>
        /// The selected <see cref="DomainOfExpertise" />
        /// </summary>
        public DomainOfExpertise SelectedDomainOfExpertise
        {
            get => this.selectedDomainOfExpertise;
            set => this.RaiseAndSetIfChanged(ref this.selectedDomainOfExpertise, value);
        }

        /// <summary>
        /// A collection of available <see cref="EngineeringModelSetup" />
        /// </summary>
        public IEnumerable<EngineeringModelSetup> AvailableEngineeringModelSetups { get; set; }

        /// <summary>
        /// A collection of available <see cref="IterationData" />
        /// </summary>
        public IEnumerable<IterationData> AvailableIterationSetups { get; set; }

        /// <summary>
        /// A collection of available <see cref="DomainOfExpertise" />
        /// </summary>
        public IEnumerable<DomainOfExpertise> AvailablesDomainOfExpertises { get; set; }

        /// <summary>
        /// Value asserting that the session is on way to open
        /// </summary>
        public bool IsOpeningSession
        {
            get => this.isOpeningSession;
            set => this.RaiseAndSetIfChanged(ref this.isOpeningSession, value);
        }

        /// <summary>
        /// Initializes this view model properties
        /// </summary>
        public void InitializesProperties()
        {
            this.SelectedDomainOfExpertise = null;
            this.SelectedEngineeringModel = null;
            this.SelectedIterationSetup = null;
            this.IsOpeningSession = false;

            this.AvailableEngineeringModelSetups = this.sessionService.GetParticipantModels()
                .Where(x => x.IterationSetup.Any(setup => this.sessionService.OpenIterations.Items.All(i => i.Iid != setup.IterationIid)))
                .OrderBy(x => x.Name);
        }

        /// <summary>
        /// Opens the <see cref="EngineeringModel" /> based on the selected field
        /// </summary>
        /// <returns></returns>
        public async Task OpenSession()
        {
            if (this.SelectedIterationSetup != null && this.SelectedDomainOfExpertise != null)
            {
                this.IsOpeningSession = true;

                await this.sessionService.ReadIteration(this.SelectedEngineeringModel.IterationSetup
                    .First(x => x.Iid == this.SelectedIterationSetup.IterationSetupId), this.SelectedDomainOfExpertise);

                this.IsOpeningSession = false;
            }
        }

        /// <summary>
        /// Preselects the <see cref="Iteration" /> to open
        /// </summary>
        /// <param name="modelId">The <see cref="Guid" /> of the <see cref="EngineeringModel" /></param>
        /// <param name="iterationId">The <see cref="Guid" /> of the <see cref="Iteration" /> to open</param>
        /// <param name="domainId">The <see cref="Guid" /> of the <see cref="DomainOfExpertise" /> to select</param>
        public void PreSelectIteration(Guid modelId, Guid iterationId, Guid domainId)
        {
            this.selectedEngineeringModel = this.AvailableEngineeringModelSetups.FirstOrDefault(x => x.Iid == modelId);
            var iterationSetup = this.SelectedEngineeringModel?.IterationSetup.FirstOrDefault(x => x.IterationIid == iterationId);

            if (iterationSetup != null)
            {
                this.SelectedIterationSetup = new IterationData(iterationSetup);
            }

            this.AvailablesDomainOfExpertises = this.sessionService.GetModelDomains(this.SelectedEngineeringModel);
            this.SelectedDomainOfExpertise = this.AvailablesDomainOfExpertises.FirstOrDefault(x => x.Iid == domainId);
        }

        /// <summary>
        /// Compute the available collection based on the selected <see cref="EngineeringModelSetup" />
        /// </summary>
        private void ComputeAvailableCollections()
        {
            this.SelectedDomainOfExpertise = null;
            this.SelectedIterationSetup = null;

            if (this.SelectedEngineeringModel == null)
            {
                this.AvailablesDomainOfExpertises = new List<DomainOfExpertise>();
                this.AvailableIterationSetups = new List<IterationData>();
            }
            else
            {
                this.AvailablesDomainOfExpertises = this.sessionService.GetModelDomains(this.SelectedEngineeringModel);

                this.AvailableIterationSetups = this.SelectedEngineeringModel.IterationSetup
                    .Where(x => this.sessionService.OpenIterations.Items.All(i => i.Iid != x.IterationIid))
                    .OrderBy(x => x.IterationNumber)
                    .Select(x => new IterationData(x));
            }
        }
    }
}
