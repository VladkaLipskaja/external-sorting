namespace Altium.DataGeneration
{
    public static class FileGenerateOptions
    {
        public const int MinNumber = 0;
        public const int MaxNumber = 10_000;

        public const int MaxStringPartNumber = 50;

        public const int MinLength = 1;

        public static readonly string ResultedTextFileDirectory = $"{Directory.GetCurrentDirectory()}/../../../../Altium.DataProcessing/input.txt";

        public static readonly int ChunkNumber = 2000;

        public static readonly char[] PossibleUpperSymbols =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z'
        };
        
        public static readonly char[] PossibleSymbols =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
            'v', 'w', 'x', 'y', 'z'
        };
    }
}