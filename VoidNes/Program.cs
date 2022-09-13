namespace VoidNes;

internal class Program
{
    public static void Main(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            Console.WriteLine("No ROM file has been specified as a parameter.");
            return;
        }

        using var game = new Emulator(args[0]);
        game.Run();
    }
}
