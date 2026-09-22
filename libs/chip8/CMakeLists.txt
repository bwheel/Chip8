cmake_minimum_required(VERSION 3.21)
project(chip8 LANGUAGES C)

set(CMAKE_C_STANDARD 23)
set(CMAKE_C_STANDARD_REQUIRED ON)

# generates compile_commands.json - nice for editors/clangd
set(CMAKE_EXPORT_COMPILE_COMMANDS ON)

# Lets the shared library link on Windows until explicit export macros exist.
set(CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS ON)

add_subdirectory(src)
add_subdirectory(apps)
