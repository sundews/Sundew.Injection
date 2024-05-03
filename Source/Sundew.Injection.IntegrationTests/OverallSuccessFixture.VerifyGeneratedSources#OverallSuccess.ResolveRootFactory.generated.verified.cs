//HintName: OverallSuccess.ResolveRootFactory.generated.cs
#nullable enable
namespace OverallSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class ResolveRootFactory : global::OverallSuccess.IResolveRootFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::OverallSuccess.SundewInjection.LifecycleHandler lifecycleHandler;
        private readonly global::OverallSuccess.RequiredInterface.IRequiredParameters requiredParameters;
        private readonly global::OverallSuccess.MultipleImplementations.IMultipleImplementationForArray multipleImplementationForArrayA;
        private readonly global::OverallSuccess.MultipleImplementations.IMultipleImplementationForArray multipleImplementationForArrayB;
        private readonly global::OverallSuccess.MultipleImplementations.IMultipleImplementationForArray[] multipleImplementationForArrayArray;
        private readonly global::OverallSuccess.RequiredInterface.IInjectedSeparately injectedSeparatelyForInterfaceSingleInstancePerFactory;
        private readonly global::OverallSuccess.IGeneratedOperationFactory generatedOperationFactory;
        private readonly global::OverallSuccess.RequiredInterface.IInjectedByType injectedByType;
        private readonly global::OverallSuccess.InterfaceSegregationBindings.IInterfaceSegregationOverridableNew interfaceSegregationOverridableNewImplementation;
        private readonly global::OverallSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory interfaceSingleInstancePerFactory;
        private readonly global::OverallSuccess.RequiredInterface.IInjectedSeparately injectedSeparatelyForImplementationSingleInstancePerFactory;
        private readonly global::OverallSuccess.InterfaceImplementationBindings.IInjectableByInterface injectableByInterface;
        private readonly global::OverallSuccess.OptionalInterface.OptionalParameters optionalParameters;
        private readonly string name;
        private readonly global::OverallSuccess.SingleInstancePerFactory.ImplementationSingleInstancePerFactory implementationSingleInstancePerFactory;
        private readonly global::OverallSuccessDependency.DependencyFactory dependencyFactory;
        private readonly global::OverallSuccessDependency.ManualMultipleDependencyFactory manualMultipleDependencyFactory;
        private readonly global::OverallSuccessDependency.ManualSingletonDependencyFactory manualSingletonDependencyFactory;
        private readonly global::OverallSuccessDependency.ManualDependencyFactory manualDependencyFactory;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ResolveRootFactory(
            global::OverallSuccess.RequiredInterface.IRequiredParameters requiredParameters,
            global::OverallSuccess.RequiredInterface.IInjectedSeparately injectedSeparatelyForInterfaceSingleInstancePerFactory,
            global::OverallSuccess.RequiredInterface.IInjectedByType injectedByType,
            global::OverallSuccess.RequiredInterface.IInjectedSeparately injectedSeparatelyForImplementationSingleInstancePerFactory,
            global::OverallSuccess.OptionalInterface.OptionalParameters optionalParameters,
            string name,
            global::Sundew.Injection.ILifecycleParameters? lifecycleParameters = null)
        {
            this.lifecycleParameters = lifecycleParameters ?? new global::Sundew.Injection.LifecycleParameters(
                false,
                false,
                default(global::Initialization.Interfaces.IInitializationReporter),
                default(global::Disposal.Interfaces.IDisposalReporter));
            this.lifecycleHandler = new global::OverallSuccess.SundewInjection.LifecycleHandler(this.lifecycleParameters, this.lifecycleParameters);
            this.requiredParameters = requiredParameters;
            this.multipleImplementationForArrayA = new global::OverallSuccess.MultipleImplementations.MultipleImplementationForArrayA(this.requiredParameters.SecondSpecificallyNamedModuleParameter);
            this.lifecycleHandler.TryAdd(this.multipleImplementationForArrayA);
            this.multipleImplementationForArrayB = new global::OverallSuccess.MultipleImplementations.MultipleImplementationForArrayB(this.requiredParameters.FirstSpecificallyNamedModuleParameter);
            this.lifecycleHandler.TryAdd(this.multipleImplementationForArrayB);
            this.multipleImplementationForArrayArray = new global::OverallSuccess.MultipleImplementations.IMultipleImplementationForArray[] { this.multipleImplementationForArrayA, this.multipleImplementationForArrayB };
            this.injectedSeparatelyForInterfaceSingleInstancePerFactory = injectedSeparatelyForInterfaceSingleInstancePerFactory;
            this.generatedOperationFactory = new global::OverallSuccess.GeneratedOperationFactory();
            this.injectedByType = injectedByType;
            this.interfaceSegregationOverridableNewImplementation = this.OnCreateInterfaceSegregationOverridableNew(this.injectedByType);
            this.lifecycleHandler.TryAdd(this.interfaceSegregationOverridableNewImplementation);
            this.interfaceSingleInstancePerFactory = new global::OverallSuccess.SingleInstancePerFactory.InterfaceSingleInstancePerFactory(
                this.injectedSeparatelyForInterfaceSingleInstancePerFactory,
                this.multipleImplementationForArrayArray,
                this.generatedOperationFactory,
                this.interfaceSegregationOverridableNewImplementation);
            this.lifecycleHandler.TryAdd(this.interfaceSingleInstancePerFactory);
            this.injectedSeparatelyForImplementationSingleInstancePerFactory = injectedSeparatelyForImplementationSingleInstancePerFactory;
            this.optionalParameters = optionalParameters;
            this.injectableByInterface = this.optionalParameters.InjectableByInterface ?? this.lifecycleHandler.TryAdd(new global::OverallSuccess.InterfaceImplementationBindings.InjectableByInterface(this.requiredParameters.SingleModuleRequiredConstructorMethodParameter));
            this.name = name;
            this.implementationSingleInstancePerFactory = new global::OverallSuccess.SingleInstancePerFactory.ImplementationSingleInstancePerFactory(
                this.injectedSeparatelyForImplementationSingleInstancePerFactory,
                this.injectableByInterface,
                this.interfaceSegregationOverridableNewImplementation,
                this.name);
            this.dependencyFactory = new global::OverallSuccessDependency.DependencyFactory();
            this.lifecycleHandler.TryAdd(this.dependencyFactory);
            this.manualMultipleDependencyFactory = new global::OverallSuccessDependency.ManualMultipleDependencyFactory();
            this.manualSingletonDependencyFactory = new global::OverallSuccessDependency.ManualSingletonDependencyFactory();
            this.lifecycleHandler.TryAdd(this.manualSingletonDependencyFactory);
            this.manualDependencyFactory = new global::OverallSuccessDependency.ManualDependencyFactory();
        }

        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public global::OverallSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory InterfaceSingleInstancePerFactory
        {
            get
            {
                this.lifecycleHandler.Initialize();
                return this.interfaceSingleInstancePerFactory;
            }
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::OverallSuccess.IResolveRoot CreateResolveRoot(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::OverallSuccess.RequiredInterface.IRequiredService> requiredService,
            global::OverallSuccess.RequiredInterface.RequiredParameter requiredParameter,
            global::OverallSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::OverallSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
        {
            var constructedResolveRoot = this.CreateResolveRootUninitialized(
                integers,
                defaultItem,
                requiredService,
                requiredParameter,
                injectableSingleInstancePerRequest,
                interfaceSegregation);
            this.lifecycleHandler.Initialize();
            return constructedResolveRoot.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public async global::System.Threading.Tasks.Task<global::OverallSuccess.IResolveRoot> CreateResolveRootAsync(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::OverallSuccess.RequiredInterface.IRequiredService> requiredService,
            global::OverallSuccess.RequiredInterface.RequiredParameter requiredParameter,
            global::OverallSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::OverallSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
        {
            var constructedResolveRoot = this.CreateResolveRootUninitialized(
                integers,
                defaultItem,
                requiredService,
                requiredParameter,
                injectableSingleInstancePerRequest,
                interfaceSegregation);
            await this.lifecycleHandler.InitializeAsync().ConfigureAwait(false);
            return constructedResolveRoot.Object;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        [global::Sundew.Injection.IndirectFactoryTargetAttribute]
        public global::Sundew.Injection.Constructed<global::OverallSuccess.IResolveRoot> CreateResolveRootUninitialized(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::OverallSuccess.RequiredInterface.IRequiredService> requiredService,
            global::OverallSuccess.RequiredInterface.RequiredParameter requiredParameter,
            global::OverallSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::OverallSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var generic = global::OverallSuccess.InjectionDeclaration.CreateGeneric<int>(defaultItem);
            injectableSingleInstancePerRequest ??= childLifecycleHandler.TryAdd(new global::OverallSuccess.SingleInstancePerRequest.InjectableSingleInstancePerRequest(this.requiredParameters.SingleModuleRequiredCreateMethodParameter, integers, generic));
            interfaceSegregation ??= childLifecycleHandler.TryAdd(new global::OverallSuccess.InterfaceSegregationBindings.InterfaceSegregationImplementation(injectableSingleInstancePerRequest));
            var selectConstructorForIntercepted = new global::OverallSuccess.ConstructorSelection.SelectConstructor(this.implementationSingleInstancePerFactory, injectableSingleInstancePerRequest, interfaceSegregation);
            childLifecycleHandler.TryAdd(selectConstructorForIntercepted);
            var newInstanceAndDisposableForResources = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? childLifecycleHandler.TryAdd(new global::OverallSuccess.NewInstance.NewInstanceAndDisposable(this, default(global::OverallSuccess.OptionalInterface.IOmittedOptional)));
            var newInstanceAndDisposableForIntercepted = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? childLifecycleHandler.TryAdd(new global::OverallSuccess.NewInstance.NewInstanceAndDisposable(this, default(global::OverallSuccess.OptionalInterface.IOmittedOptional)));
            var overrideableNewImplementationForIntercepted = this.OnCreateOverrideableNewImplementation(injectableSingleInstancePerRequest, this.injectableByInterface, requiredParameter);
            childLifecycleHandler.TryAdd(overrideableNewImplementationForIntercepted);
            var overrideableNewImplementationForResolveRoot = this.OnCreateOverrideableNewImplementation(injectableSingleInstancePerRequest, this.injectableByInterface, requiredParameter);
            childLifecycleHandler.TryAdd(overrideableNewImplementationForResolveRoot);
            var newInstanceAndDisposableForConstructedChild = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? childLifecycleHandler.TryAdd(new global::OverallSuccess.NewInstance.NewInstanceAndDisposable(this, default(global::OverallSuccess.OptionalInterface.IOmittedOptional)));
            var constructedChildForResolveRoot = new global::OverallSuccess.ChildFactory.ConstructedChild(newInstanceAndDisposableForConstructedChild, this.dependencyFactory.CreateUninitialized().Object, this.manualMultipleDependencyFactory.CreateNewInstance());
            childLifecycleHandler.TryAdd(constructedChildForResolveRoot);

            static global::System.Collections.Generic.IEnumerable<global::OverallSuccess.MultipleImplementations.IMultipleImplementationForEnumerable> CreateMultipleImplementationForEnumerable()
            {
                yield return new global::OverallSuccess.MultipleImplementations.MultipleImplementationForEnumerableA();
                yield return new global::OverallSuccess.MultipleImplementations.MultipleImplementationForEnumerableB();
            }

            var resolveRootResult = new global::OverallSuccess.ResolveRoot(
                new global::OverallSuccess.InterfaceImplementationBindings.Intercepted(
                    this.multipleImplementationForArrayArray,
                    global::OverallSuccess.InjectionDeclaration.CreateFeatureService1(
                        this.interfaceSingleInstancePerFactory,
                        injectableSingleInstancePerRequest,
                        requiredService.Invoke(),
                        interfaceSegregation),
                    selectConstructorForIntercepted,
                    new global::OverallSuccess.UnboundType.Resources(newInstanceAndDisposableForResources),
                    newInstanceAndDisposableForIntercepted,
                    overrideableNewImplementationForIntercepted),
                this.interfaceSingleInstancePerFactory,
                overrideableNewImplementationForResolveRoot,
                constructedChildForResolveRoot,
                CreateMultipleImplementationForEnumerable(),
                new global::OverallSuccess.NestingTypes.NestedConsumer(new global::OverallSuccess.NestingTypes.Nestee.Nested()),
                new global::OverallSuccess.ManualFactories.ManualFactoriesDependant(this.manualSingletonDependencyFactory.ManualSingletonDependency, this.manualDependencyFactory.CreateNewInstance()));
            this.lifecycleHandler.TryAdd(resolveRootResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::OverallSuccess.IResolveRoot>(resolveRootResult, childLifecycleHandler);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public global::OverallSuccess.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverC()
        {
            return new global::OverallSuccess.TypeResolver.MultipleImplementationForTypeResolverC();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(global::OverallSuccess.IResolveRoot resolveRoot)
        {
            this.lifecycleHandler.Dispose(resolveRoot);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::OverallSuccess.IResolveRoot resolveRoot)
        {
            return this.lifecycleHandler.DisposeAsync(resolveRoot);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose()
        {
            this.lifecycleHandler.Dispose();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync()
        {
            return this.lifecycleHandler.DisposeAsync();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        protected virtual global::OverallSuccess.InterfaceSegregationBindings.IInterfaceSegregationOverridableNew OnCreateInterfaceSegregationOverridableNew(global::OverallSuccess.RequiredInterface.IInjectedByType injectedByType)
        {
            return new global::OverallSuccess.InterfaceSegregationBindings.InterfaceSegregationOverridableNewImplementation(injectedByType);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        protected virtual global::OverallSuccess.OverridableNew.OverrideableNewImplementation OnCreateOverrideableNewImplementation(global::OverallSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest, global::OverallSuccess.InterfaceImplementationBindings.IInjectableByInterface injectableByInterface, global::OverallSuccess.RequiredInterface.RequiredParameter requiredParameter)
        {
            return new global::OverallSuccess.OverridableNew.OverrideableNewImplementation(injectableSingleInstancePerRequest, injectableByInterface, requiredParameter);
        }
    }
}
