using System;
using System.Security.Cryptography;
using System.Text;

using FileTransfer.App.Services;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileTransfer.App.Tests;

[TestClass]
public class FileHashCalculatorTests
{
    [TestMethod]
    public void ComputeChunkMd5_UsesOnlyBytesRead()
    {
        const string input = "hello";
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        string expectedHash = Convert.ToHexString(
            MD5.HashData(inputBytes));

        byte[] buffer = Encoding.UTF8.GetBytes(
            input + "-extra-data");

        var calculator = new FileHashCalculator();

        string actualHash = calculator.ComputeChunkMd5(
            buffer,
            inputBytes.Length);

        Assert.AreEqual(expectedHash, actualHash);
    }

    [TestMethod]
    public void ComputeFileSha256_ReturnsExpectedHash()
    {
        // Arrange
        string filePath = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid()}.txt");

        const string content = "File transfer test content.";
        File.WriteAllText(filePath, content, Encoding.UTF8);

        byte[] fileBytes = File.ReadAllBytes(filePath);
        byte[] expectedHashBytes;

        using (SHA256 sha256 = SHA256.Create())
        {
            expectedHashBytes = sha256.ComputeHash(fileBytes);
        }

        string expectedHash = Convert.ToHexString(
            expectedHashBytes).ToLowerInvariant();

        var calculator = new FileHashCalculator();

        try
        {
            // Act
            string actualHash = calculator.ComputeFileSha256(filePath);

            // Assert
            Assert.AreEqual(expectedHash, actualHash);
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}