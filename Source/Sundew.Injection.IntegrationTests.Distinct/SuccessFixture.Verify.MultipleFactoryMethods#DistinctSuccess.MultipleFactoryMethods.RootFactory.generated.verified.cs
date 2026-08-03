//HintName: DistinctSuccess.MultipleFactoryMethods.RootFactory.generated.cs
#nullable enable
namespace DistinctSuccess.MultipleFactoryMethods
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory("CreateMultipleA", "CreateMultipleB")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class RootFactory : global::DistinctSuccess.MultipleFactoryMethods.IMultipleFactory
    {
        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public RootFactory()
        {
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public global::DistinctSuccess.MultipleFactoryMethods.IMultiple CreateMultipleA()
        {
            return new global::DistinctSuccess.MultipleFactoryMethods.MultipleA();
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public global::DistinctSuccess.MultipleFactoryMethods.IMultiple CreateMultipleB()
        {
            return new global::DistinctSuccess.MultipleFactoryMethods.MultipleB();
        }
    }
}
