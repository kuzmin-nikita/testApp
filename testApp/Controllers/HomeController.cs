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
        static string[] variants = { "привет", "пока", "яблоко", "клубника", "лес", "поле", "дом", "гараж", "собака", "кошка", "мама", "папа", "кофе", "чай" };
        static string guessedWord = variants[new Random().Next(0, variants.Length)] /*"привет"*/;
        static char[] hintWord = guessedWord.ToCharArray();

        //Ответы системы пользователю
        static List<string> results = new List<string>();
        string[] wrongAnswers =  {"Неверно.", "Подумай получше.", "Ты далёк от истины.", "Подумай ещё.",
            "Ну что так долго?", "И долго мы так будем?", "Я устал ждать.", "Ну сколько можно?"};
        static int errorsCounter = 0;

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Results = results;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string word)
        {
            string answer = "";
            if (word.ToLower().Replace(" ", "") != guessedWord)
            {
                //Подсказка на каждую третью неверную попытку
                if (errorsCounter == 2)
                {
                    if (word.Length < guessedWord.Length)
                    {
                        answer = "Слово длиннее.";
                    }
                    else if (word.Length > guessedWord.Length)
                    {
                        answer = "Слово короче.";
                    }
                    //Если длина слова угадана, начинаются подсказки по буквам
                    else
                    {
                        answer = "Угадана длина слова, и ";
                        int index = 0;
                        bool flag = false;
                        foreach (char c in hintWord)
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
                                index = new Random().Next(0, guessedWord.Length);
                            } while (hintWord[index] == ' ');
                            answer += $"{index + 1}-ая буква слова - это {hintWord[index]}";
                            hintWord[index] = ' ';
                        }
                        else answer += "подсказаны все буквы слова.";
                }
                errorsCounter = 0;
                }
                //Ответ системы на неверное слово
                else
                {
                    answer = wrongAnswers[new Random().Next(0, wrongAnswers.Length)];
                    errorsCounter++;
                }
            }
            //Ответ системы на угаданное слово и смена слова
            else
            {
                answer = "Верно. Поздравляю! Новое слово загадано.";
                guessedWord = variants[new Random().Next(0, variants.Length)];
                hintWord = guessedWord.ToCharArray();
                errorsCounter = 0;
            }
            results.Add(word);
            results.Add(answer);
            ViewBag.Results = results;
            return View();
        }

    }
}