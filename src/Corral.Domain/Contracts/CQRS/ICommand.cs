// ------------------------------------------------------------------------------------------------
// <copyright file="ICommand.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.Contracts.CQRS;

/// <summary>
///   Base interface for all CQRS commands.
/// </summary>
/// <typeparam name="TResponse">The response type returned after command execution.</typeparam>
/// <remarks>
///   A command represents an action that modifies the system state.
///   Commands must be immutable and contain all necessary data to perform the action.
///   This interface is purely a domain contract and has no external dependencies.
///   Application layer records implementing commands will also implement MediatR.IRequest.
/// </remarks>
public interface ICommand<TResponse>
{
}
