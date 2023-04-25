// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncrementalValuesProviderSplitExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator;

using Microsoft.CodeAnalysis;
using Sundew.Base.Primitives.Computation;

internal static class IncrementalValuesProviderSplitExtensions
{
    public static (IncrementalValuesProvider<TSuccess> SuccessProvider, IncrementalValuesProvider<TError> ErrorProvider) SegregateByResult<TSuccess, TError>(this IncrementalValuesProvider<R<TSuccess, TError>> resultProvider)
    {
        var injectionDefinitionsProvider = resultProvider.Where(x => x.IsSuccess).Select((x, c) => x.Value);

        var injectionDefinitionErrorsProvider = resultProvider.Where(x => !x.IsSuccess).Select((x, c) => x.Error);

        return (injectionDefinitionsProvider, injectionDefinitionErrorsProvider);
    }
}