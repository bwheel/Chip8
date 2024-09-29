using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibChip8
{
    public class Chip8Constants
    {

        // Addresses
        public const ushort RAM_START_ADDRESS = 0x000;
        public const ushort FONT_START_ADDRESS = 0x50;
        public const ushort FONT_END_ADDRESS = 0x1FF;
        public const ushort PROGRAM_START_ADDRESS = 0x200;
        public const ushort ETI_660_START_ADDRESS = 0x600;

        // Sprites
        public const int FONT_LETTER_WIDTH = 8;

        // Registers/Memory
        public const int MEMORY_SIZE = 0xFFF;
        public const int VX_SIZE = 16;
        public const int STACK_SIZE = 16;
        public const int KEYBOARD_BUFFER_SIZE = 16;

        // TIMERS
        public const int TIMER_FREQUENCY_HZ = 60;

        // Keyboard
        public const int KEY_PRESSED_DOWN = 1;

        // Display
        public const int DISPLAY_WIDTH = 64;
        public const int DISPLAY_HEIGHT = 32;
        public const int DISPLAY_BUFFER_SIZE = DISPLAY_WIDTH * DISPLAY_HEIGHT;
    }
}
