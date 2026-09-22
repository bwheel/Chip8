# chip8

## Native library and .NET wrapper

- [`libs/chip8`](libs/chip8) is the C23 emulator core, built with CMake as a shared library.
- [`src/Chip8.Interop`](src/Chip8.Interop) is a C# class library (`Chip8.Interop.Chip8Emulator`) that P/Invokes into it.
  `dotnet build` runs CMake for you (requires `cmake` and a C compiler) and copies `libchip8.so`/`chip8.dll`/`libchip8.dylib`
  next to the assembly. Set `-p:Chip8NativeLibPath=/path/to/libchip8.so` to use a prebuilt library instead.
- [`tests/Chip8.Interop.Tests`](tests/Chip8.Interop.Tests) has xunit tests: `dotnet test tests/Chip8.Interop.Tests`.

```csharp
using var emulator = new Chip8Emulator();
emulator.LoadRom("IBM_Logo.ch8");
for (int i = 0; i < 100; i++) emulator.Step();
ReadOnlySpan<byte> pixels = emulator.FrameBuffer; // 64x32, one byte per pixel
```
