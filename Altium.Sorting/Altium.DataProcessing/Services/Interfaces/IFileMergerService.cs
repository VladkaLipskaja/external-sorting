namespace Altium.DataProcessing
{
    public interface IFileMergerService
    {
        Task MergeFilesAsync(string[] sortedFiles, Stream target, CancellationToken cancellationToken);
    }
}