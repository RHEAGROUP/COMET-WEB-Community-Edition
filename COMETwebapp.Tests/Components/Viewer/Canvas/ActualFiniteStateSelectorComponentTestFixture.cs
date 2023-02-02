﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActualFiniteStateSelectorComponentTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar
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

using COMETwebapp.Components.Viewer.Canvas;

namespace COMETwebapp.Tests.Components.Viewer.Canvas
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using BlazorStrap;

    using Bunit;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Types;

    using COMETwebapp.ViewModels.Components.Viewer.Canvas;

    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ActualFiniteStateSelectorComponentTestFixture
    {
        private ActualFiniteStateSelectorComponent actualFiniteStateSelectorComponent;

        private TestContext context;
        private ConcurrentDictionary<CacheKey, Lazy<Thing>> cache;
        private Uri uri = new Uri("http://www.rheagroup.com");
        private IRenderedComponent<ActualFiniteStateSelectorComponent> rendererComponent;
        private IActualFiniteStateSelectorViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            this.context = new TestContext();

            this.viewModel = new ActualFiniteStateSelectorViewModel();

            this.context.Services.AddSingleton<IActualFiniteStateSelectorViewModel>(this.viewModel);
            this.context.Services.AddBlazorStrap();
            this.cache = new ConcurrentDictionary<CacheKey, Lazy<Thing>>();

            var actualFiniteState1 = new ActualFiniteState(Guid.NewGuid(), this.cache, this.uri);
            var actualFiniteState2 = new ActualFiniteState(Guid.NewGuid(), this.cache, this.uri);
            var actualFiniteStateList1 = new ActualFiniteStateList(Guid.NewGuid(), this.cache, this.uri);
            actualFiniteStateList1.ActualState.Add(actualFiniteState1);
            actualFiniteStateList1.ActualState.Add(actualFiniteState2);

            var actualFiniteState3 = new ActualFiniteState(Guid.NewGuid(), this.cache, this.uri);
            var actualFiniteState4 = new ActualFiniteState(Guid.NewGuid(), this.cache, this.uri);
            var actualFiniteStateList2 = new ActualFiniteStateList(Guid.NewGuid(), this.cache, this.uri);
            actualFiniteStateList2.ActualState.Add(actualFiniteState3);
            actualFiniteStateList2.ActualState.Add(actualFiniteState4);

            var listOfActualStateList = new List<ActualFiniteStateList>() { actualFiniteStateList1, actualFiniteStateList2 };
            this.viewModel.ActualFiniteStateListsCollection = listOfActualStateList;

            this.rendererComponent = this.context.RenderComponent<ActualFiniteStateSelectorComponent>();
        }

        [Test]
        public void VerifyThatActualStateCanBeClicked()
        {
            var actualFiniteState = this.rendererComponent.Find(".actual-finite-state");
            actualFiniteState.Click();
        }
    }
}
