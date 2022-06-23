using System.Collections.Generic;
using System.Text;

namespace CSharpMicroManager.CQRS.CodeGeneration.Handlers;

public static class SourceGenerationHelper
{

    public static string GenerateExtensionClass(List<CommandHandlerToGenerate> handlersToGenerate)
    {
        var sb = new StringBuilder();
        sb.Append(@"
            namespace CSharpMicroManager.CQRS.CodeGeneration
            {");

        foreach (var handlerToGenerate in handlersToGenerate)
        {

            sb.Append($@"
                public static partial class {handlerToGenerate.Name}
                {{}}");


        }

        sb.Append("}");

        return sb.ToString();
    }
}