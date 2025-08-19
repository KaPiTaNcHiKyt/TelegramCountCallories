namespace CountCallories.ClassObjects;

public class EnergyValue(int kcal, int proteins, int carbohydrates, int fats)
{
    public int Kcal { get; set; } = kcal;
    public int Proteins { get; set; } = proteins;
    public int Carbohydrates { get; set; } = carbohydrates;
    public int Fats { get; set; } = fats;
}