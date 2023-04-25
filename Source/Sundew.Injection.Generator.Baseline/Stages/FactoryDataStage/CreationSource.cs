// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreationSource.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.FactoryDataStage;

using Sundew.Injection.Generator.TypeSystem;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record CreationSource
{
    public static CreationSource From(DefiniteMethod registrationMethod)
    {
        if (registrationMethod.IsConstructor)
        {
            return ConstructorCall(registrationMethod.ContainingType);
        }

        return StaticMethodCall(registrationMethod.ContainingType, registrationMethod);
    }
}

internal sealed record ArrayCreation(DefiniteType ElementType) : CreationSource;

internal sealed record ConstructorCall(DefiniteType Type) : CreationSource;

internal sealed record StaticMethodCall(DefiniteType Type, DefiniteMethod Method) : CreationSource;