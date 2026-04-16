// ------------------------------------------------------------------------------------------------
// <copyright file="GetUserSettingsQuery.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Queries.GetUserSettings;

/// <summary>
///   Query to retrieve the singleton user settings.
/// </summary>
public record GetUserSettingsQuery : IQuery<UserSettings>, IRequest<UserSettings>;
