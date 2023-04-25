// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreationExpression.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

using System;
using System.Collections.Generic;
using CreationSource = Sundew.Injection.Generator.Stages.FactoryDataStage.CreationSource;

internal sealed record CreationExpression
    (CreationSource CreationSource, IReadOnlyList<Expression> Arguments) : InvocationExpressionBase(Arguments)
{
    public CreationExpression(CreationSource creationSource)
        : this(creationSource, Array.Empty<Expression>())
    {
    }
}