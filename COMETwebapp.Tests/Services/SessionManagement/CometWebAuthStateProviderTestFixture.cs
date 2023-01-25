﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometWebAuthStateProviderTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2023 RHEA System S.A.
//
//    Author: Justine Veirier d'aiguebonne, Sam Gerené, Alex Vorobiev, Alexander van Delft
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

namespace COMETwebapp.Tests.Services.SessionManagement
{
    using System.Threading.Tasks;

    using CDP4Common.SiteDirectoryData;

    using COMETwebapp.Services.SessionManagement;
    using COMETwebapp.SessionManagement;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class CometWebAuthStateProviderTestFixture
    {
        private CometWebAuthStateProvider cometWebAuthStateProvider;
        private Mock<ISessionService> sessionAnchor;

        [SetUp]
        public void SetUp()
        {
            var activePerson = new Person
            {
                GivenName = "John",
                Surname = "Doe"
            };

            sessionAnchor = new Mock<ISessionService>();
            sessionAnchor.Setup(x => x.Session.ActivePerson).Returns(activePerson);

            cometWebAuthStateProvider = new CometWebAuthStateProvider(sessionAnchor.Object);
        }

        [Test]
        public async Task Verify_that_GetAuthenticationStateAsync_returns_an_AuthenticationState()
        {
            sessionAnchor.Setup(x => x.IsSessionOpen).Returns(true);

            var authenticationState = await cometWebAuthStateProvider.GetAuthenticationStateAsync();

            Assert.That(authenticationState, Is.Not.Null);

            Assert.That(authenticationState.User.Identity.Name, Is.EqualTo("John Doe"));
        }
    }
}