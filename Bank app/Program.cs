namespace Bank_app
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Här startar jag programmet. Menyn sköter resten.
            Menu menu = new Menu();
            menu.Start();
        }
    }
}
