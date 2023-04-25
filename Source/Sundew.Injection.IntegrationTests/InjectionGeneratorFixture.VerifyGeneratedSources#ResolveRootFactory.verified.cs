//HintName: ResolveRootFactory.cs
namespace AllFeaturesSuccess
{
    public class ResolveRootFactory : global::AllFeaturesSuccess.IResolveRootFactory
    {
        private readonly global::AllFeaturesSuccess.RequiredInterface.IRequiredParameters requiredParameters;
        private readonly global::AllFeaturesSuccess.MultipleImplementations.IMultipleImplementation multipleImplementationA;
        private readonly global::AllFeaturesSuccess.MultipleImplementations.IMultipleImplementation multipleImplementationB;
        private readonly global::AllFeaturesSuccess.MultipleImplementations.IMultipleImplementation[] multipleImplementationArray;
        private readonly global::AllFeaturesSuccess.RequiredInterface.IInjectedSeparately injectedSeparatelyForInterfaceSingleInstancePerFactory;
        private readonly global::AllFeaturesSuccess.GeneratedOperationFactory generatedOperationFactory;
        private readonly global::AllFeaturesSuccess.RequiredInterface.IInjectedByType injectedByType;
        private readonly global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregationOverridableNew interfaceSegregationOverridableNewImplementation;
        private readonly global::AllFeaturesSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory interfaceSingleInstancePerFactory;
        private readonly global::AllFeaturesSuccess.RequiredInterface.IInjectedSeparately injectedSeparatelyForImplementationSingleInstancePerFactory;
        private readonly global::AllFeaturesSuccess.InterfaceImplementationBindings.IInjectableByInterface injectableByInterface;
        private readonly global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters;
        private readonly string name;
        private readonly global::AllFeaturesSuccess.SingleInstancePerFactory.ImplementationSingleInstancePerFactory implementationSingleInstancePerFactory;
        private readonly global::Sundew.Injection.Disposal.DisposingList<global::System.IDisposable> factoryConstructorDisposingList = new global::Sundew.Injection.Disposal.DisposingList<global::System.IDisposable>();
        private readonly global::Sundew.Injection.Disposal.WeakKeyDisposingDictionary<global::AllFeaturesSuccess.IResolveRoot> factoryMethodDisposingDictionary = new global::Sundew.Injection.Disposal.WeakKeyDisposingDictionary<global::AllFeaturesSuccess.IResolveRoot>();

        public ResolveRootFactory(
            global::AllFeaturesSuccess.RequiredInterface.IRequiredParameters requiredParameters, 
            global::AllFeaturesSuccess.RequiredInterface.IInjectedSeparately injectedSeparatelyForInterfaceSingleInstancePerFactory, 
            global::AllFeaturesSuccess.RequiredInterface.IInjectedByType injectedByType, 
            global::AllFeaturesSuccess.RequiredInterface.IInjectedSeparately injectedSeparatelyForImplementationSingleInstancePerFactory, 
            global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters, 
            string name)
        {
            this.requiredParameters = requiredParameters;
            this.multipleImplementationA = new global::AllFeaturesSuccess.MultipleImplementations.MultipleImplementationA(this.requiredParameters.SecondSpecificallyNamedModuleParameter);
            this.factoryConstructorDisposingList.Add(this.multipleImplementationA);
            this.multipleImplementationB = new global::AllFeaturesSuccess.MultipleImplementations.MultipleImplementationB(this.requiredParameters.FirstSpecificallyNamedModuleParameter);
            this.factoryConstructorDisposingList.Add(this.multipleImplementationB);
            this.multipleImplementationArray = new global::AllFeaturesSuccess.MultipleImplementations.IMultipleImplementation[] { this.multipleImplementationA, this.multipleImplementationB };
            this.injectedSeparatelyForInterfaceSingleInstancePerFactory = injectedSeparatelyForInterfaceSingleInstancePerFactory;
            this.generatedOperationFactory = new global::AllFeaturesSuccess.GeneratedOperationFactory();
            this.injectedByType = injectedByType;
            this.interfaceSegregationOverridableNewImplementation = this.OnCreateInterfaceSegregationOverridableNew(this.injectedByType);
            this.factoryConstructorDisposingList.Add(this.interfaceSegregationOverridableNewImplementation);
            this.interfaceSingleInstancePerFactory = new global::AllFeaturesSuccess.SingleInstancePerFactory.InterfaceSingleInstancePerFactory(
                this.injectedSeparatelyForInterfaceSingleInstancePerFactory,
                this.multipleImplementationArray,
                this.generatedOperationFactory,
                this.interfaceSegregationOverridableNewImplementation);
            this.factoryConstructorDisposingList.Add(this.interfaceSingleInstancePerFactory);
            this.injectedSeparatelyForImplementationSingleInstancePerFactory = injectedSeparatelyForImplementationSingleInstancePerFactory;
            this.optionalParameters = optionalParameters;
            if (this.optionalParameters.InjectableByInterface == null)
            {
                var ownedInjectableByInterface = new global::AllFeaturesSuccess.InterfaceImplementationBindings.InjectableByInterface(this.requiredParameters.SingleModuleRequiredConstructorMethodParameter);
                this.factoryConstructorDisposingList.Add(ownedInjectableByInterface);
                this.injectableByInterface = ownedInjectableByInterface;
            }
            else
            {
                this.injectableByInterface = this.optionalParameters.InjectableByInterface;
            }

            this.name = name;
            this.implementationSingleInstancePerFactory = new global::AllFeaturesSuccess.SingleInstancePerFactory.ImplementationSingleInstancePerFactory(
                this.injectedSeparatelyForImplementationSingleInstancePerFactory,
                this.injectableByInterface,
                this.interfaceSegregationOverridableNewImplementation,
                this.name);
        }

