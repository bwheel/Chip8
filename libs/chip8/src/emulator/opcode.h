#ifndef OPCODE_H
#define OPCODE_H

#include <stdint.h>

#include <chip8/chip8.h>
#include "emulator.h"

typedef struct chip8_instruction_t
{
    uint16_t raw;
    uint16_t nnn;
    uint8_t nn;
    uint8_t op;
    uint8_t x;
    uint8_t y;
    uint8_t n;
} chip8_instruction_t;

typedef chip8_error_t (*chip8_opcode_function_t)(chip8_emulator_t *emulator, const chip8_instruction_t *instruction);

extern const chip8_opcode_function_t opcode_table[16];
#endif // OPCODE_H