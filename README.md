# Introduction

Simple command/query pipeline implementation in .NET

* CSharpMicroManger.CQRS
  [![Build Status](https://dev.azure.com/czadokonrad/CSharpMicroManager/_apis/build/status/CSharpMicroManager.CQRS?repoName=CSharpMicroManager.CQRS&branchName=main)](https://dev.azure.com/czadokonrad/CSharpMicroManager/_build/latest?definitionId=6&repoName=CSharpMicroManager.CQRS&branchName=main)

* CSharpMicroManger.CQRS.Abstractions
  [![Build Status](https://dev.azure.com/czadokonrad/CSharpMicroManager/_apis/build/status/CSharpMicroManager.CQRS.Abstractions?repoName=CSharpMicroManager.CQRS&branchName=main)](https://dev.azure.com/czadokonrad/CSharpMicroManager/_build/latest?definitionId=7&repoName=CSharpMicroManager.CQRS&branchName=main)
* CSharpMicroManger.CQRS.Extensions
  [![Build Status](https://dev.azure.com/czadokonrad/CSharpMicroManager/_apis/build/status/CSharpMicroManager.CQRS.Extensions?repoName=CSharpMicroManager.CQRS&branchName=main)](https://dev.azure.com/czadokonrad/CSharpMicroManager/_build/latest?definitionId=24&repoName=CSharpMicroManager.CQRS&branchName=main)

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
