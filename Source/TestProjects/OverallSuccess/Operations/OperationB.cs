﻿namespace OverallSuccess.Operations;

using OverallSuccessDependency;

public class OperationB : IOperation
{
    private readonly int lhs;
    private readonly int rhs;

    public OperationB(int lhs, int rhs)
    {
        this.lhs = lhs;
        this.rhs = rhs;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public int Execute()
    {
        return this.lhs - this.rhs;
    }
}