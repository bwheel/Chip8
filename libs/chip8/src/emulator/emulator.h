#ifndef EMULATOR_H
#define EMULATOR_H

#include <stdint.h>

#include <chip8/chip8.h>

#define START_ADDRESS 0x0200
#define FONT_START_ADDRESS 0x0050
#define FONT_SIZE 80
#define SCREEN_WIDTH CHIP8_SCREEN_WIDTH
#define SCREEN_HEIGHT CHIP8_SCREEN_HEIGHT
#define SCREEN_SIZE (SCREEN_WIDTH * SCREEN_HEIGHT)
#define SPRITE_SIZE 5
#define KEYBOARD_SIZE 16
#define REGISTERS_SIZE 16
#define STACK_SIZE 16
#define MEMORY_SIZE 4096
#define STATUS_REGISTER 0xF

typedef struct chip8_emulator_t
{
    uint8_t registers[REGISTERS_SIZE];
    uint16_t index;
    uint16_t stack[STACK_SIZE];
    uint8_t stackPointer;
    uint16_t programCounter;
    uint8_t delayTimer;
    uint8_t soundTimer;
    uint8_t frameBuffer[SCREEN_SIZE];
    uint8_t memory[MEMORY_SIZE];
    uint8_t keys[KEYBOARD_SIZE];
} chip8_emulator_t;

#endif // EMULATOR_H