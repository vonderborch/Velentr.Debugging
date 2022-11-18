using Velentr.PerformanceMetrics;

namespace Velentr.DevEnv
{
    public static class Program
    {
        static void Main()
        {
            using var game = new GamePerformanceWrapper(new Game1(), "Velentr.PerformanceMetrics");
            game.Run();
        }
    }
}
