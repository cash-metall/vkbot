using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 

using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;

using VkNetExtend;
using VkNetExtend.Wall;

namespace testBot
{
    class Program
    {
        
        static void Main(string[] args)
        {
            int appID = 5747420;                      //app id
            var vk = new VkApi();
            while (true)
            {
                Console.Write("LOG: ");
                string email = Console.ReadLine();    // email or phone
                string pass;
                if (email != "test")
                {
                    Console.Write("PWD: ");
                     pass = Console.ReadLine(); // password
                }
                else
                {
                    List<string> conf = File.ReadAllLines("../../../../config.txt").ToList(); 
                    email = conf[0]; 
                    pass = conf[1]; 
                }
                Settings scope = Settings.All;
                ApiAuthParams my = new ApiAuthParams
                {
                    ApplicationId = (ulong)appID,
                    Login = email,
                    Password = pass,
                    Settings = scope
                };

                //авторизация
                try
                {
                    vk.Authorize(my);
                    Console.Clear();
                    break;
                }
                catch(Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.Message);
                }
            }


            /* Наверное нельзя юзать все 2 сразу 
            *  Ибо ошибка в апи будет со временем
            *  Там мона сделать чтобы не было
            *  Но мне и так норм */
            VkNetExtend.LongPoolWatcher vk2=new VkNetExtend.LongPoolWatcher(vk);
            vk2.StartAsync();
            
            
           var message = new Messeges(vk);
            message.Run();

            vk2.NewMessages += message.MessagesRecieved;


            // new botmsg(vk).run();

            // new Friends(vk).Run();


            while (true) if (Console.ReadKey().Key == ConsoleKey.Escape) break;

        }
        
    }
}
