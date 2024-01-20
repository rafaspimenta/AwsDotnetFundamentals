﻿using Amazon;

namespace Customers.Api.Messaging;

public class QueueSettings
{
    public const string Key = "Queue";
    public required string Name { get; init; }

    public required string Region { get; init; }
}
