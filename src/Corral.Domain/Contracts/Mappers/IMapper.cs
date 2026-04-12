// ------------------------------------------------------------------------------------------------
// <copyright file="IMapper.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.Contracts.Mappers;

/// <summary>
///   Represents a generic interface for explicit mapping between layers.
///   Provides methods to transform an object or a collection of objects.
/// </summary>
/// <typeparam name="TSource">The source type to be transformed.</typeparam>
/// <typeparam name="TDestination">The destination type after transformation.</typeparam>
public interface IMapper<TSource, TDestination>
{
  #region Methods

  /// <summary>
  ///   Maps a source object of type <typeparamref name="TSource" /> to a destination object of type
  ///   <typeparamref name="TDestination" />.
  /// </summary>
  /// <param name="source">The source object to be transformed.</param>
  /// <returns>The transformed object of type <typeparamref name="TDestination" />.</returns>
  /// <remarks>
  ///   This method is intended to provide a mechanism for converting objects between layers or
  ///   representations,
  ///   ensuring that the destination object is properly constructed based on the source object's data.
  /// </remarks>
  TDestination Map(TSource source);

  /// <summary>
  ///   Maps a collection of source objects to a list of destination objects.
  /// </summary>
  /// <param name="sources">The enumerable collection of source objects to be transformed.</param>
  /// <returns>A list of destination objects resulting from the transformation.</returns>
  /// <remarks>
  ///   This method applies the mapping logic to each element in the provided collection.
  ///   It is useful for scenarios where multiple source objects need to be transformed
  ///   into their corresponding destination representations.
  /// </remarks>
  List<TDestination> MapList(IEnumerable<TSource> sources);

  #endregion
}
