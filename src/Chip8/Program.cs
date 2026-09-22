using Chip8.Emulation;
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

    private static readonly Dictionary<SDL.SDL_Keycode, int> KeyMap = new()
    {
        [SDL.SDL_Keycode.SDLK_x] = 0x0,
        [SDL.SDL_Keycode.SDLK_1] = 0x1,
        [SDL.SDL_Keycode.SDLK_2] = 0x2,
        [SDL.SDL_Keycode.SDLK_3] = 0x3,
        [SDL.SDL_Keycode.SDLK_q] = 0x4,
        [SDL.SDL_Keycode.SDLK_w] = 0x5,
        [SDL.SDL_Keycode.SDLK_e] = 0x6,
        [SDL.SDL_Keycode.SDLK_a] = 0x7,
        [SDL.SDL_Keycode.SDLK_s] = 0x8,
        [SDL.SDL_Keycode.SDLK_d] = 0x9,
        [SDL.SDL_Keycode.SDLK_z] = 0xA,
        [SDL.SDL_Keycode.SDLK_c] = 0xB,
        [SDL.SDL_Keycode.SDLK_4] = 0xC,
        [SDL.SDL_Keycode.SDLK_r] = 0xD,
        [SDL.SDL_Keycode.SDLK_f] = 0xE,
        [SDL.SDL_Keycode.SDLK_v] = 0xF,
    };

    private const string Usage = "Usage: Chip8 [--emulator <native|dotnet>] <rom>";

    private static bool processInput(IEmulator emulator)
    {
        bool quit = false;

        while (SDL.SDL_PollEvent(out SDL.SDL_Event evt) != 0)
        {
            switch (evt.type)
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    quit = true;
                    break;

                case SDL.SDL_EventType.SDL_KEYDOWN:
                    if (evt.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                    {
                        quit = true;
                    }
                    else if (KeyMap.TryGetValue(evt.key.keysym.sym, out int downKey))
                    {
                        emulator.SetKey(downKey, true);
                    }
                    break;

                case SDL.SDL_EventType.SDL_KEYUP:
                    if (KeyMap.TryGetValue(evt.key.keysym.sym, out int upKey))
                    {
                        emulator.SetKey(upKey, false);
                    }
                    break;
            }
        }
        return quit;
    }

    private static bool TryParseArgs(string[] args, out EmulatorKind kind, out string romPath)
    {
        kind = EmulatorKind.Native;
        romPath = string.Empty;
        string? rom = null;

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];
            string? value = null;

            if (arg == "--emulator")
            {
                if (++i >= args.Length) return false;
                value = args[i];
            }
            else if (arg.StartsWith("--emulator=", StringComparison.Ordinal))
            {
                value = arg["--emulator=".Length..];
            }
            else if (arg.StartsWith("--", StringComparison.Ordinal) || rom != null)
            {
                return false;
            }
            else
            {
                rom = arg;
                continue;
            }

            if (!Enum.TryParse(value, ignoreCase: true, out kind) || !Enum.IsDefined(kind))
            {
                return false;
            }
        }

        if (rom == null || !File.Exists(rom)) return false;
        romPath = rom;
        return true;
    }

    public static int Main(string[] args)
    {
        if (!TryParseArgs(args, out EmulatorKind kind, out string filePath))
        {
            Console.Error.WriteLine(Usage);
            return 1;
        }

        IEmulator emulator;
        try
        {
            emulator = EmulatorFactory.Create(kind);
        }
        catch (DllNotFoundException ex)
        {
            Console.Error.WriteLine($"Could not load the native chip8 library: {ex.Message}");
            Console.Error.WriteLine("Use --emulator dotnet to run with the managed emulator instead.");
            return 1;
        }

        using (emulator)
        {
            emulator.LoadRom(filePath);

            SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
            var window = SDL.SDL_CreateWindow(
                title: Configuration.WINDOW_TITLE,
                x: Configuration.WINDOW_OFFSET_X,
                y: Configuration.WINDOW_OFFSET_Y,
                w: Configuration.WINDOW_WIDTH,
                h: Configuration.WINDOW_HEIGHT,
                flags: SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            var renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            try
            {
                // initializer SDL rendering 
                SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
                // Clear winow
                SDL.SDL_RenderClear(renderer);

                const int cycleDelay = 10;
                const double timerDelay = 1000.0 / 60;
                long prevTime = DateTime.Now.Ticks;
                long prevTimerTime = prevTime;
                bool quit = false;
                while (!quit)
                {
                    quit = processInput(emulator);
                    long currentTime = DateTime.Now.Ticks;
                    float deltaTime = (currentTime - prevTime) / TimeSpan.TicksPerMillisecond;

                    if ((currentTime - prevTimerTime) / (double)TimeSpan.TicksPerMillisecond >= timerDelay)
                    {
                        prevTimerTime = currentTime;
                        emulator.TickTimers();
                    }

                    if (deltaTime > cycleDelay)
                    {
                        prevTime = currentTime;
                        emulator.Step();

                        // Update screen
                        SDL.SDL_SetRenderDrawColor(renderer, Colors.BLACK.r, Colors.BLACK.g, Colors.BLACK.b, Colors.BLACK.a);
                        SDL.SDL_RenderClear(renderer);
                        SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 255, 255);
                        ReadOnlySpan<byte> frame = emulator.FrameBuffer;
                        for (int row = 0; row < 32; row++)
                        {
                            for (int col = 0; col < 64; col++)
                            {
                                bool shouldDraw = frame[row * 64 + col] > 0;

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
            }
            finally
            {
                SDL.SDL_DestroyRenderer(renderer);
                SDL.SDL_DestroyWindow(window);
                SDL.SDL_Quit();
            }
        }

        return 0;
    }
}
