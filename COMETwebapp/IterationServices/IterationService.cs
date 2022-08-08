﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationService.cs" company="RHEA System S.A.">
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

namespace COMETwebapp.IterationServices
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Helpers;
    using CDP4Common.SiteDirectoryData;
    using COMETwebapp.Model;

    /// <summary>
    /// Service to access iteration data
    /// </summary>
    public class IterationService : IIterationService
    {
        /// <summary>
        /// Save updates changes to avoid highlights after validation
        /// Save changes for each domain available in the opened session 
        /// </summary>
        public Dictionary<DomainOfExpertise, List<ParameterSubscriptionViewModel>> ValidatedUpdates { get; set; } = new Dictionary<DomainOfExpertise, List<ParameterSubscriptionViewModel>>();

        /// <summary>
        /// Save Thing Iid with edit changes in the web application
        /// </summary>
        public List<Guid> NewUpdates { get; set; } = new List<Guid>();

        /// <summary>
        /// Get all <see cref="ParameterValueSetBase"/> of the given iteration
        /// </summary>
        /// <param name="iteration">
        /// The <see cref="Iteration"/> for which the <see cref="ParameterValueSetBase"/>s list is created
        /// </param>
        /// <returns>All <see cref="ParameterValueSetBase"/></returns>
        public List<ParameterValueSetBase> GetParameterValueSetBase(Iteration? iteration)
        {
            var result = new List<ParameterValueSetBase>();
            if (iteration?.TopElement != null)
            {
                iteration?.TopElement.Parameter.ForEach(p => result.AddRange(p.ValueSet));
            }
            iteration?.Element.ForEach(e =>
            {
                e.ContainedElement.ForEach(containedElement =>
                {
                    if(containedElement.ParameterOverride.Count == 0)
                    {
                        containedElement.ElementDefinition.Parameter.ForEach(p => result.AddRange(p.ValueSet));
                    } else
                    {
                        var associatedParameterValueSet = new List<ParameterValueSet>();
                        containedElement.ParameterOverride.ForEach(p => {
                            result.AddRange(p.ValueSet);
                            p.ValueSet.ForEach(povs => associatedParameterValueSet.Add(povs.ParameterValueSet));
                        });
                        containedElement.ElementDefinition.Parameter.ForEach(p => {
                            result.AddRange(p.ValueSet.FindAll(pvs => !associatedParameterValueSet.Contains(pvs)));
                        });
                    }
                });
            });
            return result.DistinctBy(p => p.Iid).ToList();
        }

        /// <summary>
        /// Get all <see cref="NestedElement"/> of the given iteration for all options
        /// </summary>
        /// <param name="iteration">
        /// The <see cref="Iteration"/> for which the <see cref="NestedElement"/>s list is created
        /// </param>
        /// <returns>All <see cref="NestedElement"/></returns>
        public List<NestedElement> GetNestedElements(Iteration? iteration)
        {
            var nestedElementTreeGenerator = new NestedElementTreeGenerator();
            var nestedElements = new List<NestedElement>();
            if(iteration?.TopElement != null)
            {
                iteration.Option.ToList().ForEach(option => nestedElements.AddRange(nestedElementTreeGenerator.Generate(option)));
            }
            return nestedElements;
        }

        /// <summary>
        /// Get <see cref="NestedElement"/> of the given iteration and for a given option
        /// </summary>
        /// <param name="iteration">
        /// The <see cref="Iteration"/> for which the <see cref="NestedElement"/>s list is created
        /// </param>
        /// <param name="optionIid">
        /// Name of the <see cref="Option"/> for which the <see cref="NestedElement"/>s list is created
        /// </param>
        /// <returns>All <see cref="NestedElement"/> of the given option</returns>
        public List<NestedElement> GetNestedElementsByOption(Iteration? iteration, Guid? optionIid)
        {
            var nestedElementTreeGenerator = new NestedElementTreeGenerator();
            var nestedElements = new List<NestedElement>();
            if (iteration?.TopElement != null)
            {
                var option = iteration.Option.ToList().Find(option => option.Iid == optionIid);
                nestedElements.AddRange(nestedElementTreeGenerator.Generate(option));
            }
            return nestedElements;
        }

        /// <summary>
        /// Get the nested parameters from the given option
        /// </summary>
        /// <param name="iteration">
        /// The <see cref="Iteration"/> for which the <see cref="NestedParameter"/>s list is created
        /// </param>
        /// <param name="optionIid">
        /// The Iid of the option for which the <see cref="NestedParameter"/>s list is created
        /// </param>
        /// <returns>All<see cref="NestedParameter"/> of the given option</returns>
        public List<NestedParameter> GetNestedParameters(Iteration? iteration, Guid? optionIid)
        {
            var nestedElementTreeGenerator = new NestedElementTreeGenerator();
            var nestedParameters = new List<NestedParameter>();
            var option = iteration?.Option.FirstOrDefault(x => x.Iid == optionIid);

            if (option != null && iteration?.TopElement != null)
            {
                nestedParameters.AddRange(nestedElementTreeGenerator.GetNestedParameters(option));
            }
            return nestedParameters;
        }

        /// <summary>
        /// Get unused elements defintion of the opened iteration
        /// An unused element is an element not used in an option
        /// </summary>
        /// <param name="iteration">
        /// The <see cref="Iteration"/> for which the <see cref="ElementDefinition"/>s list is created
        /// </param>
        /// <returns>All unused <see cref="ElementDefinition"/></returns>
        public List<ElementDefinition> GetUnusedElementDefinitions(Iteration? iteration)
        {
            var nestedElements = this.GetNestedElements(iteration);
            var associatedElements = new List<ElementDefinition>();
            nestedElements.ForEach(element => {
                element.ElementUsage.ToList().ForEach(e => associatedElements.Add(e.ElementDefinition));
             });
            associatedElements = associatedElements.Distinct().ToList();

            var unusedElementDefinitions = new List<ElementDefinition>();
            if (iteration is not null)
            {
                unusedElementDefinitions.AddRange(iteration.Element);
            }
            unusedElementDefinitions.RemoveAll(e => associatedElements.Contains(e));

            if(iteration is not null && iteration.TopElement is not null)
            {
                unusedElementDefinitions.Remove(iteration.TopElement);
            }

            return unusedElementDefinitions;
        }

        /// <summary>
        /// Get all the unreferenced element definitions in the opened iteration
        /// An unreferenced element is an element with no associated ElementUsage
        /// </summary>
        /// <param name="iteration">
        /// The <see cref="Iteration"/> for which the <see cref="ElementDefinition"/>s list is created
        /// </param>
        /// <returns>All unreferenced <see cref="ElementDefinition"/></returns>
        public List<ElementDefinition> GetUnreferencedElements(Iteration? iteration)
        {
            var elementUsages = new List<ElementUsage>();
            iteration?.Element.ForEach(e => elementUsages.AddRange(e.ContainedElement));

            var associatedElementDefinitions = new List<ElementDefinition>();
            elementUsages.ForEach(e => associatedElementDefinitions.Add(e.ElementDefinition));

            var unreferencedElementDefinitions = new List<ElementDefinition>();
            if(iteration is not null)
            {
                unreferencedElementDefinitions.AddRange(iteration.Element);
            }
            unreferencedElementDefinitions.RemoveAll(e => associatedElementDefinitions.Contains(e));

            if (iteration is not null && iteration.TopElement is not null)
            {
                unreferencedElementDefinitions.Remove(iteration.TopElement);
            }

            return unreferencedElementDefinitions;
        }

        /// <summary>
        /// Get all <see cref="ParameterSubscription"/> of the given domain and for the given element
        /// </summary>
        /// <param name="element">The <see cref="ElementBase"> to get the subscriptions</param>
        /// <param name="currentDomainOfExpertise">The current <see cref="DomainOfExpertise"/> of the iteration</param>
        /// <returns>List of all <see cref="ParameterSubscription"/> for this element </returns>
        public List<ParameterSubscription> GetParameterSubscriptionsByElement(ElementBase element, DomainOfExpertise? currentDomainOfExpertise)
        {
            var parameterSubscriptions = new List<ParameterSubscription>();
            if(element is ElementDefinition)
            {
                parameterSubscriptions.AddRange(((ElementDefinition)element).Parameter.SelectMany(p => p.ParameterSubscription).Where(p => p.Owner == currentDomainOfExpertise));
            } else if(element is ElementUsage)
            {
                var elementUsage = (ElementUsage)element;
                if (elementUsage.ParameterOverride.Count == 0)
                {
                    parameterSubscriptions.AddRange(elementUsage.ElementDefinition.Parameter.SelectMany(p => p.ParameterSubscription).Where(p => p.Owner == currentDomainOfExpertise));
                }
                else
                {
                    var associatedParameters = new List<Parameter>();
                    elementUsage.ParameterOverride.ForEach(p =>
                    {
                        p.ParameterSubscription.ForEach(s =>
                        {
                            if (s.Owner == currentDomainOfExpertise)
                            {
                                parameterSubscriptions.Add(s);
                                associatedParameters.Add(p.Parameter);
                            }
                        });
                    });
                    parameterSubscriptions.AddRange(
                        elementUsage.ElementDefinition.Parameter.Where(p => !associatedParameters.Contains(p))
                        .SelectMany(p => p.ParameterSubscription).Where(p => p.Owner == currentDomainOfExpertise));
                }
            }
            return parameterSubscriptions.OrderBy(p => p.ParameterType.Name).ToList();
        }

        /// <summary>
        /// Gets number of updates in the iteration after a session refresh
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration"/> to get number of updates</param>
        /// <param name="currentDomainOfExpertise">The <see cref="DomainOfExpertise"/></param>
        /// <returns></returns>
        public int GetNumberUpdates(Iteration? iteration, DomainOfExpertise? currentDomainOfExpertise)
        {
            var subscribedParameters = new List<ParameterSubscription>();
            if (iteration is not null)
            {
                if(iteration.TopElement != null)
                {
                    subscribedParameters.AddRange(this.GetParameterSubscriptionsByElement(iteration.TopElement, currentDomainOfExpertise));
                }
                iteration.Element.SelectMany(e => e.ContainedElement).ToList().ForEach(e =>
                {
                    subscribedParameters.AddRange(this.GetParameterSubscriptionsByElement(e, currentDomainOfExpertise));
                });
            }
            
            var numberUpdates = 0;

            subscribedParameters.SelectMany(p => p.ValueSet).ToList().ForEach(parameterSubscriptionValueSet => 
            {
                var isUpdated = false;
                if (parameterSubscriptionValueSet.SubscribedValueSet.Revisions.LongCount() != (long)0
                    && parameterSubscriptionValueSet.SubscribedValueSet.RevisionNumber != parameterSubscriptionValueSet.SubscribedValueSet.Revisions.Last().Value.RevisionNumber)
                {
                    isUpdated = true;
                }
                if (currentDomainOfExpertise != null && this.ValidatedUpdates.TryGetValue(currentDomainOfExpertise, out var list))
                {
                    var existingValidatedParameter = list.Find(p => p.Iid == parameterSubscriptionValueSet.Iid);
                    if (existingValidatedParameter != null && parameterSubscriptionValueSet.RevisionNumber == existingValidatedParameter.RevisionNumber && parameterSubscriptionValueSet.ValueSwitch != ParameterSwitchKind.COMPUTED)
                    {
                        isUpdated = false;
                    }
                    if (existingValidatedParameter != null && parameterSubscriptionValueSet.SubscribedValueSet.RevisionNumber == existingValidatedParameter.SubscribedRevisionNumber)
                    {
                        isUpdated = false;
                    }
                }

                if (isUpdated)
                {
                    numberUpdates += 1;
                }
            });

            return numberUpdates;
        }

        /// <summary>
        /// Gets list of parameter types used in the given iteration
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration"/> for which the <see cref="ParameterType"/>s list is created</param>
        /// <returns>All <see cref="ParameterType"/>s used in the iteration</returns>
        public List<ParameterType> GetParameterTypes(Iteration? iteration)
        {
            var parameterTypes = new List<ParameterType>();
            iteration?.Element.ForEach(element =>
            {
                element.Parameter.ForEach(parameter =>
                {
                    parameterTypes.Add(parameter.ParameterType);
                });
            });
            return parameterTypes.Distinct().ToList();
        }

        /// Get all <see cref="ParameterValueSet"/> of the given iteration for one given parameter type
        /// </summary>
        /// <param name="iteration">
        /// The <see cref="Iteration"/> for which the <see cref="ParameterValueSet"/>s list is created
        /// </param>
        /// <param name="parameterType">
        /// The name of <see cref="ParameterType"/> for which the <see cref="ParameterValueSet"/>s list is created
        /// </param>
        /// <returns>All <see cref="ParameterValueSet" for the given parameter type/></returns>
        public List<ParameterValueSetBase> GetParameterValueSetsByParameterType(Iteration? iteration, string? parameterTypeName)
        {
            var result = new List<ParameterValueSetBase>();
            if (iteration?.TopElement != null)
            {
                iteration?.TopElement.Parameter.FindAll(p => p.ParameterType.Name.Equals(parameterTypeName)).ForEach(p => result.AddRange(p.ValueSet));
            }
            iteration?.Element.ForEach(e =>
            {
                e.ContainedElement.ForEach(containedElement =>
                {
                    if (containedElement.ParameterOverride.Count == 0)
                    {
                        containedElement.ElementDefinition.Parameter.FindAll(p => p.ParameterType.Name.Equals(parameterTypeName)).ForEach(p => result.AddRange(p.ValueSet));
                    }
                    else
                    {
                        var associatedParameterValueSet = new List<ParameterValueSet>();
                        containedElement.ParameterOverride.FindAll(p => p.ParameterType.Name.Equals(parameterTypeName)).ForEach(p => {
                            result.AddRange(p.ValueSet);
                            p.ValueSet.ForEach(povs => associatedParameterValueSet.Add(povs.ParameterValueSet));
                        });
                        containedElement.ElementDefinition.Parameter.FindAll(p => p.ParameterType.Name.Equals(parameterTypeName)).ForEach(p => {
                            result.AddRange(p.ValueSet.FindAll(pvs => !associatedParameterValueSet.Contains(pvs)));
                        });
                    }
                });
            });
            return result.DistinctBy(p => p.Iid).ToList();
        }
    }
}
