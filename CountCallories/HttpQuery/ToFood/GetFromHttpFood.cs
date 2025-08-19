using System.Net.Http.Json;
using CountCallories.ClassObjects;

namespace CountCallories.HttpQuery;

public static class GetFromHttpFood
{
    private static readonly HttpClient HttpClientObject;

    static GetFromHttpFood()
    {
        HttpClientObject = new HttpClient
        {
            BaseAddress = new Uri("https://world.openfoodfacts.org/api/v2/search")
        };
    }
    
    public static async Task<EnergyValue?> GetNutriments100G(string query)
    {
        var response =
            await HttpClientObject.GetAsync(
                $"?categories_tags={query}&fields=carbohydrates_100g,energy-kcal_100g,fat_100g,proteins_100g");
        if (!response.IsSuccessStatusCode) return null;
        
        var foods = await response.Content.ReadFromJsonAsync<DesirializeFoods>();

        return GetMeanNutriments100G(foods!);
    }

    private static EnergyValue GetMeanNutriments100G(DesirializeFoods foods)
    {
        var nutriments = new EnergyValue(0, 0, 0, 0);
        int length = foods.products.Length;
        
        nutriments.Kcal = (int)((from nutriment in foods.products
            select nutriment.energy_kcal_100g).Aggregate((current, next) => current + next) / length);
        
        nutriments.Proteins = (int)((from nutriment in foods.products
            select nutriment.proteins_100g).Aggregate((current, next) => current + next) / length);
        
        nutriments.Carbohydrates = (int)((from nutriment in foods.products
            select nutriment.carbohydrates_100g).Aggregate((current, next) => current + next) / length); 
        
        nutriments.Fats = (int)((from nutriment in foods.products
            select nutriment.fat_100g).Aggregate((current, next) => current + next) / length);

        return nutriments;
    }
}