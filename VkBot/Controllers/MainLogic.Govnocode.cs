using System;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IronPython.Compiler;
using IronPython;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using System.IO;
using System.Reflection;
using System.CodeDom;
using VkNet.Model.Attachments;
using System.Net;

namespace VkBot.Controllers
{
    public partial class MainLogic
    {
        private void Test(Message x)
        {
            long id = (long)x.FromId;
            long peerId = (long)x.PeerId;
            string line = null;
            foreach (var item in Users[id].Answer)
            {
                line += item + ", ";
            }
            SendMessage(_text.Body[Users[id].key] + line, peerId);
        }

        private void Setup()
        {
            BodyCommandAdd(Context.IgnatDolboeb, Test);
            BodyCommandAdd(Context.VladDolboeb, Test);
            BodyCommandAdd(Context.NoContext, x => { });
            BodyCommandAdd(Context.GetRandomNumber, GetRandomNumber);
            BodyCommandAdd(Context.Code, ExecutePythonScript);
            BodyCommandAdd(Context.Pizda, null);
            TextCommand("игнат", x => SelectContext(x, Context.IgnatDolboeb));
            TextCommand("влад", x => SelectContext(x, Context.VladDolboeb));
            TextCommand("рандом", x => SelectContext(x, Context.GetRandomNumber));
            TextCommand("code", x => SelectContext(x, Context.Code));
            TextCommand("кто", x => Why(x));
            TextCommand("пизда", x => ExecutePythonScriptPizda(x));

            PredicateAdd(new System.Tuple<Context, Level>(Context.IgnatDolboeb, Level.One), func1);
            PredicateAdd(new System.Tuple<Context, Level>(Context.IgnatDolboeb, Level.Two), func2);
            PredicateAdd(new System.Tuple<Context, Level>(Context.VladDolboeb, Level.One), func1);
            PredicateAdd(new System.Tuple<Context, Level>(Context.VladDolboeb, Level.Two), func2);
            PredicateAdd(new System.Tuple<Context, Level>(Context.GetRandomNumber, Level.One), func3);
            PredicateAdd(new System.Tuple<Context, Level>(Context.Code, Level.One), func4);
            PredicateAdd(new System.Tuple<Context, Level>(Context.Pizda, Level.One), func4);

            TextCreate(new Tuple<Context, Level>(Context.Code, Level.One), @"
                Код питона
                ");
            TextCreate(new Tuple<Context, Level>(Context.Code, Level.Two), @"
                Вывод:  
                ");
            TextCreate(new System.Tuple<Context, Level>(Context.IgnatDolboeb, Level.One), @"
                Насколько Игнат долбоёб?
                1)Слабо.
                2)Сильно.
                3)Невъебнный долбоёб.
                ");
            TextCreate(new System.Tuple<Context, Level>(Context.IgnatDolboeb, Level.Two), @"
                Точно?
                1)Да.
                2)Нет.
                ");
            TextCreate(new System.Tuple<Context, Level>(Context.IgnatDolboeb, Level.Three), @"
                Красава, иди нахуй.
                История ответов:
                ");
            TextCreate(new System.Tuple<Context, Level>(Context.VladDolboeb, Level.One), @"
                Насколько Влад долбоёб?
                1)Слабо.
                2)Сильно.
                3)Невъебнный долбоёб.
                ");
            TextCreate(new System.Tuple<Context, Level>(Context.VladDolboeb, Level.Two), @"
                Точно?
                1)Да.
                2)Нет
                ");
            TextCreate(new System.Tuple<Context, Level>(Context.VladDolboeb, Level.Three), @"
                Красава, иди нахуй.
                История ответов:
                ");
            TextCreate(new Tuple<Context, Level>(Context.GetRandomNumber, Level.One), @"
                Диапазон:
                ");
            TextCreate(new Tuple<Context, Level>(Context.GetRandomNumber, Level.Two), @"
                Число из диапазона от 
                ");

        }

        private void GetRandomNumber(Message msg)
        {
            string x = msg.Text;
            string xx = x.Replace(" ", null);
            Regex regex = new Regex(@"\d+");
            MatchCollection match = regex.Matches(x);
            if (int.TryParse(xx, out _))
            {
                if (match.Count == 2)
                {
                    int left = int.Parse(match[0].Value);
                    int right = int.Parse(match[1].Value);
                    if (left > right)
                        SendMessage("Произошла ошибка, правая часть должна быть меньше левой", msg.PeerId.Value);
                    else
                        SendMessage(_text.Body[Users[msg.FromId.Value].key] + $" {left} до {right} - " + new Random().Next(left, right), msg.PeerId.Value);

                }
                else if (match.Count == 1)
                {
                    int right = int.Parse(match[0].Value);

                    SendMessage(_text.Body[Users[msg.FromId.Value].key] + $"0 до {right} - " + new Random().Next(0, right), msg.PeerId.Value);
                }
                else
                {
                    SendMessage("Произошла ошибка, некорректный ввод", msg.PeerId.Value);
                }

            }
            else
            {
                SendMessage("Произошла ошибка, некорректный ввод", msg.PeerId.Value);
            }
        }

        private void ExecutePythonScript(Message source)
        {
            try
            {
                var engine = Python.CreateEngine();
                ScriptScope scriptScope = engine.CreateScope();
                string result = source.Text;
                Action<string> action = y => result = y;
                result = result.Replace("_", "    ");
                scriptScope.SetVariable("act", action);
                scriptScope.SetVariable("text", result);
                engine.ExecuteFile(@"/app/Python/PythonHandler.py", scriptScope);
                SendMessage(_text.Body[Users[source.FromId.Value].key] + result, source.PeerId.Value);
            }
            catch (Exception ex)
            {
                SendMessage("C#: " + ex.Message, source.PeerId.Value);

            }

        }
        private void ExecutePythonScriptPizda(Message pizda) 
        {
            try
            {
                var engine = Python.CreateEngine();
                ScriptScope scriptScope = engine.CreateScope();
                scriptScope.SetVariable("text", "15.02.21");
                engine.ExecuteFile(@"/app/Python/PythonHandler.py", scriptScope);

            }
            catch (Exception ex)
            {
                SendMessage("C#: " + ex.Message, pizda.PeerId.Value);

            }

        }
        private bool func4(Message x)
        {
            return true;
        }
        private bool func3(Message x)
        {
            string xx = x.Text.Replace(" ", null);
            Regex regex = new Regex(@"\d+");
            MatchCollection match = regex.Matches(x.Text);
            if (int.TryParse(xx, out _))
            {
                if (match.Count == 2 | match.Count == 1)
                {
                    return true;
                }
            }
            return false;
        }
        private bool func1(Message x)
        {
            int number = 0;
            bool d = int.TryParse(x.Text, out number);
            if (d)
            {
                number = int.Parse(x.Text);
                if (number <= 3)
                    return true;
            }
            return false;
        }
        private bool func2(Message x)
        {
            int number = 0;
            bool d = int.TryParse(x.Text, out number);
            if (d)
            {
                number = int.Parse(x.Text);
                if (number <= 2)
                    return true;
            }
            return false;
        }

        private void Why(Message x)
        {
            string text = null;
            foreach (var item in Users.Keys)
            {
                text += item + ", ";
            }
            SendMessage($@"Записаны: {text}", x.PeerId.Value);
        }
    }
}
