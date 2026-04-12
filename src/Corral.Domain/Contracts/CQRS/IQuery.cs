// ------------------------------------------------------------------------------------------------
// <copyright file="IQuery.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.Contracts.CQRS;

/// <summary>
///   Base interface for all CQRS queries.
/// </summary>
/// <typeparam name="TResponse">The response type returned after query execution.</typeparam>
/// <remarks>
///   A query represents a read operation that does not modify system state.
///   Queries must be immutable and have no side effects.
///   This interface is purely a domain contract and has no external dependencies.
///   Application layer records implementing queries will also implement MediatR.IRequest.
/// </remarks>
public interface IQuery<TResponse>
{
}
