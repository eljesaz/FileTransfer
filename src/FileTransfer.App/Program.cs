using FileTransfer.App.Domain;
using FileTransfer.App.Services; 

Console.Write("Enter the source file path: ");
string sourcePath = Console.ReadLine() ?? string.Empty;

Console.Write("Enter the destination directory: ");
string destinationDirectory = Console.ReadLine() ?? string.Empty;

TransferOptions options = new(
    chunkSizeBytes: 4 * 1024 * 1024,
    maxRetries: 3);


FileTransferService transferService =
    new(options);

try
{
    string fullDestinationDirectory =
    Path.GetFullPath(destinationDirectory);

    string destinationFilePath = Path.Combine(
        fullDestinationDirectory,
        Path.GetFileName(sourcePath));

    if (!Directory.Exists(fullDestinationDirectory))
    {
        Console.WriteLine(
            $"Destination directory will be created at: " +
            $"{fullDestinationDirectory}");
    }

    Progress<double> progress = new(
    percentage => Console.Write(
        $"\rProgress: {percentage:0.00}%"));

    TransferResult result =
        transferService.Transfer(
            sourcePath,
            destinationDirectory, progress);
    Console.WriteLine();
    Console.WriteLine("Transfer completed successfully.");

    if(result.Chunks.Count == 0) 
    {
        Console.WriteLine("File is empty, no chunks were created during transfer.");
    }
    else
    {
        Console.WriteLine("Chunk checksums:");

        foreach (FileChunk chunk in result.Chunks)
        {
            Console.WriteLine(
                $"Offset: {chunk.Offset}, " +
                $"Length: {chunk.Length}, " +
                $"MD5: {chunk.Md5Hash}");
        }
    }
   

    Console.WriteLine();
    Console.WriteLine($"Source SHA-256:      {result.SourceSha256}");
    Console.WriteLine($"Destination SHA-256: {result.DestinationSha256}");
    Console.WriteLine($"Hashes match:        {result.HashesMatch}");
}
catch (ArgumentException exception)
{
    Console.WriteLine($"Invalid input: {exception.Message}");
}
catch (FileNotFoundException exception)
{
    Console.WriteLine($"File error: {exception.Message}");
}
catch (InvalidDataException exception)
{
    Console.WriteLine($"Verification failed: {exception.Message}");
}
catch (IOException exception)
{
    Console.WriteLine($"I/O error: {exception.Message}");
}
catch(Exception exception)
{
    Console.WriteLine($"Something went wrong: {exception.Message}");
}

