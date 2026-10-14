using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Bank_app
{
    // here i read input 

    internal class Input
    {

        public bool EndOfInput { get; set; }

        public Input()
        {
            EndOfInput = false;
        }

        // here i read a line and notice when input has ended.
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


        // here i reapeat the question until the text is a whole number 

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


        // parse uses the computer number format. on swedish windows decimal amount is usually written 100,50
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

        // here i translate the choosen number into an enum value
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
