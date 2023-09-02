//HintName: IGeneratedOperationFactory.cs
namespace AllFeaturesSuccess
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.Injection.Generator", "0.0.0.0")]
    [global::Sundew.Injection.Factory]
    public interface IGeneratedOperationFactory : global::Sundew.Injection.IGeneratedFactory
    {
        [global::Sundew.Injection.CreateMethod]
        global::AllFeaturesSuccess.Operations.IOperation CreateOperationA(int lhs, int rhs);

        [global::Sundew.Injection.CreateMethod]
        global::AllFeaturesSuccess.Operations.IOperation CreateOperationB(int lhs, int rhs);
    }
}
