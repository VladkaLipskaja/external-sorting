namespace Altium.DataProcessing
{
    public static class LineComparer
    {
        private const string Delimiter = ". ";

        public static Comparer<string> Comparer =
            Comparer<string>.Create((s1, s2) =>
            {
                var result1 = s1.Split(Delimiter);
                var result2 = s2.Split(Delimiter);

                var compared = String.Compare(result1[1], result2[1]);

                if (compared == 0)
                {
                    if (result1[0] == result2[0]) return 0;

                    return Convert.ToInt32(result1[0]) < Convert.ToInt32(result2[0]) ? -1 : 1;
                }

                return compared;
            });
    }
}