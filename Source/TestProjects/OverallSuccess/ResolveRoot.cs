namespace OverallSuccess;

using System;
using System.Collections.Generic;
using OverallSuccess.ChildFactory;
using OverallSuccess.ManualFactories;
using OverallSuccess.MultipleImplementations;
using OverallSuccess.NestingTypes;
using OverallSuccess.OverridableNew;
using OverallSuccessDependency;

public class ResolveRoot : global::OverallSuccess.IResolveRoot
{
    private readonly OverrideableNewImplementation overrideableNewImplementation;
    private readonly ConstructedChild constructedChild;
    private readonly IEnumerable<IMultipleImplementationForEnumerable> multipleImplementationForEnumerables;
    private readonly NestedConsumer nestingConsumer;
    private readonly ManualFactoriesDependant manualFactoriesDependant;

    public ResolveRoot(
        global::OverallSuccess.InterfaceImplementationBindings.IIntercepted intercepted,
        global::OverallSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory interfaceSingleInstancePerFactory,
        OverrideableNewImplementation overrideableNewImplementation,
        ConstructedChild constructedChild,
        IEnumerable<IMultipleImplementationForEnumerable> multipleImplementationForEnumerables,
        NestedConsumer nestingConsumer,
        ManualFactoriesDependant manualFactoriesDependant)
    {
        this.Intercepted = intercepted;
        this.InterfaceSingleInstancePerFactory = interfaceSingleInstancePerFactory;
        this.overrideableNewImplementation = overrideableNewImplementation;
        this.constructedChild = constructedChild;
        this.multipleImplementationForEnumerables = multipleImplementationForEnumerables;
        this.nestingConsumer = nestingConsumer;
        this.manualFactoriesDependant = manualFactoriesDependant;
        this.Id = FactoryLifetime.Created(this);
    }

    public int Id { get; }

    public global::OverallSuccess.InterfaceImplementationBindings.IIntercepted Intercepted { get; }

    public global::OverallSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory InterfaceSingleInstancePerFactory { get; }

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
        this.manualFactoriesDependant.PrintMe(indent + 2);
    }
}