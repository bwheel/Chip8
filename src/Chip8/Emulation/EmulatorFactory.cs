namespace Chip8.Emulation;

public enum EmulatorKind
{
    Native,
    Dotnet,
}

public static class EmulatorFactory
{
    public static IEmulator Create(EmulatorKind kind) => kind switch
    {
        EmulatorKind.Native => new NativeEmulator(),
        EmulatorKind.Dotnet => new ManagedEmulator(),
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
    };
}
