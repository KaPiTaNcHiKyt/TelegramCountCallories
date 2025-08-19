using CountCallories.DataAccessLayer.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CountCallories.HandlerMessage;

public class HandlerMessageDeleteAccount(long userIdBefore)
{
    public async Task DeleteMessage(Update update)
    {
        if (update.CallbackQuery is null || update.CallbackQuery.From.Id != userIdBefore) return;
        
        Program.Bot.OnUpdate -= DeleteMessage;

        switch (update.CallbackQuery.Data)
        {
            case "Удалить":
            {
                var repository = new UsersRepository(Program.DbContextConnection);
            
                await repository.DeleteAccount(update.CallbackQuery.From.Id);
                break;
            }
            case "Оставить":
                await Program.Bot.SendMessage(update.CallbackQuery.From.Id, "Хорошего пользования)");
                break;
        }
    }
}