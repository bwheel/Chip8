using Chip8.Interop.Native;

namespace Chip8.Interop;

/// <summary>
/// Managed wrapper around a native CHIP-8 emulator instance (libs/chip8).
/// </summary>
public sealed class Chip8Emulator : IDisposable
{
    /// <summary>Display width in pixels.</summary>
    public const int DisplayWidth = 64;

    /// <summary>Display height in pixels.</summary>
    public const int DisplayHeight = 32;

    /// <summary>Number of keys on the keypad (0x0-0xF).</summary>
    public const int KeyCount = 16;

    /// <summary>Number of general purpose registers (V0-VF).</summary>
    public const int RegisterCount = 16;

    /// <summary>Size of program memory available to a ROM (4 KiB minus the 0x200 reserved bytes).</summary>
    public const int MaxRomSize = 4096 - 0x200;

    private readonly EmulatorHandle _handle;

    /// <summary>
    /// Allocates and resets a new emulator.
    /// </summary>
    /// <exception cref="Chip8Exception">The native emulator could not be created.</exception>
    /// <exception cref="DllNotFoundException">The native chip8 library could not be loaded.</exception>
    public Chip8Emulator()
    {
        _handle = EmulatorHandle.Create();
    }

    /// <summary>
    /// Resets registers, memory, timers, keys and the display.
    /// </summary>
    public void Reset() => Chip8Exception.ThrowIfFailed(NativeMethods.Reset(Handle), "reset");

    /// <summary>
    /// Loads a ROM file at 0x200.
    /// </summary>
    /// <exception cref="Chip8Exception">The file could not be read or is larger than <see cref="MaxRomSize"/>.</exception>
    public void LoadRom(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        Chip8Exception.ThrowIfFailed(NativeMethods.LoadRom(Handle, path), "load_rom");
    }

    /// <summary>
    /// Loads a ROM image at 0x200.
    /// </summary>
    /// <exception cref="Chip8Exception">The ROM is larger than <see cref="MaxRomSize"/>.</exception>
    public unsafe void LoadRom(ReadOnlySpan<byte> rom)
    {
        var handle = Handle;
        fixed (byte* data = rom)
        {
            Chip8Exception.ThrowIfFailed(NativeMethods.LoadRomFromMemory(handle, data, (nuint)rom.Length), "load_rom_from_memory");
        }
    }

    /// <summary>
    /// Fetches, decodes and executes a single instruction.
    /// </summary>
    /// <exception cref="Chip8Exception">The instruction failed, e.g. <see cref="Chip8Result.NotImplemented"/>.</exception>
    public void Step() => Chip8Exception.ThrowIfFailed(NativeMethods.Step(Handle), "step");

    /// <summary>
    /// Decrements the delay and sound timers. Call at 60 Hz.
    /// </summary>
    public void TickTimers() => Chip8Exception.ThrowIfFailed(NativeMethods.TickTimers(Handle), "tick_timers");

    /// <summary>
    /// Sets the state of a keypad key.
    /// </summary>
    /// <param name="key">Key index, 0x0-0xF.</param>
    /// <param name="pressed">Whether the key is held down.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="key"/> is not in 0x0-0xF.</exception>
    public void SetKey(int key, bool pressed)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(key);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(key, KeyCount);
        Chip8Exception.ThrowIfFailed(NativeMethods.SetKey(Handle, (byte)key, pressed ? (byte)1 : (byte)0), "set_key");
    }

    /// <summary>
    /// The display, <see cref="DisplayWidth"/> x <see cref="DisplayHeight"/> pixels in row-major order, one byte per pixel (0 or 1).
    /// </summary>
    /// <remarks>
    /// This is a live view of native memory: it reflects later steps and is only valid until the emulator is disposed.
    /// </remarks>
    public unsafe ReadOnlySpan<byte> FrameBuffer
    {
        get
        {
            Chip8Exception.ThrowIfFailed(NativeMethods.GetFrameBuffer(Handle, out byte* buffer, out nuint length), "get_frame_buffer");
            return new ReadOnlySpan<byte>(buffer, checked((int)length));
        }
    }

    /// <summary>The delay and sound timer values.</summary>
    public (byte Delay, byte Sound) Timers
    {
        get
        {
            Chip8Exception.ThrowIfFailed(NativeMethods.GetTimers(Handle, out byte delay, out byte sound), "get_timers");
            return (delay, sound);
        }
    }

    /// <summary>The program counter.</summary>
    public ushort ProgramCounter
    {
        get
        {
            Chip8Exception.ThrowIfFailed(NativeMethods.GetProgramCounter(Handle, out ushort value), "get_program_counter");
            return value;
        }
    }

    /// <summary>The index (I) register.</summary>
    public ushort IndexRegister
    {
        get
        {
            Chip8Exception.ThrowIfFailed(NativeMethods.GetIndex(Handle, out ushort value), "get_index");
            return value;
        }
    }

    /// <summary>
    /// Copies the general purpose registers V0-VF into <paramref name="destination"/>.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is shorter than <see cref="RegisterCount"/>.</exception>
    public unsafe void GetRegisters(Span<byte> destination)
    {
        if (destination.Length < RegisterCount)
        {
            throw new ArgumentException($"Destination must hold at least {RegisterCount} bytes.", nameof(destination));
        }

        var handle = Handle;
        fixed (byte* registers = destination)
        {
            Chip8Exception.ThrowIfFailed(NativeMethods.GetRegisters(handle, registers), "get_registers");
        }
    }

    /// <summary>
    /// Returns the general purpose registers V0-VF.
    /// </summary>
    public byte[] GetRegisters()
    {
        var registers = new byte[RegisterCount];
        GetRegisters(registers);
        return registers;
    }

    /// <summary>
    /// Releases the native emulator.
    /// </summary>
    public void Dispose() => _handle.Dispose();

    private EmulatorHandle Handle
    {
        get
        {
            ObjectDisposedException.ThrowIf(_handle.IsClosed, this);
            return _handle;
        }
    }
}
