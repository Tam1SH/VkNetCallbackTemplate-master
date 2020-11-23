using Microsoft.Extensions.Configuration;
using System;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;
using System.Collections.Generic;

namespace VkBot.Controllers
{
    public partial class MainLogic
    {
        private static MainLogic instance;

        private Dictionary<Context, CallbackController.VKHandler> EndCommand;

        private Dictionary<Tuple<Context, Level>, Func<Message, bool>> CorrectAnswers;

        static private Dictionary<long, User> Users;

        private Dictionary<string, CallbackController.VKHandler> commands;

        private Dialog _text;

        private readonly IVkApi _vkApi;

        public static MainLogic GetInstance(IVkApi vkApi)
        {
            if (instance == null)
                instance = new MainLogic(vkApi);
            return instance;
        }

        private void init()
        {
            commands = new Dictionary<string, CallbackController.VKHandler>();
            Users = new Dictionary<long, User>();
            CorrectAnswers = new Dictionary<Tuple<Context, Level>, Func<Message, bool>>();
            EndCommand = new Dictionary<Context, CallbackController.VKHandler>();
            _text = new Dialog();
            Setup();
        }

        private bool IsCorrectAnswer(Tuple<Context, Level> tuple, Message answer)
        {
            return CorrectAnswers[tuple].Invoke(answer);
        }
        private Level FindMaxLevel(Context context)
        {
            Level max = Level.Zero;
            foreach (var item in _text.Body.Keys)
            {
                if (item.Item2 > max && item.Item1 == context)
                    max = item.Item2;
            }

            return max;
        }

        private MainLogic(IVkApi vkApi)
        {
            _vkApi = vkApi;
            Console.WriteLine("создался объект логики");
            init();
            LogicSetup();
        }
        private void LogicSetup()
        {
            CallbackController.VKMessageAction_New += CallbackController_VKMessageAction_New;
        }
        private void SelectContext(Message msg, Context context)
        {
            long id = (long)msg.FromId;
            long peerId = (long)msg.PeerId;
            Users[id].CurrentLevel = Level.One;
            Users[id].context = context;
            SendMessage(_text.Body[Users[id].key], peerId);
        }

        private void CommandBody(Context context, Message msg)
        {
            EndCommand[context].Invoke(msg);
        }
        /// <summary>
        /// Команда, которая будет выполнятся в конце диалога
        /// </summary>
        private void BodyCommandAdd(Context context, CallbackController.VKHandler vKHandler)
        {
            EndCommand.Add(context, vKHandler);
        }
        /// <summary>
        /// Сообщения пользователя, на которую будет реагировать бот. 
        /// 
        /// Выбор контекста для диалога, либо выполнение команды
        /// </summary>
        private void TextCommand(string command, CallbackController.VKHandler vKHandler)
        {
            commands.Add(command, vKHandler);
        }
        /// <summary>
        /// Проверка сообщения от пользователя на контекстное соответствие
        /// </summary>
        private void PredicateAdd(Tuple<Context, Level> tuple, Func<Message, bool> CheckForCorrect)
        {
            CorrectAnswers.Add(tuple, CheckForCorrect);
        }
        /// <summary>
        /// Текст для пользователя
        /// </summary>
        private void TextCreate(Tuple<Context, Level> tuple, string text)
        {
            _text.Body.Add(tuple, text);

        }
        private void ContextAnswer(Message x)
        {
            long id = (long)x.FromId;
            long peerId = (long)x.PeerId;
            //SendMessage("Ответ, выбранный тобой, пидрила: " + x.Text, x.PeerId.Value);

            if (IsCorrectAnswer(Users[id].key, x))
            {

                Users[id].Answer.Add(x.Text);
                Users[id].CurrentLevel++;
                //SendMessage("Ответ корректный", peerId);
                if (FindMaxLevel(Users[id].context) <= Users[id].CurrentLevel)
                {
                    CommandBody(Users[id].context, x);
                    CreateUser(x, false);
                }
                else
                {
                    SendMessage(_text.Body[Users[id].key], peerId);
                }
            }
            else
            {
                CreateUser(x, false);
                SendMessage("Некорректный ввод", peerId);
            }
            //SendMessage("текущий уровень: " + dialogs[id].CurrentLevel + " максимальный уровень: " + FindMaxLevel(dialogs[id].context), x.PeerId.Value);
        }

        private void CreateUser(Message msg, bool IsCreated)
        {
            if (IsCreated)
            {
                Users.Add(msg.UserId.Value, new User());
            }
            else
            {
                Users[msg.UserId.Value] = null;
                Users.Remove(msg.UserId.Value);
            }
        }
        private void Run(Message msg)
        {
            CreateUser(msg, true);

            if (commands.ContainsKey(msg.Text.ToLower()) && 
                Users[msg.FromId.Value].CurrentLevel == Level.Zero)
                commands[msg.Text.ToLower()].Invoke(msg);

            else if(Users[msg.FromId.Value].CurrentLevel > Level.Zero)
                ContextAnswer(msg);






















        }

        private void Debug(Message msg)
        {
            SendMessage($@" 
                        Идентификатор беседы = {msg?.PeerId}
                        Идентификатор сообщения = {msg?.ConversationMessageId} 
                        Идентификатор автора 1 = {msg?.FromId}
                        Идентификатор автора 2 = {msg?.UserId}
                        Текущий уровень в диалоге =  {Users[msg.FromId.Value].CurrentLevel}
                        Текущий контекст в диалоге = {Users[msg.FromId.Value].context}
                        Максимальный уровень в диалоге = {FindMaxLevel(Users[msg.FromId.Value].context)}
                        Тело сообщения = '{msg?.Text}'
                        ", msg.PeerId.Value);
        }

        private void SendMessage(string text, long id)
        {
            try
            {
                _vkApi.Messages.Send(new MessagesSendParams
                {
                    RandomId = new Random().Next(),
                    PeerId = id,
                    Message = text,

                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void CallbackController_VKMessageAction_New(Message msg)
        {
            Run(msg);
        }


    }

}
