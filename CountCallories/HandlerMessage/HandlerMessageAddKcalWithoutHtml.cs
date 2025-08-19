using CountCallories.ClassObjects;
using CountCallories.DataAccessLayer.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CountCallories.HandlerMessage;

public class HandlerMessageAddKcalWithoutHtml(long userIdBefore)
{
    public async Task AddKkal(Message message, UpdateType update)
    {
        if (message.From!.Id != userIdBefore) return;
        
        Program.Bot.OnMessage -= AddKkal;
        Program.Bot.OnMessage += AnswerForCommand.BaseCommand;
        
        var textMessageArray = message.Text!.Split(' ');
        if (textMessageArray.Length == 4)
        {
            if (Int32.TryParse(textMessageArray[0], out int kkal) 
                && Int32.TryParse(textMessageArray[1], out int protein)
                && Int32.TryParse(textMessageArray[2], out int fats) 
                && Int32.TryParse(textMessageArray[3], out int carbohydrates))
            {
                var food = new EnergyValue(kkal, protein, carbohydrates, fats);
            
                await Program.Bot.SendMessage(message.From.Id,
                    $"Каллорий: {kkal} \nБелков: {protein} \nЖиров: {fats} \nУглеводов: {carbohydrates} \nВерно?",
                    replyMarkup: new InlineKeyboardButton[]
                    {
                        "Да",
                        "Нет, переделать"
                    });
                Program.Bot.OnUpdate += TempTestForAdd;
                
                Program.TempFood[$"Food{message.From.Id}"] = food;
            }
            else 
                await Program.Bot.SendMessage(message.From.Id, "Неверный ввод, попробуйте снова, нажав на кнопку.");
        }
        else
            await Program.Bot.SendMessage(message.From.Id, "Неверный ввод, попробуйте снова, нажав на кнопку.");
    }
    private async Task TempTestForAdd(Update update)
    {
        if (update.CallbackQuery!.From.Id != userIdBefore) return;
        
        if (update.CallbackQuery.Data == "Да")
        {
            var energyValue = Program.TempFood[$"Food{update.CallbackQuery.From.Id}"];
            var repository = new UsersRepository(Program.DbContextConnection);
            
            await repository.AddDay(update.CallbackQuery.From.Id, energyValue.Kcal, energyValue.Proteins, energyValue.Fats, energyValue.Carbohydrates);
            
            await Program.Bot.SendMessage(update.CallbackQuery.From.Id, "Успешно добавлено!");
        }
        else await Program.Bot.SendMessage(update.CallbackQuery.From.Id, "Попробуйте снова!");
        
        Program.Bot.OnUpdate -= TempTestForAdd;
        Program.TempFood.Remove($"Food{update.CallbackQuery.From.Id}");
    }
}