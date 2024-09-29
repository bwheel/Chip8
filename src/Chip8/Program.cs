using LibChip8;
using SDL2;
using static SDL2.SDL;

namespace Chip8;

public class Program
{

    public static bool processInput(Chip8Model model)
    {

        bool quit = false;
        SDL_Event evt;

        while (SDL_PollEvent(out evt) != 0)
        {
            switch (evt.type)
            {
                case SDL_EventType.SDL_QUIT:
                    {
                        quit = true;
                    }
                    break;

                case SDL_EventType.SDL_KEYDOWN:
                    {
                        switch (evt.key.keysym.sym)

                        {

                            case SDL_Keycode.SDLK_ESCAPE:
                                {
                                    quit = true;
                                }
                                break;

                            case SDL_Keycode.SDLK_x:
                                {
                                    model.Keys[0] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_1:
                                {
                                    model.Keys[1] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_2:
                                {
                                    model.Keys[2] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_3:
                                {
                                    model.Keys[3] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_q:
                                {
                                    model.Keys[4] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_w:
                                {
                                    model.Keys[5] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_e:
                                {
                                    model.Keys[6] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_a:
                                {
                                    model.Keys[7] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_s:
                                {
                                    model.Keys[8] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_d:
                                {
                                    model.Keys[9] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_z:
                                {
                                    model.Keys[0xA] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_c:
                                {
                                    model.Keys[0xB] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_4:
                                {
                                    model.Keys[0xC] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_r:
                                {
                                    model.Keys[0xD] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_f:
                                {
                                    model.Keys[0xE] = 1;
                                }
                                break;

                            case SDL_Keycode.SDLK_v:
                                {
                                    model.Keys[0xF] = 1;
                                }
                                break;
                        }
                    }
                    break;

                case SDL_EventType.SDL_KEYUP:
                    {
                        switch (evt.key.keysym.sym)

                        {

                            case SDL_Keycode.SDLK_x:
                                {
                                    model.Keys[0] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_1:
                                {
                                    model.Keys[1] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_2:
                                {
                                    model.Keys[2] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_3:
                                {
                                    model.Keys[3] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_q:
                                {
                                    model.Keys[4] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_w:
                                {
                                    model.Keys[5] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_e:
                                {
                                    model.Keys[6] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_a:
                                {
                                    model.Keys[7] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_s:
                                {
                                    model.Keys[8] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_d:
                                {
                                    model.Keys[9] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_z:
                                {
                                    model.Keys[0xA] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_c:
                                {
                                    model.Keys[0xB] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_4:
                                {
                                    model.Keys[0xC] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_r:
                                {
                                    model.Keys[0xD] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_f:
                                {
                                    model.Keys[0xE] = 0;
                                }
                                break;

                            case SDL_Keycode.SDLK_v:
                                {
                                    model.Keys[0xF] = 0;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        return quit;
    }

    /*
     void Update(void const* buffer, int pitch)
	{
		SDL_UpdateTexture(texture, nullptr, buffer, pitch);
		SDL_RenderClear(renderer);
		SDL_RenderCopy(renderer, texture, nullptr, nullptr);
		SDL_RenderPresent(renderer);
	}
     */
    public static void UpdateUI(nint texture, nint renderer, Chip8Model model)
    {
        throw new NotImplementedException();
    }

    public static void Main(string[] args)
    {
        _ = SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);

        Chip8Emulator emulator = new Chip8Emulator();
        Chip8Model model = new Chip8Model();
        emulator.LoadRom(model, "IBM Logo.ch8");
        int cycleDelay = 10;

        var window = SDL.SDL_CreateWindow("Chip8", 0, 0, 600, 300, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
        var renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
        var texture = SDL.SDL_CreateTexture(renderer, SDL.SDL_PIXELFORMAT_ARGB8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, 50, 50);

        long prevTime = DateTime.Now.Ticks;
        bool quit = false;
        while (!quit)
        {
            quit = processInput(model);
            long currentTime = DateTime.Now.Ticks;
            float deltaTime = currentTime - prevTime;

            if (deltaTime > cycleDelay)
            {
                prevTime = currentTime;
                emulator.Cycle(model);

            }
        }



        SDL.SDL_DestroyTexture(texture);
        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }
}