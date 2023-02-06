﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ViewerTestFixture.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.Tests.Pages.Viewer
{
    using Bunit;

    using COMETwebapp.Pages.Viewer;
    using COMETwebapp.Services.SessionManagement;
    using COMETwebapp.ViewModels.Components.Viewer.Canvas;
    using COMETwebapp.ViewModels.Pages.Viewer;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ViewerTestFixture
    {
        private TestContext context;
        private IRenderedComponent<ViewerPage> renderedComponent;
        private ViewerPage viewer;
        private Mock<IViewerViewModel> viewerViewModel;
        
        [SetUp]
        public void SetUp()
        {
            this.context = new TestContext();
            this.viewerViewModel = new Mock<IViewerViewModel>();
            this.context.Services.AddSingleton(this.viewerViewModel.Object);

            var canvasViewModel = new Mock<ICanvasViewModel>();
            this.context.Services.AddSingleton(canvasViewModel.Object);

            this.context.Services.AddSingleton<ISessionService, SessionService>();

            this.renderedComponent = this.context.RenderComponent<ViewerPage>();
            this.viewer = this.renderedComponent.Instance;
        }

        [Test]
        public void VerifyComponent()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.viewer, Is.Not.Null);
                Assert.That(this.viewer.ViewModel, Is.Not.Null);
                Assert.That(this.viewer.CanvasComponent, Is.Not.Null);
            });
        }
    }
}
