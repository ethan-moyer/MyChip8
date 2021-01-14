using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyChip8
{
    class Emu
    {
        private const int SCREENWIDTH = 64;
        private const int SCREENHEIGHT = 32;

        private Random rnd;

        private byte[] memory;
        private byte[] V;
        private ushort I, PC;
        private ushort[] stack;
        private byte SP;
        private byte delay;
        private byte sound;
        private bool[] keys;
        private int[] screen;

        public bool[] Keys { get => keys; set { keys = value; } }
        public int[] Screen { get => screen; }

        public Emu(string romPath)
        {
            //Default Values
            rnd = new Random();

            memory = new byte[0x1000];
            for (int i = 0; i < memory.Length; i++)
            {
                memory[i] = 0x0;
            }

            V = new byte[0x10];
            for (int i = 0; i < V.Length; i++)
            {
                V[i] = 0x0;
            }

            I = 0x200;
            PC = 0x200;

            stack = new ushort[16];
            for (int i = 0; i < stack.Length; i++)
            {
                stack[i] = 0;
            }

            SP = 0x0;

            delay = 0x0;
            sound = 0x0;

            keys = new bool[16];

            screen = new int[SCREENWIDTH * SCREENHEIGHT];

            //Load Font & Rom
            byte[] font =
            {
                0xF0, 0x90, 0x90, 0x90, 0xF0,
		        0x20, 0x60, 0x20, 0x20, 0x70,
		        0xF0, 0x10, 0xF0, 0x80, 0xF0,
		        0xF0, 0x10, 0xF0, 0x10, 0xF0,
		        0x90, 0x90, 0xF0, 0x10, 0x10,
		        0xF0, 0x80, 0xF0, 0x10, 0xF0,
		        0xF0, 0x80, 0xF0, 0x90, 0xF0,
		        0xF0, 0x10, 0x20, 0x40, 0x40,
		        0xF0, 0x90, 0xF0, 0x90, 0xF0,
		        0xF0, 0x90, 0xF0, 0x10, 0xF0,
		        0xF0, 0x90, 0xF0, 0x90, 0x90,
		        0xE0, 0x90, 0xE0, 0x90, 0xE0,
		        0xF0, 0x80, 0x80, 0x80, 0xF0,
		        0xE0, 0x90, 0x90, 0x90, 0xE0,
		        0xF0, 0x80, 0xF0, 0x80, 0xF0,
		        0xF0, 0x80, 0xF0, 0x80, 0x80
            };
            Array.Copy(font, 0, memory, 0x50, font.Length);

            byte[] romBytes = File.ReadAllBytes(romPath);
            Array.Copy(romBytes, 0, memory, 0x200, romBytes.Length);

            Console.WriteLine(memory[0x200]);
        }

        public void Tick()
        {
            //Get Opcode & Variables
            ushort opcode = (ushort)((memory[PC] << 8) | memory[PC + 1]);
            ushort x = (ushort)((opcode & 0x0F00) >> 8);
            ushort y = (ushort)((opcode & 0x00F0) >> 4);
            ushort nnn = (ushort)(opcode & 0x0FFF);
            ushort nn = (ushort)(opcode & 0x00FF);
            ushort n = (ushort)(opcode & 0x000F);

            //Update
            PC += 2;

            switch (opcode & 0xF000)
            {
                case 0x0000:
                    switch (opcode & 0x000F)
                    {
                        case 0x0000:
                            screen = new int[SCREENWIDTH * SCREENHEIGHT];
                            break;
                        case 0x000E:
                            SP -= 1;
                            PC = stack[SP];
                            break;
                    }
                    break;
                case 0x1000:
                    PC = nnn;
                    break;
                case 0x2000:
                    stack[SP] = PC;
                    SP += 1;
                    PC = nnn;
                    break;
                case 0x3000:
                    if (V[x] == nn)
                    {
                        PC += 2;
                    }
                    break;
                case 0x4000:
                    if (V[x] != nn)
                    {
                        PC += 2;
                    }
                    break;
                case 0x5000:
                    if (V[x] == V[y])
                    {
                        PC += 2;
                    }
                    break;
                case 0x6000:
                    V[x] = (byte)nn;
                    break;
                case 0x7000:
                    V[x] += (byte)nn;
                    break;
                case 0x8000:
                    switch (opcode & 0x000F)
                    {
                        case 0x0000:
                            V[x] = V[y];
                            break;
                        case 0x0001:
                            V[x] |= V[y];
                            break;
                        case 0x0002:
                            V[x] &= V[y];
                            break;
                        case 0x0003:
                            V[x] ^= V[y];
                            break;
                        case 0x0004:
                            int sum = V[x] + V[y];
                            if (sum > 255)
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }
                            V[x] = (byte)(sum & 0xFF);
                            break;
                        case 0x0005:
                            if (V[x] > V[y])
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }
                            V[x] -= V[y];
                            break;
                        case 0x0006:
                            V[0xF] = (byte)(V[x] & 0x1);
                            V[x] >>= 1;
                            break;
                        case 0x0007:
                            if (V[y] > V[x])
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }
                            V[x] = (byte)(V[y] - V[x]);
                            break;
                        case 0x000E:
                            V[0xF] = (byte)((V[x] & 0x80) >> 7);
                            V[x] <<= 1;
                            break;
                    }
                    break;
                case 0x9000:
                    if (V[x] != V[y])
                    {
                        PC += 2;
                    }
                    break;
                case 0xA000:
                    I = nnn;
                    break;
                case 0xB000:
                    PC = (ushort)(V[0] + nnn);
                    break;
                case 0xC000:
                    byte[] randBytes = new byte[1];
                    rnd.NextBytes(randBytes);
                    V[x] = (byte)(randBytes[0] & nn);
                    break;
                case 0xD000:
                    V[0xF] = 0;
                    for (int row = 0; row < n; row++)
                    {
                        byte line = memory[I + row];
                        for (int col = 0; col < 8; col++)
                        {
                            byte pixel = (byte)(line & (0x80 >> col));
                            if (pixel != 0x0)
                            {
                                byte totalX = (byte)(V[x] + col);
                                byte totalY = (byte)(V[y] + row);
                                totalX %= SCREENWIDTH;
                                totalY %= SCREENHEIGHT;
                                int index = (totalY * SCREENWIDTH) + totalX;
                                if (screen[index] == 1)
                                {
                                    V[0xF] = 1;
                                }
                                screen[index] ^= 1;
                            }
                        }
                    }
                    break;
                case 0xE000:
                    switch (opcode & 0x000F)
                    {
                        case 0x000E:
                            if (keys[V[x]] == true)
                            {
                                PC += 2;
                            }
                            break;
                        case 0x0001:
                            if (keys[V[x]] == false)
                            {
                                PC += 2;
                            }
                            break;
                    }
                    break;
                case 0xF000:
                    switch (opcode & 0x00FF)
                    {
                        case 0x0007:
                            V[x] = delay;
                            break;
                        case 0x000A:
                            for (int i = 0; i < keys.Length; i++)
                            {
                                if (keys[i] == true)
                                {
                                    V[x] = (byte)i;
                                    break;
                                }
                            }
                            PC -= 2;
                            break;
                        case 0x0015:
                            delay = V[x];
                            break;
                        case 0x0018:
                            sound = V[x];
                            break;
                        case 0x001E:
                            I += V[x];
                            break;
                        case 0x0029:
                            I = (ushort)(0x50 + (5 * V[x]));
                            break;
                        case 0x0033:
                            byte value = V[x];
                            
                            memory[I + 2] = (byte)(value % 10);
                            value /= 10;

                            memory[I + 1] = (byte)(value % 10);
                            value /= 10;

                            memory[I] = (byte)(value % 10);
                            break;
                        case 0x0055:
                            for (int i = 0; i <= x; i++)
                            {
                                memory[I + i] = V[i];
                            }
                            break;
                        case 0x0065:
                            for (int i = 0; i <= x; i++)
                            {
                                V[i] = memory[I + i];
                            }
                            break;
                    }
                    break;
            }

            if (delay > 0)
            {
                delay -= 1;
            }

            if (sound > 0)
            {
                sound -= 1;
            }
        }
    }
}
