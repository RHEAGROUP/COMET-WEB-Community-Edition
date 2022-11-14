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

    using COMETwebapp.Components.Viewer;

    /// <summary>
    /// The factory used for creating basic shapes of type <see cref="Primitive"/>
    /// </summary>
    public class ShapeFactory : IShapeFactory
    {
        /// <summary>
        /// Dictionary used from the creation of shapes
        /// </summary>
        Dictionary<string, Func<Primitive>> ShapeCreatorCollection;

        /// <summary>
        /// Creates a new instance of type <see cref="ShapeFactory"/>
        /// </summary>
        public ShapeFactory()
        {
            this.ShapeCreatorCollection = new Dictionary<string, Func<Primitive>>()
            {
                {"box", () => new Cube(1, 1, 1) },
                {"cylinder", () => new Cylinder(1, 1) },
                {"sphere", () => new Sphere(1) },
                {"torus", () => new Torus(1, 1) },
                {"triprism", () => new TriangularPrism(1, 1) },
                {"disc", () => new Disc(1) },
                {"hexagonalprism", () => new HexagonalPrism(1, 1) },
                {"rectangle", () => new Rectangle(1, 1) },
                {"triangle", () => new EquilateralTriangle(1) },
            };
        }

        /// <summary>
        /// Tries to create a <see cref="Primitive"/> from the data of a <see cref="ElementUsage"/>
        /// </summary>
        /// <param name="elementUsage">The <see cref="ElementUsage"/> used for creating a <see cref="Primitive"/></param>
        /// <param name="selectedOption">The current <see cref="Option"/> selected</param>
        /// <param name="states">The list of <see cref="ActualFiniteState"/> that are active</param>
        /// <returns>The created <see cref="Primitive"/></returns>
        public Primitive TryGetPrimitiveFromElementUsageParameter(ElementUsage elementUsage, Option selectedOption, List<ActualFiniteState> states)
        {
            ParameterBase? parameterBase = null;
            IValueSet? valueSet = null;
            Type parameterType = SceneProvider.ParameterShortNameToTypeDictionary[SceneProvider.ShapeKindShortName];


            if (elementUsage.ParameterOverride.Count > 0)
            {
                parameterBase = elementUsage.ParameterOverride.FirstOrDefault(x => x.ParameterType.ShortName == SceneProvider.ShapeKindShortName
                                                                                   && x.ParameterType.GetType() == parameterType);
            }

            if (parameterBase is null)
            {
                parameterBase = elementUsage.ElementDefinition.Parameter.FirstOrDefault(x => x.ParameterType.ShortName == SceneProvider.ShapeKindShortName
                                                                                             && x.ParameterType.GetType() == parameterType);
            }

            if (parameterBase is not null)
            {
                if (states.Count > 0)
                {
                    foreach (var actualFiniteState in states)
                    {
                        valueSet = parameterBase.QueryParameterBaseValueSet(selectedOption, actualFiniteState);
                        if (valueSet is not null)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    valueSet = parameterBase.QueryParameterBaseValueSet(selectedOption, null);
                }
            }

            Primitive primitive = null!;

            if (valueSet is not null)
            {
                string? shapeKind = valueSet.ActualValue.FirstOrDefault()?.ToLowerInvariant();

                if (this.ShapeCreatorCollection.ContainsKey(shapeKind))
                {
                    return this.ShapeCreatorCollection[shapeKind].Invoke();
                }
                else
                {
                    return new Cube(0.15, 0.15, 0.15);
                }
            }
            else
            {
                primitive = new Cube(0.15, 0.15, 0.15);
            }

            primitive.ElementUsage = elementUsage;

            return primitive;
        }
    }
}
