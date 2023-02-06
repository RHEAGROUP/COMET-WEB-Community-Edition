﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ParameterValueSetRow.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.Components.ParameterEditor
{
    using System.Reactive.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using CDP4Dal;

    using COMETwebapp.Model;
    using COMETwebapp.Services.IterationServices;

    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// Class for the component <see cref="ParameterValueSetRow"/>
    /// </summary>
    public partial class ParameterValueSetRow
    {
        /// <summary>
        /// The ParameterValueSet to show in the table
        /// </summary>
        [Parameter]
        public ParameterValueSetBase ParameterValueSet { get; set; }

        /// <summary>
        /// The associated Parameter to show
        /// </summary>
        [Parameter]
        public ParameterOrOverrideBase Parameter { get; set; }

        /// <summary>
        /// Sets if ParameterValueSet was edited
        /// </summary>
        private bool IsParameterValueSetEdited { get; set; }

        /// <summary>
        /// ParameterSwitchKind to show
        /// </summary>
        private string SelectedSwitchKind { get; set; }

        /// <summary>
        /// Listeners for the components to update it with edit changes
        /// </summary>
        private Dictionary<string, IDisposable> Listeners = new ();

        /// <summary>
        /// Initialize ClonedParameterValueSet 
        /// </summary>
        protected override void OnInitialized()
        {
            if (this.ParameterValueSet != null && this.IsEditable())
            {
                this.IsParameterValueSetEdited = this.IterationService.NewUpdates.Contains(this.ParameterValueSet.Iid);
            }

            if (!this.Listeners.TryGetValue("NewUpdate", out var listener))
            {
                this.Listeners.Add("NewUpdate", CDPMessageBus.Current.Listen<NewUpdateEvent>().Where(x => x.UpdatedThingIid == this.ParameterValueSet?.Iid).Subscribe(x =>
                {
                    this.IsParameterValueSetEdited = true;
                    this.StateHasChanged();
                }));
            }

            if (!this.Listeners.TryGetValue("SwitchMode", out listener))
            {
                this.Listeners.Add("SwitchMode", CDPMessageBus.Current.Listen<SwitchEvent>().Where(x => x.ParameterValuSetIid == this.ParameterValueSet?.Iid).Subscribe(x =>
                {
                    if (x.SubmitChange == null)
                    {
                        this.SelectedSwitchKind = x.SelectedSwitch.ToString();
                        this.StateHasChanged();
                    }
                    else if (x.SubmitChange == true)
                    {
                        this.UpdateChange(x.SelectedSwitch);
                    }
                }));
            }
        }

        /// <summary>
        /// Tells if ParameterValueSet is editable
        /// A <see cref="ParameterValueSetBase"/> is editable if it is owned by the active <see cref="DomainOfExpertise"/>
        /// </summary>
        private bool IsEditable()
        {
            return this.ParameterValueSet?.Owner == this.SessionService.GetDomainOfExpertise(this.SessionService.DefaultIteration);
        }

        /// <summary>
        /// Update value of <see cref="ParameterValueSet"/> when a change appears 
        /// </summary>
        private void UpdateChange(ParameterSwitchKind newValue)
        {
            if (this.ParameterValueSet != null)
            {
                if (this.IsEditable() && !this.IterationService.NewUpdates.Contains(this.ParameterValueSet.Iid))
                {
                    this.IterationService.NewUpdates.Add(this.ParameterValueSet.Iid);
                }
                
                var clonedParameterValueSet = this.ParameterValueSet.Clone(false);
                clonedParameterValueSet.ValueSwitch = newValue;
                
                this.SessionService.UpdateThings(this.SessionService.DefaultIteration, new List<Thing>()
                {
                    clonedParameterValueSet
                });
                
                this.ParameterValueSet.ValueSwitch = newValue;
                CDPMessageBus.Current.SendMessage<NewUpdateEvent>(new NewUpdateEvent(this.ParameterValueSet.Iid));
            }
        }

        /// <summary>
        /// Stop and clear Listeners of the component
        /// </summary>
        public void Dispose()
        {
            this.Listeners.Values.ToList().ForEach(l => l.Dispose());
            this.Listeners.Clear();
        }
    }
}
