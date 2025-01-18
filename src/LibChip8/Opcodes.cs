using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibChip8
{
    internal static class Opcodes
    {

        public readonly static Dictionary<ushort, Action<ushort, Model>> Table = new Dictionary<ushort, Action<ushort, Model>>();

        static Opcodes()
        {
            Table[0x00e0] = CLS_00E0;
            Table[0x00ee] = RET_00EE;
            for (ushort i = 0x1000; i < 0x1fff; i++)
                Table[i] = JP_1nnn;
            for (ushort i = 0x2000; i < 0x2fff; i++)
                Table[i] = CALL_2nnn;
            for (ushort i = 0x3000; i < 0x3fff; i++)
                Table[i] = SE_3xkk;
            for (ushort i = 0x4000; i < 0x4fff; i++)
                Table[i] = SNE_4xkk;
            for (ushort i = 0x5000; i < 0x5fff; i++)
            {
                if ((i & 0x000F) == 0)
                    Table[i] = SE_5xy0;
            }
            for (ushort i = 0x6000; i < 0x6fff; i++)
                Table[i] = LD_6xkk;
            for (ushort i = 0x7000; i < 0x7fff; i++)
                Table[i] = ADD_7xkk;
            for (ushort i = 0x8000; i < 0x8fff; i++)
            {
                ushort lsbNibble = (ushort)(i & 0x000f);
                if (lsbNibble == 0)
                    Table[i] = LD_8xy0;
                else if (lsbNibble == 1)
                    Table[i] = OR_8xy1;
                else if (lsbNibble == 2)
                    Table[i] = AND_8xy2;
                else if (lsbNibble == 3)
                    Table[i] = XOR_8xy3;
                else if (lsbNibble == 4)
                    Table[i] = ADD_8xy4;
                else if (lsbNibble == 5)
                    Table[i] = SUB_8xy5;
                else if (lsbNibble == 6)
                    Table[i] = SHR_8xy6;
                else if (lsbNibble == 7)
                    Table[i] = SUBN_8xy7;
                else if (lsbNibble == 0xe)
                    Table[i] = SHL_8xyE;
            }
            for (ushort i = 0x9000; i < 0x9fff; i++)
            {
                if ((i & 0x000f) == 0)
                    Table[i] = SNE_9xy0;
            }
            for (ushort i = 0xa000; i < 0xafff; i++)
                Table[i] = LD_Annn;
            for (ushort i = 0xb000; i < 0xbfff; i++)
                Table[i] = JP_Bnnn;
            for (ushort i = 0xc000; i < 0xcfff; i++)
                Table[i] = RND_Cxkk;
            for (ushort i = 0xd000; i < 0xdfff; i++)
                Table[i] = DRW_Dxyn;
            for (ushort i = 0xe000; i < 0xef9e; i++)
            {
                ushort lsbByte = (ushort)(i & 0x00ff);
                if (lsbByte == 0x009e)
                    Table[i] = SKP_Ex9E;
                else if (lsbByte == 0x00a1)
                    Table[i] = SKNP_ExA1;
            }
            for (ushort i = 0xf007; i < 0xff07; i++)
            {
                ushort lsbByte = (ushort)(i & 0x00ff);
                if (lsbByte == 0x0007)
                    Table[i] = LD_Fx07;
                else if (lsbByte == 0x000a)
                    Table[i] = LD_Fx0A;
                else if (lsbByte == 0x0015)
                    Table[i] = LD_Fx15;
                else if (lsbByte == 0x0018)
                    Table[i] = LD_Fx18;
                else if (lsbByte == 0x001E)
                    Table[i] = ADD_Fx1E;
                else if (lsbByte == 0x0029)
                    Table[i] = LD_Fx29;
                else if (lsbByte == 0x0033)
                    Table[i] = LD_Fx33;
                else if (lsbByte == 0x0055)
                    Table[i] = LD_Fx55;
                else if (lsbByte == 0x0065)
                    Table[i] = LD_Fx65;
            }
            for (ushort i = 0; i < ushort.MaxValue; i++)
            {
                if (!Table.ContainsKey(i))
                    Table[i] = NOOP;
            }
        }

        public static void NOOP(ushort _, Model __)
        { }

        /// <summary>
        /// Clear the display
        /// </summary>
        /// <param name="model"></param>
        public static void CLS_00E0(ushort _, Model model)
        {
            Array.Clear(model.DisplayBuffer);
        }

        /// <summary>
        /// Return from a subroutine.
        /// </summary>
        /// <param name="model"></param>
        public static void RET_00EE(ushort _, Model model)
        {
            model.PC = model.Stack[model.SP];
            model.SP--;
        }

        /// <summary>
        /// Jump to location nnn.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void JP_1nnn(ushort opcode, Model model)
        {
            model.PC = (ushort)(opcode & 0x0fff);
        }

        /// <summary>
        /// Call subroutine at nnn.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void CALL_2nnn(ushort opcode, Model model)
        {
            model.Stack[++model.SP] = (ushort)(opcode & 0x0fff);
        }

        /// <summary>
        /// Skip next instruction if Vx = kk.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void SE_3xkk(ushort opcode, Model model)
        {
            byte kk = (byte)(opcode & 0x00ff);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            if (model.V[x] == kk)
            {
                model.PC += 2;
            }
        }

        /// <summary>
        /// Skip next instruction if Vx != kk.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void SNE_4xkk(ushort opcode, Model model)
        {
            byte kk = (byte)(opcode & 0x00ff);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            if (model.V[x] != kk)
            {
                model.PC += 2;
            }
        }

        /// <summary>
        /// Skip next instruction if Vx = Vy.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void SE_5xy0(ushort opcode, Model model)
        {
            byte y = (byte)((opcode & 0x00f0) >> 4);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            if (model.V[x] == model.V[y])
            {
                model.PC += 2;
            }
        }

        /// <summary>
        /// Set Vx = kk.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void LD_6xkk(ushort opcode, Model model)
        {
            byte kk = (byte)(opcode & 0x00ff);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.V[x] = kk;
        }

        /// <summary>
        /// Set Vx = Vx + kk.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void ADD_7xkk(ushort opcode, Model model)
        {
            byte kk = (byte)(opcode & 0x00ff);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.V[x] += kk;
        }

        /// <summary>
        /// Set Vx = Vy.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void LD_8xy0(ushort opcode, Model model)
        {
            byte y = (byte)((opcode & 0x00f0) >> 4);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.V[x] = model.V[y];
        }

        /// <summary>
        /// Set Vx = Vx OR Vy.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void OR_8xy1(ushort opcode, Model model)
        {
            byte y = (byte)((opcode & 0x00f0) >> 4);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.V[x] |= model.V[y];
        }

        /// <summary>
        /// Set Vx = Vx AND  Vy.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void AND_8xy2(ushort opcode, Model model)
        {
            byte y = (byte)((opcode & 0x00f0) >> 4);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.V[x] &= model.V[y];
        }

        /// <summary>
        /// Set Vx = Vx XOR Vy.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void XOR_8xy3(ushort opcode, Model model)
        {
            byte y = (byte)((opcode & 0x00f0) >> 4);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.V[x] ^= model.V[y];
        }

        /// <summary>
        /// Set Vx = Vx ADD  Vy.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void ADD_8xy4(ushort opcode, Model model)
        {
            byte y = (byte)((opcode & 0x00f0) >> 4);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            ushort temp = (ushort)(model.V[x] + model.V[y]);
            if (temp > byte.MaxValue)
            {
                model.V[0xf] = 1;
            }
            model.V[x] = (byte)(temp & 0xff);
        }


        /// <summary>
        /// Set Vx = Vx - Vy, set VF = NOT borrow.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void SUB_8xy5(ushort opcode, Model model)
        {
            byte y = (byte)((opcode & 0x00f0) >> 4);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            if (model.V[x] > model.V[y])
            {
                model.V[0xf] = 1;
            }
            else
            {
                model.V[0xf] = 0;
            }

            model.V[x] = (byte)((model.V[x] - model.V[y]) & 0xff);
        }

        /// <summary>
        /// Set Vx = Vx SHR 1.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void SHR_8xy6(ushort opcode, Model model)
        {
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.V[0xf] &= ((byte)(model.V[x] & 0x01));

            model.V[x] >>= 1;
        }

        /// <summary>
        /// Set Vx = Vy - Vx, set VF = NOT borrow.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void SUBN_8xy7(ushort opcode, Model model)
        {
            byte y = (byte)((opcode & 0x00f0) >> 4);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            if (model.V[y] > model.V[x])
            {
                model.V[0xf] = 1;
            }
            else
            {
                model.V[0xf] = 0;
            }
            model.V[x] = (byte)(model.V[y] - model.V[x]);
        }

        /// <summary>
        /// Set Vx = Vx SHL 1.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void SHL_8xyE(ushort opcode, Model model)
        {
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.V[0xf] &= (byte)((model.V[x] >> 7) & 0x01);

            model.V[x] <<= 1;
        }

        /// <summary>
        /// Skip next instruction if Vx != Vy.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void SNE_9xy0(ushort opcode, Model model)
        {
            byte y = (byte)((opcode & 0x00f0) >> 4);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            if (model.V[y] != model.V[x])
            {
                model.PC += 2;
            }
        }

        /// <summary>
        /// Set I = nnn.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void LD_Annn(ushort opcode, Model model)
        {
            ushort nnn = (ushort)(opcode & 0x0fff);
            model.I = nnn;
        }

        /// <summary>
        /// Jump to location nnn + V0.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void JP_Bnnn(ushort opcode, Model model)
        {
            ushort nnn = (ushort)(opcode & 0x0fff);
            model.PC = (ushort)(model.V[0] + nnn);
        }

        private static Random rng = new Random();

        /// <summary>
        /// Set Vx = random byte AND kk.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void RND_Cxkk(ushort opcode, Model model)
        {
            byte kk = (byte)(opcode & 0x00ff);
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.V[x] = (byte)(rng.Next(0, 255) & kk);

        }

        public static void DRW_Dxyn(ushort opcode, Model model)
        {
            byte fontHeight = (byte)(opcode & 0x000f);
            byte y = (byte)((opcode & 0x00f0) >> 4);
            byte x = (byte)((opcode & 0x0f00) >> 8);

            byte xPos = (byte)(model.V[x] & Constants.DISPLAY_WIDTH);
            byte yPos = (byte)(model.V[y] & Constants.DISPLAY_HEIGHT);

            model.V[0xf] = 0;

            for (int row = 0; row < fontHeight; row++)
            {
                byte spritRowOfPixels = model.Ram[model.I + row];
                for (int col = 0; col < Constants.FONT_LETTER_WIDTH; col++)
                {
                    // TODO: go through the 8 pixels across the sprite
                    byte spritePixel = (byte)(spritRowOfPixels & (0b1000_0000 >> col));
                    byte screenPixel = model.DisplayBuffer[(yPos + row) * Constants.DISPLAY_WIDTH + (xPos + col)];
                    if (spritePixel != 0)
                    {
                        if (screenPixel == 0xff)
                        {
                            model.V[0xf] = 1;
                        }
                        model.DisplayBuffer[(yPos + row) * Constants.DISPLAY_WIDTH + (xPos + col)] ^= spritePixel;
                    }
                }
            }
        }

        /// <summary>
        /// Skip next instruction if key with the value of Vx is pressed.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void SKP_Ex9E(ushort opcode, Model model)
        {

            byte x = (byte)((opcode & 0x0f00) >> 8);
            if (model.Keys[model.V[x]] == Constants.KEY_PRESSED_DOWN)
            {
                model.PC += 2;
            }
        }

        /// <summary>
        /// Skip next instruction if key with the value of Vx is not pressed.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void SKNP_ExA1(ushort opcode, Model model)
        {

            byte x = (byte)((opcode & 0x0f00) >> 8);
            if (model.Keys[model.V[x]] != Constants.KEY_PRESSED_DOWN)
            {
                model.PC += 2;
            }
        }

        /// <summary>
        /// Set Vx = delay timer value.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void LD_Fx07(ushort opcode, Model model)
        {
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.V[x] = model.DT;
        }

        /// <summary>
        /// Wait for a key press, store the value of the key in Vx.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void LD_Fx0A(ushort opcode, Model model)
        {
            byte x = (byte)((opcode & 0x0f00) >> 8);

            for (int i = 0; i < Constants.KEYBOARD_BUFFER_SIZE; i++)
            {
                if (model.Keys[i] == Constants.KEY_PRESSED_DOWN)
                {
                    model.V[x] = (byte)(i);
                    return;
                }
            }

            model.PC -= 2; // repeat instruction
        }

        /// <summary>
        /// Set delay timer = Vx.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void LD_Fx15(ushort opcode, Model model)
        {
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.DT = model.V[x];
        }

        /// <summary>
        /// Set sound timer = Vx.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void LD_Fx18(ushort opcode, Model model)
        {
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.ST = model.V[x];
        }

        /// <summary>
        /// Set I = I + Vx.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void ADD_Fx1E(ushort opcode, Model model)
        {
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.I += model.V[x];
        }

        /// <summary>
        /// Set I = I + Vx.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void LD_Fx29(ushort opcode, Model model)
        {
            byte x = (byte)((opcode & 0x0f00) >> 8);
            model.I = (ushort)(Constants.FONT_START_ADDRESS + (5 * model.V[x]));
        }

        /// <summary>
        /// Store BCD representation of Vx in memory locations I, I+1, and I+2.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void LD_Fx33(ushort opcode, Model model)
        {
            byte x = (byte)((opcode & 0x0f00) >> 8);
            byte val = model.V[x];
            model.Ram[model.I + 2] = (byte)(val % 10);
            val /= 10;
            model.Ram[model.I + 1] = (byte)(val % 10);
            val /= 10;
            model.Ram[model.I] = (byte)(val % 10);
        }

        /// <summary>
        /// Store registers V0 through Vx in memory starting at location I.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void LD_Fx55(ushort opcode, Model model)
        {
            byte x = (byte)((opcode & 0x0f00) >> 8);
            for (int i = 0; i < model.V[x]; i++)
            {
                model.Ram[model.I + i] = model.V[i];
            }
        }

        /// <summary>
        /// Read registers V0 through Vx from memory starting at location I.
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="model"></param>
        public static void LD_Fx65(ushort opcode, Model model)
        {
            byte x = (byte)((opcode & 0x0f00) >> 8);
            for (int i = 0; i < model.V[x]; i++)
            {
                model.V[i] = model.Ram[model.I + i];
            }
        }
    }
}
