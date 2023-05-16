// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratedOutput.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Injection.Generator.Stages.CodeGenerationStage;

public class GeneratedOutput
{
    public GeneratedOutput(string fileName, string source)
    {
        this.FileName = fileName;
        this.Source = source;
    }

    public string FileName { get; }

    public string Source { get; }
}