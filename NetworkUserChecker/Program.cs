using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Newtonsoft.Json;

namespace NetworkUserChecker
{
    class Program
    {
        static void Main()
        {
            List<string> adresses = Parse().GetAwaiter().GetResult();
            List<User> users = new List<User>();
            users = Read();

            if (users == null || adresses == null) return;

            users = Fetch(users, adresses);
            WriteToConsole(users);
            Console.ReadKey();
        }

        static async Task<List<string>> Parse()
        {
            string source = "";
            try
            {
                source = File.ReadAllText(@"D:\Prog\report.html");
            }
            catch
            {
                Console.WriteLine("Can't read file with adresses data");
                return null;
            }

            IConfiguration config = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(config);

            IDocument document = await context.OpenAsync(req => req.Content(source));
            IHtmlCollection<IElement> Users = document.QuerySelectorAll("td:nth-child(0n+3) ");
            List<string> adresses = new List<string>();

            foreach (IElement e in Users)
            {
                adresses.Add(e.Text());
            }

            return adresses;
        }

        static List<User> Read()
        {
            string input = "";
            try
            {
                input = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "UserList.json");
                if (input == "")
                {
                    return new List<User>();
                }
                else
                {
                    return JsonConvert.DeserializeObject<List<User>>(input);
                }
            }
            catch
            {
                try
                {
                    File.Create(AppDomain.CurrentDomain.BaseDirectory + "UserList.json");
                    return null;
                }
                catch
                {
                    Console.WriteLine("Can't Create file UserList.json");
                }
                Console.WriteLine("Can't Read User list");
                return null;
            }
        }

        static List<User> Fetch(List<User> users, List<string> Adresses)
        {
            if (users.Count == 0)
            {
                for (int i = 0; i < Adresses.Count; i++)
                {
                    users.Add(new User("", Adresses[i]));
                }
            }
            else
            {
                foreach (string adress in Adresses)
                {
                    if (!users.Exists(user => user.Adress == adress))
                    {
                        users.Add(new User("", adress));
                    }
                }
            }

            WriteToJson(users);

            for (int i = 0; i < users.Count; i++)
            {
                if (Adresses.Contains(users[i].Adress)) users[i].OnlineState = true;
                else users[i].OnlineState = false;
            }

            return users;
        }

        static void WriteToConsole(List<User> users)
        {
            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine($"{users[i].Name}\t{users[i].Adress}\t{users[i].getOnlineStatus()}");
            }
        }

        static void WriteToJson(List<User> users)
        {
            string output = JsonConvert.SerializeObject(users);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "UserList.json", output);
        }
    }
}