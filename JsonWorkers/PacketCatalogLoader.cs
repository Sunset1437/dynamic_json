using System.Text.Json;
using dynamic_json.Models;

namespace dynamic_json;

public static class PacketCatalogLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public static PacketCatalog Load(string json)
    {
        var catalog = JsonSerializer.Deserialize<PacketCatalog>(json, Options) ?? throw new InvalidOperationException("Не удалось распарсить каталог");
        Validate(catalog);
        return catalog;
    }

    public static PacketCatalog LoadFromFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Файл каталога не найден: {path}", path);

        return Load(File.ReadAllText(path));
    }

    private static void Validate(PacketCatalog catalog)
    {
        var ids = new HashSet<int>();
        foreach (var p in catalog.Packets)
        {
            if (!ids.Add(p.Id))
                throw new InvalidOperationException($"Дублирующийся id пакета: {p.Id}");

            if (string.IsNullOrWhiteSpace(p.Name))
                throw new InvalidOperationException($"У пакета id={p.Id} пустое имя");

            if (p.WordType is not ("byte" or "ushort"))
                throw new InvalidOperationException($"Пакет '{p.Name}': неизвестный wordtype '{p.WordType}'");

            ValidateFields(p.Fields, p.Name);
            ValidateGroups(p.Groups, p.Name);
        }
    }

    private static void ValidateFields(List<FieldDefinition> fields, string context)
    {
        foreach (var f in fields)
        {
            if (string.IsNullOrWhiteSpace(f.Name))
                throw new InvalidOperationException($"{context}: поле без имени");

            if (f.StartBit < 0 || f.StopBit < f.StartBit)
                throw new InvalidOperationException($"{context}.{f.Name}: некорректный диапазон {f.StartBit}..{f.StopBit}");
        }
    }

    private static void ValidateGroups(List<GroupDefinition> groups, string context)
    {
        var seen = new HashSet<string>();

        foreach (var g in groups)
        {
            if (string.IsNullOrWhiteSpace(g.Name))
                throw new InvalidOperationException($"{context}: у группы пустое имя");

            if (!seen.Add(g.Name))
                throw new InvalidOperationException($"{context}: дублирующееся имя группы '{g.Name}' на одном уровне");

            ValidateFields(g.Fields, $"{context}.{g.Name}");
            ValidateGroups(g.Groups, $"{context}.{g.Name}");
        }
    }
}