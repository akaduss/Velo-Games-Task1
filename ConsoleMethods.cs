namespace Velo_Games_Task1
{
    public static class ConsoleMethods
    {
        public static string ReadUserInput()
        {
            return Console.ReadLine().Trim();
        }

        public static void PrintColoredMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void ClearConsole()
        {
            Console.Clear();
        }
    }

}
