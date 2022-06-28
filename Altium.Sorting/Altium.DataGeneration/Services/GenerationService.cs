using System.Collections.Concurrent;

namespace Altium.DataGeneration
{
    public class GenerationService : IGenerationService
    {
        public async Task GenerateDataAsync(long rowsNumber)
        {
            var twinsBuffer = string.Empty;
            var outputStream = File.Create(FileGenerateOptions.ResultedTextFileDirectory);
            await using var outputWriter = new StreamWriter(outputStream);
            for (var i = 0; i < rowsNumber / FileGenerateOptions.ChunkNumber; i++)
            {
                var rows = GetRows(twinsBuffer, FileGenerateOptions.ChunkNumber);
                foreach (var line in rows)
                {
                    await outputWriter.WriteLineAsync(line);
                }
            }

            var finalRowsNumber = rowsNumber % FileGenerateOptions.ChunkNumber;

            if (finalRowsNumber > 0)
            {
                var finalRows = GetRows(twinsBuffer, (int) finalRowsNumber);
                foreach (var line in finalRows)
                {
                    await outputWriter.WriteLineAsync(line);
                }
            }
        }

        public ConcurrentBag<string> GetRows(string twinsBuffer, int capacity)
        {
            var rows = new ConcurrentBag<string>();
            Parallel.For(0, capacity, i =>
            {
                var numberPart = GenerateNumberPart();

                if (GetRandomBool() && twinsBuffer?.Length > 0)
                {
                    rows.Add($"{numberPart}. {twinsBuffer}");
                }
                else
                {
                    var stringPart = GenerateStringPart();

                    if (GetRandomBool())
                    {
                        twinsBuffer = stringPart;
                    }

                    rows.Add($"{numberPart}. {stringPart}");
                }
            });

            return rows;
        }

        public bool GetRandomBool()
        {
            var value = ThreadSafeRandom.Next(0, 2); // 0 = false, 1 = true
            return value == 1;
        }

        public int GenerateNumberPart()
        {
            return ThreadSafeRandom.Next(FileGenerateOptions.MinNumber, FileGenerateOptions.MaxNumber);
        }

        public string? GenerateStringPart()
        {
            var length = ThreadSafeRandom.Next(FileGenerateOptions.MinLength, FileGenerateOptions.MaxStringPartNumber);

            var result = new char[length];
            
            result[0] = GetUpperSymbol();

            Parallel.For(1, length, i =>
            {
                var index = ThreadSafeRandom.Next(FileGenerateOptions.MinNumber,
                    FileGenerateOptions.PossibleSymbols.Length);
                result[i] = FileGenerateOptions.PossibleSymbols[index];
            });

            return new string(result);
        }

        private char GetUpperSymbol()
        {
            var index = ThreadSafeRandom.Next(FileGenerateOptions.MinNumber,
                FileGenerateOptions.PossibleUpperSymbols.Length);
            return FileGenerateOptions.PossibleUpperSymbols[index];
        }
    }
}