namespace AllFeaturesSuccess
{
    using System;
    using AllFeaturesSuccess.ChildFactory;
    using AllFeaturesSuccess.OverridableNew;

    public class ResolveRoot : global::AllFeaturesSuccess.IResolveRoot
    {
        private readonly OverrideableNewImplementation overrideableNewImplementation;
        private readonly ConstructedChild constructedChild;

        public ResolveRoot(global::AllFeaturesSuccess.InterfaceImplementationBindings.IIntercepted intercepted, global::AllFeaturesSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory interfaceSingleInstancePerFactory, OverrideableNewImplementation overrideableNewImplementation, ConstructedChild constructedChild)
        {
            this.overrideableNewImplementation = overrideableNewImplementation;
            this.constructedChild = constructedChild;
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
        }
    }
}