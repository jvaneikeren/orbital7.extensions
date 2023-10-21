﻿namespace Orbital7.Extensions.Apis.BetterStack;

public class Pagination
{
    [JsonPropertyName("first")]
    public string First { get; set; }

    [JsonPropertyName("last")]
    public string Last { get; set; }

    [JsonPropertyName("prev")]
    public string Prev { get; set; }

    [JsonPropertyName("next")]
    public string Next { get; set; }
}
