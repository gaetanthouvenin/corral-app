// ------------------------------------------------------------------------------------------------
// <copyright file="FenceDomainEventTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;
using Corral.Domain.DomainEvents;
using Corral.Domain.ValueObjects;

namespace Corral.Domain.Tests.DomainEvents;

public class FenceDomainEventTests
{
  #region Methods

  [Fact]
  public void FenceCreatedEvent_ShouldExposePayloadAndMetadata()
  {
    var before = DateTime.UtcNow;
    var fenceId = FenceId.Create("fence-1");

    var domainEvent = new FenceCreatedEvent(fenceId, "Dev");

    var after = DateTime.UtcNow;
    domainEvent.FenceId.ShouldBe(fenceId);
    domainEvent.Name.ShouldBe("Dev");
    domainEvent.EventId.ShouldNotBeNullOrWhiteSpace();
    domainEvent.OccurredAt.ShouldBeGreaterThanOrEqualTo(before);
    domainEvent.OccurredAt.ShouldBeLessThanOrEqualTo(after);
  }

  [Fact]
  public void FenceDeletedEvent_ShouldExposePayloadAndMetadata()
  {
    var domainEvent = new FenceDeletedEvent(FenceId.Create("fence-2"));

    domainEvent.FenceId.Value.ShouldBe("fence-2");
    domainEvent.EventId.ShouldNotBeNullOrWhiteSpace();
  }

  [Fact]
  public void FencePositionChangedEvent_ShouldExposePayloadAndMetadata()
  {
    var position = Position.Create(10, 20);
    var domainEvent = new FencePositionChangedEvent(FenceId.Create("fence-3"), position);

    domainEvent.FenceId.Value.ShouldBe("fence-3");
    domainEvent.NewPosition.ShouldBe(position);
    domainEvent.EventId.ShouldNotBeNullOrWhiteSpace();
  }

  [Fact]
  public void FenceColorChangedEvent_ShouldExposePayloadAndMetadata()
  {
    var domainEvent = new FenceColorChangedEvent(FenceId.Create("fence-4"), Color.Blue);

    domainEvent.FenceId.Value.ShouldBe("fence-4");
    domainEvent.NewColor.ShouldBe(Color.Blue);
    domainEvent.EventId.ShouldNotBeNullOrWhiteSpace();
  }

  [Fact]
  public void FenceDeactivatedEvent_ShouldExposePayloadAndMetadata()
  {
    var domainEvent = new FenceDeactivatedEvent(FenceId.Create("fence-5"));

    domainEvent.FenceId.Value.ShouldBe("fence-5");
    domainEvent.EventId.ShouldNotBeNullOrWhiteSpace();
  }

  #endregion
}
