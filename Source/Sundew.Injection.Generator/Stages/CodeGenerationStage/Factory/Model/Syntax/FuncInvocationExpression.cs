namespace Sundew.Injection.Generator.Stages.CodeGenerationStage.Factory.Model.Syntax;

internal sealed record FuncInvocationExpression(Expression DelegateAccessor, bool IsNullable) : Expression;