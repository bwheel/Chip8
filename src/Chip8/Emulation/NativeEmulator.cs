using Chip8.Interop;

namespace Chip8.Emulation;

/// <summary>
/// <see cref="IEmulator"/> backed by the native emulator in libs/chip8.
/// </summary>
public sealed class NativeEmulator : IEmulator
{
    private readonly Chip8Emulator _emulator = new();

    public void LoadRom(string path) => _emulator.LoadRom(path);

    public void Step() => _emulator.Step();

    public void TickTimers() => _emulator.TickTimers();

    public void SetKey(int key, bool pressed) => _emulator.SetKey(key, pressed);

    public ReadOnlySpan<byte> FrameBuffer => _emulator.FrameBuffer;

    public void Dispose() => _emulator.Dispose();
}
