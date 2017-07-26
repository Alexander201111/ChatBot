using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatBotDirtyJhonny
{
    public partial class Form1 : Form
    {
        Dictionary<int, string> greetings = new Dictionary<int, string>(100);
        BackgroundWorker bw;
        bool flag = true;
        public Form1()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //

            this.bw = new BackgroundWorker();
            this.bw.DoWork += bw_DoWork;
            greetings.Add(1, "Привет");
            greetings.Add(3, "привет");
            greetings.Add(2, "Hello");
            greetings.Add(4, "Hi");
            greetings.Add(5, "hello");
            greetings.Add(6, "Приффки");
        }

        async void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var key = e.Argument as String; // получаем ключ из аргументов
            try
            {
                var Bot = new Telegram.Bot.TelegramBotClient(key); // инициализируем API
                await Bot.SetWebhookAsync(""); // Обязательно! убираем старую привязку к вебхуку для бота
                await Bot.SendTextMessageAsync(191116543, "Привет! Я онлайн.");


                // Inlin'ы
                Bot.OnInlineQuery += async (object si, Telegram.Bot.Args.InlineQueryEventArgs ei) =>
                {
                    var query = ei.InlineQuery.Query;

                    var msg = new Telegram.Bot.Types.InputMessageContents.InputTextMessageContent
                    {
                        MessageText = @"test",
                        ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html,
                    };


                    Telegram.Bot.Types.InlineQueryResults.InlineQueryResult[] results = {
                        new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle
                        {
                            Id = "1",
                            Title = "Test title",
                            Description = "Test description",
                            InputMessageContent = msg,
                        },
                        new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultAudio
                        {
                            Url = "http://tesetpath.ru/",
                            Id = "2",
                            FileId = "123423433",
                            Title = "Test title",
                        },
                        new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultPhoto
                        {
                            Id = "3",
                            Url = "test url",
                            ThumbUrl = "test thumb",
                            Caption = "Text under the photo",
                            Description = "Description",
                        },
                        new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultVideo
                        {
                            Id = "4",
                            Url = "test url",
                            ThumbUrl = "test thumb",
                            Title = "Text",
                            MimeType = "video/mp4",
                        }
                    };


                    await Bot.AnswerInlineQueryAsync(ei.InlineQuery.Id, results);
                };

                // Callback'и от кнопок
                Bot.OnCallbackQuery += async (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
                {
                    var message = ev.CallbackQuery.Message;
                    if (ev.CallbackQuery.Data == "callback1")
                    {
                        await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Test " + ev.CallbackQuery.Data, true);
                    }
                    else
                    if (ev.CallbackQuery.Data == "callback2")
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Test");
                        await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id); // отсылаем пустое, чтобы убрать "частики" на кнопке
                    }
                };

                Bot.OnUpdate += async (object su, Telegram.Bot.Args.UpdateEventArgs evu) =>
                {
                    if (evu.Update.CallbackQuery != null || evu.Update.InlineQuery != null) return; // в этом блоке нам келлбэки и инлайны не нужны
                    var update = evu.Update;
                    var message = update.Message;
                    if (message == null) return;
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
                    {
                        if (greetings.ContainsValue(message.Text))
                        {
                            // в ответ на приветствие из словаря выводим сообщение
                            Random rnd = new Random();
                            int ran = rnd.Next(1, 7);
                            await Bot.SendTextMessageAsync(message.Chat.Id, greetings[ran] + ", " + message.Chat.FirstName + "=)");
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Ты наверное хочешь узнать кто я такой? Так вот, меня зовут... Грязный Джонни. Я обычный телеграм бот, который только учится. Поэтому я вас прошу, не пишите сюда всякие гадости, а то будет вам \"а-тя-тя\".");
                            //to reply u need add this ", replyToMessageId: message.MessageId" to SendTextMessageAsync 
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Простите, я пока вас не понимаю=(");
                        }

                        //if (message.Text == "/getimage")
                        //{
                        //    // в ответ на команду /getimage выводим картинку
                        //    await Bot.SendPhotoAsync(message.Chat.Id, "picture url", "description");
                        //}

                        // inline buttons
                        //if (message.Text == "/ibuttons")
                        //{
                        //    var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                        //                            new Telegram.Bot.Types.InlineKeyboardButton[][]
                        //                            {
                        //                                    // First row
                        //                                    new [] {
                        //                                        // First column
                        //                                        new Telegram.Bot.Types.InlineKeyboardButton("test1","callback1"),

                        //                                        // Second column
                        //                                        new Telegram.Bot.Types.InlineKeyboardButton("test2","callback2"),
                        //                                    },
                        //                            }
                        //                        );

                        //    await Bot.SendTextMessageAsync(message.Chat.Id, "test msg", false, false, 0, keyboard, Telegram.Bot.Types.Enums.ParseMode.Default);
                        //}

                        // reply buttons
                        //if (message.Text == "/rbuttons")
                        //{
                        //    var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup
                        //    {
                        //        Keyboard = new[] {
                        //                        new[] // row 1
                        //                        {
                        //                            new Telegram.Bot.Types.KeyboardButton("test1"),
                        //                            new Telegram.Bot.Types.KeyboardButton("test2")
                        //                        },
                        //                    },
                        //        ResizeKeyboard = true
                        //    };

                        //    await Bot.SendTextMessageAsync(message.Chat.Id, "test msg", false, false, 0, keyboard, Telegram.Bot.Types.Enums.ParseMode.Default);
                        //}
                        //// обработка reply кнопок
                        //if (message.Text.ToLower() == "test1")
                        //{
                        //    await Bot.SendTextMessageAsync(message.Chat.Id, "Test11", replyToMessageId: message.MessageId);
                        //}
                        //if (message.Text.ToLower() == "test2")
                        //{
                        //    await Bot.SendTextMessageAsync(message.Chat.Id, "Test22", replyToMessageId: message.MessageId);
                        //}
                    }
                };

                // запускаем прием обновлений
                Bot.StartReceiving();
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                Console.WriteLine(ex.Message); // если ключ не подошел - пишем об этом в консоль отладки
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!flag)
            {
                Close();
            }
            this.bw.RunWorkerAsync(""); // передаем этот token в виде аргумента методу bw_DoWork, token можете узнать http://vk.com/madmix37
            this.flag = false;
            button1.Text = "Stop Bot";
        }

        
    }
}
