#include <string.h>

#include <chip8/chip8.h>
#include "opcode.h"
#include "emulator.h"

static inline chip8_error_t chip8_opcode_00E0_CLEAR(chip8_emulator_t *emulator, [[maybe_unused]] const chip8_instruction_t *instruction)
{
    memset(emulator->frameBuffer, 0, sizeof(emulator->frameBuffer));
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_00EE_RETURN(chip8_emulator_t *emulator, [[maybe_unused]] const chip8_instruction_t *instruction)
{
    if (emulator->stackPointer == 0)
    {
        return CHIP8_ERROR;
    }
    emulator->stackPointer--;
    emulator->programCounter = emulator->stack[emulator->stackPointer];
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_1NNN_JUMP(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    emulator->programCounter = instruction->nnn;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_2NNN_CALL(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (emulator->stackPointer >= STACK_SIZE)
    {
        return CHIP8_ERROR;
    }
    emulator->stack[emulator->stackPointer] = emulator->programCounter + 2;
    emulator->stackPointer++;
    emulator->programCounter = instruction->nnn;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_3XNN_SKIP_EQ(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    if (emulator->registers[instruction->x] == instruction->nn)
    {
        emulator->programCounter += 4;
    }
    else
    {
        emulator->programCounter += 2;
    }
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_4XNN_SKIP_NEQ(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    if (emulator->registers[instruction->x] != instruction->nn)
    {
        emulator->programCounter += 4;
    }
    else
    {
        emulator->programCounter += 2;
    }
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_5XY0_SKIP_EQ_REG(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE || instruction->y >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    if (emulator->registers[instruction->x] == emulator->registers[instruction->y])
    {
        emulator->programCounter += 4;
    }
    else
    {
        emulator->programCounter += 2;
    }
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_6XNN_LOAD(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->registers[instruction->x] = instruction->nn;
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_7XNN_ADD(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->registers[instruction->x] += instruction->nn;
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_8XY0_LOAD_REG(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE || instruction->y >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->registers[instruction->x] = emulator->registers[instruction->y];
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_8XY1_OR(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE || instruction->y >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->registers[instruction->x] |= emulator->registers[instruction->y];
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_8XY2_AND(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE || instruction->y >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->registers[instruction->x] &= emulator->registers[instruction->y];
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_8XY3_XOR(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE || instruction->y >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->registers[instruction->x] ^= emulator->registers[instruction->y];
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_8XY4_ADD_REG(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE || instruction->y >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    uint16_t sum = emulator->registers[instruction->x] + emulator->registers[instruction->y];
    emulator->registers[STATUS_REGISTER] = (sum > 0xFF) ? 1 : 0; // Set carry flag
    emulator->registers[instruction->x] = sum & 0xFF;            // Store the lower 8 bits
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_8XY5_SUB_REG(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE || instruction->y >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->registers[STATUS_REGISTER] = (emulator->registers[instruction->x] > emulator->registers[instruction->y]) ? 1 : 0; // Set borrow flag
    emulator->registers[instruction->x] -= emulator->registers[instruction->y];
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_8XY6_SHR(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->registers[STATUS_REGISTER] = emulator->registers[instruction->x] & 0x1; // Set least significant bit flag
    emulator->registers[instruction->x] >>= 1;
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_8XY7_SUBN_REG(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE || instruction->y >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->registers[STATUS_REGISTER] = (emulator->registers[instruction->y] > emulator->registers[instruction->x]) ? 1 : 0; // Set borrow flag
    emulator->registers[instruction->x] = emulator->registers[instruction->y] - emulator->registers[instruction->x];
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_8XYE_SHL(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->registers[STATUS_REGISTER] = (emulator->registers[instruction->x] & 0x80) >> 7; // Set most significant bit flag
    emulator->registers[instruction->x] <<= 1;
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_9XY0_SKIP_NEQ_REG(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE || instruction->y >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    if (emulator->registers[instruction->x] != emulator->registers[instruction->y])
    {
        emulator->programCounter += 4;
    }
    else
    {
        emulator->programCounter += 2;
    }
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_ANNN_LOAD_INDEX(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    emulator->index = instruction->nnn;
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_BNNN_JUMP_V0(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    emulator->programCounter = instruction->nnn + emulator->registers[0];
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_CXNN_RND(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    uint8_t random_value = rand() % 256; // Generate a random number between 0 and 255
    emulator->registers[instruction->x] = random_value & instruction->nn;
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_DXYN_DRAW(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE || instruction->y >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }

    uint8_t x = emulator->registers[instruction->x] % SCREEN_WIDTH;
    uint8_t y = emulator->registers[instruction->y] % SCREEN_HEIGHT;
    uint8_t height = instruction->n;

    emulator->registers[STATUS_REGISTER] = 0; // Reset collision flag

    for (uint8_t row = 0; row < height; row++)
    {
        uint8_t sprite_byte = emulator->memory[emulator->index + row];
        for (uint8_t col = 0; col < 8; col++)
        {
            if ((sprite_byte & (0x80 >> col)) != 0) // Check if the bit is set
            {
                uint16_t pixel_index = ((y + row) % SCREEN_HEIGHT) * SCREEN_WIDTH + ((x + col) % SCREEN_WIDTH);
                if (emulator->frameBuffer[pixel_index] == 1)
                {
                    emulator->registers[STATUS_REGISTER] = 1; // Set collision flag
                }
                emulator->frameBuffer[pixel_index] ^= 1; // Toggle pixel
            }
        }
    }

    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_EX9E_SKIP_KEY(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    uint8_t key = emulator->registers[instruction->x];
    if (key >= KEYBOARD_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    if (emulator->keys[key])
    {
        emulator->programCounter += 4;
    }
    else
    {
        emulator->programCounter += 2;
    }
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_EXA1_SKIP_NOKEY(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    uint8_t key = emulator->registers[instruction->x];
    if (key >= KEYBOARD_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    if (!emulator->keys[key])
    {
        emulator->programCounter += 4;
    }
    else
    {
        emulator->programCounter += 2;
    }
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_FX07_LOAD_DELAY(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->registers[instruction->x] = emulator->delayTimer;
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_FX0A_WAIT_KEY(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    for (uint8_t key = 0; key < KEYBOARD_SIZE; key++)
    {
        if (emulator->keys[key])
        {
            emulator->registers[instruction->x] = key;
            emulator->programCounter += 2;
            return CHIP8_OK;
        }
    }
    // If no key is pressed, do not advance the program counter
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_FX15_SET_DELAY(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->delayTimer = emulator->registers[instruction->x];
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_FX18_SET_SOUND(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->soundTimer = emulator->registers[instruction->x];
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_FX1E_ADD_INDEX(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->index += emulator->registers[instruction->x];
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_FX29_LOAD_FONT(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    uint8_t character = emulator->registers[instruction->x];
    if (character > 0xF)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    emulator->index = FONT_START_ADDRESS + (character * SPRITE_SIZE);
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_FX33_BCD(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    uint8_t value = emulator->registers[instruction->x];
    emulator->memory[emulator->index] = value / 100;
    emulator->memory[emulator->index + 1] = (value / 10) % 10;
    emulator->memory[emulator->index + 2] = value % 10;
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_FX55_STORE_REGS(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    for (uint8_t i = 0; i <= instruction->x; i++)
    {
        emulator->memory[emulator->index + i] = emulator->registers[i];
    }
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_FX65_LOAD_REGS(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    if (instruction->x >= REGISTERS_SIZE)
    {
        return CHIP8_INVALID_ARGUMENT;
    }
    for (uint8_t i = 0; i <= instruction->x; i++)
    {
        emulator->registers[i] = emulator->memory[emulator->index + i];
    }
    emulator->programCounter += 2;
    return CHIP8_OK;
}

static inline chip8_error_t chip8_opcode_not_implemented([[maybe_unused]] chip8_emulator_t *emulator, [[maybe_unused]] const chip8_instruction_t *instruction)
{
    return CHIP8_NOT_IMPLEMENTED;
}

static inline chip8_error_t chip8_0xxx(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    switch (instruction->nn)
    {
    case 0xE0:
        return chip8_opcode_00E0_CLEAR(emulator, instruction);
    case 0xEE:
        return chip8_opcode_00EE_RETURN(emulator, instruction);
    default:
        return CHIP8_NOT_IMPLEMENTED;
    }
}
static inline chip8_error_t chip8_8xxx(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    switch (instruction->n)
    {
    case 0x0:
        return chip8_opcode_8XY0_LOAD_REG(emulator, instruction);
    case 0x1:
        return chip8_opcode_8XY1_OR(emulator, instruction);
    case 0x2:
        return chip8_opcode_8XY2_AND(emulator, instruction);
    case 0x3:
        return chip8_opcode_8XY3_XOR(emulator, instruction);
    case 0x4:
        return chip8_opcode_8XY4_ADD_REG(emulator, instruction);
    case 0x5:
        return chip8_opcode_8XY5_SUB_REG(emulator, instruction);
    case 0x6:
        return chip8_opcode_8XY6_SHR(emulator, instruction);
    case 0x7:
        return chip8_opcode_8XY7_SUBN_REG(emulator, instruction);
    case 0xE:
        return chip8_opcode_8XYE_SHL(emulator, instruction);
    default:
        return CHIP8_NOT_IMPLEMENTED;
    }
}

static inline chip8_error_t chip8_Fxxx(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    switch (instruction->nn)
    {
    case 0x07:
        return chip8_opcode_FX07_LOAD_DELAY(emulator, instruction);
    case 0x0A:
        return chip8_opcode_FX0A_WAIT_KEY(emulator, instruction);
    case 0x15:
        return chip8_opcode_FX15_SET_DELAY(emulator, instruction);
    case 0x18:
        return chip8_opcode_FX18_SET_SOUND(emulator, instruction);
    case 0x1E:
        return chip8_opcode_FX1E_ADD_INDEX(emulator, instruction);
    case 0x29:
        return chip8_opcode_FX29_LOAD_FONT(emulator, instruction);
    case 0x33:
        return chip8_opcode_FX33_BCD(emulator, instruction);
    case 0x55:
        return chip8_opcode_FX55_STORE_REGS(emulator, instruction);
    case 0x65:
        return chip8_opcode_FX65_LOAD_REGS(emulator, instruction);
    default:
        return CHIP8_NOT_IMPLEMENTED;
    }
}

static inline chip8_error_t chip8_Exxx(chip8_emulator_t *emulator, const chip8_instruction_t *instruction)
{
    switch (instruction->nn)
    {
    case 0x9E:
        return chip8_opcode_EX9E_SKIP_KEY(emulator, instruction);
    case 0xA1:
        return chip8_opcode_EXA1_SKIP_NOKEY(emulator, instruction);
    default:
        return CHIP8_NOT_IMPLEMENTED;
    }
}

const chip8_opcode_function_t opcode_table[16] = {
    [0x0] = chip8_0xxx,
    [0x1] = chip8_opcode_1NNN_JUMP,
    [0x2] = chip8_opcode_2NNN_CALL,
    [0x3] = chip8_opcode_3XNN_SKIP_EQ,
    [0x4] = chip8_opcode_4XNN_SKIP_NEQ,
    [0x5] = chip8_opcode_5XY0_SKIP_EQ_REG,
    [0x6] = chip8_opcode_6XNN_LOAD,
    [0x7] = chip8_opcode_7XNN_ADD,
    [0x8] = chip8_8xxx,
    [0x9] = chip8_opcode_9XY0_SKIP_NEQ_REG,
    [0xA] = chip8_opcode_ANNN_LOAD_INDEX,
    [0xB] = chip8_opcode_BNNN_JUMP_V0,
    [0xC] = chip8_opcode_CXNN_RND,
    [0xD] = chip8_opcode_DXYN_DRAW,
    [0xE] = chip8_Exxx,
    [0xF] = chip8_Fxxx,
};
