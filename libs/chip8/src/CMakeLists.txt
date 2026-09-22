add_library(chip8 SHARED
    emulator/emulator.c
    emulator/opcode.c
)
add_library(chip8::chip8 ALIAS chip8)

target_compile_definitions(chip8 PRIVATE CHIP8_BUILDING)

target_include_directories(chip8
    PUBLIC
        $<BUILD_INTERFACE:${PROJECT_SOURCE_DIR}/include>
    PRIVATE
        ${CMAKE_CURRENT_SOURCE_DIR}/emulator   # internal-only headers
)

# Ship the public headers next to the built library: <lib dir>/include/chip8/
add_custom_command(TARGET chip8 POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_directory
        ${PROJECT_SOURCE_DIR}/include/chip8
        $<TARGET_FILE_DIR:chip8>/include/chip8
    COMMENT "Copying public headers to output directory"
)

if(MSVC)
    target_compile_options(chip8 PRIVATE /W4)
else()
    target_compile_options(chip8 PRIVATE -Wall -Wextra)
endif()
