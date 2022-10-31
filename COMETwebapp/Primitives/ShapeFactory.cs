﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShapeFactory.cs" company="RHEA System S.A.">
//    Copyright (c) 2022 RHEA System S.A.
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

namespace COMETwebapp.Primitives
{
    using System.Collections.Generic;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    /// <summary>
    /// The factory used for creating basic shapes of type <see cref="Primitive"/>
    /// </summary>
    public class ShapeFactory : IShapeFactory
    {
        /// <summary>
        /// Tries to create a <see cref="Primitive"/> from the <see cref="ElementUsage"/>
        /// </summary>
        /// <param name="elementUsage">The <see cref="ElementUsage"/> used for creating a <see cref="Primitive"/></param>
        /// <param name="selectedOption">The current <see cref="Option"/> selected</param>
        /// <param name="states">The list of <see cref="ActualFiniteState"/> that are active</param>
        /// <param name="basicShape">The basic shape of type <see cref="Primitive"/></param>
        /// <returns></returns>
        public Task<Primitive> TryGetPrimitiveFromElementUsageParameter(ElementUsage elementUsage, Option selectedOption, List<ActualFiniteState> states)
        {
            var parameter = elementUsage.ElementDefinition.Parameter.FirstOrDefault(x => x.ParameterType.ShortName == "kind"
                      && (x.ParameterType is EnumerationParameterType || x.ParameterType is TextParameterType));

            IValueSet? valueSet = null;

            if (parameter is not null)
            {
                foreach (var actualFiniteState in states)
                {
                    valueSet = parameter.QueryParameterBaseValueSet(selectedOption, actualFiniteState);
                    if (valueSet is not null)
                    {
                        break;
                    }
                }
            }

            if(valueSet is not null)
            {
                string? shapeKind = valueSet.ActualValue.FirstOrDefault()?.ToLowerInvariant();
                switch (shapeKind)
                {
                    case "box": return Task.FromResult<Primitive>(new Cube(1, 1, 1));
                    case "cylinder": return Task.FromResult<Primitive>(new Cylinder(1, 1));
                    case "sphere": return Task.FromResult<Primitive>(new Sphere(1));
                    case "torus": return Task.FromResult<Primitive>(new Torus(1, 1));
                    case "triprism": throw new NotImplementedException();
                    case "tetrahedron": throw new NotImplementedException();
                    case "capsule": throw new NotImplementedException();
                    
                    default: return Task.FromResult<Primitive>(new Cube(1, 1, 1)); 
                }
            }
            else
            {
                return Task.FromResult<Primitive>(new Cube(1, 1, 1));
            }      
        }
    }
}
