using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using sahnee_bot.Util;

namespace sahnee_bot.Embeds
{
    public class EmbedGenerator
    {
        
        public List<EmbedBuilder> GenerateSahneeBotEmbed(
            string title, string description,
            string extraField1Title, string extraField1Content,
            string additionalExtraFieldTitles)
        {
            //get the amount of embeds that have to be generated
            int charCountTotal =
                title.Length + description.Length + extraField1Title.Length + extraField1Content.Length;
            int amountOfEmbeds = 1;
            if (charCountTotal >= StaticInternalConfiguration.CharacterLimitMessage)
            {
                //set the amount of embeds that will be generated
                amountOfEmbeds = (int)Math.Ceiling(charCountTotal / (double)StaticInternalConfiguration.CharacterLimitMessage);
            }
            List<EmbedBuilder> embeds = new List<EmbedBuilder>();
            //separate the full string into the parts
            List<string> extraFieldContentStrings =
                SplitStringsToMatchMaxOutputSize(extraField1Content, amountOfEmbeds);
            //check if the string split really needs the amount of splits calculated above or if we need less
            if (extraFieldContentStrings.Count < amountOfEmbeds)
            {
                amountOfEmbeds = extraFieldContentStrings.Count;
            }
            //generate the embeds
            for (int i = 0; i < amountOfEmbeds; i++)
            {
                EmbedBuilder tempEmbed = new EmbedBuilder
                {
                    Title = title,
                    Color = Color.Purple,
                    Description = description
                };
                //default footer
                tempEmbed.WithFooter(footer => footer.Text = "proudly presented by sahnee.dev");
                //default author
                tempEmbed.WithAuthor(author => 
                { 
                    author.Name = "sahnee-bot";
                    author.Url = "https://github.com/Sahnee-DE/sahnee-bot";
                    author.IconUrl = "https://sahnee.dev/wp-content/uploads/2020/04/sahnee-bot-300x300.png";
                });
                //add the extra field
                tempEmbed.AddField(i == 0 ? extraField1Title : additionalExtraFieldTitles
                    , extraFieldContentStrings[i]);
                //add to the list to return
                embeds.Add(tempEmbed);
            }

            return embeds;
        }

        private List<string> SplitStringsToMatchMaxOutputSize(string fullText, int amountOfEmbeds)
        {
            string[] tempStrings = fullText.Split("\n");
            List<string> filledStrings = new List<string>();
            string tempString = "";
            for (int i = 0; i < tempStrings.Length; i++)
            {
                //append, because there is still some space left
                if (tempString.Length + tempStrings[i].Length + 4 < StaticInternalConfiguration.CharacterLimitMessage)
                {
                    tempString += tempStrings[i] + "\n";
                }
                //commit to the array and start new array member
                else
                {
                    filledStrings.Add(tempString + "\n");
                    tempString = tempStrings[i] + "\n";
                }
            }
            filledStrings.Add(tempString);

            return filledStrings;
        }
        
    }
}
