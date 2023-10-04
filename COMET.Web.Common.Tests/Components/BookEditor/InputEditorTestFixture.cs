﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="InputEditorTestFixture.cs" company="RHEA System S.A.">
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

namespace COMET.Web.Common.Tests.Components.BookEditor
{
    using System.Text.Json;

    using Bunit;

    using CDP4Common.ReportingData;
    using CDP4Common.SiteDirectoryData;

    using COMET.Web.Common.Components;
    using COMET.Web.Common.Components.BookEditor;
    using COMET.Web.Common.Services.SessionManagement;
    using COMET.Web.Common.Test.Helpers;

    using DevExpress.Blazor;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class InputEditorTestFixture
    {
        private TestContext context;
        private IRenderedComponent<InputEditor<Book>> component;
        private Book book;
        private List<DomainOfExpertise> activeDomains;
        private List<Category> availableCategories;
        private Mock<ISessionService> sessionService;
        private MockHttpMessageHandler mockHttpMessageHandler;
        private HttpClient httpClient;
        private const string BookName = "Book Example";
        private const string BookShortName = "bookExample";
        
        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.sessionService = new Mock<ISessionService>();
            this.context.Services.AddSingleton(this.sessionService.Object);
            this.mockHttpMessageHandler = new MockHttpMessageHandler();
            this.httpClient = this.mockHttpMessageHandler.ToHttpClient();
            this.httpClient.BaseAddress = new Uri("http://localhost/");
            this.context.Services.AddScoped(_ => this.httpClient);
            var httpResponse = new HttpResponseMessage();
            httpResponse.Content = new StringContent("{\n  \"ShowName\": true,\n  \"ShowShortName\" : true \n}\n");
            this.mockHttpMessageHandler.When(HttpMethod.Get, "/_content/CDP4.WEB.Common/BookInputConfiguration.json").Respond(_ => httpResponse);

            this.activeDomains = new List<DomainOfExpertise>
            {
                new() { Name = "Sys" }
            };

            this.availableCategories = new List<Category>
            {
                new() { Name = "Category" }
            };

            this.book = new Book()
            {
                Name = BookName,
                ShortName = BookShortName,
                Owner = this.activeDomains.First(),
                Category = this.availableCategories
            };

            this.component = this.context.RenderComponent<InputEditor<Book>>(parameters =>
            {
                parameters.Add(p => p.Item, this.book);
                parameters.Add(p => p.ActiveDomains, this.activeDomains);
                parameters.Add(p => p.AvailableCategories, this.availableCategories);
            });
        }

        [Test]
        public void VerifyComponent()
        {
            var textboxes = this.component.FindComponents<DxTextBox>();
            var combobox = this.component.FindComponent<DxComboBox<DomainOfExpertise, DomainOfExpertise>>();
            var categoryComboBox = this.component.FindComponent<MultiComboBox<Category>>();

            var nameTextbox = textboxes.FirstOrDefault(x => x.Instance.Text == BookName);
            var shortNameTextbox = textboxes.FirstOrDefault(x => x.Instance.Text == BookShortName);
            
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(nameTextbox);
                Assert.IsNotNull(shortNameTextbox);
                Assert.That(combobox.Instance.Value, Is.EqualTo(this.activeDomains.First()));
                Assert.That(categoryComboBox.Instance, Is.Not.Null);
            });
            
            this.component.Render();
            
            Assert.Multiple(() =>
            {
                Assert.That(categoryComboBox.Instance.Data, Is.EquivalentTo(this.availableCategories));
                Assert.That(categoryComboBox.Instance.Values, Is.EquivalentTo(this.availableCategories));
            });
        }
    }
}

