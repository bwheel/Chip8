using System.Runtime.InteropServices;

namespace Chip8.Interop.Native;

/// <summary>
/// P/Invoke declarations for libs/chip8 (include/chip8/chip8.h).
/// </summary>
internal static unsafe partial class NativeMethods
{
    private const string Library = "chip8";

    [LibraryImport(Library, EntryPoint = "chip8_emulator_open")]
    internal static partial Chip8Result Open(out nint emulator);

    [LibraryImport(Library, EntryPoint = "chip8_emulator_free")]
    internal static partial Chip8Result Free(nint emulator);

    [LibraryImport(Library, EntryPoint = "chip8_emulator_reset")]
    internal static partial Chip8Result Reset(EmulatorHandle emulator);

    [LibraryImport(Library, EntryPoint = "chip8_emulator_load_rom", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial Chip8Result LoadRom(EmulatorHandle emulator, string romPath);

    [LibraryImport(Library, EntryPoint = "chip8_emulator_load_rom_from_memory")]
    internal static partial Chip8Result LoadRomFromMemory(EmulatorHandle emulator, byte* romData, nuint romSize);

    [LibraryImport(Library, EntryPoint = "chip8_emulator_step")]
    internal static partial Chip8Result Step(EmulatorHandle emulator);

    [LibraryImport(Library, EntryPoint = "chip8_emulator_tick_timers")]
    internal static partial Chip8Result TickTimers(EmulatorHandle emulator);

    [LibraryImport(Library, EntryPoint = "chip8_emulator_set_key")]
    internal static partial Chip8Result SetKey(EmulatorHandle emulator, byte key, byte pressed);

    [LibraryImport(Library, EntryPoint = "chip8_emulator_get_frame_buffer")]
    internal static partial Chip8Result GetFrameBuffer(EmulatorHandle emulator, out byte* buffer, out nuint length);

    [LibraryImport(Library, EntryPoint = "chip8_emulator_get_timers")]
    internal static partial Chip8Result GetTimers(EmulatorHandle emulator, out byte delay, out byte sound);

    [LibraryImport(Library, EntryPoint = "chip8_emulator_get_registers")]
    internal static partial Chip8Result GetRegisters(EmulatorHandle emulator, byte* registers);

    [LibraryImport(Library, EntryPoint = "chip8_emulator_get_program_counter")]
    internal static partial Chip8Result GetProgramCounter(EmulatorHandle emulator, out ushort programCounter);

    [LibraryImport(Library, EntryPoint = "chip8_emulator_get_index")]
    internal static partial Chip8Result GetIndex(EmulatorHandle emulator, out ushort index);
}
