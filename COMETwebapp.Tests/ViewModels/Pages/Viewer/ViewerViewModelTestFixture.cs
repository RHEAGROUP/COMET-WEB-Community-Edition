﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ViewerViewModelTestFixture.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.Tests.ViewModels.Pages.Viewer
{
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.EngineeringModelData;
    
    using COMETwebapp.Services.SessionManagement;
    using COMETwebapp.Utilities;
    using COMETwebapp.ViewModels.Pages.Viewer;
    
    using Moq;
    
    using NUnit.Framework;
    
    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ViewerViewModelTestFixture
    {
        private TestContext context;
        private IViewerViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            this.context = new TestContext();

            var sessionServiceMock = new Mock<ISessionService>();

            var elementUsage1 = new ElementUsage() { Name = "element1" };
            var elementUsage2 = new ElementUsage() { Name = "element2" };
            var elementUsage3 = new ElementUsage() { Name = "element3" };
            var elementUsage4 = new ElementUsage() { Name = "element4" };
            
            var elementDefinition1 = new ElementDefinition()
            {
                Name = "element1",
                ContainedElement = { elementUsage1 }
            };
            
            var elementDefinition2 = new ElementDefinition()
            {
                Name = "element2",
                ContainedElement = { elementUsage2 }
            };
            
            var elementDefinition3 = new ElementDefinition()
            {
                Name = "element3",
                ContainedElement = { elementUsage3 }
            };
            
            var elementDefinition4 = new ElementDefinition()
            {
                Name = "element4",
                ContainedElement = { elementUsage4 }
            };

            elementUsage1.ElementDefinition = elementDefinition1;
            elementUsage2.ElementDefinition = elementDefinition2;
            elementUsage3.ElementDefinition = elementDefinition3;
            elementUsage4.ElementDefinition = elementDefinition4;

            var topElement = new ElementDefinition()
            {
                Name = "topElement",
                ContainedElement =
                {
                    elementDefinition1.ContainedElement[0], 
                    elementDefinition2.ContainedElement[0], 
                    elementDefinition3.ContainedElement[0], 
                    elementDefinition4.ContainedElement[0],
                }
            };

            var iteration = new Iteration();
            iteration.TopElement = topElement;
            iteration.Element.AddRange(new List<ElementDefinition>(){elementDefinition1,elementDefinition2,elementDefinition3,elementDefinition4});
            iteration.Option.Add(new Option());
            iteration.Option.Add(new Option());
            iteration.DefaultOption = iteration.Option.First();

            var possibleState1 = new PossibleFiniteState();
            var possibleState2 = new PossibleFiniteState();

            var actualState1 = new ActualFiniteState()
            {
                PossibleState = { possibleState1 }
            };

            var actualState2 = new ActualFiniteState()
            {
                PossibleState = { possibleState2 }
            };

            var possibleFiniteStateList = new PossibleFiniteStateList();
            possibleFiniteStateList.DefaultState = possibleState1;
            possibleFiniteStateList.PossibleState.Add(possibleState1);
            possibleFiniteStateList.PossibleState.Add(possibleState2);

            var actualFiniteStateList = new ActualFiniteStateList();
            actualFiniteStateList.ActualState.Add(actualState1);
            actualFiniteStateList.ActualState.Add(actualState2);

            iteration.ActualFiniteStateList.Add(actualFiniteStateList);
            iteration.ActualFiniteStateList.Add(new ActualFiniteStateList());

            sessionServiceMock.Setup(x => x.DefaultIteration).Returns(iteration);

            var selectionMediatorMock = new Mock<ISelectionMediator>();

            this.viewModel = new ViewerViewModel(sessionServiceMock.Object, selectionMediatorMock.Object);
        }

        [Test]
        public void VerifyViewModel()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.SessionService, Is.Not.Null);
                Assert.That(this.viewModel.SelectionMediator, Is.Not.Null);
                Assert.That(this.viewModel.Elements, Is.Not.Null);
                Assert.That(this.viewModel.Elements, Has.Count.EqualTo(5));
                Assert.That(this.viewModel.TotalOptions, Is.Not.Null);
                Assert.That(this.viewModel.TotalOptions, Has.Count.EqualTo(2));
                Assert.That(this.viewModel.SelectedOption, Is.Not.Null);
                Assert.That(this.viewModel.ListActualFiniteStateLists, Is.Not.Null);
                Assert.That(this.viewModel.ListActualFiniteStateLists, Has.Count.EqualTo(2));
                Assert.That(this.viewModel.SelectedActualFiniteStates, Is.Not.Null);
                Assert.That(this.viewModel.SelectedActualFiniteStates, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void VerifyInitializeElements()
        {
            var elements = this.viewModel.InitializeElements();

            Assert.Multiple(() =>
            {
                Assert.That(elements, Is.Not.Null);
                Assert.That(elements, Has.Count.EqualTo(5));
                Assert.That(elements.Any(x => x.Name == "topElement"), Is.True);
                Assert.That(elements.Any(x => x.Name == "element1"), Is.True);
                Assert.That(elements.Any(x => x.Name == "element2"), Is.True);
                Assert.That(elements.Any(x => x.Name == "element3"), Is.True);
                Assert.That(elements.Any(x => x.Name == "element4"), Is.True);
            });
        }

        [Test]
        public void VerifyCreateTree()
        {
            var rootNode = this.viewModel.CreateTree(this.viewModel.Elements);
            
            Assert.Multiple(() =>
            {
                Assert.That(rootNode, Is.Not.Null);
                Assert.That(rootNode.GetFlatListOfDescendants(true), Has.Count.EqualTo(5));
            });
        }

        [Test]
        public void VerifyOnOptionChange()
        {
            var previousOption = this.viewModel.SelectedOption;
            this.viewModel.OnOptionChange(this.viewModel.TotalOptions.Last());
            Assert.That(previousOption, Is.Not.EqualTo(this.viewModel.SelectedOption));
        }

        [Test]
        public void VerifyOnActualFiniteStateChanged()
        {
            var previousState = this.viewModel.SelectedActualFiniteStates;
        }
    }
}
