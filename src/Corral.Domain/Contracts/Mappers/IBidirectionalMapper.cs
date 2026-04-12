// ------------------------------------------------------------------------------------------------
// <copyright file="IBidirectionalMapper.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.Contracts.Mappers;

/// <summary>
///   Represents a generic interface for bidirectional mappers, allowing transformations
///   between a source type and a destination type.
/// </summary>
/// <typeparam name="TSource">The source type to be transformed.</typeparam>
/// <typeparam name="TDestination">The destination type after transformation.</typeparam>
public interface IBidirectionalMapper<TSource, TDestination>
{
  #region Methods

  /// <summary>
  ///   Maps a source object to a destination object.
  /// </summary>
  /// <param name="source">The source object to be transformed.</param>
  /// <returns>The transformed destination object.</returns>
  TDestination MapToDestination(TSource source);

  /// <summary>
  ///   Maps a destination object to a source object.
  /// </summary>
  /// <param name="destination">The destination object to be transformed.</param>
  /// <returns>The transformed source object.</returns>
  TSource MapToSource(TDestination destination);

  #endregion
}
