﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ParameterTableViewModel.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.ViewModels.Components.ParameterEditor
{
    using CDP4Common.EngineeringModelData;

    using CDP4Dal;

    using COMETwebapp.Model;
    using COMETwebapp.Services.SessionManagement;

    using DynamicData;

    using Microsoft.AspNetCore.Components;
    
    using ReactiveUI;

    /// <summary>
    /// ViewModel for the <see cref="COMETwebapp.Components.ParameterEditor.ParameterTable"/>
    /// </summary>
    public class ParameterTableViewModel : ReactiveObject, IParameterTableViewModel
    {
        /// <summary>
        /// Gets or sets the <see cref="ISessionService"/>
        /// </summary>
        [Inject]
        public ISessionService SessionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ParameterBaseRowViewModel"/> for this <see cref="ParameterTableViewModel"/>
        /// </summary>
        public SourceList<ParameterBaseRowViewModel> Rows { get; set; } = new();

        /// <summary>
        /// Creates a new instance of <see cref="ParameterTableViewModel"/>
        /// </summary>
        /// <param name="sessionService">the <see cref="ISessionService"/></param>
        public ParameterTableViewModel(ISessionService sessionService)
        {
            this.SessionService = sessionService;
        }

        /// <summary>
        /// Initializes this <see cref="IParameterTableViewModel"/> with the <see cref="IEnumerable{T}"/> of <see cref="ElementBase"/>
        /// </summary>
        /// <param name="elements">the elements of the table</param>
        public void InitializeViewModel(IEnumerable<ElementBase> elements)
        {
            this.Rows.Clear();
            this.CreateParameterBaseRowViewModels(elements);
        }

        /// <summary>
        /// Creates the <see cref="ParameterBaseRowViewModel"/> for the <param name="elements"></param>
        /// </summary>
        /// <param name="elements">the elements used for creating each <see cref="ParameterBaseRowViewModel"/></param>
        /// <returns>an <see cref="IEnumerable{T}"/> of <see cref="ParameterBaseRowViewModel"/></returns>
        public void CreateParameterBaseRowViewModels(IEnumerable<ElementBase> elements)
        {
            foreach (var element in elements)
            {
                if (element is ElementDefinition elementDefinition)
                {
                    elementDefinition.Parameter.ForEach(parameter =>
                    {
                        parameter.ValueSet.ForEach(valueSet =>
                        {
                            this.Rows.Add(new ParameterBaseRowViewModel(this.SessionService,parameter, valueSet));
                        });
                    });
                }
                else if (element is ElementUsage elementUsage)
                {
                    if (elementUsage.ParameterOverride.Any())
                    {
                        elementUsage.ParameterOverride.ForEach(parameter =>
                        {
                            parameter.ValueSet.ForEach(valueSet =>
                            {
                                this.Rows.Add(new ParameterBaseRowViewModel(this.SessionService, parameter, valueSet));
                            });
                        });
                    }
                    else
                    {
                        elementUsage.ElementDefinition.Parameter.ForEach(parameter =>
                        {
                            parameter.ValueSet.ForEach(valueSet =>
                            {
                                this.Rows.Add(new ParameterBaseRowViewModel(this.SessionService, parameter, valueSet));
                            });
                        });
                    }
                }
            }
        }
    }
}