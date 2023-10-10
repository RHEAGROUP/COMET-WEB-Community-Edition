// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserManagementTableViewModel.cs" company="RHEA System S.A.">
//    Copyright (c) 2023 RHEA System S.A.
//
//    Authors: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Nabil Abbar
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

namespace COMETwebapp.ViewModels.Components.UserManagement
{
    using AntDesign;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.Events;
    using CDP4Dal.Permission;

    using COMET.Web.Common.Services.SessionManagement;
    using COMET.Web.Common.ViewModels.Components.Applications;

    using COMETwebapp.Services.ShowHideDeprecatedThingsService;
    using COMETwebapp.ViewModels.Components.UserManagement.Rows;

    using DevExpress.Blazor;

    using DynamicData;

    using ReactiveUI;

    /// <summary>
    /// View model used to manage <see cref="Person" />
    /// </summary>
    public class UserManagementTableViewModel : SingleIterationApplicationBaseViewModel, IUserManagementTableViewModel
    {
        /// <summary>
        /// Injected property to get access to <see cref="IPermissionService" />
        /// </summary>
        private readonly IPermissionService permissionService;

        /// <summary>
        /// Injected property to get access to <see cref="ISessionService" />
        /// </summary>
        private readonly ISessionService sessionService;

        /// <summary>
        /// Backing field for <see cref="IsOnDeprecationMode" />
        /// </summary>
        private bool isOnDeprecationMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagementTableViewModel" /> class.
        /// </summary>
        /// <param name="sessionService">The <see cref="ISessionService" /></param>
        /// <param name="showHideDeprecatedThingsService">The <see cref="IShowHideDeprecatedThingsService" /></param>
        public UserManagementTableViewModel(ISessionService sessionService, IShowHideDeprecatedThingsService showHideDeprecatedThingsService) : base(sessionService)
        {
            this.sessionService = sessionService;
            this.permissionService = sessionService.Session.PermissionService;
            this.ShowHideDeprecatedThingsService = showHideDeprecatedThingsService;

            this.InitializeSubscriptions(new List<Type> { typeof(Person) });
        }

        /// <summary>
        /// Injected property to get access to <see cref="IShowHideDeprecatedThingsService" />
        /// </summary>
        public IShowHideDeprecatedThingsService ShowHideDeprecatedThingsService { get; }

        /// <summary>
        /// The <see cref="Person" /> to create or edit
        /// </summary>
        public Person Person { get; set; } = new();

        /// <summary>
        /// A reactive collection of <see cref="PersonRowViewModel" />
        /// </summary>
        public SourceList<PersonRowViewModel> Rows { get; } = new();

        /// <summary>
        /// The <see cref="EmailAddress" /> to create
        /// </summary>
        public EmailAddress EmailAddress { get; set; } = new();

        /// <summary>
        /// The <see cref="TelephoneNumber" /> to create
        /// </summary>
        public TelephoneNumber TelephoneNumber { get; set; } = new();

        /// <summary>
        /// Gets or sets the data source for the grid control.
        /// </summary>
        public SourceList<Person> DataSource { get; } = new();

        /// <summary>
        /// Available <see cref="Organization" />s
        /// </summary>
        public IEnumerable<Organization> AvailableOrganizations { get; set; }

        /// <summary>
        /// Available <see cref="PersonRole" />s
        /// </summary>
        public IEnumerable<PersonRole> AvailablePersonRoles { get; set; }

        /// <summary>
        /// Available <see cref="DomainOfExpertise" />s
        /// </summary>
        public IEnumerable<DomainOfExpertise> AvailableDomains { get; set; }

        /// <summary>
        /// Available <see cref="VcardEmailAddressKind" />s
        /// </summary>
        public IEnumerable<VcardEmailAddressKind> EmailAddressKinds { get; set; } = Enum.GetValues(typeof(VcardEmailAddressKind)).Cast<VcardEmailAddressKind>();

