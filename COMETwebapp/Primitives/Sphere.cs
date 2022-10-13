﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sphere.cs" company="RHEA System S.A.">
//    Copyright (c) 2022 RHEA System S.A.
//
//    Author: Jaime Bernar
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
    /// <summary>
    /// Sphere primitive type
    /// </summary>
    public class Sphere : PositionablePrimitives
    {
        /// <summary>
        /// Basic type name
        /// </summary>
        public override string Type { get; protected set; } = "Sphere";

        /// <summary>
        /// The radius of the <see cref="Sphere"/>
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="Sphere"/> class
        /// </summary>
        /// <param name="radius"></param>
        public Sphere(double radius)
        {
            this.Radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Sphere"/> class
        /// </summary>
        /// <param name="x">position along the x axis</param>
        /// <param name="y">position along the y axis</param>
        /// <param name="z">position along the z axis</param>
        /// <param name="radius">the radius of the sphere</param>
        public Sphere(double x, double y, double z, double radius)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Radius = radius;
        }
    }
}
