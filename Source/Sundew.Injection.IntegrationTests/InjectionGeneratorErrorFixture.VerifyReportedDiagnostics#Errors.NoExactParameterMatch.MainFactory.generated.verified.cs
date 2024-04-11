//HintName: Errors.NoExactParameterMatch.MainFactory.generated.cs
#nullable enable
namespace Errors.NoExactParameterMatch
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
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        public global::Errors.NoExactParameterMatch.Root Create()
        {
            var parameter = new global::Errors.NoExactParameterMatch.Parameter();
            return new global::Errors.NoExactParameterMatch.Root(parameter);
        }
    }
}
