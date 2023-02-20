﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="SubscriptionDashboardBodyViewModel.cs" company="RHEA System S.A.">
//     Copyright (c) 2023 RHEA System S.A.
// 
//     Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Théate Antoine, Nabil Abbar
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

namespace COMETwebapp.ViewModels.Components.SubscriptionDashboard
{
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;

    using COMETwebapp.Extensions;
    using COMETwebapp.Services.SessionManagement;
    using COMETwebapp.ViewModels.Components.Shared;
    using COMETwebapp.ViewModels.Components.Shared.Selectors;

    using ReactiveUI;

    /// <summary>
    /// View Model that handle the logic for the Subscription Dashboard application
    /// </summary>
    public class SubscriptionDashboardBodyViewModel : SingleIterationApplicationBaseViewModel, ISubscriptionDashboardBodyViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionDashboardBodyViewModel" /> class.
        /// </summary>
        /// <param name="sessionService">The <see cref="ISessionService" /></param>
        /// <param name="subscribedTable">The <see cref="ISubscribedTableViewModel" /></param>
        public SubscriptionDashboardBodyViewModel(ISessionService sessionService, ISubscribedTableViewModel subscribedTable) : base(sessionService)
        {
            this.SubscribedTable = subscribedTable;

            this.Disposables.Add(this.WhenAnyValue(x => x.OptionSelector.SelectedOption,
                    x => x.ParameterTypeSelector.SelectedParameterType)
                .Subscribe(_ => this.UpdateTables()));
        }

        /// <summary>
        /// Gets the <see cref="ISubscribedTableViewModel" />
        /// </summary>
        public ISubscribedTableViewModel SubscribedTable { get; }

        /// <summary>
        /// Gets the <see cref="IDomainOfExpertiseSubscriptionTableViewModel" />
        /// </summary>
        public IDomainOfExpertiseSubscriptionTableViewModel DomainOfExpertiseSubscriptionTable { get; } = new DomainOfExpertiseSubscriptionTableViewModel();

        /// <summary>
        /// Gets the <see cref="IOptionSelectorViewModel" />
        /// </summary>
        public IOptionSelectorViewModel OptionSelector { get; } = new OptionSelectorViewModel();

        /// <summary>
        /// Gets the <see cref="IParameterTypeSelectorViewModel" />
        /// </summary>
        public IParameterTypeSelectorViewModel ParameterTypeSelector { get; } = new ParameterTypeSelectorViewModel();

        /// <summary>
        /// Update this view model properties when the <see cref="Iteration" /> has changed
        /// </summary>
        protected override void OnIterationChanged()
        {
            base.OnIterationChanged();

            this.OptionSelector.CurrentIteration = this.CurrentIteration;
            this.ParameterTypeSelector.CurrentIteration = this.CurrentIteration;

            var ownedSubscriptions = this.CurrentIteration?.QueryOwnedParameterSubscriptions(this.CurrentDomain).ToList()
                                     ?? new List<ParameterSubscription>();

            var availableOptions = this.CurrentIteration?.Option.ToList();

            var subscribedParameters = this.CurrentIteration?.QuerySubscribedParameterByOthers(this.CurrentDomain).ToList()
                                       ?? new List<ParameterOrOverrideBase>();

            var availableParameterTypes = ownedSubscriptions.Select(x => x.ParameterType).ToList();
            availableParameterTypes.AddRange(subscribedParameters.Select(x => x.ParameterType));
            this.ParameterTypeSelector.FilterAvailableParameterTypes(availableParameterTypes.Select(x => x.Iid).Distinct());

            this.SubscribedTable.UpdateProperties(ownedSubscriptions, availableOptions, this.CurrentIteration);
            this.DomainOfExpertiseSubscriptionTable.UpdateProperties(subscribedParameters);
        }

        /// <summary>
        /// Handles the refresh of the current <see cref="ISession" />
        /// </summary>
        protected override void OnSessionRefreshed()
        {
            this.OnIterationChanged();
            this.UpdateTables();
        }

        /// <summary>
        /// Handles the change of <see cref="DomainOfExpertise" />
        /// </summary>
        protected override void OnDomainChanged()
        {
            base.OnDomainChanged();
            this.OnIterationChanged();
        }

        /// <summary>
        /// Updates the <see cref="ISubscribedTableViewModel" /> and <see cref="IDomainOfExpertiseSubscriptionTableViewModel" />
        /// </summary>
        private void UpdateTables()
        {
            this.SubscribedTable.ApplyFilters(this.OptionSelector.SelectedOption, this.ParameterTypeSelector.SelectedParameterType);
            this.DomainOfExpertiseSubscriptionTable.ApplyFilters(this.OptionSelector.SelectedOption, this.ParameterTypeSelector.SelectedParameterType);
        }
    }
}
