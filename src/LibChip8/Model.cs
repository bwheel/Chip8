using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibChip8
{
    public class Model
    {
        /// <summary>
        /// Application memory in RAM.
        /// The Chip-8 language is capable of accessing up to 4KB (4,096 bytes) of RAM, from location 0x000 (0) to 0xFFF (4095). The first 512 bytes, from 0x000 to 0x1FF, are where the original interpreter was located, and should not be used by programs.
        ///
        /// Most Chip-8 programs start at location 0x200 (512), but some begin at 0x600 (1536). Programs beginning at 0x600 are intended for the ETI 660 computer.
        /// </summary>
        public byte[] Ram { get; set; }

        /// <summary>
        /// Vx general purpose registers.
        /// There are 16 general purpose 8-bit registers, usually referred to as Vx, where x is a hexadecimal digit (0 through F).
        /// </summary>
        public byte[] V { get; set; }

        /// <summary>
        /// The I register. This register is generally used to store memory addresses, so only the lowest (rightmost) 12 bits are usually used.
        /// </summary>
        public ushort I { get; set; }

        /// <summary>
        /// Delay Timer. Decrements at a rate of <see cref="Constants.TIMER_FREQUENCY_HZ"/>
        /// </summary>
        public byte DT { get; set; }

        /// <summary>
        /// SoundTimer. Decrements at a rate of <see cref="Constants.TIMER_FREQUENCY_HZ"/>
        /// </summary>
        public byte ST { get; set; }

        /// <summary>
        /// Program Counter
        /// </summary>
        public ushort PC { get; set; }

        /// <summary>
        /// Stack Pointer
        /// </summary>
        public byte SP { get; set; }

        /// <summary>
        /// The Stack
        /// </summary>
        public ushort[] Stack { get; set; }

        /// <summary>
        /// Display buffer
        /// </summary>
        public byte[] DisplayBuffer { get; set; }

        /// <summary>
        /// Keyboard state buffer
        /// </summary>
        public byte[] Keys { get; set; }

        public Model()
        {
            Ram = new byte[Constants.MEMORY_SIZE];
            V = new byte[Constants.VX_SIZE];
            I = 0;
            DT = 0;
            ST = 0;
            PC = Constants.PROGRAM_START_ADDRESS;
            SP = 0;
            Stack = new ushort[Constants.STACK_SIZE];
            DisplayBuffer = new byte[Constants.DISPLAY_BUFFER_SIZE];
            Keys = new byte[Constants.KEYBOARD_BUFFER_SIZE];

            // Initialize the ram with the sprite by default.
            Array.Copy(FontData.LETTERS, 0, Ram, Constants.FONT_START_ADDRESS, FontData.LETTERS.Length);
        }
    }
}
