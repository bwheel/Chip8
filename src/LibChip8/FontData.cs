using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibChip8
{
    internal static class FontData
    {
        public static readonly byte[] LETTERS = [
            // 0
            0b11110000,
            0b10010000,
            0b10010000,
            0b10010000,
            0b11110000,
            //1
            0b00100000,
            0b01100000,
            0b00100000,
            0b00100000,
            0b01110000,
            //2
            0b11110000,
            0b00010000,
            0b11110000,
            0b10000000,
            0b11110000,
            //3
            0b11110000,
            0b00010000,
            0b11110000,
            0b00010000,
            0b11110000,
            // 4
            0b10010000,
            0b10010000,
            0b11110000,
            0b00010000,
            0b00010000,
            //5
            0b11110000,
            0b10000000,
            0b11110000,
            0b00010000,
            0b11110000,
            //6
            0b11110000,
            0b10000000,
            0b11110000,
            0b10010000,
            0b11110000,
            // 7
            0b11110000,
            0b00010000,
            0b00100000,
            0b01000000,
            0b01000000,
            // 8
            0b11110000,
            0b10010000,
            0b11110000,
            0b10010000,
            0b11110000,
            //9
            0b11110000,
            0b10010000,
            0b11110000,
            0b00010000,
            0b11110000,
            // a
            0b11110000,
            0b10010000,
            0b11110000,
            0b10010000,
            0b10010000,
            // b
            0b11100000,
            0b10010000,
            0b11100000,
            0b10010000,
            0b11100000,
            // c
            0b11110000,
            0b10000000,
            0b10000000,
            0b10000000,
            0b11110000,
            // d
            0b11100000,
            0b10010000,
            0b10010000,
            0b10010000,
            0b11100000,
            // e
            0b11110000,
            0b10000000,
            0b11110000,
            0b10000000,
            0b11110000,
            // f
            0b11110000,
            0b10000000,
            0b11110000,
            0b10000000,
            0b10000000
            ];
        public static readonly byte[] LETTER_0 = LETTERS!.Take(5).ToArray();
        public static readonly byte[] LETTER_1 = LETTERS!.Skip(5).Take(5).ToArray();
        public static readonly byte[] LETTER_2 = LETTERS!.Skip(10).Take(5).ToArray();
        public static readonly byte[] LETTER_3 = LETTERS!.Skip(15).Take(5).ToArray();
        public static readonly byte[] LETTER_4 = LETTERS!.Skip(20).Take(5).ToArray();
        public static readonly byte[] LETTER_5 = LETTERS!.Skip(25).Take(5).ToArray();
        public static readonly byte[] LETTER_6 = LETTERS!.Skip(30).Take(5).ToArray();
        public static readonly byte[] LETTER_7 = LETTERS!.Skip(35).Take(5).ToArray();
        public static readonly byte[] LETTER_8 = LETTERS!.Skip(40).Take(5).ToArray();
        public static readonly byte[] LETTER_9 = LETTERS!.Skip(45).Take(5).ToArray();
        public static readonly byte[] LETTER_A = LETTERS!.Skip(50).Take(5).ToArray();
        public static readonly byte[] LETTER_b = LETTERS!.Skip(55).Take(5).ToArray();
        public static readonly byte[] LETTER_C = LETTERS!.Skip(60).Take(5).ToArray();
        public static readonly byte[] LETTER_D = LETTERS!.Skip(65).Take(5).ToArray();
        public static readonly byte[] LETTER_E = LETTERS!.Skip(70).Take(5).ToArray();
        public static readonly byte[] LETTER_F = LETTERS!.Skip(75).Take(5).ToArray();
    }
}
