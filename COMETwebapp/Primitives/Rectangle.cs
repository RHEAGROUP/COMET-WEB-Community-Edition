﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Rectangle.cs" company="RHEA System S.A.">
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
    /// Rectangle primitive type
    /// </summary>
    public class Rectangle : BasicPrimitive
    {
        /// <summary>
        /// Basic type name
        /// </summary>
        public override string Type { get; protected set; } = "Rectangle";

        /// <summary>
        /// The width of the rectangle
        /// </summary>
        public double Width { get; private set; }

        /// <summary>
        /// The height of the rectangle
        /// </summary>
        public double Height { get; private set; }

        /// <summary>
        /// Creates a new instance of type <see cref="Rectangle"/>
        /// </summary>
        public Rectangle(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Set the dimensions of the <see cref="BasicPrimitive"/> from the <see cref="ElementUsage"/> parameters
        /// </summary>
        /// <param name="elementUsage">the <see cref="ElementUsage"/> used for the dimensioning</param>
        /// <param name="selectedOption">the current <see cref="Option"/> selected</param>
        /// <param name="states">the <see cref="ActualFiniteState"/> that are going to be used to dimensioning the <see cref="BasicPrimitive"/></param>
        public override void SetDimensionsFromElementUsageParameters(Option selectedOption, List<ActualFiniteState> states)
        {
            var widthValueSet = this.GetElementUsageValueSet(selectedOption, states, SceneProvider.WidthShortName);
            var heightValueSet = this.GetElementUsageValueSet(selectedOption, states, SceneProvider.HeightShortName);

            if (widthValueSet is not null && double.TryParse(widthValueSet.ActualValue.First(), out double w))
            {
                this.Width = w;
            }

            if (heightValueSet is not null && double.TryParse(heightValueSet.ActualValue.First(), out double h))
            {
                this.Height = h;
            }
        }
    }
}
