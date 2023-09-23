//HintName: AllFeaturesSuccess.ResolveRootFactory.generated.cs
#nullable enable
namespace AllFeaturesSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ResolveRootFactory : global::AllFeaturesSuccess.IResolveRootFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::Sundew.Injection.LifecycleHandler lifecycleHandler;
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
        private readonly global::AllFeaturesSuccessDependency.DependencyFactory dependencyFactory;
        private readonly global::AllFeaturesSuccessDependency.ManualDependencyFactory manualDependencyFactory;

        public ResolveRootFactory(
            global::AllFeaturesSuccess.RequiredInterface.IRequiredParameters requiredParameters,
            global::AllFeaturesSuccess.RequiredInterface.IInjectedSeparately injectedSeparatelyForInterfaceSingleInstancePerFactory,
            global::AllFeaturesSuccess.RequiredInterface.IInjectedByType injectedByType,
            global::AllFeaturesSuccess.RequiredInterface.IInjectedSeparately injectedSeparatelyForImplementationSingleInstancePerFactory,
            global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters,
            string name,
            global::Sundew.Injection.ILifecycleParameters? lifecycleParameters = null)
        {
            if (lifecycleParameters == null)
            {
                var ownedLifecycleParameters = new global::Sundew.Injection.LifecycleParameters(
                    false,
                    false,
                    default(global::Initialization.Interfaces.IInitializationReporter),
                    default(global::Disposal.Interfaces.IDisposalReporter));
                this.lifecycleParameters = ownedLifecycleParameters;
            }
            else
            {
                this.lifecycleParameters = lifecycleParameters;
            }

            this.lifecycleHandler = new global::Sundew.Injection.LifecycleHandler(this.lifecycleParameters, this.lifecycleParameters);
            this.requiredParameters = requiredParameters;
            this.multipleImplementationA = new global::AllFeaturesSuccess.MultipleImplementations.MultipleImplementationA(this.requiredParameters.SecondSpecificallyNamedModuleParameter);
            this.lifecycleHandler.TryAdd(this.multipleImplementationA);
            this.multipleImplementationB = new global::AllFeaturesSuccess.MultipleImplementations.MultipleImplementationB(this.requiredParameters.FirstSpecificallyNamedModuleParameter);
            this.lifecycleHandler.TryAdd(this.multipleImplementationB);
            this.multipleImplementationArray = new global::AllFeaturesSuccess.MultipleImplementations.IMultipleImplementation[] { this.multipleImplementationA, this.multipleImplementationB };
            this.injectedSeparatelyForInterfaceSingleInstancePerFactory = injectedSeparatelyForInterfaceSingleInstancePerFactory;
            this.generatedOperationFactory = new global::AllFeaturesSuccess.GeneratedOperationFactory();
            this.injectedByType = injectedByType;
            this.interfaceSegregationOverridableNewImplementation = this.OnCreateInterfaceSegregationOverridableNew(this.injectedByType);
            this.lifecycleHandler.TryAdd(this.interfaceSegregationOverridableNewImplementation);
            this.interfaceSingleInstancePerFactory = new global::AllFeaturesSuccess.SingleInstancePerFactory.InterfaceSingleInstancePerFactory(
                this.injectedSeparatelyForInterfaceSingleInstancePerFactory,
                this.multipleImplementationArray,
                this.generatedOperationFactory,
                this.interfaceSegregationOverridableNewImplementation);
            this.lifecycleHandler.TryAdd(this.interfaceSingleInstancePerFactory);
            this.injectedSeparatelyForImplementationSingleInstancePerFactory = injectedSeparatelyForImplementationSingleInstancePerFactory;
            this.optionalParameters = optionalParameters;
            if (this.optionalParameters.InjectableByInterface == null)
            {
                var ownedInjectableByInterface = new global::AllFeaturesSuccess.InterfaceImplementationBindings.InjectableByInterface(this.requiredParameters.SingleModuleRequiredConstructorMethodParameter);
                this.lifecycleHandler.TryAdd(ownedInjectableByInterface);
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
            this.dependencyFactory = new global::AllFeaturesSuccessDependency.DependencyFactory();
            this.lifecycleHandler.TryAdd(this.dependencyFactory);
            this.manualDependencyFactory = new global::AllFeaturesSuccessDependency.ManualDependencyFactory();
        }

        public global::AllFeaturesSuccess.IResolveRoot CreateResolveRoot(
            int[] defaultItems,
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService,
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
        {
            var constructedResolveRoot = this.CreateResolveRootUninitialized(
                defaultItems,
                requiredService,
                injectableSingleInstancePerRequest,
                interfaceSegregation);
            this.lifecycleHandler.Initialize();
            return constructedResolveRoot.Object;
        }

        public async global::System.Threading.Tasks.Task<global::AllFeaturesSuccess.IResolveRoot> CreateResolveRootAsync(
            int[] defaultItems,
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService,
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
        {
            var constructedResolveRoot = this.CreateResolveRootUninitialized(
                defaultItems,
                requiredService,
                injectableSingleInstancePerRequest,
                interfaceSegregation);
            await this.lifecycleHandler.InitializeAsync().ConfigureAwait(false);
            return constructedResolveRoot.Object;
        }

        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.CreateMethod]
        public global::Sundew.Injection.Constructed<global::AllFeaturesSuccess.IResolveRoot> CreateResolveRootUninitialized(
            int[] defaultItems,
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService,
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var immutableList = global::AllFeaturesSuccess.FactoryDeclaration.CreateList<int>(defaultItems);
            if (injectableSingleInstancePerRequest == null)
            {
                var ownedInjectableSingleInstancePerRequest = new global::AllFeaturesSuccess.SingleInstancePerRequest.InjectableSingleInstancePerRequest(this.requiredParameters.SingleModuleRequiredCreateMethodParameter, immutableList);
                childLifecycleHandler.TryAdd(ownedInjectableSingleInstancePerRequest);
                injectableSingleInstancePerRequest = ownedInjectableSingleInstancePerRequest;
            }

            if (interfaceSegregation == null)
            {
                var ownedInterfaceSegregationImplementation = new global::AllFeaturesSuccess.InterfaceSegregationBindings.InterfaceSegregationImplementation(injectableSingleInstancePerRequest);
                childLifecycleHandler.TryAdd(ownedInterfaceSegregationImplementation);
                interfaceSegregation = ownedInterfaceSegregationImplementation;
            }

            var selectConstructorForIntercepted = new global::AllFeaturesSuccess.ConstructorSelection.SelectConstructor(this.implementationSingleInstancePerFactory, injectableSingleInstancePerRequest, interfaceSegregation);
            childLifecycleHandler.TryAdd(selectConstructorForIntercepted);
            var newInstanceAndDisposableForResources = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? new global::AllFeaturesSuccess.NewInstance.NewInstanceAndDisposable(default(global::AllFeaturesSuccess.OptionalInterface.IOmittedOptional));
            childLifecycleHandler.TryAdd(newInstanceAndDisposableForResources);
            var newInstanceAndDisposableForIntercepted = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? new global::AllFeaturesSuccess.NewInstance.NewInstanceAndDisposable(default(global::AllFeaturesSuccess.OptionalInterface.IOmittedOptional));
            childLifecycleHandler.TryAdd(newInstanceAndDisposableForIntercepted);
            var overrideableNewImplementationForIntercepted = this.OnCreateOverrideableNewImplementation(injectableSingleInstancePerRequest, this.injectableByInterface);
            childLifecycleHandler.TryAdd(overrideableNewImplementationForIntercepted);
            var overrideableNewImplementationForResolveRoot = this.OnCreateOverrideableNewImplementation(injectableSingleInstancePerRequest, this.injectableByInterface);
            childLifecycleHandler.TryAdd(overrideableNewImplementationForResolveRoot);
            var newInstanceAndDisposableForConstructedChild = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? new global::AllFeaturesSuccess.NewInstance.NewInstanceAndDisposable(default(global::AllFeaturesSuccess.OptionalInterface.IOmittedOptional));
            childLifecycleHandler.TryAdd(newInstanceAndDisposableForConstructedChild);
            var constructedDependencyForConstructedChild = this.dependencyFactory.CreateUninitialized();
            childLifecycleHandler.TryAdd(constructedDependencyForConstructedChild);
            var dependencyForConstructedChild = constructedDependencyForConstructedChild.Object;
            childLifecycleHandler.TryAdd(dependencyForConstructedChild);
            var manualDependencyForConstructedChild = this.manualDependencyFactory.Create();
            childLifecycleHandler.TryAdd(manualDependencyForConstructedChild);
            var constructedChildForResolveRoot = new global::AllFeaturesSuccess.ChildFactory.ConstructedChild(newInstanceAndDisposableForConstructedChild, dependencyForConstructedChild, manualDependencyForConstructedChild);
            childLifecycleHandler.TryAdd(constructedChildForResolveRoot);
            var resolveRootResult = new global::AllFeaturesSuccess.ResolveRoot(
                new global::AllFeaturesSuccess.InterfaceImplementationBindings.Intercepted(
                    global::AllFeaturesSuccess.FactoryDeclaration.CreateList<global::AllFeaturesSuccess.MultipleImplementations.IMultipleImplementation>(this.multipleImplementationArray),
                    global::AllFeaturesSuccess.FactoryDeclaration.CreateFeatureService1(
                        this.interfaceSingleInstancePerFactory,
                        injectableSingleInstancePerRequest,
                        requiredService.Invoke(),
                        interfaceSegregation),
                    selectConstructorForIntercepted,
                    new global::AllFeaturesSuccess.UnboundType.Resources(newInstanceAndDisposableForResources),
                    newInstanceAndDisposableForIntercepted,
                    overrideableNewImplementationForIntercepted),
                this.interfaceSingleInstancePerFactory,
                overrideableNewImplementationForResolveRoot,
                constructedChildForResolveRoot);
            this.lifecycleHandler.TryAdd(resolveRootResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::AllFeaturesSuccess.IResolveRoot>(resolveRootResult, childLifecycleHandler);
        }

        [global::Sundew.Injection.CreateMethod]
        public global::AllFeaturesSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory CreateInterfaceSingleInstancePerFactory()
        {
            return this.interfaceSingleInstancePerFactory;
        }

        public void Dispose(global::AllFeaturesSuccess.IResolveRoot resolveRoot)
        {
            this.lifecycleHandler.Dispose(resolveRoot);
        }

        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::AllFeaturesSuccess.IResolveRoot resolveRoot)
        {
            return this.lifecycleHandler.DisposeAsync(resolveRoot);
        }

        public void Dispose()
        {
            this.lifecycleHandler.Dispose();
        }

        public global::System.Threading.Tasks.ValueTask DisposeAsync()
        {
            return this.lifecycleHandler.DisposeAsync();
        }

        protected virtual global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregationOverridableNew OnCreateInterfaceSegregationOverridableNew(global::AllFeaturesSuccess.RequiredInterface.IInjectedByType injectedByType)
        {
            return new global::AllFeaturesSuccess.InterfaceSegregationBindings.InterfaceSegregationOverridableNewImplementation(injectedByType);
        }

        protected virtual global::AllFeaturesSuccess.OverridableNew.OverrideableNewImplementation OnCreateOverrideableNewImplementation(global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest, global::AllFeaturesSuccess.InterfaceImplementationBindings.IInjectableByInterface injectableByInterface)
        {
            return new global::AllFeaturesSuccess.OverridableNew.OverrideableNewImplementation(injectableSingleInstancePerRequest, injectableByInterface);
        }
    }
}
