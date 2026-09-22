add_executable(chip8-cli main.c)
target_link_libraries(chip8-cli PRIVATE chip8::chip8)
