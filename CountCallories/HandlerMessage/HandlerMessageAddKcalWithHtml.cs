using CountCallories.DataAccessLayer.Repositories;
using CountCallories.HttpQuery;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CountCallories.HandlerMessage;

public class HandlerMessageAddKcalWithHtml(long userIdBefore)
{
    public async Task GetMessageWithFoodAndWeight(Message message, UpdateType updateType)
    {
        if (message.From!.Id != userIdBefore) return;
        
        Program.Bot.OnMessage -= GetMessageWithFoodAndWeight;
        Program.Bot.OnMessage += AnswerForCommand.BaseCommand;
        
        var textMessageArray = message.Text!.Split(' ');
        if (textMessageArray.Length == 2)
        {
            if (int.TryParse(textMessageArray[1], out int weight))
            {
                var nutriments100G = await GetFromHttpFood.GetNutriments100G(textMessageArray[0]);
                if (nutriments100G is null or { Fats: 0, Proteins: 0, Carbohydrates: 0, Kcal: 0 })
                    await Program.Bot.SendMessage(message.From.Id, "По вашему запросу ничего не найдено");

                Program.TempFood[$"Food{message.From.Id}"] = nutriments100G!;
                
                await Program.Bot.SendMessage(message.From.Id,
                    $"Добавляем {nutriments100G!.Kcal} ккал\n" +
                    $"{nutriments100G.Proteins} белков\n" +
                    $"{nutriments100G.Fats} жиров\n" +
                    $"{nutriments100G.Carbohydrates} углеводов",
                    replyMarkup: new InlineKeyboardButton[]
                    {
                        "Да",
                        "Нет"
                    });
                Program.Bot.OnUpdate += TempTestForAdd;
            }
            else 
                await Program.Bot.SendMessage(message.From.Id, "Неверный ввод, попробуйте снова, нажав на кнопку 1.");
        }
        else
            await Program.Bot.SendMessage(message.From.Id, "Неверный ввод, попробуйте снова, нажав на кнопку 2.");
    }

    private async Task TempTestForAdd(Update update)
    {
        if (update.CallbackQuery is null || update.CallbackQuery.From.Id != userIdBefore) return;
        
        if (update is { CallbackQuery.Data: "Да" })
        {
            var repository = new UsersRepository(Program.DbContextConnection);
            var nutriments = Program.TempFood[$"Food{update.CallbackQuery.From.Id}"];

            await repository.AddDay(update.CallbackQuery.From.Id,
                nutriments.Kcal,
                nutriments.Proteins,
                nutriments.Fats,
                nutriments.Carbohydrates);
            
            await Program.Bot.SendMessage(update.CallbackQuery.From.Id, "Успешно добавлено");
        }
        else if (update is { CallbackQuery.Data: "Нет" })
        {
            await Program.Bot.SendMessage(update.CallbackQuery.From.Id, "Попробвуйте снова, нажав на кнопку.");
        }
        
        Program.TempFood.Remove($"Food{update.CallbackQuery.From.Id}");
        Program.Bot.OnUpdate -= TempTestForAdd;
    }
}