using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Bank_app
{
    // Here I keep the methods that read what the user types. 

    internal class Input
    {

        public bool EndOfInput { get; set; }

        public Input()
        {
            EndOfInput = false;
        }

        // Here I read the user's text and check if there is no more input.
        public string ReadText(string prompt)
        {
            Console.WriteLine(prompt);

            string text = Console.ReadLine();

            if (text == null)
            {
                EndOfInput = true;
                return "0";
            }

            return text;


        }


        // Here I keep asking until the user enters a whole number. 

        public int ReadNumber(string prompt)
        {
            int number = 0;
            bool valid = false;

            while (valid == false && EndOfInput == false)

            {
                string text = ReadText(prompt);

                if (EndOfInput == true)
                {
                    return 0;


                }

                try
                {
                    number = int.Parse(text);
                    valid = true;
                }
                catch
                {
                    Console.WriteLine("Skriv ett heltal.");
                }

            }
            return number;


        }


        // Here I read a decimal number using the computer's number format. On Swedish Windows, 100,50 usually works.
        public decimal ReadDecimal(string prompt)
        {
            decimal number = 0;
            bool valid = false;


            while (valid == false && EndOfInput == false)
            {
                string text = ReadText(prompt);

                if (EndOfInput == true)
                {
                    return 0;
                }
                try
                {
                    number = decimal.Parse(text);
                    valid = true;
                }
                catch
                {
                    Console.WriteLine("Skriv ett tal med datorns decimaltecken.");
                }

            }

            return number;

        }

        // Here I turn the user's choice of 1, 2 or 3 into SEK, EUR or USD.
        public Currency ReadCurrency()
        {
            Currency currency = Currency.SEK;
            bool valid = false;

            while (valid == false && EndOfInput == false)
            {
                int choice = ReadNumber("Valuta: 1 SEK, 2 EUR, 3 USD: ");

                switch (choice)
                {

                    case 1:
                        currency = Currency.SEK;
                        valid = true;
                        break;

                    case 2:
                        currency = Currency.EUR;
                        valid = true;
                        break;

                    case 3:

                        currency = Currency.USD;
                        valid = true;
                        break;




                }
            }

            return currency;
        }

    }
}
