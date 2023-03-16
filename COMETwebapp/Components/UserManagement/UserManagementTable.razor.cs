﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserManagementTable.razor.cs" company="RHEA System S.A.">
//    Copyright (c) 2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Nabil Abbar
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

namespace COMETwebapp.Components.UserManagement
{
    using CDP4Common.SiteDirectoryData;

    using COMETwebapp.ViewModels.Components.UserManagement;
    using COMETwebapp.ViewModels.Components.UserManagement.Rows;

    using DevExpress.Blazor;

    using DynamicData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    /// <summary>
    /// Support class for the <see cref="UserManagementTable" />
    /// </summary>
    public partial class UserManagementTable
    {
        /// <summary>
        /// The <see cref="IUserManagementTableViewModel" /> for this component
        /// </summary>
        [Inject]
        public IUserManagementTableViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the grid control that is being customized.
        /// </summary>
        private IGrid Grid { get; set; }

        /// <summary>
        /// Method invoked when the "Show/Hide Deprecated Items" checkbox is checked or unchecked.
        /// </summary>
        /// <param name="value">A <see cref="bool" /> that indicates whether deprecated items should be shown or hidden.</param>
        public void HideOrShowDeprecatedItems(bool value)
        {
            if (value)
            {
                this.Grid.FilterBy("IsDeprecated", GridFilterRowOperatorType.Equal, false);
            }
            else
            {
                this.Grid.ClearFilter();
            }
        }

        /// <summary>
        /// Method invoked when a custom summary calculation is required, allowing you to
        /// perform custom calculations based on the data displayed in the grid.
        /// </summary>
        /// <param name="e">A <see cref="GridCustomSummaryEventArgs" /></param>
        public static void CustomSummary(GridCustomSummaryEventArgs e)
        {
            switch (e.SummaryStage)
            {
                case GridCustomSummaryStage.Start:
                    e.TotalValue = 0;
                    break;
                case GridCustomSummaryStage.Calculate:
                    if (e.DataItem is PersonRowViewModel { IsActive: false })
                    {
                        e.TotalValue = (int)e.TotalValue + 1;
                    }

                    break;
                case GridCustomSummaryStage.Finalize:
                default:
                    break;
            }
        }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        protected override Task OnInitializedAsync()
        {
            this.ViewModel.OnInitialized();

            this.Disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsAllowedToWrite).Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            this.Disposables.Add(this.ViewModel.Rows.CountChanged.Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
            this.Disposables.Add(this.ViewModel.Rows.Connect().AutoRefresh().Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            return base.OnInitializedAsync();
        }

        /// <summary>
        /// Method invoked when creating a new person
        /// </summary>
        /// <param name="e">A <see cref="GridCustomizeEditModelEventArgs" /></param>
        private void CustomizeEditPerson(GridCustomizeEditModelEventArgs e)
        {
            var dataItem = (Person)e.DataItem;

            if (dataItem == null)
            {
                e.EditModel = new Person();
            }

            this.ViewModel.Person = new Person();
        }

        /// <summary>
        /// Method invoked when the summary text of a summary item is being displayed, allowing you to customize
        /// the text as needed. Override this method to modify the summary text based on specific conditions.
        /// </summary>
        /// <param name="e">A <see cref="GridCustomizeSummaryDisplayTextEventArgs" /></param>
        private static void CustomizeSummaryDisplayText(GridCustomizeSummaryDisplayTextEventArgs e)
        {
            if (e.Item.Name == "Inactive")
            {
                e.DisplayText = $"{e.Value} Inactive";
            }
        }

        /// <summary>
        /// Method invoked to highlight deprecated persons
        /// </summary>
        /// <param name="e">A <see cref="GridCustomizeElementEventArgs" /></param>
        private static void DisableDeprecatedPerson(GridCustomizeElementEventArgs e)
        {
            if (e.ElementType == GridElementType.DataRow && (bool)e.Grid.GetRowValue(e.VisibleIndex, "IsDeprecated"))
            {
                e.CssClass = "highlighted-item";
            }
        }
    }
}
