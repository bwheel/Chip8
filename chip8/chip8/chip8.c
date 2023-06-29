

#include "chip8.h"

#include <stdio.h>
#include <stdbool.h>
#include <stdlib.h>
#include <memory.h>
#include <time.h>


// TODO: only define a handle in the struct and have that returned instead of Emulator.
// and keep Emulator inside the Core.

Emulator* Chip8_Init() {
	srand(time(NULL));

	Emulator emulator;

	emulator.programCounter = START_ADDRESS;
	return &emulator;
}

bool Chip8_Destroy(Emulator* pEmulator) {
	// TODO: free other pointers from within the struct.
	free(pEmulator);
	return true;

}

bool Chip8_LoadFile(Emulator* pEmulator, const char* filename) {
	return false;
}

bool Chip8_Run(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	while (true) {
		// fetch
		const uint8_t opcode = pEmulator->memory[emulator.programCounter];

		// decode
		// TODO: check opcode values for function from table

		// run
		// TODO: call appropriate Opcode method
		emulator.programCounter += 2;
	}

	return false;
}


/// <summary>
/// Clear the display.
/// </summary>
/// <param name="pemulator"></param>
void Opcode_00E0_CLS(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	memset(emulator.frameBuffer, 0, SCREEN_SIZE);
}

/// <summary>
/// Return from a subroutine.
/// The interpreter sets the program counter to the address at the top of the stack, then subtracts 1 from the stack pointer.
/// </summary>
/// <param name="pemulator"></param>
void Opcode_00EE_RET(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	emulator.programCounter = emulator.stack[emulator.stackPointer];
	emulator.stackPointer--;
}


/// <summary>
/// Call subroutine at nnn.
/// The interpreter increments the stack pointer, then puts the current PC on the top of the stack. The PC is then set to nnn.
/// </summary>
void Opcode_2NNN_CALL(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint16_t nnn = emulator.opcode & 0x0fffu;
	emulator.stackPointer++;
	emulator.stack[emulator.stackPointer] = emulator.programCounter;
	emulator.programCounter = nnn;
}

/// <summary>
/// Skip next instruction if Vx = kk.
/// The interpreter compares register Vx to kk, and if they are equal, increments the program counter by 2.
/// </summary>
void Opcode_3xkk_SE(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t kk = emulator.opcode & 0x00ffu;
	if (emulator.registers[vx] == kk) {
		emulator.programCounter += 2;
	}
}

/// <summary>
/// Skip next instruction if Vx != kk.
/// The interpreter compares register Vx to kk, and if they are not equal, increments the program counter by 2.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_4xkk_SNE(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t kk = emulator.opcode & 0x00ffu;
	if (emulator.registers[vx] != kk) {
		emulator.programCounter += 2;
	}
}

/// <summary>
/// Skip next instruction if Vx = Vy.
/// The interpreter compares register Vx to register Vy, and if they are equal, increments the program counter by 2.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_5xy0_SE(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t vy = (emulator.opcode & 0x00f0u) >> 4;
	if (emulator.registers[vx] == emulator.registers[vy]) {
		emulator.programCounter += 2;
	}
}

/// <summary>
/// Set Vx = kk.
/// The interpreter puts the value kk into register Vx.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_6xkk_LD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t kk = emulator.opcode & 0x00ffu;
	emulator.registers[vx] = kk;
}

/// <summary>
/// Set Vx = Vx + kk.
/// Adds the value kk to the value of register Vx, then stores the result in Vx.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_7xkk_ADD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t kk = emulator.opcode & 0x00ffu;
	emulator.registers[vx] += kk;
	// TODO: overflow?
}

/// <summary>
/// Set Vx = Vy.
/// Stores the value of register Vy in register Vx.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_8xy0_LD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t vy = (emulator.opcode & 0x00f0u) >> 4;
	emulator.registers[vx] = emulator.registers[vy];
}

/// <summary>
/// Set Vx = Vx OR Vy.
/// Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx. A bitwise OR compares the corrseponding bits from two values, and if either bit is 1, then the same bit in the result is also 1. Otherwise, it is 0.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_8xy1_OR(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t vy = (emulator.opcode & 0x00f0u) >> 4;
	emulator.registers[vx] |= emulator.registers[vy];
}

