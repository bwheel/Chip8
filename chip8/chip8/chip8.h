#pragma once
#include <stdint.h>
#include <stdbool.h>


#define START_ADDRESS 0x0200
#define FONT_ADDRESS 0x0000
#define SCREEN_SIZE 32*64
#define SPRITE_SIZE 5

const uint8_t FONT[80] = {
	(uint8_t)0xf0, (uint8_t)0x90, (uint8_t)0x90, (uint8_t)0x90, (uint8_t)0xf0, // "0"
	(uint8_t)0x20, (uint8_t)0x60, (uint8_t)0x20, (uint8_t)0x20, (uint8_t)0x70, // "1"
	(uint8_t)0xf0, (uint8_t)0x10, (uint8_t)0xf0, (uint8_t)0x80, (uint8_t)0xf0, // "2"
	(uint8_t)0xf0, (uint8_t)0x10, (uint8_t)0xf0, (uint8_t)0x10, (uint8_t)0xf0, // "3"
	(uint8_t)0x90, (uint8_t)0x90, (uint8_t)0xf0, (uint8_t)0x10, (uint8_t)0x10, // "4"
	(uint8_t)0xf0, (uint8_t)0x80, (uint8_t)0xf0, (uint8_t)0x10, (uint8_t)0xf0, // "5"
	(uint8_t)0xf0, (uint8_t)0x80, (uint8_t)0xf0, (uint8_t)0x90, (uint8_t)0xf0, // "6"
	(uint8_t)0xf0, (uint8_t)0x10, (uint8_t)0x20, (uint8_t)0x40, (uint8_t)0x40, // "7"
	(uint8_t)0xf0, (uint8_t)0x90, (uint8_t)0xf0, (uint8_t)0x90, (uint8_t)0xf0, // "8"
	(uint8_t)0xf0, (uint8_t)0x90, (uint8_t)0xf0, (uint8_t)0x10, (uint8_t)0xf0, // "9"
	(uint8_t)0xf0, (uint8_t)0x90, (uint8_t)0xf0, (uint8_t)0x90, (uint8_t)0x90, // "A"
	(uint8_t)0xe0, (uint8_t)0x90, (uint8_t)0xe0, (uint8_t)0x90, (uint8_t)0xe0, // "B"
	(uint8_t)0xf0, (uint8_t)0x80, (uint8_t)0x80, (uint8_t)0x80, (uint8_t)0xf0, // "C"
	(uint8_t)0xe0, (uint8_t)0x90, (uint8_t)0x90, (uint8_t)0x90, (uint8_t)0xe0, // "D"
	(uint8_t)0xf0, (uint8_t)0x80, (uint8_t)0xf0, (uint8_t)0x80, (uint8_t)0xf0, // "E"
	(uint8_t)0xf0, (uint8_t)0x80, (uint8_t)0xf0, (uint8_t)0x80, (uint8_t)0x80, // "F"

};

typedef struct Emulator {
	uint8_t registers[16];
	uint16_t index;
	uint8_t stack[16];
	uint8_t stackPointer;
	uint16_t programCounter;
	uint8_t delayTimer;
	uint8_t soundTimer;
	uint8_t frameBuffer[SCREEN_SIZE];
	uint8_t memory[4096];
	uint8_t opcode;
} Emulator;


Emulator* Chip8_Init();
bool Chip8_Destroy(Emulator* pEmulator);
bool Chip8_LoadFile(Emulator* pEmulator, const char* filename);
bool Chip8_Run(Emulator* pEmulator);
