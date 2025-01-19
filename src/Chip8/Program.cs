using LibChip8;
using SDL2;


namespace Chip8;

public class Program
{

    private static class Colors
    {
        public static readonly SDL.SDL_Color WHITE = new SDL.SDL_Color()
        {
            r = 255,
            b = 255,
            g = 255,
            a = 255,
        };
        public static readonly SDL.SDL_Color BLACK = new SDL.SDL_Color()
        {
            r = 0,
            g = 0,
            b = 0,
            a = 255,
        };
        public static readonly SDL.SDL_Color NEON_GREEN = new SDL.SDL_Color()
        {
            r = 119,
            g = 235,
            b = 52,
            a = 255,
        };
    }

    private static class Configuration
    {
        public const string WINDOW_TITLE = "Chip8";
        public const int WINDOW_OFFSET_X = 100;
        public const int WINDOW_OFFSET_Y = 100;
        public const int WINDOW_WIDTH = 1280;
        public const int WINDOW_HEIGHT = 640;
        public const int PIXEL_WIDTH = WINDOW_WIDTH / 64;
        public const int PIXEL_HEIGHT = WINDOW_HEIGHT / 32;
    }

    private static bool processInput(Model model)
    {
        bool quit = false;
        SDL.SDL_Event evt;

        while (SDL.SDL_PollEvent(out evt) != 0)
        {
            switch (evt.type)
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    {
                        quit = true;
                    }
                    break;

                case SDL.SDL_EventType.SDL_KEYDOWN:
                    {
                        switch (evt.key.keysym.sym)

                        {

                            case SDL.SDL_Keycode.SDLK_ESCAPE:
                                {
                                    quit = true;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_x:
                                {
                                    model.Keys[0] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_1:
                                {
                                    model.Keys[1] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_2:
                                {
                                    model.Keys[2] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_3:
                                {
                                    model.Keys[3] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_q:
                                {
                                    model.Keys[4] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_w:
                                {
                                    model.Keys[5] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_e:
                                {
                                    model.Keys[6] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_a:
                                {
                                    model.Keys[7] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_s:
                                {
                                    model.Keys[8] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_d:
                                {
                                    model.Keys[9] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_z:
                                {
                                    model.Keys[0xA] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_c:
                                {
                                    model.Keys[0xB] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_4:
                                {
                                    model.Keys[0xC] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_r:
                                {
                                    model.Keys[0xD] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_f:
                                {
                                    model.Keys[0xE] = 1;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_v:
                                {
                                    model.Keys[0xF] = 1;
                                }
                                break;
                        }
                    }
                    break;

                case SDL.SDL_EventType.SDL_KEYUP:
                    {
                        switch (evt.key.keysym.sym)

                        {

                            case SDL.SDL_Keycode.SDLK_x:
                                {
                                    model.Keys[0] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_1:
                                {
                                    model.Keys[1] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_2:
                                {
                                    model.Keys[2] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_3:
                                {
                                    model.Keys[3] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_q:
                                {
                                    model.Keys[4] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_w:
                                {
                                    model.Keys[5] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_e:
                                {
                                    model.Keys[6] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_a:
                                {
                                    model.Keys[7] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_s:
                                {
                                    model.Keys[8] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_d:
                                {
                                    model.Keys[9] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_z:
                                {
                                    model.Keys[0xA] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_c:
                                {
                                    model.Keys[0xB] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_4:
                                {
                                    model.Keys[0xC] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_r:
                                {
                                    model.Keys[0xD] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_f:
                                {
                                    model.Keys[0xE] = 0;
                                }
                                break;

                            case SDL.SDL_Keycode.SDLK_v:
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

    public static void Main(string[] args)
    {
        string filePath = args.Length > 0 && File.Exists(args.First())
            ? args[0]
            : throw new ArgumentException("Invalid file.");
        Emulator emulator = new Emulator();
        Model model = new Model();
        emulator.LoadRom(model, filePath);


        SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
        var window = SDL.SDL_CreateWindow(
            title: Configuration.WINDOW_TITLE,
            x: Configuration.WINDOW_OFFSET_X,
            y: Configuration.WINDOW_OFFSET_Y,
            w: Configuration.WINDOW_WIDTH,
            h: Configuration.WINDOW_HEIGHT,
            flags: SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
        var renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        // initializer SDL rendering 
        SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
        // Clear winow
        SDL.SDL_RenderClear(renderer);

        const int cycleDelay = 10;
        long prevTime = DateTime.Now.Ticks;
        bool quit = false;
        while (!quit)
        {
            quit = processInput(model);
            long currentTime = DateTime.Now.Ticks;
            float deltaTime = (currentTime - prevTime) / TimeSpan.TicksPerMillisecond;

            if (deltaTime > cycleDelay)
            {
                prevTime = currentTime;
                emulator.Cycle(model);

                // Update screen
                SDL.SDL_SetRenderDrawColor(renderer, Colors.BLACK.r, Colors.BLACK.g, Colors.BLACK.b, Colors.BLACK.a);
                SDL.SDL_RenderClear(renderer);
                SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 255, 255);
                for (int row = 0; row < 32; row++)
                {
                    for (int col = 0; col < 64; col++)
                    {
                        bool shouldDraw = model.DisplayBuffer[row * 64 + col] > 0;

                        SDL.SDL_Rect rect = new SDL.SDL_Rect()
                        {
                            x = col * Configuration.PIXEL_WIDTH,
                            y = row * Configuration.PIXEL_HEIGHT,
                            w = Configuration.PIXEL_WIDTH,
                            h = Configuration.PIXEL_HEIGHT
                        };

                        // Render rect
                        if (shouldDraw)
                        {
                            SDL.SDL_SetRenderDrawColor(renderer, Colors.NEON_GREEN.r, Colors.NEON_GREEN.g, Colors.NEON_GREEN.b, Colors.NEON_GREEN.a);
                            SDL.SDL_RenderFillRect(renderer, ref rect);
                        }
                        else
                        {
                            SDL.SDL_SetRenderDrawColor(renderer, Colors.WHITE.r, Colors.WHITE.g, Colors.WHITE.b, Colors.WHITE.a);
                            SDL.SDL_RenderDrawRect(renderer, ref rect);
                        }


                    }
                }
                // Render the rect to the screen
                SDL.SDL_RenderPresent(renderer);

            }
        }

        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }
}
