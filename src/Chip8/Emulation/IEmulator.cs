namespace Chip8.Emulation;

/// <summary>
/// Backend-agnostic view of a CHIP-8 emulator, implemented by both the native and the managed emulator.
/// </summary>
public interface IEmulator : IDisposable
{
    /// <summary>Loads a ROM file at 0x200.</summary>
    void LoadRom(string path);

    /// <summary>Fetches, decodes and executes a single instruction.</summary>
    void Step();

    /// <summary>Decrements the delay and sound timers. Call at 60 Hz.</summary>
    void TickTimers();

    /// <summary>Sets the state of a keypad key.</summary>
    /// <param name="key">Key index, 0x0-0xF.</param>
    /// <param name="pressed">Whether the key is held down.</param>
    void SetKey(int key, bool pressed);

    /// <summary>The 64x32 display in row-major order, one byte per pixel (0 or 1).</summary>
    ReadOnlySpan<byte> FrameBuffer { get; }
}
