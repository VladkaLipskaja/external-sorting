namespace Altium.DataProcessing
{
    public static class FileSortOptions
    {
        public const string SplitFileEnd = "_split.txt";

        public const string SortedFileEnd = "_sorted.txt";
        
        public const string UsedFileEnd = "_used.txt";
        
        public const string MergedFileEnd = "_merged.txt";
        
        public static readonly string CurrentDirectory = Directory.GetCurrentDirectory();

        public const int TempFileSize = 2 * 1024 * 1024;

        public const int MergeChunkAmount = 10;
    }   
}