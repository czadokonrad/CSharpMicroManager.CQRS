# Introduction

Simple command/query pipeline implementation in .NET

* CSharpMicroManger.CQRS
  [![Build Status](https://dev.azure.com/czadokonrad/CSharpMicroManager/_apis/build/status/CSharpMicroManager.CQRS?repoName=CSharpMicroManager.CQRS&branchName=main)](https://dev.azure.com/czadokonrad/CSharpMicroManager/_build/latest?definitionId=6&repoName=CSharpMicroManager.CQRS&branchName=main)
  [![CSharpMicroManager.CQRS package in csharp-micromanager-feed feed in Azure Artifacts](https://feeds.dev.azure.com/czadokonrad/d7b143bd-01a8-4e66-818b-e4d2d672b42c/_apis/public/Packaging/Feeds/csharp-micromanager-feed/Packages/93f2d6fb-bc03-4ac8-9205-23101fcdbc7e/Badge)](https://dev.azure.com/czadokonrad/CSharpMicroManager/_artifacts/feed/csharp-micromanager-feed/NuGet/CSharpMicroManager.CQRS/11.0.0-pre)
* CSharpMicroManger.CQRS.Abstractions
  [![Build Status](https://dev.azure.com/czadokonrad/CSharpMicroManager/_apis/build/status/CSharpMicroManager.CQRS.Abstractions?repoName=CSharpMicroManager.CQRS&branchName=main)](https://dev.azure.com/czadokonrad/CSharpMicroManager/_build/latest?definitionId=7&repoName=CSharpMicroManager.CQRS&branchName=main)
  [![CSharpMicroManager.CQRS.Abstractions package in csharp-micromanager-feed feed in Azure Artifacts](https://feeds.dev.azure.com/czadokonrad/d7b143bd-01a8-4e66-818b-e4d2d672b42c/_apis/public/Packaging/Feeds/csharp-micromanager-feed/Packages/0a03a717-2441-40ac-89a3-eedeb2296cfc/Badge)](https://dev.azure.com/czadokonrad/CSharpMicroManager/_artifacts/feed/csharp-micromanager-feed/NuGet/CSharpMicroManager.CQRS.Abstractions/6.2.0-pre)
* CSharpMicroManger.CQRS.Extensions
  [![Build Status](https://dev.azure.com/czadokonrad/CSharpMicroManager/_apis/build/status/CSharpMicroManager.CQRS.Extensions?repoName=CSharpMicroManager.CQRS&branchName=main)](https://dev.azure.com/czadokonrad/CSharpMicroManager/_build/latest?definitionId=24&repoName=CSharpMicroManager.CQRS&branchName=main)
  [![CSharpMicroManager.CQRS.Extensions package in csharp-micromanager-feed feed in Azure Artifacts](https://feeds.dev.azure.com/czadokonrad/d7b143bd-01a8-4e66-818b-e4d2d672b42c/_apis/public/Packaging/Feeds/csharp-micromanager-feed/Packages/c6fc1b40-a758-4b0e-9fe7-8a4479b95793/Badge)](https://dev.azure.com/czadokonrad/CSharpMicroManager/_artifacts/feed/csharp-micromanager-feed/NuGet/CSharpMicroManager.CQRS.Extensions/1.0.0)

Provides a simple way to perform some action in the convenient moment of the execution of `ICommandHandler<TCommand>` 


# CommandHandler Pipelines

## PreCommandHandlerPipeline
This pipeline is a proper place for actions which must be executed before real `ICommandHandler<TCommand>` processing.
Here are welcomed such tasks as command validation or permission validation.

## CommandHandlerPipeline
This pipeline is meant to be used for `ICommandHandlerPipe<TCommand>` items which must be executed in same scope within which `ICommandHandler<TCommand>` itself will be executed.
For example exception handling, unit of work or domain events dispatching scenarios

## PostCommandHandlerPipeline
This pipeline purpose is for some post handling actions which will be executed after
`CommandHandlerPipeline`. These are some optional actions which fail will not be observed as `ICommandHandler<TCommand>` execution error.
