namespace Altium.DataProcessing
{
    public interface IFileSplitterService
    {
        Task<(string[] filenames, int maxLineCapacity)> GetFileChunksAsync(Stream source,
            CancellationToken cancellationToken, int tempFileSize);
    }
}