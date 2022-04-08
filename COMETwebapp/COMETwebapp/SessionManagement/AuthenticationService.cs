﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationService.cs" company="RHEA System S.A.">
//    Copyright (c) 2022 RHEA System S.A.
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

namespace COMETwebapp.SessionManagement
{
    using CDP4Common.SiteDirectoryData;
    using CDP4Dal;
    using CDP4Dal.DAL;
    using CDP4ServicesDal;
    using Microsoft.AspNetCore.Components.Authorization;

    /// <summary>
    /// The purpose of the <see cref="AuthenticationService"/> is to authenticate against
    /// a E-TM-10-25 Annex C.2 data source
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// The (injected) <see cref="AuthenticationStateProvider"/>
        /// </summary>
        private readonly AuthenticationStateProvider authStateProvider;

        /// <summary>
        /// The (injected) <see cref="ISessionAnchor"/> that provides access to the <see cref="ISession"/>
        /// </summary>
        private readonly ISessionAnchor sessionAnchor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="sessionAnchor">
        /// The (injected) <see cref="ISessionAnchor"/> that provides access to the <see cref="ISession"/>
        /// </param>
        /// <param name="authenticationStateProvider">
        /// The (injected) <see cref="AuthenticationStateProvider"/>
        /// </param>
        public AuthenticationService(ISessionAnchor sessionAnchor, AuthenticationStateProvider authenticationStateProvider)
        {
            this.authStateProvider = authenticationStateProvider;
            this.sessionAnchor = sessionAnchor;
        }

        /// <summary>
        /// Login (authenticate) with authentication information to a data source
        /// </summary>
        /// <param name="authenticationDto">
        /// The authentication information with data source, username and password
        /// </param>
        /// <returns>
        /// True when the authentication is done
        /// </returns>
        public async Task<Boolean> Login(AuthenticationDto authenticationDto)
        {
            var uri = new Uri(authenticationDto.SourceAddress);
            var dal = new CdpServicesDal();
            var credentials = new Credentials(authenticationDto.UserName, authenticationDto.Password, uri);

            var session = new Session(dal, credentials);
            await session.Open();

            this.sessionAnchor.Session = session;

            Console.WriteLine($"user:{session.ActivePerson.Name}");

            ((CometWebAuthStateProvider)this.authStateProvider).NotifyAuthenticationStateChanged();

            return true;
        }

        /// <summary>
        /// Logout from the data source
        /// </summary>
        /// <returns>
        /// a <see cref="Task"/>
        /// </returns>
        public async Task Logout()
        {
            if (this.sessionAnchor.Session != null)
            {
                await this.sessionAnchor.Session.Close();
                this.sessionAnchor.Session = null;
            }
     
            ((CometWebAuthStateProvider)this.authStateProvider).NotifyAuthenticationStateChanged();
        }
        
    }
}
