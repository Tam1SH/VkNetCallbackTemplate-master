using System;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;
using System.Collections.Generic;

namespace VkBot.Controllers
{
    public partial class MainLogic
    {
        private enum Context
        {
            NoContext, IgnatDolboeb, VladDolboeb, GetRandomNumber, Code, Pizda
        }
        private enum Level
        {
            Zero, One, Two, Three, Four, Five, Six
        }
        private class User
        {
            public Level CurrentLevel { get; set; }
            public List<string> Answer { get; set; }
            public Context context { get; set; }
            public Tuple<Context, Level> key { get => new Tuple<Context, Level>(context, CurrentLevel); }
            public User()
            {
                CurrentLevel = Level.Zero;
                context = Context.NoContext;
                Answer = new List<string>();
            }
        }
        private class Dialog
        {
            public Dictionary<System.Tuple<Context, Level>, string> Body { get; set; } = new Dictionary<System.Tuple<Context, Level>, string>();

            public Dialog()
            {
            }

        }
    }
    
}
