namespace Altium.DataGeneration
{
    public static class ThreadSafeRandom
    {
        private static readonly Random _rnd = new();

        public static int Next(int min, int max)
        {
            lock (_rnd) return _rnd.Next(min, max);
        }
    }
}