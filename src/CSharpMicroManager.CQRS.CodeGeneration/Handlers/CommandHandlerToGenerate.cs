using System;
using System.Collections.Generic;

namespace CSharpMicroManager.CQRS.CodeGeneration.Handlers;

public readonly struct CommandHandlerToGenerate
{
    public readonly string Name;
    public readonly List<Type> Dependencies;

    public CommandHandlerToGenerate(string name, List<Type> dependencies)
    {
        Name = name;
        Dependencies = dependencies;
    }
}