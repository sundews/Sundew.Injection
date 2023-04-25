namespace AllFeaturesSuccess
{
    using System;
    using AllFeaturesSuccess.OverridableNew;

    public class ResolveRoot : global::AllFeaturesSuccess.IResolveRoot
    {
        private readonly OverrideableNewImplementation overrideableNewImplementation;

        public ResolveRoot(global::AllFeaturesSuccess.InterfaceImplementationBindings.IIntercepted intercepted, global::AllFeaturesSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory interfaceSingleInstancePerFactory, OverrideableNewImplementation overrideableNewImplementation)
        {
            this.overrideableNewImplementation = overrideableNewImplementation;
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
        }
    }
}