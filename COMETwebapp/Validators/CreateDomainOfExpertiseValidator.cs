﻿
// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="CreateDomainOfExpertiseValidator.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.Validators
{
    using CDP4Common.SiteDirectoryData;

    using FluentValidation;

    /// <summary>
    /// A class to validate the <see cref="DomainOfExpertise"/>
    /// </summary>
    public class CreateDomainOfExpertiseValidator : AbstractValidator<DomainOfExpertise>
    {
        /// <summary>
        /// Instantiates a new <see cref="CreateDomainOfExpertiseValidator"/>
        /// </summary>
        public CreateDomainOfExpertiseValidator() : base()
        {
            this.RuleFor(x => x.ShortName).NotEmpty();
            this.RuleFor(x => x.ShortName).Must(IsShortNameValid).WithMessage("Shortnames should not contain white spaces");
            this.RuleFor(x => x.Name).NotEmpty();
        }

        /// <summary>
        /// Checks if the given shortname is valid
        /// </summary>
        /// <param name="shortName">The shortname to check</param>
        /// <returns>true if the shortname is valid, otherwise false</returns>
        private static bool IsShortNameValid(string shortName)
        {
            if (shortName != null)
            {
                return !shortName.Contains(' ');
            }

            return false;
        }
    }
}
