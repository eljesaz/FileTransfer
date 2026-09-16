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
    TransferResult result =
        transferService.Transfer(
            sourcePath,
            destinationDirectory);

    Console.WriteLine("Transfer completed successfully.");
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