﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="MeasurementUnitsTableViewModelTestFixture.cs" company="RHEA System S.A.">
//     Copyright (c) 2023-2024 RHEA System S.A.
// 
//     Authors: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Antoine Théate, João Rua
// 
//     This file is part of CDP4-COMET WEB Community Edition
//     The CDP4-COMET WEB Community Edition is the RHEA Web Application implementation of ECSS-E-TM-10-25 Annex A and Annex C.
// 
//     The CDP4-COMET WEB Community Edition is free software; you can redistribute it and/or
//     modify it under the terms of the GNU Affero General Public
//     License as published by the Free Software Foundation; either
//     version 3 of the License, or (at your option) any later version.
// 
//     The CDP4-COMET WEB Community Edition is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace COMETwebapp.Tests.ViewModels.Components.ReferenceData
{
    using CDP4Common.CommonData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;
    using CDP4Dal.Events;
    using CDP4Dal.Permission;

    using COMET.Web.Common.Enumerations;
    using COMET.Web.Common.Services.SessionManagement;

    using COMETwebapp.Services.ShowHideDeprecatedThingsService;
    using COMETwebapp.ViewModels.Components.ReferenceData.MeasurementUnits;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class MeasurementUnitsTableViewModelTestFixture
    {
        private MeasurementUnitsTableViewModel viewModel;
        private Mock<ISessionService> sessionService;
        private Mock<IPermissionService> permissionService;
        private Mock<ILogger<MeasurementUnitsTableViewModel>> loggerMock;
        private Assembler assembler;
        private CDPMessageBus messageBus;
        private Mock<IShowHideDeprecatedThingsService> showHideService;
        private MeasurementUnit measurementUnit;

        [SetUp]
        public void Setup()
        {
            this.sessionService = new Mock<ISessionService>();
            this.permissionService = new Mock<IPermissionService>();
            this.showHideService = new Mock<IShowHideDeprecatedThingsService>();
            this.messageBus = new CDPMessageBus();
            this.loggerMock = new Mock<ILogger<MeasurementUnitsTableViewModel>>();

            this.measurementUnit = new SimpleUnit()
            {
                ShortName = "unit",
                Name = "unit"
            };

            var siteReferenceDataLibrary = new SiteReferenceDataLibrary()
            {
                ShortName = "rdl",
            };

            var siteDirectory = new SiteDirectory()
            {
                ShortName = "siteDirectory"
            };

            siteReferenceDataLibrary.Unit.Add(this.measurementUnit);
            siteDirectory.SiteReferenceDataLibrary.Add(siteReferenceDataLibrary);

            this.assembler = new Assembler(new Uri("http://localhost:5000/"), this.messageBus);
            var lazyMeasurementUnit = new Lazy<Thing>(this.measurementUnit);
            this.assembler.Cache.TryAdd(new CacheKey(), lazyMeasurementUnit);

            this.permissionService.Setup(x => x.CanWrite(this.measurementUnit.ClassKind, this.measurementUnit.Container)).Returns(true);
            var session = new Mock<ISession>();
            session.Setup(x => x.PermissionService).Returns(this.permissionService.Object);
            session.Setup(x => x.Assembler).Returns(this.assembler);
            session.Setup(x => x.RetrieveSiteDirectory()).Returns(siteDirectory);
            this.sessionService.Setup(x => x.Session).Returns(session.Object);
            this.sessionService.Setup(x => x.GetSiteDirectory()).Returns(siteDirectory);

            this.viewModel = new MeasurementUnitsTableViewModel(this.sessionService.Object, this.showHideService.Object, this.messageBus, this.loggerMock.Object);
        }

        [TearDown]
        public void Teardown()
        {
            this.messageBus.ClearSubscriptions();
            this.viewModel.Dispose();
        }

        [Test]
        public void VerifyInitializeViewModel()
        {
            this.viewModel.InitializeViewModel();

            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.Rows.Count, Is.EqualTo(1));
                Assert.That(this.viewModel.Rows.Items.First().Thing, Is.EqualTo(this.measurementUnit));
            });
        }

        [Test]
        public void VerifyMeasurementUnitRowProperties()
        {
            this.viewModel.InitializeViewModel();
            var measurementUnitRow = this.viewModel.Rows.Items.First();

            Assert.Multiple(() =>
            {
                Assert.That(measurementUnitRow.ContainerName, Is.EqualTo("rdl"));
                Assert.That(measurementUnitRow.Name, Is.EqualTo(this.measurementUnit.Name));
                Assert.That(measurementUnitRow.ShortName, Is.EqualTo(this.measurementUnit.ShortName));
                Assert.That(measurementUnitRow.Thing, Is.EqualTo(this.measurementUnit));
                Assert.That(measurementUnitRow.IsAllowedToWrite, Is.EqualTo(true));
                Assert.That(measurementUnitRow.Type, Is.EqualTo(nameof(SimpleUnit)));
            });
        }

        [Test]
        public void VerifySessionRefresh()
        {
            this.viewModel.InitializeViewModel();

            this.messageBus.SendMessage(SessionStateKind.RefreshEnded);
            Assert.That(this.viewModel.Rows, Has.Count.EqualTo(1));

            var siteReferenceDataLibrary = new SiteReferenceDataLibrary()
            {
                ShortName = "newShortname"
            };

            var measurementUnitTest = new SimpleUnit()
            {
                Iid = Guid.NewGuid(),
                Container = siteReferenceDataLibrary
            };

            this.messageBus.SendObjectChangeEvent(measurementUnitTest, EventKind.Added);
            this.messageBus.SendMessage(SessionStateKind.RefreshEnded);

            this.messageBus.SendObjectChangeEvent(this.viewModel.Rows.Items.First().Thing, EventKind.Removed);
            this.messageBus.SendMessage(SessionStateKind.RefreshEnded);

            this.messageBus.SendObjectChangeEvent(this.viewModel.Rows.Items.First().Thing, EventKind.Updated);
            this.messageBus.SendMessage(SessionStateKind.RefreshEnded);

            Assert.That(this.viewModel.Rows, Has.Count.EqualTo(1));

            this.messageBus.SendObjectChangeEvent(siteReferenceDataLibrary, EventKind.Updated);
            this.messageBus.SendObjectChangeEvent(new PersonRole(), EventKind.Updated);
            this.messageBus.SendMessage(SessionStateKind.RefreshEnded);

            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.Rows.Items.First().ContainerName, Is.EqualTo(siteReferenceDataLibrary.ShortName));
                this.permissionService.Verify(x => x.CanWrite(measurementUnitTest.ClassKind, It.IsAny<Thing>()), Times.AtLeast(this.viewModel.Rows.Count));
            });
        }
        
        [Test]
         public async Task VerifyRowOperations()
         {
             this.viewModel.InitializeViewModel();
             var measurementUnitRow = this.viewModel.Rows.Items.First();
             measurementUnitRow.IsDeprecated = false;

             Assert.Multiple(() =>
             {
                 Assert.That(measurementUnitRow, Is.Not.Null);
                 Assert.That(this.viewModel.IsOnDeprecationMode, Is.EqualTo(false));
             });

             this.viewModel.OnDeprecateUnDeprecateButtonClick(measurementUnitRow);

             Assert.Multiple(() =>
             {
                 Assert.That(this.viewModel.IsOnDeprecationMode, Is.EqualTo(true));
                 Assert.That(this.viewModel.Thing, Is.EqualTo(measurementUnitRow.Thing));
             });
             
             this.viewModel.OnCancelPopupButtonClick();
             Assert.That(this.viewModel.IsOnDeprecationMode, Is.EqualTo(false));

             await this.viewModel.OnConfirmPopupButtonClick();
             this.sessionService.Verify(x => x.UpdateThings(It.IsAny<SiteDirectory>(), It.Is<MeasurementUnit>(c => c.IsDeprecated == true)));

             this.viewModel.Thing.IsDeprecated = true;
             await this.viewModel.OnConfirmPopupButtonClick();
             this.sessionService.Verify(x => x.UpdateThings(It.IsAny<SiteDirectory>(), It.Is<MeasurementUnit>(c => c.IsDeprecated == false)));
        }
    }
}
