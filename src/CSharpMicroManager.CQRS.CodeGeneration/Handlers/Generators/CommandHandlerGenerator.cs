using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using CSharpMicroManager.CQRS.CodeGeneration.Attributes;

namespace CSharpMicroManager.CQRS.CodeGeneration.Handlers.Generators;

[CommandHandlerToGenerate]
public class X
{ 
        
}

[Generator]
public sealed class CommandHandlerGenerator : IIncrementalGenerator
{
    private const string AttributeFullName = "CSharpMicroManager.CQRS.CodeGeneration.CommandHandlerToGenerateAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Debugger.Launch();
        //add marker attribute to the compilation
        //context.RegisterPostInitializationOutput(ctx =>
        //{
        //    ctx.AddSource("CommandHandlerGenerationAttribute.g.cs",
        //                  SourceText.From(AttributeGenerationHelper.CommandHandlerAttribute, Encoding.UTF8));
        //});

        //do a simple filter for classes with attribute from above
        var classWithCommandHandlerAttrDeclarations = context
            .SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsClassWithAttribute(s),
                transform: static (ctx, _) => GetCommandHandlerTargetForGeneration(ctx))
            .Where(static s => s is not null);

        // Combine the selected clasess with the `Compilation`
        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClasses
            = context.CompilationProvider.Combine(classWithCommandHandlerAttrDeclarations.Collect())!;

        // Generate the source using the compilation and classes
        context.RegisterSourceOutput(compilationAndClasses,
            static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }


    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        if(classes.IsDefaultOrEmpty)
        {
            return;
        }

        var distinctClasses = classes.Distinct();

        var classesToGenerate = GetCommandHandlersToGenerate(compilation, distinctClasses, context.CancellationToken);

        if (classesToGenerate.Any())
        {
            // generate the source code and add it to the output
            var result = SourceGenerationHelper.GenerateExtensionClass(classesToGenerate);
            context.AddSource("CommandHandlerGenerationExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }

    private static List<CommandHandlerToGenerate> GetCommandHandlersToGenerate(
        Compilation compilation, 
        IEnumerable<ClassDeclarationSyntax> classes, 
        CancellationToken cancellationToken)
    {
        var handlersToGenerate = new List<CommandHandlerToGenerate>();

        var handlerToGenerateAttribute = compilation.GetTypeByMetadataName(AttributeFullName);

        if(handlerToGenerateAttribute is null)
        {
            return handlersToGenerate;
        }

        foreach(var @class in classes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(@class.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(@class) is not INamedTypeSymbol classSymbol)
            {
                // something went wrong, bail out
                continue;
            }

            var className = classSymbol.ToString();

            handlersToGenerate.Add(new CommandHandlerToGenerate(className, new List<System.Type>()));
        }

        return handlersToGenerate;
    }

    private static bool IsClassWithAttribute(SyntaxNode node) =>
        node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };

    private static ClassDeclarationSyntax? GetCommandHandlerTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclSyntax = (ClassDeclarationSyntax)context.Node;

        foreach(var attributeListSyntax in classDeclSyntax.AttributeLists)
        {
            foreach(var attributeSyntax in attributeListSyntax.Attributes)
            {
                if(context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attrFullName = attributeSymbol.ContainingType.ToDisplayString();

                if(attrFullName.Equals(AttributeFullName))
                {
                    return classDeclSyntax;
                }
            }
        }

        return null;
    }
}