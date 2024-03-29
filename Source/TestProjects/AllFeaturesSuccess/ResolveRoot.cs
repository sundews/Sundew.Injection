namespace AllFeaturesSuccess;

using System;
using System.Collections.Generic;
using AllFeaturesSuccess.ChildFactory;
using AllFeaturesSuccess.MultipleImplementations;
using AllFeaturesSuccess.OverridableNew;

public class ResolveRoot : global::AllFeaturesSuccess.IResolveRoot
{
    private readonly OverrideableNewImplementation overrideableNewImplementation;
    private readonly ConstructedChild constructedChild;
    private readonly IEnumerable<IMultipleImplementationForEnumerable> multipleImplementationForEnumerables;

    public ResolveRoot(
        global::AllFeaturesSuccess.InterfaceImplementationBindings.IIntercepted intercepted,
        global::AllFeaturesSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory interfaceSingleInstancePerFactory,
        OverrideableNewImplementation overrideableNewImplementation,
        ConstructedChild constructedChild,
        IEnumerable<IMultipleImplementationForEnumerable> multipleImplementationForEnumerables)
    {
        this.overrideableNewImplementation = overrideableNewImplementation;
        this.constructedChild = constructedChild;
        this.multipleImplementationForEnumerables = multipleImplementationForEnumerables;
        this.Intercepted = intercepted;
        this.InterfaceSingleInstancePerFactory = interfaceSingleInstancePerFactory;
    }

    public global::AllFeaturesSuccess.InterfaceImplementationBindings.IIntercepted Intercepted { get; }

    public global::AllFeaturesSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory InterfaceSingleInstancePerFactory { get; }

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
    }
}