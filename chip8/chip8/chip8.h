#pragma once
#include <stdint.h>
#include <stdbool.h>


#define START_ADDRESS 0x0200
#define FONT_START_ADDRESS 0x0050
#define FONT_SIZE 80
#define SCREEN_WIDTH 64
#define SCREEN_HEIGHT 32
#define SCREEN_SIZE SCREEN_WIDTH*SCREEN_HEIGHT
#define SPRITE_SIZE 5
#define KEYBOARD_SIZE 16
#define REGISTERS_SIZE 16
#define STACK_SIZE 16
#define MEMORY_SIZE 4096


typedef struct Emulator {
	uint8_t registers[REGISTERS_SIZE];
	uint16_t index;
	uint16_t stack[STACK_SIZE];
	uint8_t stackPointer;
	uint16_t programCounter;
	uint8_t delayTimer;
	uint8_t soundTimer;
	uint8_t frameBuffer[SCREEN_SIZE];
	uint8_t memory[MEMORY_SIZE];
	uint8_t opcode;
	uint8_t keys[KEYBOARD_SIZE];
} Emulator;


Emulator* Chip8_Init();
bool Chip8_Destroy(Emulator* pEmulator);
bool Chip8_LoadFile(Emulator* pEmulator, const char* filename);
bool Chip8_LoadRom(Emulator* pEmulator, const uint8_t* buffer, uint32_t length);
bool Chip8_ExecuteStep(Emulator* pEmulator);
