using Newtonsoft.Json.Linq;
using System;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks.Dataflow;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace TelegramChatBot
{
    internal class Program
    {
        private static string baseAddres = "https://realt.by/grodno-region/sale/flats/?page=";
        static List<string> idApartments = new List<string>();
        const string token = "5451929652:AAFyXfqTQSwSQyd4VG5Dtn6LeWpMM2PNvUE";
        static string baseCard = "https://realt.by/grodno-region/sale-flats/object/";

        static void Main(string[] args)
        { 
            TelegramBotClient botClient = new TelegramBotClient(token);
            botClient.StartReceiving(UpdateHandler,Errors);
            Console.ReadLine();    
        }

        async private static Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken token)
        {

            var message = update.Message;

            string messageText = message.Text;

            Console.WriteLine($"{message.Chat.FirstName} {message.Chat.LastName} | прислал вам новое сообщение с текстом | {messageText} ");

            switch (messageText.ToLower())
            {
                case "/start":
                    for (; ; )
                    {
                        for (int i = 1; i < 29; i++)
                        {
                            using (HttpClient cl = new HttpClient())
                            {
                                using (var response = cl.GetAsync(baseAddres + i).Result)
                                {
                                    var html = response.Content.ReadAsStringAsync().Result;
                                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                                    doc.LoadHtml(html);
                                    var apartments = doc.DocumentNode.SelectSingleNode("//script[@id='__NEXT_DATA__']");
                                    var jobj = JObject.Parse(apartments.InnerText);
                                    foreach (var item in jobj["props"]["pageProps"]["initialState"]["objectsListing"]["objects"])
                                    {
                                        if (!idApartments.Contains((item["code"]).ToString()))
                                        {
                                            botClient.SendTextMessageAsync(message.Chat.Id, baseCard + item["code"].ToString() + "/");
                                            idApartments.Add(item["code"].ToString());
                                        }
                                    }
                                }
                            }
                        }
                        Thread.Sleep(600000);
                    }
                    break;
            }
        }
        private static Task Errors(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}