using CountCallories.DataAccessLayer.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CountCallories.HandlerMessage;

public static class AnswerForCommand
{
    public static async Task BaseCommand(Message message, UpdateType update)
    {
        switch (message.Text)
        {
            case "Ввести название блюда":
                await AddKcalWithHtml(message.From!.Id);
                return;
            
            case "Добавить калории к списку":
                await AddKcalWithoutHtml(message.From!.Id);
                return;
            
            case "Сколько калорий я съел за сегодня":
                await GetToday(message.From!.Id);
                return;
            
            case "Сколько калорий я съел за эту неделю":
                await GetWeek(message.From!.Id);
                return;
        }
    }

    public static async Task SpecialCommand(Message message, UpdateType update)
    {
        switch (message.Text)
        {
            case "/start":
                await Program.Bot.SendMessage(message.From!.Id, "Выберите кнопку ниже");
                break;
            case "/delete":
                await Program.Bot.SendMessage(message.From!.Id, "Удалить все данные о вашем аккаунте?",
                    replyMarkup: new InlineKeyboardButton[]
                    {
                        "Удалить",
                        "Оставить"
                    });
                var messageDeleteAccount = new HandlerMessageDeleteAccount(message.From!.Id);
                Program.Bot.OnUpdate += messageDeleteAccount.DeleteMessage;
                return;
        }
    }

    private static async Task AddKcalWithHtml(long userId)
    {
        await Program.Bot.SendMessage(userId, 
            "Пришлите пожалуйста название блюда (желательно на английском) и его примерный вес в граммах через пробел.");
        var addWithHtml = new HandlerMessageAddKcalWithHtml(userId);
        Program.Bot.OnMessage += addWithHtml.GetMessageWithFoodAndWeight;
        Program.Bot.OnMessage -= BaseCommand;
    }

    private static async Task AddKcalWithoutHtml(long userId)
    {
        var nextMessageHandler = new HandlerMessageAddKcalWithoutHtml(userId);
        await Program.Bot.SendMessage(userId,
            "Пришлите мне следующим сообщением через пробел, сколько " +
            "\"каллорий\" \"белков\" \"жиров\" и \"углеводов\" вы съели.");   
                
        Program.Bot.OnMessage += nextMessageHandler.AddKkal;
        Program.Bot.OnMessage -= BaseCommand;
    }

    private static async Task GetToday(long userId)
    {
        var repository = new UsersRepository(Program.DbContextConnection);
        var resultUser = await repository.GetToday(userId);
        
        if (resultUser is not null)
        {
            await Program.Bot.SendMessage(userId, $"Вы за сегодня съели:\n" +
                                                  $"{resultUser.Kcal} каллорий,\n" +
                                                  $"{resultUser.Protein} белков,\n" +
                                                  $"{resultUser.Fats} жиров,\n" +
                                                  $"{resultUser.Carbohydrates} углеводов");
        }
        else
            await Program.Bot.SendMessage(userId, "Вы сегодня ещё не добавляли");
    }
    
    private static async Task GetWeek(long userId)
    {
        var repository = new UsersRepository(Program.DbContextConnection);
        var resultUser = await repository.GetWeek(userId);
        
        var answerText = resultUser.Aggregate(string.Empty, (current, next) => current + $"{next.DateWhenAdd} - Ккал: {next.Kcal}, " +
                                                                   $"Белков: {next.Protein}, Жиров: {next.Fats}, Углеводов: {next.Carbohydrates}.\n");

        await Program.Bot.SendMessage(userId, answerText);
    }
}