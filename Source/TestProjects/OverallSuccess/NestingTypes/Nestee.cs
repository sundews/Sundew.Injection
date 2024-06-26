﻿namespace OverallSuccess.NestingTypes;

using OverallSuccessDependency;

public class Nestee : IIdentifiable
{
    public Nestee()
    {
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public class Nested : IIdentifiable
    {
        public Nested()
        {
            this.Id = FactoryLifetime.Created(this);
        }

        public int Id { get; }
    }
}