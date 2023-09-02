namespace AllFeaturesSuccess.ConstructorSelection
{
    using System;
    using System.Threading.Tasks;
    using AllFeaturesSuccess.InterfaceSegregationBindings;
    using AllFeaturesSuccess.SingleInstancePerFactory;
    using AllFeaturesSuccess.SingleInstancePerRequest;

    public class SelectConstructor : ISelectConstructor
    {
        private readonly ImplementationSingleInstancePerFactory implementationSingleInstancePerFactory;
        private readonly IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest;
        private readonly IInterfaceSegregationA interfaceSegregationA;

        public SelectConstructor(ImplementationSingleInstancePerFactory implementationSingleInstancePerFactory, IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest, IInterfaceSegregationA interfaceSegregationA)
        {
            this.implementationSingleInstancePerFactory = implementationSingleInstancePerFactory;
            this.injectableSingleInstancePerRequest = injectableSingleInstancePerRequest;
            this.interfaceSegregationA = interfaceSegregationA;
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        public void PrintMe(int indent)
        {
            Console.WriteLine(new string(' ', indent) + this.GetType().Name);
            this.implementationSingleInstancePerFactory.PrintMe(indent + 2);
            this.injectableSingleInstancePerRequest.PrintMe(indent + 2);
            this.interfaceSegregationA.PrintMe(indent + 2);
        }
    }
}