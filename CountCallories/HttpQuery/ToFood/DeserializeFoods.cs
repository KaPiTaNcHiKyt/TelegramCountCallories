using System.Text.Json.Serialization;

namespace CountCallories.HttpQuery;

public class DesirializeFoods
{
    public int count { get; set; }
    public int page { get; set; }
    public int page_count { get; set; }
    public int page_size { get; set; }
    public Products[] products { get; set; }
    public int skip { get; set; }
}

public class Products
{
    [JsonPropertyName("energy-kcal_100g")]
    public float energy_kcal_100g { get; set; }
    public float fat_100g { get; set; }
    public float proteins_100g { get; set; }
    public float carbohydrates_100g { get; set; }
}

