using System;

namespace Bank_app
{
    // Here I read what the user types and keep checking waiting payments.
    internal class Input
    {
        private Bank bank;

        public bool EndOfInput { get; set; }

        // Here I save the bank so I can check its queue while the user types.
        public Input(Bank bank)
        {
            this.bank = bank;
            EndOfInput = false;
        }

        // Here I read the text one key at a time so payments can run while the user waits.
        public string ReadText(string prompt)
        {
            // Check the real clock and finish payments that are due.
            bank.ProcessTransactions(DateTime.Now);

            Console.Write(prompt);

            // ReadLine handles input from a file or another program, including the end of input.
            if (Console.IsInputRedirected == true)
            {
                // The question mark means that ReadLine can also return null.
                string? line = Console.ReadLine();

                // Check the real clock and finish payments that are due.
                bank.ProcessTransactions(DateTime.Now);

                if (line == null)
                {
                    EndOfInput = true;
                    return "0";
                }

                return line;
            }

            string text = "";
            bool finished = false;

            while (finished == false)
            {
                // Check the real clock and finish payments that are due.
                bank.ProcessTransactions(DateTime.Now);

                // Only read a key when one is waiting, so the loop can keep checking payments.
                if (Console.KeyAvailable == true)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    // Enter finishes this line of text.
                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        finished = true;
                    }
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        if (text.Length > 0)
                        {
                            // Backspace rebuilds the text without its last character.
                            string shorterText = "";

                            for (int i = 0; i < text.Length - 1; i++)
                            {
                                shorterText = shorterText + text[i];
                            }

                            text = shorterText;

                            Console.Write("\b \b");
                        }
                    }
                    else if (key.KeyChar >= ' ')
                    {
                        text = text + key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                }
                else
                {
                    // Pause for 50 milliseconds so the waiting loop does not run needlessly fast.
                    System.Threading.Thread.Sleep(50);
                }
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
                    // Try to turn the text into an integer. The catch asks again if that fails.
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

        // Here I read a number using either a comma or a dot for the decimal part.
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

                // Start over for each new attempt to enter a decimal number.
                number = 0;
                decimal place = 1;
                bool hasDecimal = false;
                bool hasDigit = false;
                bool negative = false;
                valid = true;

                try
                {
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (text[i] == '-' && i == 0)
                        {
                            negative = true;
                        }
                        else if (text[i] == ',' || text[i] == '.')
                        {
                            if (hasDecimal == true)
                            {
                                valid = false;
                            }

                            hasDecimal = true;
                        }
                        else if (text[i] >= '0' && text[i] <= '9')
                        {
                            // Subtracting '0' turns a digit character into its number, for example '5' becomes 5.
                            int digit = text[i] - '0';
                            hasDigit = true;

                            // Before the separator, build the whole number. After it, add tenths, hundredths and so on.
                            if (hasDecimal == false)
                            {
                                number = number * 10 + digit;
                            }
                            else
                            {
                                place = place * 10;
                                number = number + digit / place;
                            }
                        }
                        else
                        {
                            valid = false;
                        }
                    }

                    if (hasDigit == false)
                    {
                        valid = false;
                    }

                    if (negative == true)
                    {
                        number = 0 - number;
                    }
                }
                catch
                {
                    valid = false;
                }

                if (valid == false)
                {
                    Console.WriteLine("Skriv ett tal, till exempel 100,50 eller 100.50. Använd inga tusentalsavskiljare.");
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

                    default:
                        Console.WriteLine("Välj 1, 2 eller 3.");
                        break;
                }
            }

            return currency;
        }
    }
}