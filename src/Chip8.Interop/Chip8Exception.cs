namespace Chip8.Interop;

/// <summary>
/// Thrown when a call into the native CHIP-8 library returns a failure code.
/// </summary>
public class Chip8Exception : Exception
{
    /// <summary>The result code returned by the native library.</summary>
    public Chip8Result Result { get; }

    /// <summary>The native operation that failed, e.g. <c>load_rom</c>.</summary>
    public string Operation { get; }

    /// <summary>Creates an exception for a failed native call.</summary>
    /// <param name="result">The result code returned by the native library.</param>
    /// <param name="operation">The native operation that failed.</param>
    public Chip8Exception(Chip8Result result, string operation)
        : base($"chip8_emulator_{operation} failed with {result} ({(int)result}).")
    {
        Result = result;
        Operation = operation;
    }

    internal static void ThrowIfFailed(Chip8Result result, string operation)
    {
        if (result != Chip8Result.Ok)
        {
            throw new Chip8Exception(result, operation);
        }
    }
}
