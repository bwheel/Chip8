# chip8

A CHIP-8 emulator core written in C23, built as a shared library (`chip8::chip8`) with a small command-line app (`chip8-cli`) alongside it.

## Layout

```
libs/chip8/
├── CMakeLists.txt        # project definition (C23, exports compile_commands.json)
├── include/chip8/
│   └── chip8.h           # public API
├── src/
│   ├── CMakeLists.txt    # builds the shared library
│   └── emulator/
│       ├── emulator.h    # emulator state and machine constants (internal)
│       ├── emulator.c    # lifecycle, ROM loading, fetch/decode/execute step
│       ├── opcode.h      # instruction struct and opcode table (internal)
│       └── opcode.c      # opcode implementations
└── apps/chip8/
    ├── CMakeLists.txt    # builds chip8-cli
    └── main.c            # CLI entry point (currently a placeholder)
```

Only `include/chip8/chip8.h` is public. The headers under `src/emulator/` are private to the library.

## Building

Requires CMake 3.21+, a build tool (the presets use Unix Makefiles) and a C23 compiler.

```sh
cmake --preset debug          # or: release
cmake --build --preset debug
```

Output goes to `build/<preset>/`.

## Public API

Declared in [include/chip8/chip8.h](include/chip8/chip8.h). Every function returns a `chip8_error_t`:

| Value                   | Meaning                               |
| ----------------------- | ------------------------------------- |
| `CHIP8_OK`              | Success                               |
| `CHIP8_ERROR`           | Generic failure                       |
| `CHIP8_INVALID_ARGUMENT`| Bad argument                          |
| `CHIP8_NOT_IMPLEMENTED` | Opcode or feature not implemented     |

| Function                                  | Purpose                                                        |
| ----------------------------------------- | -------------------------------------------------------------- |
| `chip8_emulator_open(chip8_emulator_t **)`| Allocate and reset an emulator, returned through the out param |
| `chip8_emulator_free`                     | Release an emulator                                            |
| `chip8_emulator_reset`                    | Reset registers, memory (reloading the font), timers, display  |
| `chip8_emulator_load_rom`                 | Load a ROM file (up to `4096 - 0x200` bytes) at `0x200`        |
| `chip8_emulator_load_rom_from_memory`     | Load a ROM from a buffer at `0x200`                            |
| `chip8_emulator_step`                     | Fetch, decode and execute one instruction                      |
| `chip8_emulator_tick_timers`              | Decrement the delay and sound timers (call at 60 Hz)           |
| `chip8_emulator_set_key`                  | Set the state of key `0x0`-`0xF`                               |
| `chip8_emulator_get_frame_buffer`         | Pointer to the 64×32 frame buffer (1 byte per pixel)           |
| `chip8_emulator_get_timers`               | Read the delay and sound timers                                |
| `chip8_emulator_get_registers`            | Copy `V0`–`VF`                                                 |
| `chip8_emulator_get_program_counter`      | Read the program counter                                       |
| `chip8_emulator_get_index`                | Read the index register                                        |

All public functions are marked `CHIP8_API` so they are exported from the shared library on every platform.

## Emulator model

- 4 KiB memory, program loaded at `0x200`, font sprites addressed from `0x050`
- 16 8-bit registers (`V0`–`VF`, with `VF` as the flag register), 16-bit index register
- 16-level call stack
- 64×32 monochrome frame buffer
- 16-key keypad
- Delay and sound timers

## Opcode coverage

All 16 top-level opcode groups are dispatched through `opcode_table` in `opcode.c`. Implemented instructions:

- `00E0`, `00EE`: clear screen, return
- `1NNN`, `2NNN`, `BNNN`: jump, call, jump with `V0` offset
- `3XNN`, `4XNN`, `5XY0`, `9XY0`: conditional skips
- `6XNN`, `7XNN`, `ANNN`, `CXNN`: load, add, set index, random
- `8XY0`–`8XY7`, `8XYE`: register arithmetic and logic (`8XY0` load, `OR`, `AND`, `XOR`, `ADD`, `SUB`, `SHR`, `SUBN`, `SHL`)
- `DXYN`: draw sprite
- `EX9E`, `EXA1`: skip on key state
- `FX07`, `FX0A`, `FX15`, `FX18`, `FX1E`, `FX29`, `FX33`, `FX55`, `FX65`: timers, key wait, index/font, BCD, register store/load

Unrecognised opcodes return `CHIP8_NOT_IMPLEMENTED` from `chip8_emulator_step`.

## Status

Work in progress. Known gaps:

- `chip8-cli` only prints a greeting; it does not load or run a ROM yet.
- `chip8_emulator_step` does not tick the timers; the host must call `chip8_emulator_tick_timers` at 60 Hz.
- No display, input or audio front end exists in C. The .NET wrapper in [`src/Chip8.Interop`](../../src/Chip8.Interop) exposes the state a front end needs.
