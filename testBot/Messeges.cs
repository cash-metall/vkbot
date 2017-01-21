using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.ObjectModel;
using VkNet;
using VkNet.Model.RequestParams;
using System.Speech.Synthesis;
using ConsoleBot;


namespace testBot
{
    class Messeges
    {
        VkApi api;
        public Messeges(VkApi api) { this.api = api; }
        BaDMan bot;

        public void Run()
        {
            bot = new BaDMan();
            bot.Initialize();
        }

        public int commands(string mess)
        {
            if (mess.Contains("#суперпостнастену")) return 3;
            if (mess.Contains("смысл") && mess.Contains("жизн")) return 2;
            if (mess.Contains("время")) return 1;

            //string[] patternadd = { "Добавь меня!|Го дружить|Добавь в друзья", "автор|Автор|Создатель|Создатель|Разработчик|разработчик", "курсові|дипломні|реферати|курсовые|дипломные|рефераты|подпишись|приєднуйся до|підпишись" };
            return 0;
        }

        public void checkmess(VkNet.Model.Message mes)
        {

          //  speech(mes.Body, (long)mes.UserId, (long)mes.Id);
            //Если хотите чтобы чисто озвучивало сообшения то отмечайте их как прочтенные
            //api.Messages.MarkAsRead((long)mes.Id); 

            string reply="not define";

            int cs = commands(mes.Body);
            try
            {
                switch (cs)
                {
                    case 2:
                        reply = "42";
                        break;
                    case 1:
                        reply = System.DateTime.Now.ToString();
                        break;
                    case 3:
                        reply = superpost(mes) ? "готово" : "ошибка!";
                        break;
                    default:
                        string ms = mes.Body;
                        ms = ms.Replace("пес", "");
                        reply = bot.getOutput(ms);
                        break;

                }
            }
            catch { Console.WriteLine("какая то ошибка. хз"); }
            

            Console.WriteLine("======================================");
            printmes(mes, true);
            Console.Write("ОТВЕТ: ");
           
            Console.WriteLine(reply);
            Console.WriteLine("======================================");
            if ((mes.ChatId > 0) && (!mes.Body.Contains("пес"))) return;
            if (mes.ContainsEmojiSmiles) sendmsg(reply, mes.ChatId, mes.UserId,1);
            else
            sendmsg(reply, mes.ChatId, mes.UserId);

        }

        public void printmes(VkNet.Model.Message a, bool f)
        {

            string body = "from id: "+a.FromId+"\nmessage: "+a.Body;
            if (f)
            {
                body += "\nAction: " + a.Action;
                body += "\nActionEmail: " + a.ActionEmail;
                body += "\nActionMid: " + a.ActionMid;
                body += "\nActionText: " + a.ActionText;
                body += "\nAdminID: " + a.AdminId;
                body += "\nchatID: " + a.ChatId;
                body += "\nsmile: " + a.ContainsEmojiSmiles.ToString();
                body += "\nID: " + a.Id;
                body += "\nOwnerID: " + a.OwnerId;
                body += "\nPhoto200: " + a.Photo200;
                body += "\nUserID: " + a.UserId;
                body += "\nID: " + a.UsersCount;
                foreach (VkNet.Model.Attachments.Attachment att in a.Attachments)
                {
                    body += "\nAttachment: [" + att.Type.ToString()+"] "+att.ToString();
                   // if (att.Type.ge)
                }
            }
            Console.WriteLine(body);

        }
        public void sendmsg(string str, long? chat, long? user, uint? smile=0)
        {
            try
            {
                if (chat>0)
                    api.Messages.Send(new MessagesSendParams { Message = str, ChatId = chat, StickerId=smile });
                else
                    api.Messages.Send(new MessagesSendParams { Message = str, UserId = user, StickerId = smile });
            }
            catch
            {
                Console.WriteLine("err of send msg");
            }

        }

        public bool superpost(VkNet.Model.Message a)
        {
            try
            {
                api.Wall.Post(new WallPostParams
                {
                    Message = a.Body,
                    OwnerId = api.UserId
                });
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void speech(string mess, long userid, long mesid)
        {
            SpeechSynthesizer speaker = new SpeechSynthesizer();
            var voiceList = speaker.GetInstalledVoices();
            speaker.SelectVoice(voiceList[0].VoiceInfo.Name);
            speaker.SetOutputToDefaultAudioDevice();
            speaker.Rate = 0;
            speaker.Volume = 100;
            VkNet.Model.User p = null;
            try
            {
                p = api.Users.Get(userid, VkNet.Enums.Filters.ProfileFields.Sex);
            }
            catch
            {
                Console.WriteLine("err of get username ");
            }
            string speechText = "Cообщение от ";
            speaker.SpeakAsync(speechText + p.FirstName + " " + p.LastName);
            speaker.SpeakAsync(mess);

        }
        public void MessagesRecieved(VkApi owner, ReadOnlyCollection<VkNet.Model.Message> messages)
        {
            foreach (var mes in messages)
            {
                if (mes.ReadState == VkNet.Enums.MessageReadState.Unreaded)
                {
                    Console.WriteLine("=======================");
                    if (mes.FromId != api.UserId)
                    {
                        checkmess(mes);
                        speech(mes.Body, (long)mes.UserId, (long)mes.Id);
                    }
                }
            }
        }
    }
}
