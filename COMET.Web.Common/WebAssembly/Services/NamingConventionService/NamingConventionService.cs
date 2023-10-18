// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="NamingConventionService.cs" company="RHEA System S.A.">
//    Copyright (c) 2023 RHEA System S.A.
// 
//    Authors: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Théate Antoine, Nabil Abbar
// 
//    This file is part of COMET WEB Community Edition
//    The COMET WEB Community Edition is the RHEA Web Application implementation of ECSS-E-TM-10-25
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

namespace COMET.Web.Common.WebAssembly.Services.NamingConventionService
{
    using System.Collections.Immutable;
    using System.Net;
    using System.Text.Json;

    using COMET.Web.Common.Services.NamingConventionService;
    using COMET.Web.Common.Utilities;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The <see cref="NamingConventionService" /> provides static information based on defined naming convention, like for names of
    /// <see cref="Category" /> to use for example
    /// </summary>
    public class NamingConventionService<TEnum> : BaseNamingConventionService<TEnum> where TEnum : Enum
    {
        /// <summary>
        /// The <see cref="ILogger{TCategoryName}" />
        /// </summary>
        private readonly ILogger<INamingConventionService<TEnum>> logger;

        /// <summary>
        /// The <see cref="HttpClient"/>
        /// </summary>
        private readonly HttpClient httpClient;

        public NamingConventionService(ILogger<INamingConventionService<TEnum>> logger, HttpClient httpClient) : base(logger)
        {
            this.logger = logger;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Gets the naming convention configuration
        /// </summary>
        /// <returns>A <see cref="IReadOnlyDictionary{TKey,TValue}"/> of the naming convention configuration</returns>
        public override async Task<IReadOnlyDictionary<string, string>> GetNamingConventionConfiguration()
        {
            try
            {
                var path = ContentPathBuilder.BuildPath("naming_convention.json");
                var response = await this.httpClient.GetAsync(path);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStreamAsync();
                    var namingConvention = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                    return new Dictionary<string, string>(namingConvention, StringComparer.OrdinalIgnoreCase);
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    this.logger.LogError("Naming conventions file not found at {path}", path);
                    return ImmutableDictionary<string, string>.Empty;
                }

                this.logger.LogError("Error fetching naming conventions. Status code: {response}", response.StatusCode);
                return ImmutableDictionary<string, string>.Empty;
            }
            catch (Exception e)
            {
                this.logger.LogCritical("Exception has been raised : {message}", e.Message);
                return ImmutableDictionary<string, string>.Empty;
            }
        }
    }
}
