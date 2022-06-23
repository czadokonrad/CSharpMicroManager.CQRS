namespace CSharpMicroManager.CQRS.CodeGeneration.Handlers;

public static class AttributeGenerationHelper
{
    public const string CommandHandlerAttribute = @"
            namespace CSharpMicroManager.CQRS.CodeGeneration
            {
                [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
                public sealed class CommandHandlerToGenerateAttribute : Attribute
                {
                }
            }";
}