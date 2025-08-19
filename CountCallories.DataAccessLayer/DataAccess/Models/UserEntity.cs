namespace CountCalloriesDataAccessLayer.DataAccess;

public class UserEntity
{
    public long UserId { get; set; }
    public string DateWhenAdd { get; set; }
    public int Kcal { get; set; }
    public int Protein { get; set; }
    public int Fats { get; set; }
    public int Carbohydrates { get; set; }
}