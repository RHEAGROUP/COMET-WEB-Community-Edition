// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ThingExtensions.cs" company="RHEA System S.A.">
//     Copyright (c) 2023 RHEA System S.A.
// 
//     Authors: Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar, Théate Antoine, Nabil Abbar
// 
//     This file is part of COMET WEB Community Edition
//     The COMET WEB Community Edition is the RHEA Web Application implementation of ECSS-E-TM-10-25 Annex A and Annex C.
// 
//     The COMET WEB Community Edition is free software; you can redistribute it and/or
//     modify it under the terms of the GNU Affero General Public
//     License as published by the Free Software Foundation; either
//     version 3 of the License, or (at your option) any later version.
// 
//     The COMET WEB Community Edition is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace COMETwebapp.Extensions
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Helpers;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using COMET.Web.Common.Extensions;

    /// <summary>
    /// Extension class for <see cref="Thing" />
    /// </summary>
    public static class ThingExtensions
    {
        /// <summary>
        /// Queries all <see cref="ParameterValueSetBase" /> of the given iteration
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        /// <returns>A collection of <see cref="ParameterValueSetBase" /></returns>
        public static IEnumerable<ParameterValueSetBase> QueryParameterValueSetBase(this Iteration iteration)
        {
            var valueSets = new List<ParameterValueSetBase>();

            if (iteration.TopElement != null)
            {
                valueSets.AddRange(iteration.TopElement.Parameter.SelectMany(x => x.ValueSet));
            }

            foreach (var elementUsage in iteration.Element.SelectMany(elementDefinition => elementDefinition.ContainedElement))
            {
                if (!elementUsage.ParameterOverride.Any())
                {
                    valueSets.AddRange(elementUsage.ElementDefinition.Parameter.SelectMany(x => x.ValueSet));
                }
                else
                {
                    valueSets.AddRange(elementUsage.ParameterOverride.SelectMany(x => x.ValueSet));

                    valueSets.AddRange(elementUsage.ElementDefinition.Parameter.Where(x => elementUsage.ParameterOverride.All(o => o.Parameter.Iid != x.Iid))
                        .SelectMany(x => x.ValueSet));
                }
            }

            return valueSets.DistinctBy(x => x.Iid);
        }

        /// <summary>
        /// Queries all <see cref="NestedParameter" /> that belongs to a given <see cref="Option" />
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /> to get the <see cref="NestedParameter" />s</param>
        /// <param name="option">The <see cref="Option" /></param>
        /// <returns>A collection of <see cref="NestedParameter" /></returns>
        public static IEnumerable<NestedParameter> QueryNestedParameters(this Iteration iteration, Option option)
        {
            var generator = new NestedElementTreeGenerator();
            return iteration.TopElement == null ? Enumerable.Empty<NestedParameter>() : generator.GetNestedParameters(option);
        }

        /// <summary>
        /// Queries all <see cref="ParameterOrOverrideBase" /> that belongs to a given <see cref="Option" />
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /> to get the <see cref="ParameterOrOverrideBase" />s</param>
        /// <param name="option">The <see cref="Option" /></param>
        /// <returns>A collection of <see cref="ParameterOrOverrideBase" /></returns>
        public static IEnumerable<ParameterOrOverrideBase> QueryParameterAndOverrideBases(this Iteration iteration, Option option)
        {
            var elements = iteration.QueryNestedElements(option);
            var parameters = new List<ParameterOrOverrideBase>();

            foreach (var nestedElement in elements.Select(x => x.GetElementBase()))
            {
                parameters.AddRange(nestedElement.QueryParameterAndOverrideBases());
            }

            return parameters.DistinctBy(x => x.Iid);
        }

        /// <summary>
        /// Queries all <see cref="ParameterOrOverrideBase" /> that belongs to a given <see cref="Option" /> owned by a
        /// <see cref="DomainOfExpertise" />
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /> to get the <see cref="ParameterOrOverrideBase" />s</param>
        /// <param name="option">The <see cref="Option" /></param>
        /// <param name="domain">The <see cref="DomainOfExpertise" /></param>
        /// <returns>A collection of <see cref="ParameterOrOverrideBase" /></returns>
        public static IEnumerable<ParameterOrOverrideBase> QueryParameterAndOverrideBases(this Iteration iteration, Option option, DomainOfExpertise domain)
        {
            var elements = iteration.QueryNestedElements(option);
            var parameters = new List<ParameterOrOverrideBase>();

            foreach (var nestedElement in elements.Select(x => x.GetElementBase()))
            {
                parameters.AddRange(nestedElement.QueryParameterAndOverrideBases(domain));
            }

            return parameters.DistinctBy(x => x.Iid);
        }

        /// <summary>
        /// Queries all <see cref="ParameterOrOverrideBase" /> contains in an <see cref="ElementBase" />
        /// </summary>
        /// <param name="elementBase">The <see cref="ElementBase" /></param>
        /// <returns>A collection of <see cref="ParameterOrOverrideBase" /></returns>
        /// <remarks>
        /// If the <see cref="ElementBase" /> is an <see cref="ElementUsage" />, it will retrieve all
        /// <see cref="ParameterOverride" />
        /// and <see cref="Parameter" /> of its <see cref="ElementDefinition" />
        /// </remarks>
        public static IEnumerable<ParameterOrOverrideBase> QueryParameterAndOverrideBases(this ElementBase elementBase)
        {
            switch (elementBase)
            {
                case ElementDefinition elementDefinition:
                    return elementDefinition.Parameter;
                case ElementUsage usage:
                    var parameterAndOverrideBases = new List<ParameterOrOverrideBase>(usage.ParameterOverride);
                    parameterAndOverrideBases.AddRange(usage.ElementDefinition.QueryParameterAndOverrideBases());
                    return parameterAndOverrideBases;
                default:
                    return Enumerable.Empty<ParameterOrOverrideBase>();
            }
        }

        /// <summary>
        /// Queries all <see cref="ParameterOrOverrideBase" /> contains in an <see cref="ElementBase" /> owned by a
        /// <see cref="DomainOfExpertise" />
        /// </summary>
        /// <param name="elementBase">The <see cref="ElementBase" /></param>
        /// <param name="domain">The <see cref="DomainOfExpertise" /> owner</param>
        /// <returns>A collection of <see cref="ParameterOrOverrideBase" /></returns>
        /// <remarks>
        /// If the <see cref="ElementBase" /> is an <see cref="ElementUsage" />, it will retrieve all
        /// <see cref="ParameterOverride" />
        /// and <see cref="Parameter" /> of its <see cref="ElementDefinition" />
        /// </remarks>
        public static IEnumerable<ParameterOrOverrideBase> QueryParameterAndOverrideBases(this ElementBase elementBase, DomainOfExpertise domain)
        {
            switch (elementBase)
            {
                case ElementDefinition elementDefinition:
                    return elementDefinition.Parameter.Where(x => x.Owner.Iid == domain.Iid);
                case ElementUsage usage:
                    var parameterAndOverrideBases = new List<ParameterOrOverrideBase>(usage.ParameterOverride.Where(x => x.Owner.Iid == domain.Iid));
                    parameterAndOverrideBases.AddRange(usage.ElementDefinition.QueryParameterAndOverrideBases(domain));
                    return parameterAndOverrideBases;
                default:
                    return Enumerable.Empty<ParameterOrOverrideBase>();
            }
        }

        /// <summary>
        /// Queries all the unreferenced <see cref="ElementDefinition" /> in an <see cref="Iteration" />
        /// An unreferenced element is an element with no associated ElementUsage
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        /// <returns>All unreferenced <see cref="ElementDefinition" /></returns>
        public static IEnumerable<ElementDefinition> QueryUnreferencedElements(this Iteration iteration)
        {
            var elementUsages = iteration.Element.SelectMany(x => x.ContainedElement).ToList();

            var associatedElementDefinitions = elementUsages.Select(x => x.ElementDefinition);

            var unreferencedElementDefinitions = iteration.Element.ToList();
            unreferencedElementDefinitions.RemoveAll(x => associatedElementDefinitions.Any(e => e.Iid == x.Iid));
            unreferencedElementDefinitions.RemoveAll(x => x.Iid == iteration.TopElement.Iid);

            return unreferencedElementDefinitions;
        }

        /// <summary>
        /// Queries unused <see cref="ElementDefinition" /> in an <see cref="Iteration" />
        /// An unused element is an element not used in an option
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        /// <returns>All unused <see cref="ElementDefinition" /></returns>
        public static IEnumerable<ElementDefinition> QueryUnusedElementDefinitions(this Iteration iteration)
        {
            var nestedElements = iteration.QueryNestedElements().ToList();

            var associatedElements = nestedElements.SelectMany(x => x.ElementUsage.Select(e => e.ElementDefinition))
                .DistinctBy(x => x.Iid).ToList();

            var unusedElementDefinitions = iteration.Element.ToList();
            unusedElementDefinitions.RemoveAll(e => associatedElements.Any(x => x.Iid == e.Iid));
            unusedElementDefinitions.RemoveAll(x => x.Iid == iteration.TopElement?.Iid);
            return unusedElementDefinitions;
        }

        /// <summary>
        /// Gets the <see cref="ElementBase" /> from this iteration
        /// </summary>
        /// <param name="iteration">the iteration used for retrieving the elements</param>
        /// <returns>an <see cref="IEnumerable{ElementBase}" /></returns>
        /// <exception cref="ArgumentNullException">if the iteration is null</exception>
        public static IEnumerable<ElementBase> QueryElementsBase(this Iteration iteration)
        {
            if (iteration is null)
            {
                throw new ArgumentNullException(nameof(iteration));
            }

            var elements = new List<ElementBase>();

            if (iteration.TopElement is not null)
            {
                elements.Add(iteration.TopElement);
            }

            iteration.Element.ForEach(e => elements.AddRange(e.ContainedElement));

            return elements;
        }

        /// <summary>
        /// Queries all <see cref="ParameterSubscription" /> owned by a given <see cref="DomainOfExpertise" />
        /// contained into an <see cref="Iteration" />
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        /// <param name="domain">The <see cref="DomainOfExpertise" /></param>
        /// <returns>A collection of <see cref="ParameterSubscription" /></returns>
        public static IEnumerable<ParameterSubscription> QueryOwnedParameterSubscriptions(this Iteration iteration, DomainOfExpertise domain)
        {
            var subscriptions = new List<ParameterSubscription>();

            if (iteration.TopElement != null)
            {
                subscriptions.AddRange(iteration.TopElement.QueryOwnedParameterSubscriptions(domain));
            }

            subscriptions.AddRange(iteration.Element.SelectMany(x => x.ContainedElement).SelectMany(x => x.QueryOwnedParameterSubscriptions(domain)));
            return subscriptions.DistinctBy(x => x.Iid).OrderBy(p => p.ParameterType.Name);
        }

        /// <summary>
        /// Queries all <see cref="ParameterSubscription" /> owned by a given <see cref="DomainOfExpertise" />
        /// contained into an <see cref="ElementBase" />
        /// </summary>
        /// <param name="element">The <see cref="ElementBase" /></param>
        /// <param name="domain">The <see cref="DomainOfExpertise" /></param>
        /// <returns>A collection of <see cref="ParameterSubscription" /></returns>
        public static IEnumerable<ParameterSubscription> QueryOwnedParameterSubscriptions(this ElementBase element, DomainOfExpertise domain)
        {
            var subscriptions = new List<ParameterSubscription>();

            switch (element)
            {
                case ElementDefinition elementDefinition:
                    subscriptions.AddRange(elementDefinition.Parameter.QueryOwnedParameterSubscriptions(domain));
                    break;
                case ElementUsage elementUsage when !elementUsage.ParameterOverride.Any():
                    return elementUsage.ElementDefinition.QueryOwnedParameterSubscriptions(domain);
                case ElementUsage elementUsage:
                    var notOverridenParameters = elementUsage.ElementDefinition.Parameter.Where(x => elementUsage.ParameterOverride.All(p => p.Parameter.Iid != x.Iid));

                    subscriptions.AddRange(elementUsage.ParameterOverride.QueryOwnedParameterSubscriptions(domain));

                    subscriptions.AddRange(notOverridenParameters.QueryOwnedParameterSubscriptions(domain));
                    break;
            }

            return subscriptions;
        }

        /// <summary>
        /// Queries all <see cref="ParameterSubscription" /> owned by a given <see cref="DomainOfExpertise" />
        /// contained into a collection of <see cref="ParameterOrOverrideBase" />
        /// </summary>
        /// <param name="parameterOrOverrideBases">The collection of <see cref="ParameterOrOverrideBase" /></param>
        /// <param name="domain">The <see cref="DomainOfExpertise" /></param>
        /// <returns>A collection of <see cref="ParameterSubscription" /></returns>
        public static IEnumerable<ParameterSubscription> QueryOwnedParameterSubscriptions(this IEnumerable<ParameterOrOverrideBase> parameterOrOverrideBases,
            DomainOfExpertise domain)
        {
            return parameterOrOverrideBases.Where(x => x.Owner.Iid != domain.Iid)
                .SelectMany(x => x.ParameterSubscription.Where(p => p.Owner.Iid == domain.Iid));
        }

        /// <summary>
        /// Queries owned <see cref="ParameterOrOverrideBase" /> contained into an <see cref="Iteration" />
        /// that contains <see cref="ParameterSubscription" /> of other <see cref="DomainOfExpertise" />
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        /// <param name="domain">The <see cref="DomainOfExpertise" /></param>
        /// <returns>A collection of <see cref="ParameterSubscription" /></returns>
        public static IEnumerable<ParameterOrOverrideBase> QuerySubscribedParameterByOthers(this Iteration iteration, DomainOfExpertise domain)
        {
            var subscriptions = new List<ParameterOrOverrideBase>();

            if (iteration.TopElement != null)
            {
                subscriptions.AddRange(iteration.TopElement.QuerySubscribedParameterByOthers(domain));
            }

            subscriptions.AddRange(iteration.Element.SelectMany(x => x.ContainedElement).SelectMany(x => x.QuerySubscribedParameterByOthers(domain)));
            return subscriptions.DistinctBy(x => x.Iid).OrderBy(p => p.ParameterType.Name);
        }

        /// <summary>
        /// Queries owned <see cref="ParameterOrOverrideBase" /> contained into an <see cref="ElementBase" />
        /// that contains <see cref="ParameterSubscription" /> of other <see cref="DomainOfExpertise" />
        /// </summary>
        /// <param name="element">The <see cref="ElementBase" /></param>
        /// <param name="domain">The <see cref="DomainOfExpertise" /></param>
        /// <returns>A collection of <see cref="ParameterSubscription" /></returns>
        public static IEnumerable<ParameterOrOverrideBase> QuerySubscribedParameterByOthers(this ElementBase element, DomainOfExpertise domain)
        {
            var subscriptions = new List<ParameterOrOverrideBase>();

            switch (element)
            {
                case ElementDefinition elementDefinition:
                    subscriptions.AddRange(elementDefinition.Parameter.QuerySubscribedParameterByOthers(domain));
                    break;
                case ElementUsage elementUsage when !elementUsage.ParameterOverride.Any():
                    return elementUsage.ElementDefinition.QuerySubscribedParameterByOthers(domain);
                case ElementUsage elementUsage:
                    var notOverridenParameters = elementUsage.ElementDefinition.Parameter.Where(x => elementUsage.ParameterOverride.All(p => p.Parameter.Iid != x.Iid));
                    subscriptions.AddRange(elementUsage.ParameterOverride.QuerySubscribedParameterByOthers(domain));
                    subscriptions.AddRange(notOverridenParameters.QuerySubscribedParameterByOthers(domain));
                    break;
            }

            return subscriptions;
        }

        /// <summary>
        /// Queries all owned <see cref="ParameterOrOverrideBase" /> contained into a collection of
        /// <see cref="ParameterOrOverrideBase" />
        /// that contains <see cref="ParameterSubscription" /> of other <see cref="DomainOfExpertise" />
        /// </summary>
        /// <param name="parameterOrOverrideBases">The collection of <see cref="ParameterOrOverrideBase" /></param>
        /// <param name="domain">The <see cref="DomainOfExpertise" /></param>
        /// <returns>A collection of <see cref="ParameterSubscription" /></returns>
        public static IEnumerable<ParameterOrOverrideBase> QuerySubscribedParameterByOthers(this IEnumerable<ParameterOrOverrideBase> parameterOrOverrideBases,
            DomainOfExpertise domain)
        {
            return parameterOrOverrideBases.Where(x => x.Owner.Iid == domain.Iid
                                                       && x.ParameterSubscription.Any(p => p.Owner.Iid != domain.Iid));
        }

        /// <summary>
        /// Query the evolution of <see cref="ParameterSubscriptionValueSet" /> contained into a
        /// <see cref="ParameterSubscription" />
        /// </summary>
        /// <param name="parameterSubscription">The <see cref="ParameterSubscription" /></param>
        /// <returns>
        /// All changes for each <see cref="ParameterSubscriptionValueSet" />,
        /// collected inside a <see cref="Dictionary{TKey,TValue}" /> where the key is the revision number
        /// </returns>
        public static Dictionary<Guid, Dictionary<int, ValueArray<string>>> QueryParameterSubscriptionValueSetEvolution(this ParameterSubscription parameterSubscription)
        {
            var changes = new Dictionary<Guid, Dictionary<int, ValueArray<string>>>();

            foreach (var parameterSubscriptionValueSet in parameterSubscription.ValueSet)
            {
                changes[parameterSubscriptionValueSet.Iid] = parameterSubscriptionValueSet.QueryParameterSubscriptionValueSetEvolution();
            }

            return changes;
        }

        /// <summary>
        /// Queries the evolution of the <see cref="ValueArray{T}" /> of the Computed Value of the
        /// <see cref="ParameterSubscriptionValueSet" />
        /// </summary>
        /// <param name="valueSet">The <see cref="ParameterBase" /></param>
        /// <returns>All changes, collected inside a <see cref="Dictionary{TKey,TValue}" /> where the key is the revision number</returns>
        public static Dictionary<int, ValueArray<string>> QueryParameterSubscriptionValueSetEvolution(this ParameterSubscriptionValueSet valueSet)
        {
            var changes = new Dictionary<int, ValueArray<string>>();
            var subscribedParameterValueSet = valueSet.SubscribedValueSet;

            if (!subscribedParameterValueSet.Revisions.Any() || !subscribedParameterValueSet.Revisions.Keys.Any(x => x >= valueSet.RevisionNumber))
            {
                changes.Add(subscribedParameterValueSet.RevisionNumber, subscribedParameterValueSet.Published);
                return changes;
            }
            
            var currentRevisionNumber = subscribedParameterValueSet.Revisions.Keys.Where(x => x >= valueSet.RevisionNumber).Min();
            var currentValueSet = ((ParameterValueSetBase)subscribedParameterValueSet.Revisions[currentRevisionNumber]).Published;

            changes.Add(currentRevisionNumber, currentValueSet);

            foreach (var revisionNumber in subscribedParameterValueSet.Revisions.Keys.Where(x => x > currentRevisionNumber).ToList())
            {
                var publishedValueAtRevisionNumber = ((ParameterValueSetBase)subscribedParameterValueSet.Revisions[revisionNumber]).Published;

                if (!publishedValueAtRevisionNumber.ContainsSameValues(currentValueSet))
                {
                    currentRevisionNumber = revisionNumber;
                    currentValueSet = publishedValueAtRevisionNumber;
                    changes.Add(currentRevisionNumber, currentValueSet);
                }
            }

            if (currentRevisionNumber != subscribedParameterValueSet.RevisionNumber && !subscribedParameterValueSet.Published.ContainsSameValues(currentValueSet))
            {
                changes.Add(subscribedParameterValueSet.RevisionNumber, subscribedParameterValueSet.Published);
            }

            return changes;
        }
    }
}