/// <summary>
/// Set Vx = Vx AND Vy.
/// Performs a bitwise AND on the values of Vx and Vy, then stores the result in Vx. A bitwise AND compares the corrseponding bits from two values, and if both bits are 1, then the same bit in the result is also 1. Otherwise, it is 0.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_8xy2_AND(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t vy = (emulator.opcode & 0x00f0u) >> 4;
	emulator.registers[vx] &= emulator.registers[vy];
}

/// <summary>
/// Set Vx = Vx XOR Vy.
/// Performs a bitwise exclusive OR on the values of Vx and Vy, then stores the result in Vx. An exclusive OR compares the corrseponding bits from two values, and if the bits are not both the same, then the corresponding bit in the result is set to 1. Otherwise, it is 0.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_8xy3_XOR(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t vy = (emulator.opcode & 0x00f0u) >> 4;
	emulator.registers[vx] ^= emulator.registers[vy];
}

/// <summary>
/// Set Vx = Vx + Vy, set VF = carry.
/// The values of Vx and Vy are added together. If the result is greater than 8 bits (i.e., > 255,) VF is set to 1, otherwise 0. Only the lowest 8 bits of the result are kept, and stored in Vx.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_8xy4_ADD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t vy = (emulator.opcode & 0x00f0u) >> 4;
	const uint16_t total = emulator.registers[vx] + emulator.registers[vy];
	if (total > 255) {
		emulator.registers[0xf] = 1;
	}
	else {
		emulator.registers[0xf] = 0;
	}
	emulator.registers[vx] = (total & 0x00ff);
}

/// <summary>
/// Set Vx = Vx - Vy, set VF = NOT borrow.
/// If Vx > Vy, then VF is set to 1, otherwise 0. Then Vy is subtracted from Vx, and the results stored in Vx.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_8xy5_SUB(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t vy = (emulator.opcode & 0x00f0u) >> 4;
	if (emulator.registers[vx] > emulator.registers[vy]) {
		emulator.registers[0xf] = 1;
	}
	else {
		emulator.registers[0xf] = 0;
	}
	emulator.registers[vx] -= emulator.registers[vy];
}

/// <summary>
/// Set Vx = Vx SHR 1.
/// If the least-significant bit of Vx is 1, then VF is set to 1, otherwise 0. Then Vx is divided by 2.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_8xy6_SHR(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	//const  uint8_t vy = (emulator.opcode & 0x00f0u) >> 4;
	if ((emulator.registers[vx] & 0x01) == 1) {
		emulator.registers[0xf] = 1;
	}
	else {
		emulator.registers[0xf] = 0;
	}
	emulator.registers[vx] >>= 1;
}

/// <summary>
/// Set Vx = Vy - Vx, set VF = NOT borrow.
/// If Vy > Vx, then VF is set to 1, otherwise 0. Then Vx is subtracted from Vy, and the results stored in Vx.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_8xy7_SUBN(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t vy = (emulator.opcode & 0x00f0u) >> 4;
	if (emulator.registers[vy] > emulator.registers[vx]) {
		emulator.registers[0xf] = 1;
	}
	else {
		emulator.registers[0xf] = 0;
	}
	emulator.registers[vx] -= emulator.registers[vy];
}

/// <summary>
/// Set Vx = Vx SHL 1.
/// If the most-significant bit of Vx is 1, then VF is set to 1, otherwise to 0. Then Vx is multiplied by 2.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_8xyE_SHL(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	//uint8_t vy = (emulator.opcode & 0x00f0u) >> 4;
	if ((emulator.registers[vx] & 0x1) == 1) {
		emulator.registers[0xf] = 1;
	}
	else {
		emulator.registers[0xf] = 0;
	}
	emulator.registers[vx] <<= 1;
}

/// <summary>
/// Skip next instruction if Vx != Vy.
/// The values of Vx and Vy are compared, and if they are not equal, the program counter is increased by 2.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_9xy0_SNE(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = (emulator.opcode & 0x0f00u) >> 8;
	const uint8_t vy = (emulator.opcode & 0x00f0u) >> 4;
	if (emulator.registers[vx] != emulator.registers[vy]) {
		emulator.programCounter += 2;
	}
}

/// <summary>
/// Set I = nnn.
/// The value of register I is set to nnn.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Annn_LD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint16_t nnn = emulator.opcode & 0x0fff;
	emulator.index = nnn;
}

