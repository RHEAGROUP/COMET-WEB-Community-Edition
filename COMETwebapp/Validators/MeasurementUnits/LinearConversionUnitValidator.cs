﻿
// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="LinearConversionUnitValidator.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.Validators.MeasurementUnits
{
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Validation;

    using COMET.Web.Common.Extensions;

    using FluentValidation;

    /// <summary>
    /// A class to validate the <see cref="LinearConversionUnit"/>
    /// </summary>
    public class LinearConversionUnitValidator : AbstractValidator<LinearConversionUnit>
    {
        /// <summary>
        /// Instantiates a new <see cref="LinearConversionUnitValidator"/>
        /// </summary>
        public LinearConversionUnitValidator(IValidationService validationService) : base()
        {
            this.Include(new MeasurementUnitValidator(validationService));
            this.RuleFor(x => x.ReferenceUnit).NotEmpty().Validate(validationService, nameof(LinearConversionUnit.ReferenceUnit));
            this.RuleFor(x => x.ConversionFactor).Validate(validationService, nameof(LinearConversionUnit.ConversionFactor));
        }
    }
}
