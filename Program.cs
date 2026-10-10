using System.Text.Json;
using dynamic_json;
using dynamic_json.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var packetsPath = Path.Combine(AppContext.BaseDirectory, "packets.json");
        PacketCatalog catalog = PacketCatalogLoader.LoadFromFile(packetsPath);

        PacketDefinition? packet = catalog.Packets.FirstOrDefault(p => p.Id == 3);
        if (packet is null)
        {
            Console.WriteLine("Пакет с id=3 не найден");
            return;
        }

        byte[] unparsedPayload = new byte[]
        {
            0x00, 0x03, 0x00, 0x03,
            0x00, 0x00, 0x00, 0x01,
            0x00, 0x00, 0x00, 0x02,
            0x00, 0x00, 0x00, 0x03,
            0x00, 0x00, 0x00, 0x04,
            0x00, 0x00, 0x00, 0x05
        };

        Dictionary<string, object> parsed = PayloadParser.Parse(packet, unparsedPayload);

        var message = new MessageDto
        {
            Id = packet.Id,
            Direction = packet.Dir,
            Name = packet.Name,
            Timestamp = DateTime.Now,
            RawPayloadHex = Convert.ToHexString(unparsedPayload),
            ParsedPayload = parsed
        };

        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });

        Console.WriteLine(json);
    }
}