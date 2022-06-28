namespace Altium.DataProcessing
{
    public interface IFileSorterService
    {
        Task<string[]> SortFilesAsync(string[] sourceFiles, CancellationToken token, int maxLineCapacity);
    }
}