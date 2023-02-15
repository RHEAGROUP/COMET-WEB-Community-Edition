﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ISubscriptionService.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.Services.SubscriptionService
{
    using CDP4Common.EngineeringModelData;

    using COMETwebapp.Model;
    using COMETwebapp.Utilities.DisposableObject;

    /// <summary>
    /// This service tracks change one <see cref="ISubscriptionService" /> for all opened <see cref="Iteration" />
    /// </summary>
    public interface ISubscriptionService: IDisposableObject
    {
        /// <summary>
        /// The current number of new <see cref="ParameterSubscription" /> updates
        /// </summary>
        int SubscriptionUpdateCount { get; set; }

        /// <summary>
        /// A <see cref="IReadOnlyDictionary{TKey,TValue}" /> to provide access to the tracked subscriptions
        /// </summary>
        IReadOnlyDictionary<Guid, List<TrackedParameterSubscription>> TrackedSubscriptions { get; }

        /// <summary>
        /// Updates the tracked subscriptions for all open <see cref="Iteration" />
        /// </summary>
        void UpdateTrackedSubscriptions();

        /// <summary>
        /// Updates the tracked subscriptions for an <see cref="Iteration" />
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        void UpdateTrackedSubscriptions(Iteration iteration);
    }
}
