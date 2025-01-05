using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("UtilTests")]

namespace Tile_Slayer
{
    internal static class Util
    {

        // Sets a new bit in a bitboard
        public static ulong SetBitboardCell(ulong bitBoard, int x, int y, bool value)
        {
            if (x < 0 || y < 0)
            {
                throw new ArgumentOutOfRangeException("The x or y was too small");
            }
            if (x > 7 || y > 7)
            {
                throw new ArgumentOutOfRangeException("The x or y was too large");
            }

            int bitPosition = y * 8 + x;  // The math stays the same, just parameter names change

            if (value == true)
            {
                return bitBoard |= (1UL << bitPosition);
            }
            else
            {
                return bitBoard &= ~(1UL << bitPosition);
            }
        }

        /// <summary>
        /// Gets the value of the bool at position x, y
        /// </summary>
        /// <param name="bitBoard">The bitboard to query.</param>
        /// <param name="x">The 0-indexed column value.</param>
        /// <param name="y">The 0-indexed row value.</param>
        /// <returns></returns>
        public static bool GetBitboardCell(ulong bitBoard, int x, int y)
        {

            int bitPosition = x * 8 + y;
            return (bitBoard & (1UL << bitPosition)) != 0;
        }

        /// <summary>
        /// Gets the value of the bool at index.
        /// Index starts in the top left corner and goes left to right, top to bottom ending in the bottom right corner.
        /// </summary>
        /// <param name="bitboard">The bitboard to query.</param>
        /// <param name="index">The 0-index index</param>
        /// <returns></returns>
        public static bool GetBitboardCell(ulong bitboard, int index)
        {
            return (bitboard & (1UL << index)) != 0;
        }

