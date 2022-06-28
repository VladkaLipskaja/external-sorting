namespace Altium.DataGeneration
{
    public interface IGenerationService
    {
        Task GenerateDataAsync(long rowsNumber);
    }
}