namespace MDTools
{
    class Program
    {
        static void Main(string[] args)
        {
            ShiftyToShenBaxFormatter.Run();

            NMRSTARv2_1.Run("data.txt");
        }
    }
}
