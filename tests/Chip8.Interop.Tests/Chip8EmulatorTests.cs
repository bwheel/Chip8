using Chip8.Interop;

namespace Chip8.Interop.Tests;

public class Chip8EmulatorTests
{
    private static readonly string IbmLogoPath = Path.Combine(AppContext.BaseDirectory, "IBM_Logo.ch8");

    [Fact]
    public void NewEmulator_StartsAtProgramStartWithBlankDisplay()
    {
        using var emulator = new Chip8Emulator();

        Assert.Equal(0x200, emulator.ProgramCounter);
        Assert.Equal(Chip8Emulator.DisplayWidth * Chip8Emulator.DisplayHeight, emulator.FrameBuffer.Length);
        Assert.DoesNotContain((byte)1, emulator.FrameBuffer.ToArray());
        Assert.Equal((0, 0), emulator.Timers);
    }

    [Fact]
    public void Dispose_IsIdempotentAndBlocksFurtherUse()
    {
        var emulator = new Chip8Emulator();
        emulator.Dispose();
        emulator.Dispose();

        Assert.Throws<ObjectDisposedException>(() => emulator.Step());
        Assert.Throws<ObjectDisposedException>(() => emulator.ProgramCounter);
    }

    [Fact]
    public void LoadRomFile_AndStep_DrawsToFrameBuffer()
    {
        using var emulator = new Chip8Emulator();
        emulator.LoadRom(IbmLogoPath);

        // IBM Logo is a tight loop after drawing; 100 steps is plenty to finish drawing.
        for (int i = 0; i < 100; i++)
        {
            emulator.Step();
        }

        Assert.Contains((byte)1, emulator.FrameBuffer.ToArray());
    }

    [Fact]
    public void Reset_ClearsDisplayAndRestoresProgramCounter()
    {
        using var emulator = new Chip8Emulator();
        emulator.LoadRom(IbmLogoPath);
        for (int i = 0; i < 100; i++)
        {
            emulator.Step();
        }

        emulator.Reset();

        Assert.Equal(0x200, emulator.ProgramCounter);
        Assert.DoesNotContain((byte)1, emulator.FrameBuffer.ToArray());
    }

    [Fact]
    public void LoadRomFromSpan_ExecutesInstructions()
    {
        using var emulator = new Chip8Emulator();
        // 6A2A: V[A] = 0x2A; A123: I = 0x123
        emulator.LoadRom([0x6A, 0x2A, 0xA1, 0x23]);

        emulator.Step();
        emulator.Step();

        Assert.Equal(0x2A, emulator.GetRegisters()[0xA]);
        Assert.Equal(0x123, emulator.IndexRegister);
        Assert.Equal(0x204, emulator.ProgramCounter);
    }

    [Fact]
    public void LoadRom_TooLarge_Throws()
    {
        using var emulator = new Chip8Emulator();

        var ex = Assert.Throws<Chip8Exception>(() => emulator.LoadRom(new byte[Chip8Emulator.MaxRomSize + 1]));
        Assert.Equal(Chip8Result.InvalidArgument, ex.Result);
    }

    [Fact]
    public void LoadRom_MissingFile_Throws()
    {
        using var emulator = new Chip8Emulator();

        Assert.Throws<Chip8Exception>(() => emulator.LoadRom(Path.Combine(AppContext.BaseDirectory, "does-not-exist.ch8")));
    }

    [Fact]
    public void Step_UnknownOpcode_ThrowsNotImplemented()
    {
        using var emulator = new Chip8Emulator();
        emulator.LoadRom([0x00, 0x00]);

        var ex = Assert.Throws<Chip8Exception>(() => emulator.Step());
        Assert.Equal(Chip8Result.NotImplemented, ex.Result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(16)]
    public void SetKey_OutOfRange_Throws(int key)
    {
        using var emulator = new Chip8Emulator();

        Assert.Throws<ArgumentOutOfRangeException>(() => emulator.SetKey(key, true));
    }

    [Fact]
    public void SetKey_ValidKey_DoesNotThrow()
    {
        using var emulator = new Chip8Emulator();

        emulator.SetKey(0xF, true);
        emulator.SetKey(0xF, false);
    }

    [Fact]
    public void TickTimers_DecrementsDelayTimer()
    {
        using var emulator = new Chip8Emulator();
        // 6005: V0 = 5; F015: DT = V0
        emulator.LoadRom([0x60, 0x05, 0xF0, 0x15]);
        emulator.Step();
        emulator.Step();
        Assert.Equal(5, emulator.Timers.Delay);

        emulator.TickTimers();
        emulator.TickTimers();

        Assert.Equal(3, emulator.Timers.Delay);
    }

    [Fact]
    public void GetRegisters_SpanTooShort_Throws()
    {
        using var emulator = new Chip8Emulator();

        Assert.Throws<ArgumentException>(() => emulator.GetRegisters(new byte[4]));
    }
}
