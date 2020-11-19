using System.Collections.Generic;

namespace sahnee_bot.Util
{
    public class Util
    {
        /// <summary>
        /// Removes unwanted characters specified inside this method
        /// </summary>
        /// <param name="text">the text that will be checked</param>
        /// <returns></returns>
        public string RemoveSpecialCharacters(string text)
        {
            char[] unwantedCharacters = { '!', '"', '§', '$', '%', '&', '/', '(', ')', '=', '?', '`', '´', '\\',
                '}', ']', '[', '{', '³', '²', ',', ';', '.', ':', '#', '\'', '+', '*', '~', '<', '>', '|', '@', '€'};

            //Remove unwanted characters
            foreach (char singleCharacter in unwantedCharacters)
            {
                text = text.Replace(singleCharacter.ToString(), "");
            }
            return text;
        }
        
    }
}
