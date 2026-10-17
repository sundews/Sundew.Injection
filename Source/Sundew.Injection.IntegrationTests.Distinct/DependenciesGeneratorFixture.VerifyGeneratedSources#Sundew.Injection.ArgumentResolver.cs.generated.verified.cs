//HintName: Sundew.Injection.ArgumentResolver.cs.generated.cs
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace DistinctSuccess.SundewInjection
{
    using System;

    internal readonly struct ArgumentResolver
    {
        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ArgumentResolver(Type[] parameterTypes, ResolveWithArguments resolve)
        {
            this.ParameterTypes = parameterTypes;
            this.Resolve = resolve;
        }

        public Type[] ParameterTypes { get; }

        public ResolveWithArguments Resolve { get; }

        public bool Accepts(ReadOnlySpan<object?> arguments, bool exactly)
        {
            if (arguments.Length != this.ParameterTypes.Length)
            {
                return false;
            }

            for (var i = 0; i < arguments.Length; i++)
            {
                if (!Accepts(this.ParameterTypes[i], arguments[i], exactly))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool Accepts(Type parameterType, object? argument, bool exactly)
        {
            if (argument == null)
            {
                return !parameterType.IsValueType || Nullable.GetUnderlyingType(parameterType) != null;
            }

            var argumentType = argument.GetType();
            if (argumentType == parameterType || Nullable.GetUnderlyingType(parameterType) == argumentType)
            {
                return true;
            }

            return !exactly && parameterType.IsAssignableFrom(argumentType);
        }
    }
}
