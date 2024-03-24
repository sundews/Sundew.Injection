// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncrementalValuesProviderSplitExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator;

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Text;

internal static class IncrementalValuesProviderSplitExtensions
{
    public static (IncrementalValuesProvider<TSuccess> SuccessProvider, IncrementalValuesProvider<TError> ErrorProvider) SegregateByResult<TSuccess, TError>(this IncrementalValuesProvider<R<TSuccess, TError>> resultProvider)
    {
        var successProvider = resultProvider.Where(x => x.IsSuccess).Select((x, c) => x.Value!);

        var errorsProvider = resultProvider.Where(x => !x.IsSuccess).Select((x, c) => x.Error!);

        return (successProvider, errorsProvider);
    }

    public static (IncrementalValueProvider<TSuccess> SuccessProvider, IncrementalValuesProvider<ValueList<Diagnostic>> ErrorProvider) SegregateByResult<TSuccess>(this IncrementalValueProvider<R<TSuccess, ValueList<Diagnostic>>> resultProvider)
    {
        var many = resultProvider.SelectMany((x, _) => ImmutableArray.Create(x));
        var successProvider = resultProvider.Select((x, _) =>
        {
            if (x.IsSuccess)
            {
                return x.Value;
            }

            throw new NotSupportedException(x.Error.JoinToString(','));
        });
        var errorProvider = many.Where(x => !x.IsSuccess).Select((x, c) => x.Error!);
        return (successProvider, errorProvider);
    }

    public static (IncrementalValuesProvider<TSuccess> SuccessProvider, IncrementalValuesProvider<ValueList<Diagnostic>> ErrorProvider) SegregateByResult<TSuccess>(this IncrementalValuesProvider<ValueArray<R<TSuccess, ValueList<Diagnostic>>>> resultProvider)
    {
        var many = resultProvider.SelectMany((x, _) => x);
        var successProvider = many.Where(x => x.IsSuccess).Select((x, c) => x.Value!);
        var errorsProvider = many.Where(x => !x.IsSuccess).Select((x, c) => x.Error!);
        return (successProvider, errorsProvider);
    }
}