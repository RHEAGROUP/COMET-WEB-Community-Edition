﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BookEditorBodyViewModel.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.ViewModels.Components.BookEditor
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.ReportingData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;

    using COMET.Web.Common.Services.SessionManagement;
    using COMET.Web.Common.ViewModels.Components;

    using DynamicData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    /// <summary>
    /// ViewModel for the BookEditorBody component
    /// </summary>
    public class BookEditorBodyViewModel : SingleIterationApplicationBaseViewModel, IBookEditorBodyViewModel
    {
        /// <summary>
        /// Backing field for the <see cref="IsOnBookCreation"/> property
        /// </summary>
        private bool isOnBookCreation;

        /// <summary>
        /// Backing field for the <see cref="IsOnSectionCreation"/> property
        /// </summary>
        private bool isOnSectionCreation;

        /// <summary>
        /// Backing field for the <see cref="IsOnPageCreation"/> property
        /// </summary>
        private bool isOnPageCreation;

        /// <summary>
        /// Backing field for the <see cref="IsOnNoteCreation"/> property
        /// </summary>
        private bool isOnNodeCreation;

        /// <summary>
        /// Backing field for the <see cref="IsOnEditMode"/> property
        /// </summary>
        private bool isOnEditMode;

        /// <summary>
        /// Backing field for the <see cref="IsOnCreateMode"/> property
        /// </summary>
        private bool isOnCreateMode;

        /// <summary>
        /// Backing field for the <see cref="SelectedBook"/> property
        /// </summary>
        private Book selectedBook;

        /// <summary>
        /// Backing field for the <see cref="SelectedSection"/> property
        /// </summary>
        private Section selectedSection;

        /// <summary>
        /// Backing field for the <see cref="SelectedPage"/> property
        /// </summary>
        private Page selectedPage;

        /// <summary>
        /// Gets or sets the current selected <see cref="Book"/>
        /// </summary>
        public Book SelectedBook
        {
            get => this.selectedBook;
            set => this.RaiseAndSetIfChanged(ref this.selectedBook, value);
        }

        /// <summary>
        /// Gets or sets the current selected <see cref="Section"/>
        /// </summary>
        public Section SelectedSection
        {
            get => this.selectedSection;
            set => this.RaiseAndSetIfChanged(ref this.selectedSection, value);
        }

        /// <summary>
        /// Gets or sets the current selected <see cref="Page"/>
        /// </summary>
        public Page SelectedPage
        {
            get => this.selectedPage;
            set => this.RaiseAndSetIfChanged(ref this.selectedPage, value);
        }

        /// <summary>
        /// Gets or sets the collection of available <see cref="Book"/> for this <see cref="EngineeringModel"/>
        /// </summary>
        public SourceList<Book> AvailableBooks { get; set; } = new();

        /// <summary>
        /// Gets or sets the available categories
        /// </summary>
        public List<Category> AvailableCategories { get; set; } = new();

        /// <summary>
        /// Gets or sets the active <see cref="DomainOfExpertise"/>
        /// </summary>
        public List<DomainOfExpertise> ActiveDomains { get; set; } = new();

        /// <summary>
        /// Gets or sets if the ViewModel is on book creation state
        /// </summary>
        public bool IsOnBookCreation
        {
            get => this.isOnBookCreation;
            set => this.RaiseAndSetIfChanged(ref this.isOnBookCreation, value);
        }

        /// <summary>
        /// Gets or sets if the ViewModel is on section creation state
        /// </summary>
        public bool IsOnSectionCreation
        {
            get => this.isOnSectionCreation;
            set => this.RaiseAndSetIfChanged(ref this.isOnSectionCreation, value);
        }

        /// <summary>
        /// Gets or sets if the ViewModel is on page creation state
        /// </summary>
        public bool IsOnPageCreation
        {
            get => this.isOnPageCreation;
            set => this.RaiseAndSetIfChanged(ref this.isOnPageCreation, value);
        }

        /// <summary>
        /// Gets or sets if the ViewModel is on node creation state
        /// </summary>
        public bool IsOnNoteCreation
        {
            get => this.isOnNodeCreation;
            set => this.RaiseAndSetIfChanged(ref this.isOnNodeCreation, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="Book"/> that's about to be created
        /// </summary>
        public Book BookToCreate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Section"/> that's about to be created
        /// </summary>
        public Section SectionToCreate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Page"/> that's about to be created
        /// </summary>
        public Page PageToCreate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Note"/> that's about to be created
        /// </summary>
        public Note NoteToCreate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventCallback"/> for when an item is created
        /// </summary>
        public EventCallback OnCreateItem { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventCallback"/> for when an item has canceled it's creation
        /// </summary>
        public EventCallback OnCancelCreateItem { get; set; }

        /// <summary>
        /// Gets or sets if the ViewModel is on edit mode
        /// </summary>
        public bool IsOnEditMode
        {
            get => this.isOnEditMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnEditMode, value);
        }

        /// <summary>
        /// Gets or sets if the ViewModel is on create mode
        /// </summary>
        public bool IsOnCreateMode
        {
            get => this.isOnCreateMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnCreateMode, value);
        }

        /// <summary>
        /// Gets or sets the thing to be edited
        /// </summary>
        public Thing ThingToEdit { get; set; }

        /// <summary>
        /// Gets or sets the thing to be created
        /// </summary>
        public Thing ThingToCreate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BookEditorBodyViewModel" /> class.
        /// </summary>
        /// <param name="sessionService">The <see cref="ISessionService" /></param>
        public BookEditorBodyViewModel(ISessionService sessionService) : base(sessionService)
        {
            this.Disposables.Add(this.WhenAnyValue(x => x.SelectedBook).Subscribe(_ => this.OnSelectedBookChanged()));
            this.Disposables.Add(this.WhenAnyValue(x => x.SelectedSection).Subscribe(_ => this.OnSelectedSectionChanged()));

            this.Disposables.Add(this.WhenAnyValue(x => x.IsOnBookCreation, x=> x.IsOnSectionCreation, x=>x.IsOnPageCreation, x=>x.IsOnNoteCreation).Subscribe(_=>this.ResetDataToCreate()));
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
        /// Update this view model properties when the <see cref="Iteration" /> has changed
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        protected override async Task OnIterationChanged()
        {
            this.IsLoading = true;
            await base.OnIterationChanged();

            if (this.CurrentIteration.Container is EngineeringModel engineeringModel)
            {
                this.AvailableBooks.Edit(inner =>
                {
                    inner.Clear();
                    inner.AddRange(engineeringModel.Book);
                });

                this.ActiveDomains.Clear();
                this.ActiveDomains.AddRange(engineeringModel.EngineeringModelSetup.ActiveDomain);

                this.AvailableCategories.Clear();
                var categories = engineeringModel.RequiredRdls.SelectMany(x => x.DefinedCategory);
                this.AvailableCategories.AddRange(categories);
            }

            this.ResetDataToCreate();
            this.IsLoading = false;
        }

        /// <summary>
        /// Resets the states for creation modes
        /// </summary>
        public void ResetCreationStates()
        {
            this.IsOnBookCreation = this.IsOnSectionCreation = this.IsOnPageCreation = this.IsOnNoteCreation = false;
        }

        /// <summary>
        /// Resets the data that is going to be used for creating a book, section or page
        /// </summary>
        private void ResetDataToCreate()
        {
            this.BookToCreate = new(Guid.NewGuid(), null, null);
            this.SectionToCreate = new(Guid.NewGuid(), null, null);
            this.PageToCreate = new(Guid.NewGuid(), null, null);
        }

        /// <summary>
        /// Handler for when the selected book changed
        /// </summary>
        private void OnSelectedBookChanged()
        {
            this.SelectedSection = null;
            this.SelectedPage = null;
        }

        /// <summary>
        /// Handler for when the selected section changed
        /// </summary>
        private void OnSelectedSectionChanged()
        {
            this.SelectedPage = null;
        }

        /// <summary>
        /// Validates that the thing is a valid thing for the operations in this ViewModel
        /// </summary>
        /// <param name="thing">the thing to validate</param>
        private void ValidateThing(Thing thing)
        {
            if (thing is not Book && thing is not Section && thing is not Page && thing is not Note)
            {
                throw new ArgumentException("The thing to should be a (Book, Section, Page or Note)", nameof(thing));
            }
        }

        /// <summary>
        /// Sets the thing to be created
        /// </summary>
        /// <param name="thing">the thing</param>
        public void SetThingToCreate(Thing thing)
        {
            this.ValidateThing(thing);

            this.ThingToCreate = thing;
            this.IsOnCreateMode = true;
        }

        /// <summary>
        /// Hanlder for when the user request to create a new thing (Book,Section,Page or Note)
        /// </summary>
        /// <returns></returns>
        public async Task OnCreateThing()
        {
            if (this.ThingToCreate == null)
            {
                throw new InvalidOperationException("The thing to create can't be null");
            }

            this.ValidateThing(this.ThingToCreate);

            Thing thingContainer;

            switch (this.ThingToCreate)
            {
                case Book: 
                    thingContainer = this.CurrentIteration.Container;
                    break;
                case Section: 
                    thingContainer = this.SelectedBook;
                    break;
                case Page: 
                    thingContainer = this.SelectedSection;
                    break;
                case Note: 
                    thingContainer = this.SelectedPage;
                    break;

                default:
                    this.ThingToCreate = null;
                    this.IsOnCreateMode = false;
                    return;
            }

            await this.SessionService.CreateThing(thingContainer.Clone(false), this.ThingToCreate);

            this.ThingToCreate = null;
            this.IsOnCreateMode = false;
        }

        /// <summary>
        /// Sets the thing to be edited
        /// </summary>
        /// <param name="thing">the thing</param>
        public void SetThingToEdit(Thing thing)
        {
            this.ValidateThing(thing);

            this.ThingToEdit = thing;
            this.IsOnEditMode = true;
        }

        /// <summary>
        /// Handler for when the user request to edit a thing (Book,Section,Page or Note)
        /// </summary>
        /// <returns>an asynchronous operation</returns>
        public async Task OnEditThing()
        {
            if (this.ThingToEdit == null)
            {
                throw new InvalidOperationException("The thing to edit can't be null");
            }

            this.ValidateThing(this.ThingToEdit);

            var thingContainer = this.ThingToEdit.Container;
            var thingContainerClone = thingContainer.Clone(false);

            await this.SessionService.UpdateThing(thingContainerClone, this.ThingToEdit.Clone(false));

            this.ThingToEdit = null;
            this.IsOnEditMode = false;
        }

        /// <summary>
        /// Hanlder for when the user request to delete a thing (Book,Section,Page or Note)
        /// </summary>
        /// <param name="thing">the thing to delete</param>
        /// <returns>an asynchronous operation</returns>
        public async Task OnDeleteThing(Thing thing)
        {
            if (thing is not Book && thing is not Section && thing is not Page && thing is not Note)
            {
                throw new ArgumentException("The thing to delete should be a (Book, Section, Page or Note)", nameof(thing));
            }

            var thingContainer = thing.Container;
            var thingContainerClone = thingContainer.Clone(false);

            await this.SessionService.DeleteThing(thingContainerClone, thing.Clone(false));
            await this.OnIterationChanged();
        }
    }
}

