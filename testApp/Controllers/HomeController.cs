using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace testApp.Controllers
{
    public class HomeController : Controller
    {
        //Варианты загадываемых слов, загаданное слово, слово для подсказки
        static string[] words = { "Привет", "Пока", "Яблоко", "Клубника", "Лес", "Поле", "Дом", "Гараж", "Собака", "Кошка", "Мама", "Папа", "Кофе", "Чай" };
        
        //Ответы системы пользователю
        static string[] wrongAnswers =  {"Неверно.", "Подумай получше.", "Ты далёк от истины.", "Подумай ещё.",
            "Ну что так долго?", "И долго мы так будем?", "Я устал ждать.", "Ну сколько можно?"};

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Words = words;
            Session["results"] = new List<string>();
            Session["guessedWord"] = words[new Random().Next(0, words.Length)];
            string guessedWord = Session["guessedWord"].ToString();
            Session["hintWord"] = guessedWord;
            ViewBag.Results = Session["results"];
            Session["errorsCounter"] = 0;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string word)
        {
            ViewBag.Words = words;
            string answer = "";
            if (word.ToLower().Replace(" ", "") != Session["guessedWord"].ToString().ToLower())
            {
                //Подсказка на каждую третью неверную попытку
                if ((int)Session["errorsCounter"] == 2)
                {
                    if (word.Length < Session["guessedWord"].ToString().Length)
                    {
                        answer = "Слово длиннее.";
                    }
                    else if (word.Length > Session["guessedWord"].ToString().Length)
                    {
                        answer = "Слово короче.";
                    }
                    //Если длина слова угадана, начинаются подсказки по буквам
                    else
                    {
                        answer = "Угадана длина слова, и ";
                        int index = 0;
                        bool flag = false;
                        foreach (char c in Session["hintWord"].ToString())
                        {
                            if (c != ' ')
                            {
                                flag |= true;
                            }
                        }
                        if (flag)
                        {
                            do
                            {
                                index = new Random().Next(0, Session["guessedWord"].ToString().Length);
                            } while (Session["hintWord"].ToString()[index] == ' ');
                            answer += $"{index + 1}-ая буква слова - это {Session["hintWord"].ToString()[index]}";
                            Session["hintWord"] = Session["hintWord"].ToString().Remove(index, 1).Insert(index, " ");
                        }
                        else answer += "подсказаны все буквы слова.";
                    }
                    Session["errorsCounter"] = 0;
                }
                //Ответ системы на неверное слово
                else
                {
                    answer = wrongAnswers[new Random().Next(0, wrongAnswers.Length)];
                    int errors = (int)Session["errorsCounter"];
                    errors++;
                    Session["errorsCounter"] = errors;
                }
            }
            //Ответ системы на угаданное слово и смена слова
            else
            {
                answer = "Верно. Поздравляю! Новое слово загадано.";
                Session["guessedWord"] = words[new Random().Next(0, words.Length)];
                Session["hintWord"] = Session["guessedWord"].ToString().ToCharArray();
                Session["errorsCounter"] = 0;
            }
            var list = (List<string>)Session["results"];
            list.Add(word);
            list.Add(answer);
            Session["results"] = list;
            ViewBag.Results = Session["results"];
            return View();
        }

    }
}