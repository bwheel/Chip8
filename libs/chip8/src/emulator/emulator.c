#include <stdio.h>
#include <stdlib.h>
#include <memory.h>
#include <stdint.h>

#include "emulator.h"
#include <chip8/chip8.h>
#include "opcode.h"

static const uint8_t font_data[FONT_SIZE] = {
    0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
    0x20, 0x60, 0x20, 0x20, 0x70, // 1
    0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
    0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
    0x90, 0x90, 0xF0, 0x10, 0x10, // 4
    0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
    0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
    0xF0, 0x10, 0x20, 0x40, 0x40, // 7
    0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
    0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
    0xF0, 0x90, 0xF0, 0x90, 0x90, // A
    0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
    0xF0, 0x80, 0x80, 0x80, 0xF0, // C
    0xE0, 0x90, 0x90, 0x90, 0xE0, // D
    0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
    0xF0, 0x80, 0xF0, 0x80, 0x80, // F
};

chip8_error_t chip8_emulator_open(chip8_emulator_t **out)
{
    if (!out)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    *out = NULL;

    chip8_emulator_t *emulator = malloc(sizeof(chip8_emulator_t));
    if (!emulator)
    {
        return CHIP8_ERROR;
    }

    chip8_error_t error = chip8_emulator_reset(emulator);
    if (error != CHIP8_OK)
    {
        free(emulator);
        return error;
    }

    *out = emulator;
    return CHIP8_OK;
}

chip8_error_t chip8_emulator_free(chip8_emulator_t *emulator)
{
    free(emulator);
    return CHIP8_OK;
}

chip8_error_t chip8_emulator_reset(chip8_emulator_t *emulator)
{
    if (!emulator)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    memset(emulator->registers, 0, sizeof(emulator->registers));
    emulator->index = 0;
    memset(emulator->stack, 0, sizeof(emulator->stack));
    emulator->stackPointer = 0;
    emulator->programCounter = START_ADDRESS;
    emulator->delayTimer = 0;
    emulator->soundTimer = 0;
    memset(emulator->frameBuffer, 0, sizeof(emulator->frameBuffer));
    memset(emulator->memory, 0, sizeof(emulator->memory));
    memcpy(emulator->memory + FONT_START_ADDRESS, font_data, sizeof(font_data));
    memset(emulator->keys, 0, sizeof(emulator->keys));
    return CHIP8_OK;
}

chip8_error_t chip8_emulator_load_rom(chip8_emulator_t *emulator, const char *rom_path)
{
    if (!emulator || !rom_path)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    FILE *file = fopen(rom_path, "rb");
    if (!file)
    {
        return CHIP8_ERROR;
    }

    // Read one byte more than fits so an oversized ROM is detected.
    const size_t capacity = MEMORY_SIZE - START_ADDRESS;
    size_t bytes_read = fread(emulator->memory + START_ADDRESS, sizeof(uint8_t), capacity, file);
    int too_large = bytes_read == capacity && fgetc(file) != EOF;
    int read_failed = ferror(file);
    fclose(file);

    return (too_large || read_failed) ? CHIP8_ERROR : CHIP8_OK;
}

chip8_error_t chip8_emulator_load_rom_from_memory(chip8_emulator_t *emulator, const uint8_t *rom_data, size_t rom_size)
{
    if (!emulator || (!rom_data && rom_size > 0) || rom_size > MEMORY_SIZE - START_ADDRESS)
    {
        return CHIP8_INVALID_ARGUMENT;
    }

    if (rom_size > 0)
    {
        memcpy(emulator->memory + START_ADDRESS, rom_data, rom_size);
    }
    return CHIP8_OK;
}

chip8_error_t chip8_emulator_tick_timers(chip8_emulator_t *emulator)
{
    if (!emulator)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    if (emulator->delayTimer > 0)
    {
        emulator->delayTimer--;
    }
    if (emulator->soundTimer > 0)
    {
        emulator->soundTimer--;
    }
    return CHIP8_OK;
}

chip8_error_t chip8_emulator_set_key(chip8_emulator_t *emulator, uint8_t key, uint8_t pressed)
{
    if (!emulator || key >= KEYBOARD_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->keys[key] = pressed ? 1 : 0;
    return CHIP8_OK;
}

chip8_error_t chip8_emulator_get_frame_buffer(const chip8_emulator_t *emulator, const uint8_t **buffer, size_t *length)
{
    if (!emulator || !buffer || !length)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    *buffer = emulator->frameBuffer;
    *length = sizeof(emulator->frameBuffer);
    return CHIP8_OK;
}

chip8_error_t chip8_emulator_get_timers(const chip8_emulator_t *emulator, uint8_t *delay, uint8_t *sound)
{
    if (!emulator || !delay || !sound)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    *delay = emulator->delayTimer;
    *sound = emulator->soundTimer;
    return CHIP8_OK;
}

chip8_error_t chip8_emulator_get_registers(const chip8_emulator_t *emulator, uint8_t *registers)
{
    if (!emulator || !registers)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    memcpy(registers, emulator->registers, sizeof(emulator->registers));
    return CHIP8_OK;
}

chip8_error_t chip8_emulator_get_program_counter(const chip8_emulator_t *emulator, uint16_t *program_counter)
{
    if (!emulator || !program_counter)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    *program_counter = emulator->programCounter;
    return CHIP8_OK;
}

chip8_error_t chip8_emulator_get_index(const chip8_emulator_t *emulator, uint16_t *index)
{
    if (!emulator || !index)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    *index = emulator->index;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_fetch_instruction(chip8_emulator_t *emulator, chip8_instruction_t *instruction)
{
    if (!emulator || !instruction)
    {
        return CHIP8_ERROR;
    }
    if (emulator->programCounter >= MEMORY_SIZE - 1)
    {
        return CHIP8_ERROR;
    }
    uint16_t opcode = (emulator->memory[emulator->programCounter] << 8) | emulator->memory[emulator->programCounter + 1];

    *instruction = (chip8_instruction_t){
        .raw = opcode,
        .op = (((opcode) & 0xF000u) >> 12),
        .x = (((opcode) & 0x0F00u) >> 8),
        .y = (((opcode) & 0x00F0u) >> 4),
        .n = (((opcode) & 0x000Fu)),
        .nn = (((opcode) & 0x00FFu)),
        .nnn = (((opcode) & 0x0FFFu)),
    };
    return CHIP8_OK;
}

static inline chip8_error_t chip8_decode_opcode(const chip8_instruction_t instruction, chip8_opcode_function_t *opcode_function)
{
    if (instruction.op <= 0xF)
    {
        *opcode_function = opcode_table[instruction.op];
        return CHIP8_OK;
    }
    return CHIP8_NOT_IMPLEMENTED;
}

static chip8_error_t chip8_execute_opcode(chip8_emulator_t *emulator, const chip8_opcode_function_t opcode_function, const chip8_instruction_t instruction)
{
    if (opcode_function)
    {
        return opcode_function(emulator, &instruction);
    }
    return CHIP8_NOT_IMPLEMENTED;
}

chip8_error_t chip8_emulator_step(chip8_emulator_t *emulator)
{
    // fetch
    chip8_instruction_t instruction;
    chip8_error_t error = chip8_fetch_instruction(emulator, &instruction);
    if (error != CHIP8_OK)
    {
        return error;
    }

    // decode
    chip8_opcode_function_t opcode_function;
    error = chip8_decode_opcode(instruction, &opcode_function);
    if (error != CHIP8_OK)
    {
        return error;
    }

    // execute
    return chip8_execute_opcode(emulator, opcode_function, instruction);
}
