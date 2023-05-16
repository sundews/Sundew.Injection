﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvocationExpressionBase.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

using System.Collections.Generic;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal abstract partial record InvocationExpressionBase(IReadOnlyList<Expression> Arguments) : Expression;