﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="EnumerationParameterTypeEditorTestFixture.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.Tests.Components.Shared.ParameterTypeEditors
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bunit;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using COMETwebapp.Components.Shared.ParameterTypeEditors;
    using COMETwebapp.Tests.Helpers;
    using COMETwebapp.ViewModels.Components.Shared.ParameterEditors;

    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;

    using Moq;

    using NUnit.Framework;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class EnumerationParameterTypeEditorTestFixture
    {
        private TestContext context;
        private IRenderedComponent<EnumerationParameterTypeEditor> renderedComponent;
        private EnumerationParameterTypeEditor editor;
        private bool eventCallbackCalled = false;
        private Mock<IParameterEditorBaseViewModel<EnumerationParameterType>> viewModelMock;
        private EventCallback<IValueSet> eventCallback;

        [SetUp]
        public void SetUp()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();

            var enumerationValues = new List<string> { "cube", "sphere", "cylinder" };

            var parameterValueSet = new ParameterValueSet
            {
                Iid = Guid.NewGuid(),
                ValueSwitch = ParameterSwitchKind.MANUAL,
                Manual = new ValueArray<string>(enumerationValues),
            };

            var enumerationData = new OrderedItemList<EnumerationValueDefinition>(null)
            {
                new EnumerationValueDefinition
                {
                    Iid = Guid.NewGuid(),
                    Name = enumerationValues[0]
                },
                new EnumerationValueDefinition
                {
                    Iid = Guid.NewGuid(),
                    Name = enumerationValues[1]
                },
                new EnumerationValueDefinition
                {
                    Iid = Guid.NewGuid(),
                    Name = enumerationValues[2]
                }
            };

            var parameterType = new EnumerationParameterType
            {
                Iid = Guid.NewGuid(),
            };
            
            parameterType.ValueDefinition.AddRange(enumerationData);

            this.viewModelMock = new Mock<IParameterEditorBaseViewModel<EnumerationParameterType>>();
            this.viewModelMock.Setup(x => x.ParameterType).Returns(parameterType);
            this.viewModelMock.Setup(x => x.ValueSet).Returns(parameterValueSet);
            
            this.eventCallback = new EventCallbackFactory().Create(this, (IValueSet _) =>
            {
                this.eventCallbackCalled = true;
            });

            this.renderedComponent = this.context.RenderComponent<EnumerationParameterTypeEditor>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModelMock.Object);
                parameters.Add(p => p.ParameterValueChanged, this.eventCallback);
            });
            
            this.editor = this.renderedComponent.Instance;
        }

        [TearDown]
        public void TearDown()
        {
            this.context.Dispose();
        }

        [Test]
        public void VerifyComponent()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.renderedComponent, Is.Not.Null);
                Assert.That(this.editor, Is.Not.Null);
                Assert.That(this.editor.ViewModel, Is.Not.Null);
                Assert.That(this.editor.ParameterValueChanged, Is.Not.Null);
            });
        }

        [Test]
        public async Task VerifyParameterValueChanged()
        {
            var textbox = this.renderedComponent.FindComponent<DxComboBox<string,string>>();
            Assert.That(textbox, Is.Not.Null);
            await this.editor.ParameterValueChanged.InvokeAsync();
            Assert.That(this.eventCallbackCalled, Is.True);
        }

        [Test]
        public void VerifyThatComponetCanBeReadOnly()
        {
            this.viewModelMock.Setup(x => x.IsReadOnly).Returns(true);

            this.renderedComponent.SetParametersAndRender(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModelMock.Object);
                parameters.Add(p => p.ParameterValueChanged, this.eventCallback);
            });

            var textbox = this.renderedComponent.FindComponent<DxComboBox<string, string>>();
            this.editor.ViewModel.IsReadOnly = true;
            Assert.That(textbox.Instance.ReadOnly, Is.True);
        }
    }
}