        /// <summary>
        /// Indicates if the <see cref="EmailAddress" /> is the default email address
        /// </summary>
        public bool IsDefaultEmail { get; set; }

        /// <summary>
        /// Available <see cref="VcardTelephoneNumberKind" />s
        /// </summary>
        public IEnumerable<VcardTelephoneNumberKind> TelephoneNumberKinds { get; set; } = Enum.GetValues(typeof(VcardTelephoneNumberKind)).Cast<VcardTelephoneNumberKind>();

        /// <summary>
        /// Indicates if the <see cref="TelephoneNumber" /> is the default telephone number
        /// </summary>
        public bool IsDefaultTelephoneNumber { get; set; }

        /// <summary>
        /// Indicates if confirmation popup is visible
        /// </summary>
        public bool IsOnDeprecationMode
        {
            get => this.isOnDeprecationMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnDeprecationMode, value);
        }

        /// <summary>
        /// popum message dialog
        /// </summary>
        public string PopupDialog { get; set; }

        /// <summary>
        /// Method invoked when confirming the deprecation/un-deprecation of a <see cref="Person" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task OnConfirmButtonClick()
        {
            if (this.Person.IsDeprecated)
            {
                await this.UnDeprecatingPerson();
            }
            else
            {
                await this.DeprecatingPerson();
            }

            this.IsOnDeprecationMode = false;
        }

        /// <summary>
        /// Method invoked when canceling the deprecation/un-deprecation of a <see cref="Person" />
        /// </summary>
        public void OnCancelButtonClick()
        {
            this.IsOnDeprecationMode = false;
        }

        /// <summary>
        /// Tries to create a new <see cref="Person" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task AddingPerson()
        {
            var thingsToCreate = new List<Thing>();

            if (this.IsDefaultEmail)
            {
                this.Person.DefaultEmailAddress = this.EmailAddress;
            }

            if (this.IsDefaultTelephoneNumber)
            {
                this.Person.DefaultTelephoneNumber = this.TelephoneNumber;
            }

            this.Person.EmailAddress.Add(this.EmailAddress);
            this.Person.TelephoneNumber.Add(this.TelephoneNumber);
            thingsToCreate.Add(this.Person);
            await this.sessionService.CreateThings(this.sessionService.GetSiteDirectory(), thingsToCreate);
        }

        /// <summary>
        /// Action invoked when the deprecate or undeprecate button is clicked
        /// </summary>
        /// <param name="personRow">A <see cref="PersonRowViewModel" /> that represents the person to deprecate or undeprecate </param>
        public void OnDeprecateUnDeprecateButtonClick(PersonRowViewModel personRow)
        {
            this.Person = new Person();
            this.Person = personRow.Person;
            this.PopupDialog = this.Person.IsDeprecated ? "You are about to un-deprecate the user: " + personRow.PersonName : "You are about to deprecate the user: " + personRow.PersonName;
            this.IsOnDeprecationMode = true;
        }

        /// <summary>
        /// Tries to activate or disactivate a <see cref="Person" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ActivatingOrDisactivatingPerson(GridDataColumnCellDisplayTemplateContext context, bool value)
        {
            var personRow = (PersonRowViewModel)context.DataItem;
            var personToActivateOrDesactivate = new List<Person>();
            var personToUpdate = personRow.Person;
            var clonedPerson = personToUpdate.Clone(false);
            clonedPerson.IsActive = value;
            personToActivateOrDesactivate.Add(clonedPerson);
            await this.sessionService.UpdateThings(this.sessionService.GetSiteDirectory(), personToActivateOrDesactivate);
        }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        public void OnInitialized()
        {
            this.DataSource.AddRange(this.sessionService.Session.RetrieveSiteDirectory().Person);
            this.DataSource.Items.ForEach(x => this.Rows.Add(new PersonRowViewModel(x)));
            this.AvailableOrganizations = this.sessionService.Session.RetrieveSiteDirectory().Organization;
            this.AvailablePersonRoles = this.sessionService.Session.RetrieveSiteDirectory().PersonRole;
            this.AvailableDomains = this.sessionService.Session.RetrieveSiteDirectory().Domain;
            this.RefreshAccessRight();
        }