        public static void PrintBitboard(ulong bitBoard, int size = 8, bool invert = false)
        {
            StringBuilder sb = new StringBuilder();

            // Prints the puzzle ID so we always know which puzzle we are displaying
            sb.Append(bitBoard + "\n");

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    if (invert == true)
                    {
                        if (GetBitboardCell(bitBoard, row, col) == true)
                        {
                            sb.Append("- ");
                        }
                        else
                        {
                            sb.Append("1 ");
                        }
                    }
                    else
                    {
                        if (GetBitboardCell(bitBoard, row, col) == true)
                        {
                            sb.Append("1 ");
                        }
                        else
                        {
                            sb.Append("- ");
                        }
                    }
                }
                sb.Append('\n');
            }
            Console.WriteLine(sb.ToString());
        }

        // Rotates an 8x8 bitBoard 90 degrees counter clockwise
        public static ulong Rotate90CCFast_8x8(ulong bitBoard)
        {
            bitBoard =
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0000000100000001000000010000000100000001000000010000000100000001) << 56) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0000001000000010000000100000001000000010000000100000001000000010) << 48) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0000010000000100000001000000010000000100000001000000010000000100) << 40) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0000100000001000000010000000100000001000000010000000100000001000) << 32) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0001000000010000000100000001000000010000000100000001000000010000) << 24) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0010000000100000001000000010000000100000001000000010000000100000) << 16) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0100000001000000010000000100000001000000010000000100000001000000) << 8) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b1000000010000000100000001000000010000000100000001000000010000000));

            return bitBoard;
        }

        public static ulong Rotate90CCFast_7x7(ulong bitBoard)
        {
            bitBoard =
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0000000100000001000000010000000100000001000000010000000100000001) << 42) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0000001000000010000000100000001000000010000000100000001000000010) << 35) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0000010000000100000001000000010000000100000001000000010000000100) << 28) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0000100000001000000010000000100000001000000010000000100000001000) << 21) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0001000000010000000100000001000000010000000100000001000000010000) << 14) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0010000000100000001000000010000000100000001000000010000000100000) << 7) |
                 (Bmi2.X64.ParallelBitExtract(bitBoard, 0b0100000001000000010000000100000001000000010000000100000001000000));

            return bitBoard;
        }




        public static ulong FlipHorizontally(ulong bitBoard)
        {
            bitBoard = (bitBoard << 56) |
            ((bitBoard << 40) & 0b0000000011111111000000000000000000000000000000000000000000000000) |
            ((bitBoard << 24) & 0b0000000000000000111111110000000000000000000000000000000000000000) |
            ((bitBoard << 8) & 0b0000000000000000000000001111111100000000000000000000000000000000) |
            ((bitBoard >> 8) & 0b0000000000000000000000000000000011111111000000000000000000000000) |
            ((bitBoard >> 24) & 0b0000000000000000000000000000000000000000111111110000000000000000) |
            ((bitBoard >> 40) & 0b0000000000000000000000000000000000000000000000001111111100000000) |
            ((bitBoard >> 56));

            return bitBoard;
        }




        // ################
        // # Rows for 1x1 #
        // ################
        private readonly static ulong row_0_1x1 = 0x1;


        // ###################
        // # Columns for 1x1 #
        // ###################
        private readonly static ulong col_0_1x1 = 0x1;

        public static readonly ulong[] rows_1x1 = [row_0_1x1];
        public static readonly ulong[] rowsBelow_1x1 = [];

        public static readonly ulong[] cols_1x1 = [col_0_1x1];
        public static readonly ulong[] colsRight_1x1 = [];




        // ################
        // # Rows for 2x2 #
        // ################
        private readonly static ulong row_0_2x2 = 0x3;
        private readonly static ulong row_0_below_2x2 = 0x300;

        private readonly static ulong row_1_2x2 = 0x300;


        // ###################
        // # Columns for 2x2 #
        // ###################
        private readonly static ulong col_0_2x2 = 0x101;
        private readonly static ulong col_0_right_2x2 = 0x202;

        private readonly static ulong col_1_2x2 = 0x202;

        public static readonly ulong[] rows_2x2 = [row_0_2x2, row_1_2x2];
        public static readonly ulong[] rowsBelow_2x2 = [row_0_below_2x2];

        public static readonly ulong[] cols_2x2 = [col_0_2x2, col_1_2x2];
        public static readonly ulong[] colsRight_2x2 = [col_0_right_2x2];




        // ################
        // # Rows for 3x3 #
        // ################
        private readonly static ulong row_0_3x3 = 0x7;
        private readonly static ulong row_0_below_3x3 = 0x70700;

        private readonly static ulong row_1_3x3 = 0x700;
        private readonly static ulong row_1_below_3x3 = 0x70000;

        private readonly static ulong row_2_3x3 = 0x70000;



        // ###################
        // # Columns for 3x3 #
        // ###################
        private readonly static ulong col_0_3x3 = 0x10101;
        private readonly static ulong col_0_right_3x3 = 0x60606;

        private readonly static ulong col_1_3x3 = 0x20202;
        private readonly static ulong col_1_right_3x3 = 0x40404;

        private readonly static ulong col_2_3x3 = 0x40404;


        public static readonly ulong[] rows_3x3 = [row_0_3x3, row_1_3x3, row_2_3x3];
        public static readonly ulong[] rowsBelow_3x3 = [row_0_below_3x3, row_1_below_3x3];

        public static readonly ulong[] cols_3x3 = [col_0_3x3, col_1_3x3, col_2_3x3];
        public static readonly ulong[] colsRight_3x3 = [col_0_right_3x3, col_1_right_3x3];




        // ################
        // # Rows for 4x4 #
        // ################
        private readonly static ulong row_0_4x4 = 0xf;
        private readonly static ulong row_0_below_4x4 = 0xf0f0f00;

        private readonly static ulong row_1_4x4 = 0xf00;
        private readonly static ulong row_1_below_4x4 = 0xf0f0000;

        private readonly static ulong row_2_4x4 = 0xf0000;
        private readonly static ulong row_2_below_4x4 = 0xf000000;

        private readonly static ulong row_3_4x4 = 0xf000000;


        // ###################
        // # Columns for 4x4 #
        // ###################
        private readonly static ulong col_0_4x4 = 0x1010101;
        private readonly static ulong col_0_right_4x4 = 0xe0e0e0e;

        private readonly static ulong col_1_4x4 = 0x2020202;
        private readonly static ulong col_1_right_4x4 = 0xc0c0c0c;

        private readonly static ulong col_2_4x4 = 0x4040404;
        private readonly static ulong col_2_right_4x4 = 0x8080808;

        private readonly static ulong col_3_4x4 = 0x8080808;

        public static readonly ulong[] rows_4x4 = [row_0_4x4, row_1_4x4, row_2_4x4, row_3_4x4];
        public static readonly ulong[] rowsBelow_4x4 = [row_0_below_4x4, row_1_below_4x4, row_2_below_4x4];

        public static readonly ulong[] cols_4x4 = [col_0_4x4, col_1_4x4, col_2_4x4, col_3_4x4];
        public static readonly ulong[] colsRight_4x4 = [col_0_right_4x4, col_1_right_4x4, col_2_right_4x4];




        // ################
        // # Rows for 5x5 #
        // ################
        private readonly static ulong row_0_5x5 = 0x1f;
        private readonly static ulong row_0_below_5x5 = 0x1f1f1f1f00;

        private readonly static ulong row_1_5x5 = 0x1f00;
        private readonly static ulong row_1_below_5x5 = 0x1f1f1f0000;

        private readonly static ulong row_2_5x5 = 0x1f0000;
        private readonly static ulong row_2_below_5x5 = 0x1f1f000000;

        private readonly static ulong row_3_5x5 = 0x1f000000;
        private readonly static ulong row_3_below_5x5 = 0x1f00000000;

        private readonly static ulong row_4_5x5 = 0x1f00000000;


        // ###################
        // # Columns for 5x5 #
        // ###################
        private readonly static ulong col_0_5x5 = 0x101010101;
        private readonly static ulong col_0_right_5x5 = 0x1e1e1e1e1e;

        private readonly static ulong col_1_5x5 = 0x202020202;
        private readonly static ulong col_1_right_5x5 = 0x1c1c1c1c1c;

        private readonly static ulong col_2_5x5 = 0x404040404;
        private readonly static ulong col_2_right_5x5 = 0x1818181818;

        private readonly static ulong col_3_5x5 = 0x808080808;
        private readonly static ulong col_3_right_5x5 = 0x1010101010;

        private readonly static ulong col_4_5x5 = 0x1010101010;

        public static readonly ulong[] rows_5x5 = [row_0_5x5, row_1_5x5, row_2_5x5, row_3_5x5, row_4_5x5];
        public static readonly ulong[] rowsBelow_5x5 = [row_0_below_5x5, row_1_below_5x5, row_2_below_5x5, row_3_below_5x5];

        public static readonly ulong[] cols_5x5 = [col_0_5x5, col_1_5x5, col_2_5x5, col_3_5x5, col_4_5x5];
        public static readonly ulong[] colsRight_5x5 = [col_0_right_5x5, col_1_right_5x5, col_2_right_5x5, col_3_right_5x5];




        // ################
        // # Rows for 6x6 #
        // ################
        private readonly static ulong row_0_6x6 = 0x3f;
        private readonly static ulong row_0_below_6x6 = 0x3f3f3f3f3f00;

        private readonly static ulong row_1_6x6 = 0x3f00;
        private readonly static ulong row_1_below_6x6 = 0x3f3f3f3f0000;

        private readonly static ulong row_2_6x6 = 0x3f0000;
        private readonly static ulong row_2_below_6x6 = 0x3f3f3f000000;

        private readonly static ulong row_3_6x6 = 0x3f000000;
        private readonly static ulong row_3_below_6x6 = 0x3f3f00000000;

        private readonly static ulong row_4_6x6 = 0x3f00000000;
        private readonly static ulong row_4_below_6x6 = 0x3f0000000000;

        private readonly static ulong row_5_6x6 = 0x3f0000000000;



        // ###################
        // # Columns for 6x6 #
        // ###################
        private readonly static ulong col_0_6x6 = 0x10101010101;
        private readonly static ulong col_0_right_6x6 = 0x3e3e3e3e3e3e;

        private readonly static ulong col_1_6x6 = 0x20202020202;
        private readonly static ulong col_1_right_6x6 = 0x3c3c3c3c3c3c;

        private readonly static ulong col_2_6x6 = 0x40404040404;
        private readonly static ulong col_2_right_6x6 = 0x383838383838;

        private readonly static ulong col_3_6x6 = 0x80808080808;
        private readonly static ulong col_3_right_6x6 = 0x303030303030;

        private readonly static ulong col_4_6x6 = 0x101010101010;
        private readonly static ulong col_4_right_6x6 = 0x202020202020;

        private readonly static ulong col_5_6x6 = 0x202020202020;

        public static readonly ulong[] rows_6x6 = [row_0_6x6, row_1_6x6, row_2_6x6, row_3_6x6, row_4_6x6, row_5_6x6];
        public static readonly ulong[] rowsBelow_6x6 = [row_0_below_6x6, row_1_below_6x6, row_2_below_6x6, row_3_below_6x6, row_4_below_6x6];

        public static readonly ulong[] cols_6x6 = [col_0_6x6, col_1_6x6, col_2_6x6, col_3_6x6, col_4_6x6, col_5_6x6];
        public static readonly ulong[] colsRight_6x6 = [col_0_right_6x6, col_1_right_6x6, col_2_right_6x6, col_3_right_6x6, col_4_right_6x6];





        // ################
        // # Rows for 7x7 #
        // ################
        private readonly static ulong[] rows_7x7_ = [
            0x7f,
            0x7f00,
            0x7f0000,
            0x7f000000,
            0x7f00000000,
            0x7f0000000000,
            ];

        private readonly static ulong row_0_7x7 = 0x7f;
        private readonly static ulong row_0_below_7x7 = 0x7f7f7f7f7f7f00;

        private readonly static ulong row_1_7x7 = 0x7f00;
        private readonly static ulong row_1_below_7x7 = 0x7f7f7f7f7f0000;

        private readonly static ulong row_2_7x7 = 0x7f0000;
        private readonly static ulong row_2_below_7x7 = 0x7f7f7f7f000000;

        private readonly static ulong row_3_7x7 = 0x7f000000;
        private readonly static ulong row_3_below_7x7 = 0x7f7f7f00000000;

        private readonly static ulong row_4_7x7 = 0x7f00000000;
        private readonly static ulong row_4_below_7x7 = 0x7f7f0000000000;

        private readonly static ulong row_5_7x7 = 0x7f0000000000;
        private readonly static ulong row_5_below_7x7 = 0x7f000000000000;

        private readonly static ulong row_6_7x7 = 0x7f000000000000;


        // ###################
        // # Columns for 7x7 #
        // ###################
        private readonly static ulong col_0_7x7 = 0x1010101010101;
        private readonly static ulong col_0_right_7x7 = 0x7e7e7e7e7e7e7e;

        private readonly static ulong col_1_7x7 = 0x2020202020202;
        private readonly static ulong col_1_right_7x7 = 0x7c7c7c7c7c7c7c;

        private readonly static ulong col_2_7x7 = 0x4040404040404;
        private readonly static ulong col_2_right_7x7 = 0x78787878787878;

        private readonly static ulong col_3_7x7 = 0x8080808080808;
        private readonly static ulong col_3_right_7x7 = 0x70707070707070;

        private readonly static ulong col_4_7x7 = 0x10101010101010;
        private readonly static ulong col_4_right_7x7 = 0x60606060606060;

        private readonly static ulong col_5_7x7 = 0x20202020202020;
        private readonly static ulong col_5_right_7x7 = 0x40404040404040;

        private readonly static ulong col_6_7x7 = 0x40404040404040;

        public static readonly ulong[] rows_7x7 = [row_0_7x7, row_1_7x7, row_2_7x7, row_3_7x7, row_4_7x7, row_5_7x7, row_6_7x7];
        public static readonly ulong[] rowsBelow_7x7 = [row_0_below_7x7, row_1_below_7x7, row_2_below_7x7, row_3_below_7x7, row_4_below_7x7, row_5_below_7x7];

        public static readonly ulong[] cols_7x7 = [col_0_7x7, col_1_7x7, col_2_7x7, col_3_7x7, col_4_7x7, col_5_7x7, col_6_7x7];
        public static readonly ulong[] colsRight_7x7 = [col_0_right_7x7, col_1_right_7x7, col_2_right_7x7, col_3_right_7x7, col_4_right_7x7, col_5_right_7x7];




        // ################
        // # Rows for 8x8 #
        // ################
        private readonly static ulong row_0_8x8 = 0xff;
        private readonly static ulong row_0_below_8x8 = 0xffffffffffffff00;

        private readonly static ulong row_1_8x8 = 0xff00;
        private readonly static ulong row_1_below_8x8 = 0xffffffffffff0000;

        private readonly static ulong row_2_8x8 = 0xff0000;
        private readonly static ulong row_2_below_8x8 = 0xffffffffff000000;

        private readonly static ulong row_3_8x8 = 0xff000000;
        private readonly static ulong row_3_below_8x8 = 0xffffffff00000000;

        private readonly static ulong row_4_8x8 = 0xff00000000;
        private readonly static ulong row_4_below_8x8 = 0xffffff0000000000;

        private readonly static ulong row_5_8x8 = 0xff0000000000;
        private readonly static ulong row_5_below_8x8 = 0xffff000000000000;

        private readonly static ulong row_6_8x8 = 0xff000000000000;
        private readonly static ulong row_6_below_8x8 = 0xff00000000000000;

        private readonly static ulong row_7_8x8 = 0xff00000000000000;

        // ###################
        // # Columns for 8x8 #
        // ###################
        private readonly static ulong col_0_8x8 = 0x101010101010101;
        private readonly static ulong col_0_right_8x8 = 0xfefefefefefefefe;

        private readonly static ulong col_1_8x8 = 0x202020202020202;
        private readonly static ulong col_1_right_8x8 = 0xfcfcfcfcfcfcfcfc;

        private readonly static ulong col_2_8x8 = 0x404040404040404;
        private readonly static ulong col_2_right_8x8 = 0xf8f8f8f8f8f8f8f8;

        private readonly static ulong col_3_8x8 = 0x808080808080808;
        private readonly static ulong col_3_right_8x8 = 0xf0f0f0f0f0f0f0f0;

        private readonly static ulong col_4_8x8 = 0x1010101010101010;
        private readonly static ulong col_4_right_8x8 = 0xe0e0e0e0e0e0e0e0;

        private readonly static ulong col_5_8x8 = 0x2020202020202020;
        private readonly static ulong col_5_right_8x8 = 0xc0c0c0c0c0c0c0c0;

        private readonly static ulong col_6_8x8 = 0x4040404040404040;
        private readonly static ulong col_6_right_8x8 = 0x8080808080808080;

        private readonly static ulong col_7_8x8 = 0x8080808080808080;

        public static readonly ulong[] rows_8x8 = [row_0_8x8, row_1_8x8, row_2_8x8, row_3_8x8, row_4_8x8, row_5_8x8, row_6_8x8, row_7_8x8];
        public static readonly ulong[] rowsBelow_8x8 = [row_0_below_8x8, row_1_below_8x8, row_2_below_8x8, row_3_below_8x8, row_4_below_8x8, row_5_below_8x8, row_6_below_8x8];

        public static readonly ulong[] cols_8x8 = [col_0_8x8, col_1_8x8, col_2_8x8, col_3_8x8, col_4_8x8, col_5_8x8, col_6_8x8, col_7_8x8];
        public static readonly ulong[] colsRight_8x8 = [col_0_right_8x8, col_1_right_8x8, col_2_right_8x8, col_3_right_8x8, col_4_right_8x8, col_5_right_8x8, col_6_right_8x8];





        // Deletes empty rows and columns and shifts the remaining up and left
        // Note: This removes empty rows and columns BETWEEN the board too
        public static ulong ShiftAndRemoveEmpty(ulong bitBoard, int size)
        {
            ulong[] rows = new ulong[size];
            ulong[] rowsBelow = new ulong[size - 1];
            ulong[] cols = new ulong[size];
            ulong[] colsRight = new ulong[size - 1];

            switch (size)
            {
                case 1:
                    //rows = rows_1x1;
                    //cols = cols_1x1;
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    rows = rows_6x6;
                    rowsBelow = rowsBelow_6x6;
                    cols = cols_6x6;
                    colsRight = colsRight_6x6;
                    break;
                case 7:
                    rows = rows_7x7;
                    rowsBelow = rowsBelow_7x7;
                    cols = cols_7x7;
                    colsRight = colsRight_7x7;
                    break;
                case 8:
                    rows = rows_8x8;
                    rowsBelow = rowsBelow_8x8;
                    cols = cols_8x8;
                    colsRight = colsRight_8x8;
                    break;

                default:
                    break;
            }

            for (int i = 0; i < rows.Length; i++)
            {
                while ((bitBoard & rows[i]) == 0 && (bitBoard & rowsBelow[i]) != 0)
                {
                    bitBoard = (bitBoard & rowsBelow[i]) >> 8 | (bitBoard & ~rowsBelow[i]);
                }
            }

            for (int i = 0; i < cols.Length - 1; i++)
            {
                while ((bitBoard & cols[i]) == 0 && (bitBoard & colsRight[i]) != 0)
                {
                    bitBoard = (bitBoard & colsRight[i]) >> 1 | (bitBoard & ~colsRight[i]);
                }
            }

            return bitBoard;
        }


        // Deletes empty rows and columns and shifts the remaining up and to the left
        // Note: This only shifts the bitboard to the top left corner.
        // Note: It doesn't remove interior rows/cols like ShiftAndRemoveEmpty does
        public static ulong ShiftUpAndLeft(ulong bitBoard)
        {
            while ((bitBoard & 0x1010101010101ff) == 0)
            {
                bitBoard = (bitBoard & 0xffffffffffffff00) >> 9;
            }
            while ((bitBoard & 0xff) == 0)
            {
                bitBoard = (bitBoard & 0xffffffffffffff00) >> 8;
            }

            while ((bitBoard & 0x101010101010101) == 0)
            {
                bitBoard = (bitBoard & 0xfefefefefefefefe) >> 1;
            }

            return bitBoard;
        }

        /// <summary>
        /// Takes in a bitBoard and checks it against all 8 symmetries (4 rotations and their mirrors)<br></br>
        /// Outputs the smallest one
        /// </summary>
        /// <param name="bitBoard">The bitBoard to canonicalize</param>
        /// <param name="verboseLogging">If true, output inforamtion about the process to the console for debugging purposes</param>
        /// <returns>Returns the min hash from the puzzle</returns>
        public static ulong CanonicalizeBitboard(ulong bitBoard)
        {
            ulong minValue = ShiftUpAndLeft(bitBoard);
            ulong current;

            // Rotation 90
            bitBoard = Rotate90CCFast_8x8(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            // Rotation 180
            bitBoard = Rotate90CCFast_8x8(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            // Rotation 270
            bitBoard = Rotate90CCFast_8x8(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            // Flip and do all rotations again
            bitBoard = FlipHorizontally(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            bitBoard = Rotate90CCFast_8x8(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            bitBoard = Rotate90CCFast_8x8(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            bitBoard = Rotate90CCFast_8x8(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            return minValue;
        }


        public static ulong CanonicalizeBitboard2(ulong bitBoard)
        {
            ulong minValue = ShiftUpAndLeft(bitBoard);
            ulong current;

            // Rotation 90
            bitBoard = Rotate90CCFast_8x8(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            // x = minValue
            // y = current
            minValue = Math.Min(minValue, current);

            // Rotation 180
            bitBoard = Rotate90CCFast_8x8(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            // Rotation 270
            bitBoard = Rotate90CCFast_8x8(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            // Flip and do all rotations again
            bitBoard = FlipHorizontally(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            bitBoard = Rotate90CCFast_8x8(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            bitBoard = Rotate90CCFast_8x8(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            bitBoard = Rotate90CCFast_8x8(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            return minValue;
        }

        public static void TimeAction(Action action, ulong iterations)
        {
            Stopwatch sw = Stopwatch.StartNew();
            ulong i = 0UL;
            while (i < iterations)
            {
                action.Invoke();
                i++;
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        public static ulong ConvertPatternToBitboard(string pattern)
        {
            var bitboard = 0UL;
            var position = (x: 0, y: 0);

            // Set initial position
            bitboard = SetBitboardCell(bitboard, position.x, position.y, true);

            for (int i = 0; i < pattern.Length; i++)
            {
                position = GetNextPosition(position, pattern[i]);

                // Set bit if it's the last character or different from next character
                if (i == pattern.Length - 1 || pattern[i] != pattern[i + 1])
                {
                    bitboard = SetBitboardCell(bitboard, position.x, position.y, true);
                }
            }

            return bitboard;
        }

        private static (int x, int y) GetNextPosition((int x, int y) current, char direction)
        {
            return direction switch
            {
                'R' => (current.x + 1, current.y),
                'L' => (current.x - 1, current.y),
                'U' => (current.x, current.y - 1),
                'D' => (current.x, current.y + 1),
                _ => current
            };
        }

        public static void SavePuzzlesToBinaryFile(List<ulong> puzzles, string filePath)
        {
            if (filePath.EndsWith(".bin") == false)
            {
                filePath += ".bin";
            }
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                byte[] sbytes = new byte[8];
                foreach (ulong puzzle in puzzles)
                {
                    sbytes = BitConverter.GetBytes(puzzle);
                    fs.Write(sbytes, 0, sbytes.Length);
                }
            }
        }


        /// <summary>
        /// If you don't include .bin, it'll add it automatically.
        /// </summary>
        /// <param name="puzzles"></param>
        /// <param name="filePath"></param>
        /// 
        public static bool CompareCanonical(HashSet<ulong> setA, HashSet<ulong> setB)
        {
            foreach (ulong a in setA)
            {
                if (setB.Contains(a)) return true;
            }
            throw new NotImplementedException();
        }

        public static HashSet<ulong> ReduceToCanonical(HashSet<ulong> set)
        {
            HashSet<ulong> result = new HashSet<ulong>();

            foreach (ulong a in set)
            {
                result.Add(CanonicalizeBitboard(a));
            }

            return result;
        }

        public static HashSet<ulong> ReduceToCanonical(HashSet<string> set, bool verboseLogging = false)
        {
            HashSet<ulong> result = new HashSet<ulong>();

            foreach (string a in set)
            {
                result.Add(CanonicalizeBitboard(ConvertPatternToBitboard(a)));

            }

            return result;
        }

        /// <summary>
        /// For each bit in a ulong, finds the x, y index of where the bit is on a 8x8 bitboard
        /// </summary>
        /// <param name="bitboard">The bitboard to search</param>
        /// <returns>A List containing (int x, int y) pairs for each bit in the bitboard.</returns>
        public static (HashSet<int> x, HashSet<int> y) FindXYofIndices(ulong bitboard)
        {
            // Example input
            // 2097152
            // 0 0 0 0 0 1 0 0
            // 0 0 0 0 0 1 0 0
            // 0 0 0 0 0 1 0 0
            // 0 0 0 0 0 0 0 0
            // 0 0 0 0 0 0 0 0
            // 0 0 0 0 0 0 0 0
            // 0 0 0 0 0 0 0 0
            // 0 0 0 0 0 0 0 0

            // Example output
            // x = {0, 1, 2}
            // y = {5}

            HashSet<int> x = new HashSet<int>();
            HashSet<int> y = new HashSet<int>();

            while (bitboard != 0)
            {
                // Find index of least significant 1-bit
                int index = BitOperations.TrailingZeroCount(bitboard);

                // Calculate x and y coordinates
                x.Add(index % 8);         // x is the column (0-7)
                y.Add(index / 8);         // y is the row (0-7)

                // Clear the least significant 1-bit
                bitboard &= (bitboard - 1);
            }

            return (x, y);
        }




        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="n">Size</param>
        //public static HashSet<ulong> GetStartingPositions(int n)
        //{
        //    HashSet<ulong> results = new HashSet<ulong>();

        //    for (int col = 0; col < n; col++)
        //    {
        //        for (int row = 0; row < n; row++)
        //        {
        //            results.Add(SetBitboardCell(0UL, row, col, true));
        //        }
        //    }

        //    return results;
        //}

        /// <summary>
        /// A collection is a set of all possible values from the points in startingPositions
        /// 
        /// </summary>
        /// <param name="startingPositions"></param>
        /// <returns>The collection</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static HashSet<ulong> GetCollection(ulong startingPositions)
        {
            // Need to create the masks from startingPositions

            throw new NotImplementedException();
        }



        /// <summary>
        /// This takes in a mask and returns a HashSet of all the ulongs you could make from the masked bits
        /// </summary>
        /// <param name="mask"></param>
        public static HashSet<ulong> GetPermutations(ulong mask, ulong vertex, HashSet<ulong> visited)
        {
            HashSet<ulong> permutations = new HashSet<ulong>();
            ulong temp;
            for (ulong i = 1; i < Math.Pow(2, BitOperations.PopCount(mask)); i++)
            {
                temp = Bmi2.X64.ParallelBitDeposit(i, mask) | vertex;
                if(visited.Contains(temp) == false)
                {
                    permutations.Add(temp);
                }
            }

            return ReduceToCanonical(permutations);
        }


        // These are used to mask out the GetPermutations method
        private static ulong[] boardMask = new ulong[8] { 0x1, 0x303, 0x70707, 0xf0f0f0f, 0x1f1f1f1f1f, 0x3f3f3f3f3f3f, 0x7f7f7f7f7f7f7f, 0xffffffffffffffff};

        /// <summary>
        /// Given a bitboard, returns a mask of the where the rows and columns intersect
        /// It shouldn't include the input
        /// It should factor in the puzzle size
        /// </summary>
        /// <param name="bitboard"></param>
        /// <returns></returns>
        public static ulong GetMask(ulong bitboard, int size)
        {
            // Example input and size = 7
            // 87960930238976
            // 0 0 0 0 0 0 0 0
            // 0 1 0 0 0 0 1 0
            // 0 0 0 0 0 0 0 0
            // 0 0 0 0 0 0 0 0
            // 0 0 0 0 0 0 0 0
            // 0 0 0 0 1 0 1 0
            // 0 0 0 0 0 0 0 0
            // 0 0 0 0 0 0 0 0

            // Example output
            // 0 1 0 0 1 0 1 0
            // 1 0 1 1 1 1 0 0
            // 0 1 0 0 1 0 1 0
            // 0 1 0 0 1 0 1 0
            // 0 1 0 0 1 0 1 0
            // 1 0 1 1 1 1 0 0
            // 0 1 0 0 1 0 1 0
            // 0 0 0 0 0 0 0 0

            (HashSet<int> x, HashSet<int> y) pairs = new();
            pairs = FindXYofIndices(bitboard);

            ulong mask = 0UL;
            ulong[] rows = new ulong[size];
            ulong[] cols = new ulong[size];
            switch (size)
            {
                case 1:
                    rows = rows_1x1;
                    cols = cols_1x1;
                    break;
                case 2:
                    rows = rows_2x2;
                    cols = cols_2x2;
                    break;
                case 3:
                    rows = rows_3x3;
                    cols = cols_3x3;
                    break;
                case 4:
                    rows = rows_4x4;
                    cols = cols_4x4;
                    break;
                case 5:
                    rows = rows_5x5;
                    cols = cols_5x5;
                    break;
                case 6:
                    rows = rows_6x6;
                    cols = cols_6x6;
                    break;
                case 7:
                    rows = rows_7x7;
                    cols = cols_7x7;
                    break;
                case 8:
                    rows = rows_8x8;
                    cols = cols_8x8;
                    break;
                default:
                    break;
            }
            foreach (var x in pairs.x)
            {
                mask |= cols[x];
            }

            foreach (var y in pairs.y)
            {
                mask |= rows[y];
            }

            return (mask ^ bitboard) & boardMask[size];
        }

        //public static ulong GetNext(ulong bitboard, ulong mask)
        //{
        //    Bmi2.
        //    ulong t = bitboard | (bitboard - 1);
        //    return (t + 1) | ((~(long)t & -~(long)t) - 1) >> (BSF(bitboard) + 1);
        //}
        #region Next lexigraphical bit
        //        int i = 15;
        //        int count = 10;
        //        count += i;
        //            int v = 15;
        //        int w = 15;
        //        int t = 0;
        //            while(true)
        //            {
        //                t = (v | (v - 1)) + 1;
        //                w = t | ((((t & -t) / (v & -v)) >> 1) - 1);
        //                Console.WriteLine(Convert.ToString(w, 2).PadLeft(32, '0') + "  <--");
        //                v = w;
        //                i++;
        //            }


        //}

        //public int BSF(int x)
        //{
        //    return 0;
        //}
        #endregion
    }
}
