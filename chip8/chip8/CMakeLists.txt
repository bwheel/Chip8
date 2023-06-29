# CMakeList.txt : CMake project for chip8, include source and define
# project specific logic here.
#

# Add source to this project's executable.
add_executable (chip8 "main.c" "chip8.h" "chip8.c")

if (CMAKE_VERSION VERSION_GREATER 3.12)
  set_property(TARGET chip8 PROPERTY CXX_STANDARD 20)
endif()

# TODO: Add tests and install targets if needed.
