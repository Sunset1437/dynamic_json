namespace dynamic_json.Models;

public class FieldDefinition
{
    public string Name { get; set; } = "";
    public int StartBit { get; set; }   // включительно
    public int StopBit  { get; set; }   // включительно
}

public class GroupDefinition
{
    public string Name { get; set; } = "";
    public List<FieldDefinition> Fields { get; set; } = [];
    public List<GroupDefinition> Groups { get; set; } = []; 
}

public class PacketDefinition
{
    public int Id { get; set; }
    public string Dir { get; set; } = "";
    public string Name { get; set; } = "";
    public string WordType { get; set; } = "byte"; // "byte" | "ushort"
    public List<FieldDefinition> Fields { get; set; } = [];
    public List<GroupDefinition> Groups { get; set; } = []; 
}

public class PacketCatalog
{
    public List<PacketDefinition> Packets { get; set; } = [];
}