using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibChip8;

namespace Chip8.Core.Test
{
    [TestClass]
    public class FontTest
    {
        [TestMethod]
        public void Font_LETTER_0_AreEqual()
        {
            byte[] expected = [0xF0, 0x90, 0x90, 0x90, 0xF0];
            byte[] actual  = Chip8FontData.LETTER_0;
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Font_LETTER_F_AreEqual()
        {
            byte[] expected = [0xF0, 0x80, 0xF0, 0x80, 0x80];
            byte [] actual = Chip8FontData.LETTER_F;
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Font_LETTERS_Contain_0()
        {
            byte[] expected = [0xF0, 0x90, 0x90, 0x90, 0xF0];
            byte[] actual = Chip8FontData.LETTERS;
            CollectionAssert.IsSubsetOf(expected, actual);
        }
    }
}
