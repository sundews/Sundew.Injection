// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryNameHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.Features.Factory.ResolveGraphStage;

using Sundew.Base.Text;

public static class FactoryNameHelper
{
    private const string I = "I";
    private const string Factory = "Factory";

    public static (string FactoryClassName, string FactoryInterfaceName, string @Namespace) GetFactoryNames(
        string? @namespace,
        string? name,
        string fallbackNamespace,
        string fallbackName)
    {
        var actualNamespace = @namespace.IsNullOrEmpty() ? fallbackNamespace : @namespace;
        var (className, interfaceName) = GetNames(name, fallbackName, Factory);
        return (className, interfaceName, actualNamespace);
    }

    private static (string ClassName, string InterfaceName) GetNames(string? name, string returnTypeName, string? generatedNamePostfix = null)
    {
        generatedNamePostfix ??= string.Empty;
        if (name.IsNullOrEmpty())
        {
            if (IsInterfaceName(returnTypeName))
            {
                return (GetTypeNameFromInterfaceName(returnTypeName) + generatedNamePostfix, returnTypeName + generatedNamePostfix);
            }

            return (returnTypeName + generatedNamePostfix, GetInterfaceTypeNameFromReturnTypeName(returnTypeName) + generatedNamePostfix);
        }

        if (IsInterfaceName(name))
        {
            return (GetTypeNameFromInterfaceName(name), name);
        }

        return (name, I + name);
    }

    private static string GetTypeNameFromInterfaceName(string interfaceTypeName)
    {
        return interfaceTypeName.Substring(1);
    }

    private static string GetInterfaceTypeNameFromReturnTypeName(string returnTypeName)
    {
        if (IsInterfaceName(returnTypeName))
        {
            return returnTypeName;
        }

        return I + returnTypeName;
    }

    private static bool IsInterfaceName(string name)
    {
        return name[0] == 'I' && char.IsUpper(name[1]);
    }
}