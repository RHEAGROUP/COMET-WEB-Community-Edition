﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="OpenModelViewModelTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2023 RHEA System S.A.
// 
//    Authors: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Théate Antoine, Nabil Abbar
// 
//    This file is part of COMET WEB Community Edition
//    The COMET WEB Community Edition is the RHEA Web Application implementation of ECSS-E-TM-10-25
//    Annex A and Annex C.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace COMET.Web.Common.Tests.ViewModels.Components
{
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using COMET.Web.Common.Model.Configuration;
    using COMET.Web.Common.Services.ConfigurationService;
    using COMET.Web.Common.Services.SessionManagement;
    using COMET.Web.Common.Services.VersionService;
    using COMET.Web.Common.ViewModels.Components;

    using DynamicData;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class OpenModelViewModelTestFixture
    {
        private OpenModelViewModel viewModel;
        private Mock<IConfigurationService> configurationService;
        private Mock<ISessionService> sessionService;
        private const string rdlShortName = "filterRdl";

        [SetUp]
        public void Setup()
        {
            this.configurationService = new Mock<IConfigurationService>();
            this.sessionService = new Mock<ISessionService>();
            var iterations = new SourceList<Iteration>();
            this.sessionService.Setup(x => x.OpenIterations).Returns(iterations);

            this.viewModel = new OpenModelViewModel(this.sessionService.Object, this.configurationService.Object);
            this.FillData();
        }

        [Test]
        public void VerifyInitializeViewModel()
        {
            //Initialize without server configuration
            this.viewModel.InitializesProperties();
            Assert.That(this.viewModel.AvailableEngineeringModelSetups.Count(), Is.EqualTo(5));
            
            //Initialize with server configuration without rdl filter
            var serverConfiguration = new ServerConfiguration();
            this.configurationService.Setup(x => x.ServerConfiguration).Returns(serverConfiguration);
            this.viewModel.InitializesProperties();
            Assert.That(this.viewModel.AvailableEngineeringModelSetups.Count(), Is.EqualTo(5));

            //Initialize with server configuration with rdl filter
            serverConfiguration.RdlFilter = new EngineeringModelRdlFilter()
            {
                Kinds = new[] { EngineeringModelKind.STUDY_MODEL },
                RdlShortNames = new[] { rdlShortName }
            };

            this.viewModel.InitializesProperties();
            Assert.That(this.viewModel.AvailableEngineeringModelSetups.Count(), Is.EqualTo(2));
        }

        private void FillData()
        {
            var rdl1 = new ModelReferenceDataLibrary(Guid.NewGuid(), null, null)
            {
                ShortName = "rdl1",
                RequiredRdl = new SiteReferenceDataLibrary()
                {
                    ShortName = rdlShortName
                }
            };

            var rdl2 = new ModelReferenceDataLibrary(Guid.NewGuid(), null, null)
            {
                ShortName = "rdl2",
                RequiredRdl = new SiteReferenceDataLibrary()
                {
                    ShortName = "another"
                }
            };

            var participantModels = new List<EngineeringModelSetup>()
            {
                new ()
                {
                    Name = "Model1",
                    RequiredRdl = { rdl1 },
                    IterationSetup =
                    {
                        new IterationSetup(Guid.NewGuid(), null, null)
                    }
                },
                new ()
                {
                    Name = "Model2",
                    RequiredRdl = { rdl1 },
                    IterationSetup =
                    {
                        new IterationSetup(Guid.NewGuid(), null, null)
                    }
                },
                new ()
                {
                    Name = "Model3",
                    RequiredRdl = { rdl2 },
                    IterationSetup =
                    {
                        new IterationSetup(Guid.NewGuid(), null, null)
                    }
                },
                new ()
                {
                    Name = "Model4",
                    RequiredRdl = { rdl2 },
                    IterationSetup =
                    {
                        new IterationSetup(Guid.NewGuid(), null, null)
                    }
                },                
                new ()
                {
                    Name = "Model5",
                    RequiredRdl = { rdl2 },
                    IterationSetup =
                    {
                        new IterationSetup(Guid.NewGuid(), null, null)
                    }
                }
            };
            
            this.sessionService.Setup(x => x.GetParticipantModels()).Returns(participantModels);
        }
    }
}