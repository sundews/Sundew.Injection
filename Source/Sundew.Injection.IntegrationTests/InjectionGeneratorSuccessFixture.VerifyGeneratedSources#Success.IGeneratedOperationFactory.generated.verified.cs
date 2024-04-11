//HintName: Success.IGeneratedOperationFactory.generated.cs
#nullable enable
namespace Success
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.1.0.0")]
    [global::Sundew.Injection.Factory]
    public partial interface IGeneratedOperationFactory : global::Sundew.Injection.IGeneratedFactory
    {
        [global::Sundew.Injection.BindableCreateMethodAttribute]
        global::Success.Operations.IOperation CreateOperationA(int lhs, int rhs);

        [global::Sundew.Injection.BindableCreateMethodAttribute]
        global::Success.Operations.IOperation CreateOperationB(int lhs, int rhs);
    }
}
