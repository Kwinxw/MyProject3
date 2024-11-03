using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Concurrent;

var userNames = new ConcurrentDictionary<long, string>();
using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient("7942033335:AAFq6_r9lL0rUFjxkIOM74BCO3LQmGMGZHM", cancellationToken: cts.Token); //сам бот @Kuhnya_holostyaka_bot
var me = await bot.GetMeAsync();

bot.OnMessage += OnMessage;
bot.OnUpdate += OnUpdate;

Console.WriteLine($"@{me.Username} запущен... Нажмите Enter чтобы выключить");
Console.ReadLine();
cts.Cancel();

async Task OnMessage(Message msg, UpdateType type)
{
    if (!userNames.ContainsKey(msg.Chat.Id))
    {
        if (msg.Text == "/start")
        {
            await bot.SendTextMessageAsync(msg.Chat.Id, "Введите ваше имя:");
            return;
        }

        userNames[msg.Chat.Id] = msg.Text;
        await bot.SendTextMessageAsync(msg.Chat.Id, $"Здравствуйте, {msg.Text}! Выберите действие:",
            replyMarkup: new InlineKeyboardMarkup().AddButtons("Рецепты", "Базовые задачи бота"));
        return;
    }
    if (!userNames.ContainsKey(msg.Chat.Id))
    { 

        var name = msg.Text.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            await bot.SendTextMessageAsync(msg.Chat, "Пожалуйста, введите свое настоящее имя)");
            return;
        }
        userNames[msg.Chat.Id] = name;
    }
    else
    {
        string userName = userNames[msg.Chat.Id];

        switch (msg.Text)
        {
            case "Привет":
                await bot.SendTextMessageAsync(msg.Chat, $"Здравствуйте, {userName}!");
                break;
            case "Картинка":
                await bot.SendPhotoAsync(msg.Chat, "https://avatars.mds.yandex.net/get-mpic/4113189/2a0000018f81c64e620ee265e2b9f7643df5/orig");
                break;
            case "Видео":
                await bot.SendVideoAsync(msg.Chat, "https://telegrambots.github.io/book/docs/video-countdown.mp4");
                break;
            case "Стикер":
                await bot.SendStickerAsync(msg.Chat, "https://telegrambots.github.io/book/docs/sticker-fred.webp");
                break;
            default:
                await bot.SendTextMessageAsync(msg.Chat, $"В моем функционале нет данных об этом вводе(");
                break;
        }
    }
}

async Task OnUpdate(Update update)
{
    if (update is { CallbackQuery: { } query })
    {
        string userName = userNames.TryGetValue(query.From.Id, out var name) ? name : "пользователь";
        await bot.AnswerCallbackQueryAsync(query.Id, $"Вы выбрали: {query.Data}");

        switch (query.Data)
        {
            case "Рецепты":
                await bot.SendTextMessageAsync(query.From.Id, "Какой рецепт выберите, легкий или сложный?",
                    replyMarkup: new InlineKeyboardMarkup().AddButtons("Легкий", "Сложный"));               
                break;

            case "Легкий":
                await bot.SendTextMessageAsync(query.From.Id, "Рецепт новичка: Нам понадобится лапша быстрого приготовления и кетчуп, поставим ее варить на 5 минут, потом выложим лапшу в тарелку и заправим ее кетчупом, добавим соли и перца (если нужно)");
                break;

            case "Сложный":
                await bot.SendTextMessageAsync(query.From.Id, "Рецепт профи: Нам понадобится лапша быстрого приготовления, специи и яица, варим яица и лапшу 10 минут отдельно, в лапшу добавить специй и вылить в посуду лапшу с бульеном от нее, нарезать яица на 2 части и положить к лапше, под конец добавить кетчупа и еда богов станет вкуснее!");
                break;

            case "Базовые задачи бота":
                await bot.SendTextMessageAsync(query.From.Id, $"Здравствуйте, {userName}!");
                await bot.SendPhotoAsync(query.From.Id, "https://www.rosconmarket.ru/upload/adwex.minified/webp/803/65/8036f3b5608f8d80b63c54f90ee22396.webp");
                await bot.SendVideoAsync(query.From.Id, "https://telegrambots.github.io/book/docs/video-countdown.mp4");
                await bot.SendStickerAsync(query.From.Id, "https://telegrambots.github.io/book/docs/sticker-fred.webp");
                await bot.SendTextMessageAsync(query.From.Id, "На чьей стороне твикс ты?",
                    replyMarkup: new InlineKeyboardMarkup().AddButtons("Левая", "Правая"));
                break;

            case "Левая":
                await bot.SendTextMessageAsync(query.From.Id, $"{userName}, ты лев(львица)!");
                break;

            case "Правая":
                await bot.SendTextMessageAsync(query.From.Id, $"{userName}, ты прав(а)!");
                break;
        }
    }
}