//HintName: TestPlayground.MainFactory.generated.cs
#nullable enable
namespace TestPlayground
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory("CreatedSingleInstance", "Create")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class MainFactory : global::Sundew.Injection.IGeneratedFactory
    {
        private readonly global::TestPlayground.CreatedSingleInstance createdSingleInstance;
        private readonly global::TestPlayground.ConstructorParameter constructorParameter;

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public MainFactory(global::TestPlayground.ConstructorParameter constructorParameter)
        {
            this.createdSingleInstance = new global::TestPlayground.CreatedSingleInstance();
            this.CreatedSingleInstance = this.createdSingleInstance;
            this.constructorParameter = constructorParameter;
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public partial global::TestPlayground.Root Create(global::TestPlayground.FactoryMethodParameter factoryMethodParameter)
        {
            return new global::TestPlayground.Root(this.constructorParameter, factoryMethodParameter);
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public static partial global::TestPlayground.MainFactory Constructor(global::TestPlayground.ConstructorParameter constructorParameter)
        {
            return new global::TestPlayground.MainFactory(constructorParameter);
        }
    }
}
