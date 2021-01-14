using Raylib_cs;
using System;
using System.Runtime.InteropServices;

namespace MyChip8
{
    class Program
    {
        static void Main(string[] args)
        {
            //Init
            Console.WriteLine("Enter the filename of the rom you want to load (from roms folder): ");
            string romName = Console.ReadLine();
            Emu emu = new Emu(".\\roms\\" + romName);
            Raylib.InitWindow(640, 320, "My Chip-8 Emu - " + romName);
            Raylib.SetTargetFPS(120);

            //Screen Texture
            Color[] pixelArray = new Color[64 * 32];
            GCHandle pinnedArray = GCHandle.Alloc(pixelArray, GCHandleType.Pinned);
            IntPtr pixelPointer = pinnedArray.AddrOfPinnedObject();
            Image screenImage = new Image
            {
                data = pixelPointer,
                width = 64,
                height = 32,
                format = (int)Raylib_cs.PixelFormat.UNCOMPRESSED_R8G8B8A8
            };
            Texture2D screenTexture = Raylib.LoadTextureFromImage(screenImage);
            pinnedArray.Free();

            //Game Loop
            while (Raylib.WindowShouldClose() == false)
            {
                //Input
                emu.Keys[0] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_0);
                emu.Keys[1] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_1);
                emu.Keys[2] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_2);
                emu.Keys[3] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_3);
                emu.Keys[4] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_4);
                emu.Keys[5] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_5);
                emu.Keys[6] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_6);
                emu.Keys[7] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_7);
                emu.Keys[8] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_8);
                emu.Keys[9] = Raylib.IsKeyDown(KeyboardKey.KEY_KP_9);
                emu.Keys[10] = Raylib.IsKeyDown(KeyboardKey.KEY_A);
                emu.Keys[11] = Raylib.IsKeyDown(KeyboardKey.KEY_B);
                emu.Keys[12] = Raylib.IsKeyDown(KeyboardKey.KEY_C);
                emu.Keys[13] = Raylib.IsKeyDown(KeyboardKey.KEY_D);
                emu.Keys[14] = Raylib.IsKeyDown(KeyboardKey.KEY_E);
                emu.Keys[15] = Raylib.IsKeyDown(KeyboardKey.KEY_F);

                //Update Emulator
                emu.Tick();

                //Drawing
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                for (int i = 0; i < emu.Screen.Length; i++)
                {
                    int col = i % 64;
                    int row = i / 64;

                    if (emu.Screen[i] == 1) Raylib.DrawRectangle(col * 10, row * 10, 10, 10, Color.WHITE);
                    else Raylib.DrawRectangle(col * 10, row * 10, 10, 10, Color.BLACK);
                }

                Raylib.EndDrawing();
            }

            //End Game
            Raylib.UnloadTexture(screenTexture);
            Raylib.CloseWindow();
        }
    }
}