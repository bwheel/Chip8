namespace Chip8.Interop;

/// <summary>
/// Result codes returned by the native library. Mirrors <c>chip8_error_t</c>.
/// </summary>
public enum Chip8Result
{
    /// <summary>The call succeeded.</summary>
    Ok = 0,

    /// <summary>Generic failure.</summary>
    Error = 1,

    /// <summary>An argument was rejected by the native library.</summary>
    InvalidArgument = 2,

    /// <summary>The opcode or feature is not implemented.</summary>
    NotImplemented = 99,
}
