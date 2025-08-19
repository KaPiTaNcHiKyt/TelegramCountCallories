using CountCalloriesDataAccessLayer;
using CountCalloriesDataAccessLayer.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CountCallories.DataAccessLayer.Repositories;

public class UsersRepository(CountCalloriesContext dbContext)
{
    public async Task AddDay(long userId, int kcal, int proteins, int fats, int carbohydrates)
    {
        if (await IsToday(userId))
            await UpdateThisDay(userId, kcal,  proteins, fats, carbohydrates);
        else 
            await AddNewDay(userId, kcal, proteins, fats, carbohydrates);
    }
    
    public async Task<UserEntity?> GetToday(long userId)
    {
        var now = DateTime.Today.ToString("d");
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.DateWhenAdd == now && u.UserId == userId);
    }
    
    public async Task<List<UserEntity>> GetWeek(long userId)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.DateWhenAdd)
            .ToListAsync();
    }

    public async Task DeleteAccount(long userId)
    {
        await dbContext.Users
            .Where(u => u.UserId == userId)
            .ExecuteDeleteAsync();
    }

    private async Task AddNewDay(long userId, int kcal, int protein, int fats, int carbohydrates)
    {
        await Delete(userId);
        
        var userEntity = new UserEntity()
        {
            UserId = userId,
            DateWhenAdd = DateTime.Today.ToString("d"),
            Kcal = kcal,
            Protein = protein,
            Fats = fats,
            Carbohydrates = carbohydrates
        };
        await dbContext.Users.AddAsync(userEntity);
        await dbContext.SaveChangesAsync();
    }
    
    private async Task UpdateThisDay(long userId, int kcal, int protein, int fats, int carbohydrates)
    {
        string now = DateTime.Today.ToString("d");
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.UserId == userId && u.DateWhenAdd == now);
        user!.Kcal += kcal;
        user.Protein += protein;
        user.Fats += fats;
        user.Carbohydrates += carbohydrates;
        await dbContext.SaveChangesAsync();
    }

    private async Task<bool> IsToday(long userId)
    {
        string now = DateTime.Today.ToString("d");
        return await dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.UserId == userId && u.DateWhenAdd == now);
    }

    private async Task Delete(long userId)
    {
        var nowMinusSixDays = DateTime.Today.AddDays(-6);
        
        var thisUserWithAllDates = await dbContext.Users
            .Where(u => u.UserId == userId)
            .ToListAsync();
        
        var oldUserDates = from user in thisUserWithAllDates
            where DateTime.Parse(user.DateWhenAdd) < nowMinusSixDays
            select user;
        
        dbContext.Users
            .RemoveRange(oldUserDates);
        await dbContext.SaveChangesAsync();
    }
}