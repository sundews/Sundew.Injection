//HintName: DistinctErrors.NoExactParameterMatch.MainFactory.generated.cs
#nullable enable
namespace DistinctErrors.NoExactParameterMatch
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed partial class MainFactory : global::Sundew.Injection.IGeneratedFactory
    {
        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        public MainFactory()
        {
        }

        [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
        [global::Sundew.Injection.BindableFactoryTargetAttribute]
        public global::DistinctErrors.NoExactParameterMatch.Root Create()
        {
            var parameter = new global::DistinctErrors.NoExactParameterMatch.Parameter();
            return new global::DistinctErrors.NoExactParameterMatch.Root(parameter);
        }
    }
}
