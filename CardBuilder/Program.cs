using System;
using System.Collections.Generic;
using System.IO;

namespace CardBuilder
{
    class Program
    {
        private static char _separator = (char) 31;
       
        static void Main(string[] args)
        {   
            String[] topCards = new String[] { "O", "Ag", "U", "W", "C" };
            String[] bottomCards = new String[] { "Oxygen", "Gold", "Uranium", "Tungsten", "Carbon" };
            Dictionary<String, String> cards = new Dictionary<string, string>();
            for (int i = 0; i < topCards.Length; i++)
            {
                cards.Add(topCards[i], bottomCards[i]);
            }
            FileStream fs = new FileStream(@"..\..\..\StudyCards\TestCards\ElementalTestCards.scd", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach (String card in cards.Keys)
            {
                sw.Write(card + _separator + cards[card] + _separator);
            }
            sw.Close();
            fs.Close();
            Console.WriteLine("Write Completed");
            Console.ReadLine();
        }
    }
}
