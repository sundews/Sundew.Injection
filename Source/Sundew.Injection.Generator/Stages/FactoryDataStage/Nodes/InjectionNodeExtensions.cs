// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionNodeExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage.Nodes;

using Sundew.Injection.Generator.TypeSystem;

internal static class InjectionNodeExtensions
{
    public static string GetInjectionNodeName(this InjectionNode injectionNode)
    {
        return injectionNode switch
        {
            FactoryConstructorParameterInjectionNode factoryConstructorParameterInjectionNode => factoryConstructorParameterInjectionNode.Name,
            FactoryMethodParameterInjectionNode factoryMethodParameterInjectionNode => factoryMethodParameterInjectionNode.Name,
            NewInstanceInjectionNode newInstanceInjectionNode => newInstanceInjectionNode.TargetType.GetDefiniteTypeName(),
            SingleInstancePerFactoryInjectionNode singleInstancePerFactoryInjectionNode => singleInstancePerFactoryInjectionNode.TargetType.GetDefiniteTypeName(),
            SingleInstancePerObjectInjectionNode singleInstancePerObjectInjectionNode => singleInstancePerObjectInjectionNode.TargetType.GetDefiniteTypeName(),
            SingleInstancePerRequestInjectionNode singleInstancePerRequestInjectionNode => singleInstancePerRequestInjectionNode.TargetType.GetDefiniteTypeName(),
        };
    }
}