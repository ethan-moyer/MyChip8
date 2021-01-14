using Raylib_cs;
using System;
using System.Runtime.InteropServices;

namespace MyChip8
{
    class EmuFront
    {
        private Emu emu;
        private Color[] screenArray;
        private IntPtr pixelPointer;
        private Texture2D screenTex;

        public EmuFront(string romPath)
        {
            emu = new Emu(romPath);
            screenArray = new Color[640 * 320];
            Raylib.InitWindow(640, 320, "My Chip 8 - " + romPath);
            Raylib.SetTargetFPS(90);
        }

        public void Update()
        {
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

                //Game Logic
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

            Raylib.CloseWindow();
        }
    }
}
