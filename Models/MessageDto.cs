using System;

namespace dynamic_json.Models;

public class MessageDto
{
    public long Id { get; set; }
    public string Direction { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string RawPayloadHex { get; set; } = "";
    public Dictionary<string, object> ParsedPayload { get; set; } = [];
}