        /// <summary>
        /// Remove rows related to a <see cref="ElementBase" /> that has been deleted
        /// </summary>
        /// <param name="deletedThings">A collection of deleted <see cref="ElementBase" /></param>
        public void RemoveRows(IEnumerable<Person> deletedThings)
        {
            foreach (var person in deletedThings)
            {
                var row = this.Rows.Items.FirstOrDefault(x => x.Person.Iid == person.Iid);

                if (row != null)
                {
                    this.Rows.Remove(row);
                }
            }
        }

        /// <summary>
        /// Updates rows related to <see cref="ElementBase" /> that have been updated
        /// </summary>
        /// <param name="updatedThings">A collection of updated <see cref="ElementBase" /></param>
        public void UpdateRows(IEnumerable<Person> updatedThings)
        {
            foreach (var person in updatedThings)
            {
                var row = this.Rows.Items.FirstOrDefault(x => x.Person.Iid == person.Iid);

                if (row != null)
                {
                    row.UpdateProperties(new PersonRowViewModel(person));
                }
            }
        }

        /// <summary>
        /// Tries to undeprecate a <see cref="Person" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task UnDeprecatingPerson()
        {
            var personToUnDeprecate = new List<Person>();
            var clonedPerson = this.Person.Clone(false);
            clonedPerson.IsDeprecated = false;
            personToUnDeprecate.Add(clonedPerson);
            await this.sessionService.UpdateThings(this.sessionService.GetSiteDirectory(), personToUnDeprecate);
            this.IsOnDeprecationMode = false;
        }

        /// <summary>
        /// Tries to deprecate a <see cref="Person" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task DeprecatingPerson()
        {
            var personToDeprecate = new List<Person>();
            var clonedPerson = this.Person.Clone(false);
            clonedPerson.IsDeprecated = true;
            personToDeprecate.Add(clonedPerson);
            await this.sessionService.UpdateThings(this.sessionService.GetSiteDirectory(), personToDeprecate);
            this.IsOnDeprecationMode = false;
        }

        /// <summary>
        /// Handles the refresh of the current <see cref="ISession" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        protected override async Task OnSessionRefreshed()
        {
            if (!this.AddedThings.Any() && !this.DeletedThings.Any() && !this.UpdatedThings.Any())
            {
                return;
            }

            this.IsLoading = true;
            await Task.Delay(1);

            this.RemoveRows(this.DeletedThings.OfType<Person>());
            this.UpdateRows(this.UpdatedThings.OfType<Person>());
            this.Rows.AddRange(this.AddedThings.OfType<Person>().Select(x => new PersonRowViewModel(x)));

            this.ClearRecordedChanges();
            this.RefreshAccessRight();
            this.IsLoading = false;
        }

        /// <summary>
        /// The logic used to check if a change should be recorded an <see cref="ObjectChangedEvent" />
        /// </summary>
        /// <param name="objectChangedEvent">The <see cref="ObjectChangedEvent" /></param>
        /// <returns>true if the change should be recorded, false otherwise</returns>
        protected override bool ShouldRecordChange(ObjectChangedEvent objectChangedEvent)
        {
            return true;
        }

        /// <summary>
        /// Update this view model properties
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        protected override async Task OnThingChanged()
        {
            await base.OnThingChanged();
            this.IsLoading = false;
        }

        /// <summary>
        /// Updates the active user access rights
        /// </summary>
        private void RefreshAccessRight()
        {
            foreach (var row in this.Rows.Items)
            {
                row.IsAllowedToWrite = row.Person.Iid != this.sessionService.Session.ActivePerson.Iid
                                       && this.permissionService.CanWrite(ClassKind.Person, this.sessionService.GetSiteDirectory());
            }
        }
    }
}
