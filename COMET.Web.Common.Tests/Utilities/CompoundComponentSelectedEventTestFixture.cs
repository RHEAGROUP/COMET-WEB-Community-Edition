﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="CompoundComponentSelectedEventTestFixture.cs" company="RHEA System S.A.">
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

namespace COMET.Web.Common.Tests.Utilities
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using COMET.Web.Common.Utilities;
    using COMET.Web.Common.ViewModels.Components.ParameterEditors;

    using NUnit.Framework;

    [TestFixture]
    public class CompoundComponentSelectedEventTestFixture
    {
        private ICompoundParameterTypeEditorViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            var parameterType = new CompoundParameterType()
            {
                Iid = Guid.NewGuid(),
            };
            
            var compoundValues = new List<string> { "1", "0", "3" };

            var parameterValueSet = new ParameterValueSet()
            {
                Iid = Guid.NewGuid(),
                ValueSwitch = ParameterSwitchKind.MANUAL,
                Manual = new ValueArray<string>(compoundValues),
            };

            this.viewModel = new CompoundParameterTypeEditorViewModel(parameterType, parameterValueSet, false);
        }

        [Test]
        public void VerifyCompoundComponentSelectedEventCreation()
        {
            var compoundComponentSelectedEvent = new CompoundComponentSelectedEvent((CompoundParameterTypeEditorViewModel)this.viewModel);

            Assert.Multiple(() =>
            {
                Assert.That(compoundComponentSelectedEvent.CompoundParameterTypeEditorViewModel, Is.Not.Null);
                Assert.That(compoundComponentSelectedEvent.CompoundParameterTypeEditorViewModel, Is.EqualTo(this.viewModel));
                Assert.That(compoundComponentSelectedEvent.CompoundParameterTypeEditorViewModel.ParameterType, Is.Not.Null);
            });
        }
    }
}
