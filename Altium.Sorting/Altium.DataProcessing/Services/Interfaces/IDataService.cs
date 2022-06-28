namespace Altium.DataProcessing
{
    public interface IDataService
    {
        public Task GetFileAsync();
        public Task Sort(Stream source, Stream target, CancellationToken cancellationToken);
    }
}