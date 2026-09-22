using Chip8.Managed;

namespace Chip8.Emulation;

/// <summary>
/// <see cref="IEmulator"/> backed by the managed Chip8.Managed emulator.
/// </summary>
public sealed class ManagedEmulator : IEmulator
{
    private readonly Chip8.Managed.Emulator _emulator = new();
    private readonly Model _model = new();

    public void LoadRom(string path) => _emulator.LoadRom(_model, path);

    public void Step() => _emulator.Step(_model);

    public void TickTimers() => _emulator.TickTimers(_model);

    public void SetKey(int key, bool pressed)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(key);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(key, _model.Keys.Length);
        _model.Keys[key] = pressed ? (byte)1 : (byte)0;
    }

    public ReadOnlySpan<byte> FrameBuffer => _model.DisplayBuffer;

    public void Dispose()
    {
    }
}
