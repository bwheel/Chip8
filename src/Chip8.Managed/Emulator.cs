namespace Chip8.Managed;

public class Emulator
{

    public void LoadRom(Model model, string filename)
    {
        using FileStream readStream = File.OpenRead(filename);
        using MemoryStream memoryStream = new MemoryStream();
        readStream.CopyTo(memoryStream);
        Array.Copy(memoryStream.ToArray(), 0, model.Ram, Constants.PROGRAM_START_ADDRESS, memoryStream.Length);
    }

    public void Reset(Model model)
    {
        model = new Model();
    }

    public void Step(Model model)
    {
        // fetch
        ushort opcode = (ushort)((model.Ram[model.PC] << 8) | model.Ram[model.PC + 1]);

        // decode
        var action = Opcodes.Table[opcode];

        // execute
        model.PC += 2;
        action(opcode, model);
    }

    /// <summary>
    /// Decrements the delay and sound timers. Call at <see cref="Constants.TIMER_FREQUENCY_HZ"/>.
    /// </summary>
    public void TickTimers(Model model)
    {
        if (model.ST > 0) model.ST--;
        if (model.DT > 0) model.DT--;
    }
}
