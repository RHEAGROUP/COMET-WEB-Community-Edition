﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="LoadingComponent.razor.cs" company="RHEA System S.A.">
//    Copyright (c) 2023-2024 RHEA System S.A.
// 
//    Authors: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Théate Antoine, Nabil Abbar
// 
//    This file is part of CDP4-COMET WEB Community Edition
//    The CDP4-COMET WEB Community Edition is the RHEA Web Application implementation of ECSS-E-TM-10-25
//    Annex A and Annex C.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace COMET.Web.Common.Components
{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    /// The <see cref="LoadingComponent" /> is used to warn the user that something is loading and aware him that the application is not frozen/not responsive.
    /// </summary>
    public partial class LoadingComponent
    {
        /// <summary>
        /// Value asserting that the <see cref="LoadingComponent" /> should be visible or not
        /// </summary>
        [Parameter]
        public bool IsVisible { get; set; }

        /// <summary>
        /// The child content of the component
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The Loading Caption
        /// </summary>
        public string Caption { get; set; } = "COMET Community Edition";
    }
}
