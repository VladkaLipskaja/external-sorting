namespace Altium.DataProcessing
{
    public class FileSplitterService : IFileSplitterService
    {
        public async Task<(string[] filenames, int maxLineCapacity)> GetFileChunksAsync(Stream source,
            CancellationToken cancellationToken, int tempFileSize)
        {
            var buffer = new List<byte>();
            var finalBytes = new List<byte>();
            var filenames = new List<string>();
            var totalFiles = 0;
            var maxLineCapacity = 0;

            await using (source)
            {
                while (source.Position < source.Length)
                {
                    var totalRows = FillFileSizeChunk(ref source, ref buffer, tempFileSize);
                    FillFinalChunkByte(ref source, ref finalBytes, ref buffer);

                    var filename = await CreateFileAsync(totalFiles, buffer.ToArray(), finalBytes.ToArray(), cancellationToken);
                    filenames.Add(filename);
                    totalFiles++;

                    if (totalRows > maxLineCapacity)
                    {
                        maxLineCapacity = totalRows;
                    }

                    finalBytes.Clear();
                    buffer.Clear();
                }
            }

            return (filenames.ToArray(), maxLineCapacity);
        }

        private const byte Delimiter = (byte)'\n'; // New line

        private void FillFinalChunkByte(ref Stream source, ref List<byte> finalBytes, ref List<byte> buffer)
        {
            var finalSymbol = buffer[^1];

            while (finalSymbol != Delimiter)
            {
                var value = source.ReadByte();
                if (value == -1)
                {
                    break;
                }

                finalSymbol = (byte) value;
                finalBytes.Add(finalSymbol);
            }
        }

        private int FillFileSizeChunk(ref Stream source, ref List<byte> buffer, int fileSize)
        {
            var readBytes = 0;
            var chunkRows = 0;

            while (readBytes < fileSize)
            {
                var value = source.ReadByte();
                if (value == -1)
                {
                    break;
                }

                buffer.Add((byte) value);
                if (buffer[readBytes] == Delimiter)
                {
                    chunkRows++;
                }

                readBytes++;
            }

            return chunkRows;
        }

        private async Task<string> CreateFileAsync(long index, byte[] chunk1, byte[] chunk2,
            CancellationToken cancellationToken)
        {
            var name = $"{index}{FileSortOptions.SplitFileEnd}";
            await using var splitFile = File.Create(Path.Combine(Directory.GetCurrentDirectory(), name));
            await splitFile.WriteAsync(chunk1, cancellationToken);
            if (chunk2.Length > 0)
            {
                await splitFile.WriteAsync(chunk2, cancellationToken);
            }

            return name;
        }
    }
}