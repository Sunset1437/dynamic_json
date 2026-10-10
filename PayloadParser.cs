using dynamic_json;
using dynamic_json.Models;

public static class PayloadParser
{
    public static Dictionary<string, object> Parse(PacketDefinition packet, byte[] payload)
    {
        var root = new Dictionary<string, object>();

        //поля пакета — под ключом с именем пакета
        if (packet.Fields.Count > 0)
        {
            var packetNode = new Dictionary<string, object>();
            foreach (var f in packet.Fields)
                packetNode[f.Name] = ReadField(f, payload);

            root[packet.Name] = packetNode;
        }

        // все группы пакета — отдельными ключами
        foreach (var g in packet.Groups)
            root[g.Name] = BuildGroupNode(g, payload);

        return root;
    }

    private static Dictionary<string, object> BuildGroupNode(GroupDefinition group, byte[] payload)
    {
        var node = new Dictionary<string, object>();

        foreach (var f in group.Fields)
            node[f.Name] = ReadField(f, payload);

        foreach (var child in group.Groups)
            node[child.Name] = BuildGroupNode(child, payload);

        return node;
    }

    private static object ReadField(FieldDefinition f, byte[] payload)
    {
        int bitCount = f.StopBit - f.StartBit + 1;
        ulong raw = BitFieldReader.Extract(payload, f.StartBit, f.StopBit);
        return ToValue(raw, bitCount);
    }

    private static object ToValue(ulong raw, int bitCount)
    {
        return bitCount switch
        {
            <= 8 => (byte)raw,
            <= 16 => (ushort)raw,
            _ => raw
        };
    }
}