        public global::AllFeaturesSuccess.IResolveRoot Create(
            int[] defaultItems, 
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService, 
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null, 
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
        {
            var disposingList = new global::Sundew.Injection.Disposal.DisposingList<global::System.IDisposable>();
            var immutableList = global::AllFeaturesSuccess.DemoModuleDeclaration.CreateList<int>(defaultItems);
            if (injectableSingleInstancePerRequest == null)
            {
                var ownedInjectableSingleInstancePerRequest = new global::AllFeaturesSuccess.SingleInstancePerRequest.InjectableSingleInstancePerRequest(this.requiredParameters.SingleModuleRequiredCreateMethodParameter, immutableList);
                disposingList.Add(ownedInjectableSingleInstancePerRequest);
                injectableSingleInstancePerRequest = ownedInjectableSingleInstancePerRequest;
            }

            if (interfaceSegregation == null)
            {
                var ownedInterfaceSegregationImplementation = new global::AllFeaturesSuccess.InterfaceSegregationBindings.InterfaceSegregationImplementation(injectableSingleInstancePerRequest);
                disposingList.Add(ownedInterfaceSegregationImplementation);
                interfaceSegregation = ownedInterfaceSegregationImplementation;
            }

            var selectConstructorForIntercepted = new global::AllFeaturesSuccess.ConstructorSelection.SelectConstructor(this.implementationSingleInstancePerFactory, injectableSingleInstancePerRequest, interfaceSegregation);
            disposingList.Add(selectConstructorForIntercepted);
            var newInstanceAndDisposableForResources = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? new global::AllFeaturesSuccess.NewInstance.NewInstanceAndDisposable();
            disposingList.Add(newInstanceAndDisposableForResources);
            var newInstanceAndDisposableForIntercepted = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? new global::AllFeaturesSuccess.NewInstance.NewInstanceAndDisposable();
            disposingList.Add(newInstanceAndDisposableForIntercepted);
            var resolveRoot = new global::AllFeaturesSuccess.ResolveRoot(new global::AllFeaturesSuccess.InterfaceImplementationBindings.Intercepted(
                    global::AllFeaturesSuccess.DemoModuleDeclaration.CreateList<global::AllFeaturesSuccess.MultipleImplementations.IMultipleImplementation>(this.multipleImplementationArray),
                    global::AllFeaturesSuccess.DemoModuleDeclaration.CreateFeatureService1(
                        this.interfaceSingleInstancePerFactory,
                        injectableSingleInstancePerRequest,
                        requiredService.Invoke(),
                        interfaceSegregation),
                    selectConstructorForIntercepted,
                    new global::AllFeaturesSuccess.UnboundType.Resources(newInstanceAndDisposableForResources),
                    newInstanceAndDisposableForIntercepted,
                    this.OnCreateOverrideableNewImplementation(injectableSingleInstancePerRequest, this.injectableByInterface)), this.interfaceSingleInstancePerFactory, this.OnCreateOverrideableNewImplementation(injectableSingleInstancePerRequest, this.injectableByInterface));
            this.factoryMethodDisposingDictionary.TryAdd(resolveRoot, disposingList);
            return resolveRoot;
        }

        protected virtual global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregationOverridableNew OnCreateInterfaceSegregationOverridableNew(global::AllFeaturesSuccess.RequiredInterface.IInjectedByType injectedByType)
        {
            return new global::AllFeaturesSuccess.InterfaceSegregationBindings.InterfaceSegregationOverridableNewImplementation(injectedByType);
        }

        protected virtual global::AllFeaturesSuccess.OverridableNew.OverrideableNewImplementation OnCreateOverrideableNewImplementation(global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest, global::AllFeaturesSuccess.InterfaceImplementationBindings.IInjectableByInterface injectableByInterface)
        {
            return new global::AllFeaturesSuccess.OverridableNew.OverrideableNewImplementation(injectableSingleInstancePerRequest, injectableByInterface);
        }

        public void Dispose(global::AllFeaturesSuccess.IResolveRoot resolveRoot)
        {
            this.factoryMethodDisposingDictionary.Dispose(resolveRoot);
        }

        public void Dispose()
        {
            this.factoryConstructorDisposingList.Dispose();
            this.factoryMethodDisposingDictionary.Dispose();
        }
    }
}
