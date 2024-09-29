﻿namespace LibChip8;

public class Chip8Emulator
{

    public void LoadRom(Chip8Model model, string filename)
    {
        using FileStream readStream = File.OpenRead(filename);
        using MemoryStream memoryStream = new MemoryStream();
        readStream.CopyTo(memoryStream);
        Array.Copy(memoryStream.ToArray(), 0, model.Ram, Chip8Constants.PROGRAM_START_ADDRESS, memoryStream.Length);
        
    }

    public void Reset(Chip8Model model)
    {
        model = new Chip8Model();
    }

    public void Cycle(Chip8Model model)
    {
        // fetch
        ushort opcode = (ushort)((model.Ram[model.PC] << 8) | model.Ram[model.PC + 1]);

        // decode
        var action = Chip8Opcodes.Table[opcode];

        // execute
        model.PC += 2;
        action(opcode, model);
        if (model.ST > 0) model.ST--;
        if(model.DT > 0) model.DT--;        
    }
}