/// <summary>
/// Jump to location nnn + V0.
/// The program counter is set to nnn plus the value of V0.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Bnnn_JP(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint16_t nnn = emulator.opcode & 0x0fff;
	const uint8_t v0 = emulator.registers[0];
	emulator.programCounter = nnn + v0;
}

/// <summary>
/// Set Vx = random byte AND kk.
/// The interpreter generates a random number from 0 to 255, which is then ANDed with the value kk. The results are stored in Vx. See instruction 8xy2 for more
/// information on AND.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Cxkk_RND(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = emulator.opcode & 0x0f00 >> 8;
	const uint8_t kk = emulator.opcode & 0x00ff;
	const uint8_t val = rand() % 255;
	emulator.registers[vx] = kk & val;
}

/// <summary>
/// Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.
/// The interpreter reads n bytes from memory, starting at the address stored in I. These bytes are then displayed as sprites on screen at coordinates (Vx, Vy).
/// Sprites are XORed onto the existing screen. If this causes any pixels to be erased, VF is set to 1, otherwise it is set to 0. If the sprite is positioned so
/// part of it is outside the coordinates of the display, it wraps around to the opposite side of the screen. See instruction 8xy3 for more information on XOR,
/// and section 2.4, Display, for more information on the Chip-8 screen and sprites.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Dxyn_DRW(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	const uint8_t vx = emulator.opcode & 0x0f00 >> 8;
	const uint8_t vy = emulator.opcode & 0x00f0 >> 4;
	const uint8_t* sprite = emulator.memory[emulator.index + SPRITE_SIZE];
	for (int x = vx; x < 8; x++) {
		// TODO: check if out of bounds
		for (int y = vy; y < 5; y++) {
			// TODO: check if out of bounds
			uint8_t pixel = sprite[x * y]; // TODO conver back to byte instead of x being bits.0;

		}
	}
	// TODO: implement
}

/// <summary>
/// Skip next instruction if key with the value of Vx is pressed.
/// Checks the keyboard, and if the key corresponding to the value of Vx is currently in the down position, PC is increased by 2.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Ex9E_SKP(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	// TODO: implement
}

/// <summary>
/// Skip next instruction if key with the value of Vx is not pressed.
/// Checks the keyboard, and if the key corresponding to the value of Vx is currently in the up position, PC is increased by 2.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_ExA1_SKNP(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	// TODO: implement
}

/// <summary>
/// Set Vx = delay timer value.
/// The value of DT is placed into Vx.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Fx07_LD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	// TODO: implement
}

/// <summary>
/// Wait for a key press, store the value of the key in Vx.
/// All execution stops until a key is pressed, then the value of that key is stored in Vx.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Fx0A_LD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	// TODO: implement
}

/// <summary>
/// Set delay timer = Vx.
/// DT is set equal to the value of Vx.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Fx15_LD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	// TODO: implement
}

/// <summary>
/// Set sound timer = Vx.
/// ST is set equal to the value of Vx.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Fx18_LD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	// TODO: implement
}

/// <summary>
/// Set I = I + Vx.
/// The values of I and Vx are added, and the results are stored in I.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Fx1E_ADD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	// TODO: implement
}

/// <summary>
/// Set I = location of sprite for digit Vx.
/// The value of I is set to the location for the hexadecimal sprite corresponding to the value of Vx. See section 2.4, Display, for more information on the Chip-8 hexadecimal font.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Fx29_LD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	// TODO: implement
}

/// <summary>
/// Store BCD representation of Vx in memory locations I, I+1, and I+2.
/// The interpreter takes the decimal value of Vx, and places the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Fx33_LD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	// TODO: implement
}

/// <summary>
/// Store registers V0 through Vx in memory starting at location I.
/// The interpreter copies the values of registers V0 through Vx into memory, starting at the address in I.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Fx55_LD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	// TODO: implement
}

/// <summary>
/// Read registers V0 through Vx from memory starting at location I.
/// The interpreter reads values from memory starting at location I into registers V0 through Vx.
/// </summary>
/// <param name="pEmulator"></param>
void Opcode_Fx65_LD(Emulator* pEmulator) {
	Emulator emulator = *pEmulator;
	// TODO: implement
}
