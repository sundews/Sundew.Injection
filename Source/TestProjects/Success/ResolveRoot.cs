namespace Success;

using System;
using System.Collections.Generic;
using Success.ChildFactory;
using Success.MultipleImplementations;
using Success.NestingTypes;
using Success.OverridableNew;
using SuccessDependency;

public class ResolveRoot : global::Success.IResolveRoot
{
    private readonly OverrideableNewImplementation overrideableNewImplementation;
    private readonly ConstructedChild constructedChild;
    private readonly IEnumerable<IMultipleImplementationForEnumerable> multipleImplementationForEnumerables;
    private readonly NestedConsumer nestingConsumer;

    public ResolveRoot(
        global::Success.InterfaceImplementationBindings.IIntercepted intercepted,
        global::Success.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory interfaceSingleInstancePerFactory,
        OverrideableNewImplementation overrideableNewImplementation,
        ConstructedChild constructedChild,
        IEnumerable<IMultipleImplementationForEnumerable> multipleImplementationForEnumerables,
        NestedConsumer nestingConsumer)
    {
        this.Intercepted = intercepted;
        this.InterfaceSingleInstancePerFactory = interfaceSingleInstancePerFactory;
        this.overrideableNewImplementation = overrideableNewImplementation;
        this.constructedChild = constructedChild;
        this.multipleImplementationForEnumerables = multipleImplementationForEnumerables;
        this.nestingConsumer = nestingConsumer;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public global::Success.InterfaceImplementationBindings.IIntercepted Intercepted { get; }

    public global::Success.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory InterfaceSingleInstancePerFactory { get; }

    public void PrintMe(int indent)
    {
        Console.WriteLine(new string(' ', indent) + this.GetType().Name);
        this.Intercepted.PrintMe(indent + 2);
        this.InterfaceSingleInstancePerFactory.PrintMe(indent + 2);
        this.overrideableNewImplementation.PrintMe(indent + 2);
        this.constructedChild.PrintMe(indent + 2);
        foreach (var multipleImplementationForEnumerable in this.multipleImplementationForEnumerables)
        {
            multipleImplementationForEnumerable.PrintMe(indent + 2);
        }

        this.nestingConsumer.PrintMe(indent + 2);
    }
}