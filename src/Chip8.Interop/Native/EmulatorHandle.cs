using Microsoft.Win32.SafeHandles;

namespace Chip8.Interop.Native;

/// <summary>
/// Owns a native <c>chip8_emulator_t*</c> and frees it when released.
/// </summary>
internal sealed class EmulatorHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    // Required by the P/Invoke marshaller.
    public EmulatorHandle() : base(ownsHandle: true)
    {
    }

    internal static EmulatorHandle Create()
    {
        Chip8Exception.ThrowIfFailed(NativeMethods.Open(out nint pointer), "open");

        var handle = new EmulatorHandle();
        handle.SetHandle(pointer);
        return handle;
    }

    protected override bool ReleaseHandle() => NativeMethods.Free(handle) == Chip8Result.Ok;
}
