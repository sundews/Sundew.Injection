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
        private readonly global::AllFeaturesSuccess.MultipleImplementations.IMultipleImplementationForArray multipleImplementationForArrayA;
        private readonly global::AllFeaturesSuccess.MultipleImplementations.IMultipleImplementationForArray multipleImplementationForArrayB;
        private readonly global::AllFeaturesSuccess.MultipleImplementations.IMultipleImplementationForArray[] multipleImplementationForArrayArray;
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

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public ResolveRootFactory(
            global::AllFeaturesSuccess.RequiredInterface.IRequiredParameters requiredParameters,
            global::AllFeaturesSuccess.RequiredInterface.IInjectedSeparately injectedSeparatelyForInterfaceSingleInstancePerFactory,
            global::AllFeaturesSuccess.RequiredInterface.IInjectedByType injectedByType,
            global::AllFeaturesSuccess.RequiredInterface.IInjectedSeparately injectedSeparatelyForImplementationSingleInstancePerFactory,
            global::AllFeaturesSuccess.OptionalInterface.OptionalParameters optionalParameters,
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
            this.multipleImplementationForArrayA = new global::AllFeaturesSuccess.MultipleImplementations.MultipleImplementationForArrayA(this.requiredParameters.SecondSpecificallyNamedModuleParameter);
            this.lifecycleHandler.TryAdd(this.multipleImplementationForArrayA);
            this.multipleImplementationForArrayB = new global::AllFeaturesSuccess.MultipleImplementations.MultipleImplementationForArrayB(this.requiredParameters.FirstSpecificallyNamedModuleParameter);
            this.lifecycleHandler.TryAdd(this.multipleImplementationForArrayB);
            this.multipleImplementationForArrayArray = new global::AllFeaturesSuccess.MultipleImplementations.IMultipleImplementationForArray[] { this.multipleImplementationForArrayA, this.multipleImplementationForArrayB };
            this.injectedSeparatelyForInterfaceSingleInstancePerFactory = injectedSeparatelyForInterfaceSingleInstancePerFactory;
            this.generatedOperationFactory = new global::AllFeaturesSuccess.GeneratedOperationFactory();
            this.injectedByType = injectedByType;
            this.interfaceSegregationOverridableNewImplementation = this.OnCreateInterfaceSegregationOverridableNew(this.injectedByType);
            this.lifecycleHandler.TryAdd(this.interfaceSegregationOverridableNewImplementation);
            this.interfaceSingleInstancePerFactory = new global::AllFeaturesSuccess.SingleInstancePerFactory.InterfaceSingleInstancePerFactory(
                this.injectedSeparatelyForInterfaceSingleInstancePerFactory,
                this.multipleImplementationForArrayArray,
                this.generatedOperationFactory,
                this.interfaceSegregationOverridableNewImplementation);
            this.lifecycleHandler.TryAdd(this.interfaceSingleInstancePerFactory);
            this.injectedSeparatelyForImplementationSingleInstancePerFactory = injectedSeparatelyForImplementationSingleInstancePerFactory;
            this.optionalParameters = optionalParameters;
            this.injectableByInterface = this.optionalParameters.InjectableByInterface ?? this.lifecycleHandler.TryAdd(new global::AllFeaturesSuccess.InterfaceImplementationBindings.InjectableByInterface(this.requiredParameters.SingleModuleRequiredConstructorMethodParameter));
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

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::AllFeaturesSuccess.IResolveRoot CreateResolveRoot(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService,
            global::AllFeaturesSuccess.RequiredInterface.RequiredParameter requiredParameter,
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
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
        [global::Sundew.Injection.IndirectCreateMethodAttribute]
        public async global::System.Threading.Tasks.Task<global::AllFeaturesSuccess.IResolveRoot> CreateResolveRootAsync(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService,
            global::AllFeaturesSuccess.RequiredInterface.RequiredParameter requiredParameter,
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
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
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        [global::Sundew.Injection.IndirectCreateMethodAttribute]
        public global::Sundew.Injection.Constructed<global::AllFeaturesSuccess.IResolveRoot> CreateResolveRootUninitialized(
            global::System.Collections.Generic.IEnumerable<int> integers,
            int defaultItem,
            global::System.Func<global::AllFeaturesSuccess.RequiredInterface.IRequiredService> requiredService,
            global::AllFeaturesSuccess.RequiredInterface.RequiredParameter requiredParameter,
            global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest? injectableSingleInstancePerRequest = null,
            global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregation? interfaceSegregation = null)
        {
            var childLifecycleHandler = this.lifecycleHandler.CreateChildLifecycleHandler();
            var generic = global::AllFeaturesSuccess.FactoryDeclaration.CreateGeneric<int>(defaultItem);
            injectableSingleInstancePerRequest ??= childLifecycleHandler.TryAdd(new global::AllFeaturesSuccess.SingleInstancePerRequest.InjectableSingleInstancePerRequest(this.requiredParameters.SingleModuleRequiredCreateMethodParameter, integers, generic));
            interfaceSegregation ??= childLifecycleHandler.TryAdd(new global::AllFeaturesSuccess.InterfaceSegregationBindings.InterfaceSegregationImplementation(injectableSingleInstancePerRequest));
            var selectConstructorForIntercepted = new global::AllFeaturesSuccess.ConstructorSelection.SelectConstructor(this.implementationSingleInstancePerFactory, injectableSingleInstancePerRequest, interfaceSegregation);
            childLifecycleHandler.TryAdd(selectConstructorForIntercepted);
            var newInstanceAndDisposableForResources = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? childLifecycleHandler.TryAdd(new global::AllFeaturesSuccess.NewInstance.NewInstanceAndDisposable(default(global::AllFeaturesSuccess.OptionalInterface.IOmittedOptional)));
            var newInstanceAndDisposableForIntercepted = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? childLifecycleHandler.TryAdd(new global::AllFeaturesSuccess.NewInstance.NewInstanceAndDisposable(default(global::AllFeaturesSuccess.OptionalInterface.IOmittedOptional)));
            var overrideableNewImplementationForIntercepted = this.OnCreateOverrideableNewImplementation(injectableSingleInstancePerRequest, this.injectableByInterface, requiredParameter);
            childLifecycleHandler.TryAdd(overrideableNewImplementationForIntercepted);
            var overrideableNewImplementationForResolveRoot = this.OnCreateOverrideableNewImplementation(injectableSingleInstancePerRequest, this.injectableByInterface, requiredParameter);
            childLifecycleHandler.TryAdd(overrideableNewImplementationForResolveRoot);
            var newInstanceAndDisposableForConstructedChild = this.optionalParameters.NewInstanceAndDisposableFactory?.Invoke() ?? childLifecycleHandler.TryAdd(new global::AllFeaturesSuccess.NewInstance.NewInstanceAndDisposable(default(global::AllFeaturesSuccess.OptionalInterface.IOmittedOptional)));
            var constructedDependencyForConstructedChild = this.dependencyFactory.CreateUninitialized();
            childLifecycleHandler.TryAdd(constructedDependencyForConstructedChild);
            var dependencyForConstructedChild = constructedDependencyForConstructedChild.Object;
            childLifecycleHandler.TryAdd(dependencyForConstructedChild);
            var manualDependencyForConstructedChild = this.manualDependencyFactory.Create();
            childLifecycleHandler.TryAdd(manualDependencyForConstructedChild);
            var constructedChildForResolveRoot = new global::AllFeaturesSuccess.ChildFactory.ConstructedChild(newInstanceAndDisposableForConstructedChild, dependencyForConstructedChild, manualDependencyForConstructedChild);
            childLifecycleHandler.TryAdd(constructedChildForResolveRoot);

            static global::System.Collections.Generic.IEnumerable<global::AllFeaturesSuccess.MultipleImplementations.IMultipleImplementationForEnumerable> CreateMultipleImplementationForEnumerable()
            {
                yield return new global::AllFeaturesSuccess.MultipleImplementations.MultipleImplementationForEnumerableA();
                yield return new global::AllFeaturesSuccess.MultipleImplementations.MultipleImplementationForEnumerableB();
            }

            var resolveRootResult = new global::AllFeaturesSuccess.ResolveRoot(
                new global::AllFeaturesSuccess.InterfaceImplementationBindings.Intercepted(
                    this.multipleImplementationForArrayArray,
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
                constructedChildForResolveRoot,
                CreateMultipleImplementationForEnumerable(),
                new global::AllFeaturesSuccess.NestingTypes.NestedConsumer(new global::AllFeaturesSuccess.NestingTypes.Nestee.Nested()));
            this.lifecycleHandler.TryAdd(resolveRootResult, childLifecycleHandler);
            return new global::Sundew.Injection.Constructed<global::AllFeaturesSuccess.IResolveRoot>(resolveRootResult, childLifecycleHandler);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        public global::AllFeaturesSuccess.SingleInstancePerFactory.IInterfaceSingleInstancePerFactory CreateInterfaceSingleInstancePerFactory()
        {
            return this.interfaceSingleInstancePerFactory;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        public global::AllFeaturesSuccess.TypeResolver.IMultipleImplementationForTypeResolver CreateMultipleImplementationForTypeResolverC()
        {
            return new global::AllFeaturesSuccess.TypeResolver.MultipleImplementationForTypeResolverC();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public void Dispose(global::AllFeaturesSuccess.IResolveRoot resolveRoot)
        {
            this.lifecycleHandler.Dispose(resolveRoot);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public global::System.Threading.Tasks.ValueTask DisposeAsync(global::AllFeaturesSuccess.IResolveRoot resolveRoot)
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
        protected virtual global::AllFeaturesSuccess.InterfaceSegregationBindings.IInterfaceSegregationOverridableNew OnCreateInterfaceSegregationOverridableNew(global::AllFeaturesSuccess.RequiredInterface.IInjectedByType injectedByType)
        {
            return new global::AllFeaturesSuccess.InterfaceSegregationBindings.InterfaceSegregationOverridableNewImplementation(injectedByType);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        protected virtual global::AllFeaturesSuccess.OverridableNew.OverrideableNewImplementation OnCreateOverrideableNewImplementation(global::AllFeaturesSuccess.SingleInstancePerRequest.IInjectableSingleInstancePerRequest injectableSingleInstancePerRequest, global::AllFeaturesSuccess.InterfaceImplementationBindings.IInjectableByInterface injectableByInterface, global::AllFeaturesSuccess.RequiredInterface.RequiredParameter requiredParameter)
        {
            return new global::AllFeaturesSuccess.OverridableNew.OverrideableNewImplementation(injectableSingleInstancePerRequest, injectableByInterface, requiredParameter);
        }
    }
}
