using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    internal class Secret_Word
    {
        public static void Run()
        {
            string secretWord = "";
            Console.Write("Enter the secret Word: ");
            secretWord = Program.GetStringInput().Trim();
            int attempts = 0;

            while (attempts < 3)
            {
                string guessWord = "";
                Console.Write("Enter the guest word: ");
                guessWord = Program.GetStringInput().Trim();

                if(secretWord.Length != guessWord.Length)
                {
                    Console.WriteLine("The length of the guess word is not equal to the secret word. Try again.");
                    continue;
                }

                int cow = 0, bull = 0;
                for(int i = 0; i < secretWord.Length; i++)
                {
                    if (secretWord[i] == guessWord[i])
                    {
                        bull++;
                    }
                    else if (secretWord.Contains(guessWord[i]))
                    {
                        int index = secretWord.IndexOf(guessWord[i]);
                        if (secretWord[index] != guessWord[index])
                        {
                            cow++;
                        }
                    }
                }

                if (bull == secretWord.Length)
                {
                    Console.WriteLine("Word exactly Matched");
                    return;
                }
                else
                {
                    Console.WriteLine($"Cows: {cow}, Bulls: {bull}");
                    attempts++;
                }
            }
            Console.WriteLine("Sorry Bro, you lost the game.");
        }
    }
}
