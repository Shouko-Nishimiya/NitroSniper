using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web.ModelBinding;
using Discord;
using Discord.Gateway;
using Console = Colorful.Console;
using Colorful;
using Microsoft.SqlServer.Server;
using System.Linq;
using WebSocketSharp;

namespace NitroSniper
{
    class Program
    {

        public static DiscordSocketClient mainacc = new DiscordSocketClient();
        static void Main(string[] args)
        {
            DiscordSocketClient client = new DiscordSocketClient();
            client.OnMessageReceived += OnMessageReceived;

            string path = AppDomain.CurrentDomain.BaseDirectory + "tokens.txt";

            if (File.Exists(path))
            {
                string[] tokens = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "tokens.txt");

                if (tokens == null || tokens.Length < 1)
                {
                    Console.WriteLine("Please enter the tokens in tokens.txt", Color.Cyan);
                    Console.WriteLine("Press enter to exit...");
                    Console.ReadLine();
                }
                else
                {
                    try
                    {
                        client.Login(tokens[1]);
                        mainacc.Login(tokens[0]);
                    }
                    catch
                    {
                        Console.WriteLine("Please enter valid tokens in tokens.txt", Color.Cyan);
                        Console.WriteLine("Press enter to exit...");
                        Console.ReadLine();
                    }

                    Thread.Sleep(-1);
                }
            }
            else
            {
                File.Create(path);
                Console.WriteLine("Please enter the tokens in tokens.txt", Color.Cyan);
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
        }
        private static string redeemcode(string result, Stopwatch timer)
        {
            timer.Start();
            if (result.Contains("https://discord.gift/"))
            {
                result = result.Replace("https://discord.gift/", "");

            }
            else if (result.Contains("https://discord.com/gifts/"))
            {
                result = result.Replace("https://discord.com/gifts/", "");
            }
            string rstatus;
            try
            {
                mainacc.RedeemGift(result);
                timer.Stop();

                rstatus = "REDEEMED";
            }
            catch
            {
                try
                {
                    if (mainacc.GetNitroGift(result).Consumed == true)
                    {
                        rstatus = "ALREADY REDEEMED";
                    }
                    else
                    {
                        rstatus = "ERROR REDEEMING";
                    }
                }
                catch
                {
                    rstatus = "UNKNOWN ERROR";
                }
            }
            return rstatus;
        }

        private static void OnMessageReceived(DiscordSocketClient client, Discord.MessageEventArgs args)
        {
            Stopwatch timer = new Stopwatch();
            string message = args.Message.Content.ToString();
            if (message.Contains("https://discord.gift/") || message.Contains("https://discord.com/gifts/"))
            {
                string status = redeemcode(message, timer);

                string time = "0." + timer.ElapsedMilliseconds.ToString();

                string channel = "";
                string server = "";
                string user;
                if (client.GetChannel(args.Message.Channel.Id).Type == ChannelType.DM || client.GetChannel(args.Message.Channel.Id).Type == ChannelType.Group)
                {
                    if (client.GetChannel(args.Message.Channel.Id).Type == ChannelType.DM)
                    {
                        user = args.Message.Author.User.ToString();
                    } else
                    {
                        channel = client.GetChannel(args.Message.Channel.Id).ToGroup().Name;
                        user = args.Message.Author.User.ToString();
                    }
                }
                else
                {
                    channel = client.GetChannel(args.Message.Channel.Id).Name;
                    server = client.GetGuild(args.Message.Guild.Id).Name;
                    user = args.Message.Author.User.ToString();
                }

                StyleSheet styleSheet = new StyleSheet(Color.White);
                styleSheet.AddStyle("[\\[\\]]", Color.Cyan);
                styleSheet.AddStyle(time, Color.Yellow);
                styleSheet.AddStyle("Link: ", Color.Orange);
                styleSheet.AddStyle(message, Color.White);
                styleSheet.AddStyle("Server: ", Color.Orange);
                styleSheet.AddStyle("Channel: ", Color.Orange);
                styleSheet.AddStyle("User: ", Color.Orange);
                styleSheet.AddStyle("DM from user: ", Color.Orange);

                switch (status)
                {
                    case "REDEEMED":
                        styleSheet.AddStyle(status, Color.Lime);
                        break;
                    case "ALREADY REDEEMED":
                        styleSheet.AddStyle(status, Color.Red);
                        break;
                    case "ERROR REDEEMING":
                        styleSheet.AddStyle(status, Color.Yellow);
                        break;
                    case "UNKNOWN ERROR":
                        styleSheet.AddStyle(status, Color.Yellow);
                        break;
                }

                Console.WriteLineStyled("[" + time + "] " + "Link: " + message + " " + "[" + status + "]", styleSheet);

                if (client.GetChannel(args.Message.Channel.Id).Type == ChannelType.DM || client.GetChannel(args.Message.Channel.Id).Type == ChannelType.Group)
                {

                        Console.WriteLineStyled("DM from user: " + user, styleSheet);
                        Console.WriteLine();

                } else
                {
                    Console.WriteLineStyled("Server: " + server + " Channel: " + channel + " User: " + user, styleSheet);
                    Console.WriteLine();
                }
            }
        }
    }
}
