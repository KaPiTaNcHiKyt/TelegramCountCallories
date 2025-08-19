using CountCallories.ClassObjects;
using CountCallories.DataAccessLayer.Repositories;
using CountCallories.HandlerMessage;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using CountCalloriesDataAccessLayer;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CountCallories;

public class Program
{
    private static readonly CancellationTokenSource _cts = new();
    public static TelegramBotClient Bot { get; private set; }
    private static string ConnectionString { get; set; }
    public static Dictionary<string, EnergyValue> TempFood { get; set; } = new();
    public static CountCalloriesContext DbContextConnection { get; private set; } = null!;

    public async static Task Main(string[] args)
    {
        var strings = InteractionWithJson.GetStringsFromJson.GetStrings();
        
        Bot = new TelegramBotClient(strings.BotKeyConnection);
        ConnectionString =  strings.DefaultConnection;

        var dbContextOptions = new DbContextOptionsBuilder<CountCalloriesContext>();
        dbContextOptions.UseNpgsql(ConnectionString);
        var options = dbContextOptions.Options;
        DbContextConnection = new(options);
        
        var me = Bot.GetMe();

        Bot.OnMessage += FirstMessage;

        Console.WriteLine($"{me.Result.Username} запущен. Нажмите enter чтобы закрыть бота.");
        Console.ReadLine();
        _cts.Cancel();
    }
    
    public static async Task FirstMessage(Message message, UpdateType update)
    {
        if (message.Text == "/start")
        {
            await Bot.SendMessage(message.From.Id, "Привет. Этот бот поможет тебе следить за " +
                                                           "каллориями и, соответственно, за фигурой.");
            await Bot.SendMessage(message.From.Id, "С сегодняшнего дня я начинаю считать твои калории. " +
                                                           "Чтобы добавить съеденные тобой калории нажми на кнопку ниже.",
                replyMarkup: new KeyboardButton[][]
                {
                    new KeyboardButton[]
                    {
                        "Ввести название блюда",
                        "Добавить калории к списку"
                    },
                    new KeyboardButton[]
                    {
                        "Сколько калорий я съел за сегодня",
                        "Сколько калорий я съел за эту неделю"
                    }
                });
            Bot.OnMessage -= FirstMessage;
            Bot.OnMessage += AnswerForCommand.BaseCommand;
            Bot.OnMessage += AnswerForCommand.SpecialCommand;
        }
    }
}