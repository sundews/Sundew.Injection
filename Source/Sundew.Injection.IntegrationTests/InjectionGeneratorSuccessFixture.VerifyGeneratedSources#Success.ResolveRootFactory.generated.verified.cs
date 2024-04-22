//HintName: Success.ResolveRootFactory.generated.cs
#nullable enable
namespace Success
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class ResolveRootFactory : global::Success.IResolveRootFactory
    {
        private readonly global::Sundew.Injection.ILifecycleParameters lifecycleParameters;
        private readonly global::Sundew.Injection.LifecycleHandler lifecycleHandler;
        private readonly global::Success.RequiredInterface.IRequiredParameters requiredParameters;
        private readonly global::Success.MultipleImplementations.IMultipleImplementationForArray multipleImplementationForArrayA;
        private readonly global::Success.MultipleImplementations.IMultipleImplementationForArray multipleImplementationForArrayB;
        private readonly global::Success.MultipleImplementations.IMultipleImplementationForArray[] multipleImplementationForArrayArray;
        private readonly global::Success.RequiredInterface.IInjectedSeparately injectedSeparatelyForInterfaceSingleInstancePerFactory;
        private readonly global::Success.IGeneratedOperationFactory generatedOperationFactory;
        private readonly global::Success.RequiredInterface.IInjectedByType injectedByType;
        private readonly global::Success.InterfaceSegregationBindings.IInterfaceSegregationOverridableNew interfaceSegregationOverridableNewImplementation;
        private readonly global::Success.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory interfaceSingleInstancePerFactory;
        private readonly global::Success.RequiredInterface.IInjectedSeparately injectedSeparatelyForImplementationSingleInstancePerFactory;
        private readonly global::Success.InterfaceImplementationBindings.IInjectableByInterface injectableByInterface;
        private readonly global::Success.OptionalInterface.OptionalParameters optionalParameters;
        private readonly string name;
        private readonly global::Success.SingleInstancePerFactory.ImplementationSingleInstancePerFactory implementationSingleInstancePerFactory;
        private readonly global::SuccessDependency.DependencyFactory dependencyFactory;
        private readonly global::SuccessDependency.ManualDependencyFactory manualDependencyFactory;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ResolveRootFactory(
            global::Success.RequiredInterface.IRequiredParameters requiredParameters,
            global::Success.RequiredInterface.IInjectedSeparately injectedSeparatelyForInterfaceSingleInstancePerFactory,
            global::Success.RequiredInterface.IInjectedByType injectedByType,
            global::Success.RequiredInterface.IInjectedSeparately injectedSeparatelyForImplementationSingleInstancePerFactory,
            global::Success.OptionalInterface.OptionalParameters optionalParameters,
            string name,
            global::Sundew.Injection.ILifecycleParameters? lifecycleParameters = null)
        {
            this.lifecycleParameters = lifecycleParameters ?? new global::Sundew.Injection.LifecycleParameters(
                false,
                false,
                default(global::Initialization.Interfaces.IInitializationReporter),
                default(global::Disposal.Interfaces.IDisposalReporter));
            this.lifecycleHandler = new global::Sundew.Injection.LifecycleHandler(this.lifecycleParameters, this.lifecycleParameters);
            this.requiredParameters = requiredParameters;
            this.multipleImplementationForArrayA = new global::Success.MultipleImplementations.MultipleImplementationForArrayA(this.requiredParameters.SecondSpecificallyNamedModuleParameter);
            this.lifecycleHandler.TryAdd(this.multipleImplementationForArrayA);
            this.multipleImplementationForArrayB = new global::Success.MultipleImplementations.MultipleImplementationForArrayB(this.requiredParameters.FirstSpecificallyNamedModuleParameter);
            this.lifecycleHandler.TryAdd(this.multipleImplementationForArrayB);
            this.multipleImplementationForArrayArray = new global::Success.MultipleImplementations.IMultipleImplementationForArray[] { this.multipleImplementationForArrayA, this.multipleImplementationForArrayB };
            this.injectedSeparatelyForInterfaceSingleInstancePerFactory = injectedSeparatelyForInterfaceSingleInstancePerFactory;
            this.generatedOperationFactory = new global::Success.GeneratedOperationFactory();
            this.injectedByType = injectedByType;
            this.interfaceSegregationOverridableNewImplementation = this.OnCreateInterfaceSegregationOverridableNew(this.injectedByType);
            this.lifecycleHandler.TryAdd(this.interfaceSegregationOverridableNewImplementation);
            this.interfaceSingleInstancePerFactory = new global::Success.SingleInstancePerFactory.InterfaceSingleInstancePerFactory(
                this.injectedSeparatelyForInterfaceSingleInstancePerFactory,
                this.multipleImplementationForArrayArray,
                this.generatedOperationFactory,
                this.interfaceSegregationOverridableNewImplementation);
            this.lifecycleHandler.TryAdd(this.interfaceSingleInstancePerFactory);
            this.injectedSeparatelyForImplementationSingleInstancePerFactory = injectedSeparatelyForImplementationSingleInstancePerFactory;
            this.optionalParameters = optionalParameters;
            this.injectableByInterface = this.optionalParameters.InjectableByInterface ?? this.lifecycleHandler.TryAdd(new global::Success.InterfaceImplementationBindings.InjectableByInterface(this.requiredParameters.SingleModuleRequiredConstructorMethodParameter));
            this.name = name;
            this.implementationSingleInstancePerFactory = new global::Success.SingleInstancePerFactory.ImplementationSingleInstancePerFactory(
                this.injectedSeparatelyForImplementationSingleInstancePerFactory,
                this.injectableByInterface,
                this.interfaceSegregationOverridableNewImplementation,
                this.name);
            this.dependencyFactory = new global::SuccessDependency.DependencyFactory();
            this.lifecycleHandler.TryAdd(this.dependencyFactory);
            this.manualDependencyFactory = new global::SuccessDependency.ManualDependencyFactory();
        }

        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public global::Success.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory InterfaceSingleInstancePerFactory
        {
            get
            {
                this.lifecycleHandler.Initialize();
                return this.interfaceSingleInstancePerFactory;
            }
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::Success.IResolveRoot CreateResolveRoot(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::Success.RequiredInterface.IRequiredService> requiredService,
            global::Success.RequiredInterface.RequiredParameter requiredParameter,
            global::Success.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::Success.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
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
        public async global::System.Threading.Tasks.Task<global::Success.IResolveRoot> CreateResolveRootAsync(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::Success.RequiredInterface.IRequiredService> requiredService,
            global::Success.RequiredInterface.RequiredParameter requiredParameter,
            global::Success.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::Success.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
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
        public global::Sundew.Injection.Constructed<global::Success.IResolveRoot> CreateResolveRootUninitialized(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::Success.RequiredInterface.IRequiredService> requiredService,
            global::Success.RequiredInterface.RequiredParameter requiredParameter,
            global::Success.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::Success.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var generic = global::Success.InjectionDeclaration.CreateGeneric<int>(defaultItem);
            injectableSingleInstancePerRequest ??= childLifecycleHandler.TryAdd(new global::Success.SingleInstancePerRequest.InjectableSingleInstancePerRequest(this.requiredParameters.SingleModuleRequiredCreateMethodParameter, integers, generic));
            interfaceSegregation ??= childLifecycleHandler.TryAdd(new global::Success.InterfaceSegregationBindings.InterfaceSegregationImplementation(injectableSingleInstancePerRequest));
            var selectConstructorForIntercepted = new global::Success.ConstructorSelection.SelectConstructor(this.implementationSingleInstancePerFactory, injectableSingleInstancePerRequest, interfaceSegregation);
            childLifecycleHandler.TryAdd(selectConstructorForIntercepted);
            var newInstanceAndDisposableForResources = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? childLifecycleHandler.TryAdd(new global::Success.NewInstance.NewInstanceAndDisposable(this, default(global::Success.OptionalInterface.IOmittedOptional)));
            var newInstanceAndDisposableForIntercepted = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? childLifecycleHandler.TryAdd(new global::Success.NewInstance.NewInstanceAndDisposable(this, default(global::Success.OptionalInterface.IOmittedOptional)));
            var overrideableNewImplementationForIntercepted = this.OnCreateOverrideableNewImplementation(injectableSingleInstancePerRequest, this.injectableByInterface, requiredParameter);
            childLifecycleHandler.TryAdd(overrideableNewImplementationForIntercepted);
            var overrideableNewImplementationForResolveRoot = this.OnCreateOverrideableNewImplementation(injectableSingleInstancePerRequest, this.injectableByInterface, requiredParameter);
            childLifecycleHandler.TryAdd(overrideableNewImplementationForResolveRoot);
            var newInstanceAndDisposableForConstructedChild = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? childLifecycleHandler.TryAdd(new global::Success.NewInstance.NewInstanceAndDisposable(this, default(global::Success.OptionalInterface.IOmittedOptional)));
            var constructedDependencyForConstructedChild = this.dependencyFactory.CreateUninitialized();
            childLifecycleHandler.TryAdd(constructedDependencyForConstructedChild);
            var dependencyForConstructedChild = constructedDependencyForConstructedChild.Object;
            childLifecycleHandler.TryAdd(dependencyForConstructedChild);
            var manualDependencyForConstructedChild = this.manualDependencyFactory.Create();
            childLifecycleHandler.TryAdd(manualDependencyForConstructedChild);
            var constructedChildForResolveRoot = new global::Success.ChildFactory.ConstructedChild(newInstanceAndDisposableForConstructedChild, dependencyForConstructedChild, manualDependencyForConstructedChild);
            childLifecycleHandler.TryAdd(constructedChildForResolveRoot);

            static global::System.Collections.Generic.IEnumerable<global::Success.MultipleImplementations.IMultipleImplementationForEnumerable> CreateMultipleImplementationForEnumerable()
            {
                yield return new global::Success.MultipleImplementations.MultipleImplementationForEnumerableA();
                yield return new global::Success.MultipleImplementations.MultipleImplementationForEnumerableB();
            }

            var resolveRootResult = new global::Success.ResolveRoot(
                new global::Success.InterfaceImplementationBindings.Intercepted(
                    this.multipleImplementationForArrayArray,
                    global::Success.InjectionDeclaration.CreateFeatureService1(
                        this.interfaceSingleInstancePerFactory,
                        injectableSingleInstancePerRequest,
                        requiredService.Invoke(),
                        interfaceSegregation),
                    selectConstructorForIntercepted,
                    new global::Success.UnboundType.Resources(newInstanceAndDisposableForResources),
                    newInstanceAndDisposableForIntercepted,
                    overrideableNewImplementationForIntercepted),
                this.interfaceSingleInstancePerFactory,
                overrideableNewImplementationForResolveRoot,
                constructedChildForResolveRoot,
                CreateMultipleImplementationForEnumerable(),
                new global::Success.NestingTypes.NestedConsumer(new global::Success.NestingTypes.Nestee.Nested()));
            this.lifecycleHandler.TryAdd(resolveRootResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::Success.IResolveRoot>(resolveRootResult, childLifecycleHandler);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public global::Success.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverC()
        {
            return new global::Success.TypeResolver.MultipleImplementationForTypeResolverC();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(global::Success.IResolveRoot resolveRoot)
        {
            this.lifecycleHandler.Dispose(resolveRoot);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::Success.IResolveRoot resolveRoot)
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
        protected virtual global::Success.InterfaceSegregationBindings.IInterfaceSegregationOverridableNew OnCreateInterfaceSegregationOverridableNew(global::Success.RequiredInterface.IInjectedByType injectedByType)
        {
            return new global::Success.InterfaceSegregationBindings.InterfaceSegregationOverridableNewImplementation(injectedByType);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        protected virtual global::Success.OverridableNew.OverrideableNewImplementation OnCreateOverrideableNewImplementation(global::Success.SingleInstancePerRequest.IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest, global::Success.InterfaceImplementationBindings.IInjectableByInterface injectableByInterface, global::Success.RequiredInterface.RequiredParameter requiredParameter)
        {
            return new global::Success.OverridableNew.OverrideableNewImplementation(injectableSingleInstancePerRequest, injectableByInterface, requiredParameter);
        }
    }
}
