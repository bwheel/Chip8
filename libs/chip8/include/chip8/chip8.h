#ifndef CHIP8_H
#define CHIP8_H

#include <stdint.h>
#include <stdlib.h>

#if defined(_WIN32)
#if defined(CHIP8_BUILDING)
#define CHIP8_API __declspec(dllexport)
#else
#define CHIP8_API __declspec(dllimport)
#endif
#elif defined(__GNUC__) || defined(__clang__)
#define CHIP8_API __attribute__((visibility("default")))
#else
#define CHIP8_API
#endif

#ifdef __cplusplus
extern "C"
{
#endif

#define CHIP8_SCREEN_WIDTH 64
#define CHIP8_SCREEN_HEIGHT 32
#define CHIP8_KEY_COUNT 16
#define CHIP8_REGISTER_COUNT 16

    typedef struct chip8_emulator_t chip8_emulator_t;

    typedef enum
    {
        CHIP8_OK = 0,
        CHIP8_ERROR = 1,
        CHIP8_INVALID_ARGUMENT = 2,
        CHIP8_NOT_IMPLEMENTED = 99,
    } chip8_error_t;

    /* Allocates and resets an emulator. On success *out receives the instance, which must be released with chip8_emulator_free. */
    CHIP8_API chip8_error_t chip8_emulator_open(chip8_emulator_t **out);
    CHIP8_API chip8_error_t chip8_emulator_free(chip8_emulator_t *emulator);
    CHIP8_API chip8_error_t chip8_emulator_reset(chip8_emulator_t *emulator);
    CHIP8_API chip8_error_t chip8_emulator_load_rom(chip8_emulator_t *emulator, const char *rom_path);
    CHIP8_API chip8_error_t chip8_emulator_load_rom_from_memory(chip8_emulator_t *emulator, const uint8_t *rom_data, size_t rom_size);
    CHIP8_API chip8_error_t chip8_emulator_step(chip8_emulator_t *emulator);

    /* Decrements the delay and sound timers (if non-zero). Call at 60 Hz. */
    CHIP8_API chip8_error_t chip8_emulator_tick_timers(chip8_emulator_t *emulator);

    /* Sets key state; key is 0x0-0xF. */
    CHIP8_API chip8_error_t chip8_emulator_set_key(chip8_emulator_t *emulator, uint8_t key, uint8_t pressed);

    /* Points *buffer at the emulator's frame buffer (CHIP8_SCREEN_WIDTH * CHIP8_SCREEN_HEIGHT bytes, one byte per pixel, 0 or 1).
       The pointer is owned by the emulator and valid until chip8_emulator_free. */
    CHIP8_API chip8_error_t chip8_emulator_get_frame_buffer(const chip8_emulator_t *emulator, const uint8_t **buffer, size_t *length);

    CHIP8_API chip8_error_t chip8_emulator_get_timers(const chip8_emulator_t *emulator, uint8_t *delay, uint8_t *sound);

    /* Copies CHIP8_REGISTER_COUNT bytes (V0-VF) into registers. */
    CHIP8_API chip8_error_t chip8_emulator_get_registers(const chip8_emulator_t *emulator, uint8_t *registers);
    CHIP8_API chip8_error_t chip8_emulator_get_program_counter(const chip8_emulator_t *emulator, uint16_t *program_counter);
    CHIP8_API chip8_error_t chip8_emulator_get_index(const chip8_emulator_t *emulator, uint16_t *index);

#ifdef __cplusplus
}
#endif

#endif // CHIP8_H
