// ------------------------------------------------------------------------------------------------
// <copyright file="UpdateUserSettingsCommand.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Commands.UpdateUserSettings;

/// <summary>
///   Command to update the global user settings.
/// </summary>
/// <param name="ClickMode">0 = SingleClick, 1 = DoubleClick.</param>
/// <param name="IconLayout">0 = LargeGrid, 1 = SmallGrid, 2 = List.</param>
public record UpdateUserSettingsCommand(int ClickMode, int IconLayout)
  : ICommand<UserSettings>, IRequest<UserSettings>;
