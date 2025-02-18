using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

[assembly: InternalsVisibleTo("PrimaryUnitTests")]

namespace Primary_Puzzle_Solver
{
    /// <summary>
    /// This class assumes every bitboard is 8 by 8. I want to convert it was they x and y can by anything between 0 and 7.
    /// </summary>
    internal static class Util
    {

        /// <summary>
        /// Sets a bit in a bitboard either on or off
        /// </summary>
        /// <param name="bitBoard">The original bitboard</param>
        /// <param name="col">The col to be set</param>
        /// <param name="row">The row to be set</param>
        /// <param name="value">The value to be set in (col, row)</param>
        /// <param name="width">The width of the bitboard</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ulong SetBitboardCell(ulong bitBoard, int col, int row, bool value, int width)
        {
            if (col < 0 || row < 0)
            {
                throw new ArgumentOutOfRangeException("The col or row was too small");
            }
            if (col >= width || row >= width)
            {
                throw new ArgumentOutOfRangeException("The col or row was too large");
            }

            int bitPosition = row * width + col;

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
        /// Assumes the bitboard is 8x8
        /// </summary>
        /// <param name="bitBoard"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ulong SetBitboardCell(ulong bitBoard, int col, int row, bool value)
        {
            if (col < 0 || row < 0)
            {
                throw new ArgumentOutOfRangeException("The col or row was too small");
            }
            if (col >= 8 || row >= 8)
            {
                throw new ArgumentOutOfRangeException("The col or row was too large");
            }

            int bitPosition = row * 8 + col;

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
        /// Gets the value of the bool at position (x, y)
        /// </summary>
        /// <param name="bitBoard">The bitboard to query.</param>
        /// <param name="col">The 0-indexed column value.</param>
        /// <param name="row">The 0-indexed row value.</param>
        /// <returns></returns>
        public static bool GetBitboardCell(ulong bitBoard, int col, int row, int width)
        {

            int bitPosition = row * width + col;
            return (bitBoard & (1UL << bitPosition)) != 0;
        }




        /// <summary>
        /// Gets the value of the bool at position (x, y)<br></br>
        /// Assumes the bitboard is 8x8
        /// </summary>
        /// <param name="bitBoard">The bitboard to query.</param>
        /// <param name="col">The 0-indexed column value.</param>
        /// <param name="row">The 0-indexed row value.</param>
        /// <returns></returns>
        public static bool GetBitboardCell(ulong bitBoard, int col, int row)
        {

            int bitPosition = row * 8 + col;
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




        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitboard"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="invert"></param>
        public static void PrintBitboard(ulong bitboard, int width, int height, bool invert = false)
        {
            StringBuilder sb = new StringBuilder();

            // Prints the puzzle ID so we always know which puzzle we are displaying
            sb.Append($"bitboard: {bitboard}, width: {width}, height: {height}\n");

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (invert == true)
                    {
                        if (GetBitboardCell(bitboard, col, row, width) == true)
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
                        if (GetBitboardCell(bitboard, col, row, width) == true)
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




        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitBoard"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="invert"></param>
        public static void PrintBitboard(ulong bitBoard, bool invert = false)
        {
            StringBuilder sb = new StringBuilder();

            // Prints the puzzle ID so we always know which puzzle we are displaying
            sb.Append(bitBoard + "\n");

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (invert == true)
                    {
                        if (GetBitboardCell(bitBoard, col, row, 8) == true)
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
                        if (GetBitboardCell(bitBoard, col, row, 8) == true)
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



        // Rotates an 8x8 bitboard 90 degrees counter clockwise
        public static ulong Rotate90CC(ulong bitBoard)
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



        /// <summary>
        /// This is bascially what is known a byteswap
        /// </summary>
        /// <param name="bitBoard"></param>
        /// <returns></returns>
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




        // The goal of the follows is to clean up the row_x_xxx stuff below. Good luck
        public static ulong CreateRowMask(int row, int width)
        {
            ulong rowMask = 0;
            for (int col = 0; col < width; col++)
            {
                rowMask |= 1UL << ((row * width) + col);
            }

            return rowMask;
        }

        public static ulong CreateColMask(int col, int width)
        {
            ulong colMask = 0;
            for (int row = 0; row < width; row++)
            {
                colMask |= 1UL << ((row * width) + col);
            }

            return colMask;
        }



        //// ################
        //// # Rows for 1x1 #
        //// ################
        //private readonly static ulong row_0_1x1 = 0x1;


        //// ###################
        //// # Columns for 1x1 #
        //// ###################
        //private readonly static ulong col_0_1x1 = 0x1;

        //public static readonly ulong[] rows_1x1 = [row_0_1x1];
        //public static readonly ulong[] rowsBelow_1x1 = [];

        //public static readonly ulong[] cols_1x1 = [col_0_1x1];
        //public static readonly ulong[] colsRight_1x1 = [];




        //// ################
        //// # Rows for 2x2 #
        //// ################
        //private readonly static ulong row_0_2x2 = 0x3;
        //private readonly static ulong row_0_below_2x2 = 0x300;

        //private readonly static ulong row_1_2x2 = 0x300;


        //// ###################
        //// # Columns for 2x2 #
        //// ###################
        //private readonly static ulong col_0_2x2 = 0x101;
        //private readonly static ulong col_0_right_2x2 = 0x202;

        //private readonly static ulong col_1_2x2 = 0x202;

        //public static readonly ulong[] rows_2x2 = [row_0_2x2, row_1_2x2];
        //public static readonly ulong[] rowsBelow_2x2 = [row_0_below_2x2];

        //public static readonly ulong[] cols_2x2 = [col_0_2x2, col_1_2x2];
        //public static readonly ulong[] colsRight_2x2 = [col_0_right_2x2];




        //// ################
        //// # Rows for 3x3 #
        //// ################
        //private readonly static ulong row_0_3x3 = 0x7;
        //private readonly static ulong row_0_below_3x3 = 0x70700;

        //private readonly static ulong row_1_3x3 = 0x700;
        //private readonly static ulong row_1_below_3x3 = 0x70000;

        //private readonly static ulong row_2_3x3 = 0x70000;



        //// ###################
        //// # Columns for 3x3 #
        //// ###################
        //private readonly static ulong col_0_3x3 = 0x10101;
        //private readonly static ulong col_0_right_3x3 = 0x60606;

        //private readonly static ulong col_1_3x3 = 0x20202;
        //private readonly static ulong col_1_right_3x3 = 0x40404;

        //private readonly static ulong col_2_3x3 = 0x40404;


        //public static readonly ulong[] rows_3x3 = [row_0_3x3, row_1_3x3, row_2_3x3];
        //public static readonly ulong[] rowsBelow_3x3 = [row_0_below_3x3, row_1_below_3x3];

        //public static readonly ulong[] cols_3x3 = [col_0_3x3, col_1_3x3, col_2_3x3];
        //public static readonly ulong[] colsRight_3x3 = [col_0_right_3x3, col_1_right_3x3];




        //// ################
        //// # Rows for 4x4 #
        //// ################
        //private readonly static ulong row_0_4x4 = 0xf;
        //private readonly static ulong row_0_below_4x4 = 0xf0f0f00;

        //private readonly static ulong row_1_4x4 = 0xf00;
        //private readonly static ulong row_1_below_4x4 = 0xf0f0000;

        //private readonly static ulong row_2_4x4 = 0xf0000;
        //private readonly static ulong row_2_below_4x4 = 0xf000000;

        //private readonly static ulong row_3_4x4 = 0xf000000;


        //// ###################
        //// # Columns for 4x4 #
        //// ###################
        //private readonly static ulong col_0_4x4 = 0x1010101;
        //private readonly static ulong col_0_right_4x4 = 0xe0e0e0e;

        //private readonly static ulong col_1_4x4 = 0x2020202;
        //private readonly static ulong col_1_right_4x4 = 0xc0c0c0c;

        //private readonly static ulong col_2_4x4 = 0x4040404;
        //private readonly static ulong col_2_right_4x4 = 0x8080808;

        //private readonly static ulong col_3_4x4 = 0x8080808;

        //public static readonly ulong[] rows_4x4 = [row_0_4x4, row_1_4x4, row_2_4x4, row_3_4x4];
        //public static readonly ulong[] rowsBelow_4x4 = [row_0_below_4x4, row_1_below_4x4, row_2_below_4x4];

        //public static readonly ulong[] cols_4x4 = [col_0_4x4, col_1_4x4, col_2_4x4, col_3_4x4];
        //public static readonly ulong[] colsRight_4x4 = [col_0_right_4x4, col_1_right_4x4, col_2_right_4x4];




        //// ################
        //// # Rows for 5x5 #
        //// ################
        //private readonly static ulong row_0_5x5 = 0x1f;
        //private readonly static ulong row_0_below_5x5 = 0x1f1f1f1f00;

        //private readonly static ulong row_1_5x5 = 0x1f00;
        //private readonly static ulong row_1_below_5x5 = 0x1f1f1f0000;

        //private readonly static ulong row_2_5x5 = 0x1f0000;
        //private readonly static ulong row_2_below_5x5 = 0x1f1f000000;

        //private readonly static ulong row_3_5x5 = 0x1f000000;
        //private readonly static ulong row_3_below_5x5 = 0x1f00000000;

        //private readonly static ulong row_4_5x5 = 0x1f00000000;


        //// ###################
        //// # Columns for 5x5 #
        //// ###################
        //private readonly static ulong col_0_5x5 = 0x101010101;
        //private readonly static ulong col_0_right_5x5 = 0x1e1e1e1e1e;

        //private readonly static ulong col_1_5x5 = 0x202020202;
        //private readonly static ulong col_1_right_5x5 = 0x1c1c1c1c1c;

        //private readonly static ulong col_2_5x5 = 0x404040404;
        //private readonly static ulong col_2_right_5x5 = 0x1818181818;

        //private readonly static ulong col_3_5x5 = 0x808080808;
        //private readonly static ulong col_3_right_5x5 = 0x1010101010;

        //private readonly static ulong col_4_5x5 = 0x1010101010;

        //public static readonly ulong[] rows_5x5 = [row_0_5x5, row_1_5x5, row_2_5x5, row_3_5x5, row_4_5x5];
        //public static readonly ulong[] rowsBelow_5x5 = [row_0_below_5x5, row_1_below_5x5, row_2_below_5x5, row_3_below_5x5];

        //public static readonly ulong[] cols_5x5 = [col_0_5x5, col_1_5x5, col_2_5x5, col_3_5x5, col_4_5x5];
        //public static readonly ulong[] colsRight_5x5 = [col_0_right_5x5, col_1_right_5x5, col_2_right_5x5, col_3_right_5x5];




        //// ################
        //// # Rows for 6x6 #
        //// ################
        //private readonly static ulong row_0_6x6 = 0x3f;
        //private readonly static ulong row_0_below_6x6 = 0x3f3f3f3f3f00;

        //private readonly static ulong row_1_6x6 = 0x3f00;
        //private readonly static ulong row_1_below_6x6 = 0x3f3f3f3f0000;

        //private readonly static ulong row_2_6x6 = 0x3f0000;
        //private readonly static ulong row_2_below_6x6 = 0x3f3f3f000000;

        //private readonly static ulong row_3_6x6 = 0x3f000000;
        //private readonly static ulong row_3_below_6x6 = 0x3f3f00000000;

        //private readonly static ulong row_4_6x6 = 0x3f00000000;
        //private readonly static ulong row_4_below_6x6 = 0x3f0000000000;

        //private readonly static ulong row_5_6x6 = 0x3f0000000000;



        //// ###################
        //// # Columns for 6x6 #
        //// ###################
        //private readonly static ulong col_0_6x6 = 0x10101010101;
        //private readonly static ulong col_0_right_6x6 = 0x3e3e3e3e3e3e;

        //private readonly static ulong col_1_6x6 = 0x20202020202;
        //private readonly static ulong col_1_right_6x6 = 0x3c3c3c3c3c3c;

        //private readonly static ulong col_2_6x6 = 0x40404040404;
        //private readonly static ulong col_2_right_6x6 = 0x383838383838;

        //private readonly static ulong col_3_6x6 = 0x80808080808;
        //private readonly static ulong col_3_right_6x6 = 0x303030303030;

        //private readonly static ulong col_4_6x6 = 0x101010101010;
        //private readonly static ulong col_4_right_6x6 = 0x202020202020;

        //private readonly static ulong col_5_6x6 = 0x202020202020;

        //public static readonly ulong[] rows_6x6 = [row_0_6x6, row_1_6x6, row_2_6x6, row_3_6x6, row_4_6x6, row_5_6x6];
        //public static readonly ulong[] rowsBelow_6x6 = [row_0_below_6x6, row_1_below_6x6, row_2_below_6x6, row_3_below_6x6, row_4_below_6x6];

        //public static readonly ulong[] cols_6x6 = [col_0_6x6, col_1_6x6, col_2_6x6, col_3_6x6, col_4_6x6, col_5_6x6];
        //public static readonly ulong[] colsRight_6x6 = [col_0_right_6x6, col_1_right_6x6, col_2_right_6x6, col_3_right_6x6, col_4_right_6x6];





        //// ################
        //// # Rows for 7x7 #
        //// ################
        //private readonly static ulong[] rows_7x7_ = [
        //    0x7f,
        //    0x7f00,
        //    0x7f0000,
        //    0x7f000000,
        //    0x7f00000000,
        //    0x7f0000000000,
        //    ];

        //private readonly static ulong row_0_7x7 = 0x7f;
        //private readonly static ulong row_0_below_7x7 = 0x7f7f7f7f7f7f00;

        //private readonly static ulong row_1_7x7 = 0x7f00;
        //private readonly static ulong row_1_below_7x7 = 0x7f7f7f7f7f0000;

        //private readonly static ulong row_2_7x7 = 0x7f0000;
        //private readonly static ulong row_2_below_7x7 = 0x7f7f7f7f000000;

        //private readonly static ulong row_3_7x7 = 0x7f000000;
        //private readonly static ulong row_3_below_7x7 = 0x7f7f7f00000000;

        //private readonly static ulong row_4_7x7 = 0x7f00000000;
        //private readonly static ulong row_4_below_7x7 = 0x7f7f0000000000;

        //private readonly static ulong row_5_7x7 = 0x7f0000000000;
        //private readonly static ulong row_5_below_7x7 = 0x7f000000000000;

        //private readonly static ulong row_6_7x7 = 0x7f000000000000;


        //// ###################
        //// # Columns for 7x7 #
        //// ###################
        //private readonly static ulong col_0_7x7 = 0x1010101010101;
        //private readonly static ulong col_0_right_7x7 = 0x7e7e7e7e7e7e7e;

        //private readonly static ulong col_1_7x7 = 0x2020202020202;
        //private readonly static ulong col_1_right_7x7 = 0x7c7c7c7c7c7c7c;

        //private readonly static ulong col_2_7x7 = 0x4040404040404;
        //private readonly static ulong col_2_right_7x7 = 0x78787878787878;

        //private readonly static ulong col_3_7x7 = 0x8080808080808;
        //private readonly static ulong col_3_right_7x7 = 0x70707070707070;

        //private readonly static ulong col_4_7x7 = 0x10101010101010;
        //private readonly static ulong col_4_right_7x7 = 0x60606060606060;

        //private readonly static ulong col_5_7x7 = 0x20202020202020;
        //private readonly static ulong col_5_right_7x7 = 0x40404040404040;

        //private readonly static ulong col_6_7x7 = 0x40404040404040;

        //public static readonly ulong[] rows_7x7 = [row_0_7x7, row_1_7x7, row_2_7x7, row_3_7x7, row_4_7x7, row_5_7x7, row_6_7x7];
        //public static readonly ulong[] rowsBelow_7x7 = [row_0_below_7x7, row_1_below_7x7, row_2_below_7x7, row_3_below_7x7, row_4_below_7x7, row_5_below_7x7];

        //public static readonly ulong[] cols_7x7 = [col_0_7x7, col_1_7x7, col_2_7x7, col_3_7x7, col_4_7x7, col_5_7x7, col_6_7x7];
        //public static readonly ulong[] colsRight_7x7 = [col_0_right_7x7, col_1_right_7x7, col_2_right_7x7, col_3_right_7x7, col_4_right_7x7, col_5_right_7x7];




        //// ################
        //// # Rows for 8x8 #
        //// ################
        //private readonly static ulong row_0_8x8 = 0xff;
        //private readonly static ulong row_0_below_8x8 = 0xffffffffffffff00;

        //private readonly static ulong row_1_8x8 = 0xff00;
        //private readonly static ulong row_1_below_8x8 = 0xffffffffffff0000;

        //private readonly static ulong row_2_8x8 = 0xff0000;
        //private readonly static ulong row_2_below_8x8 = 0xffffffffff000000;

        //private readonly static ulong row_3_8x8 = 0xff000000;
        //private readonly static ulong row_3_below_8x8 = 0xffffffff00000000;

        //private readonly static ulong row_4_8x8 = 0xff00000000;
        //private readonly static ulong row_4_below_8x8 = 0xffffff0000000000;

        //private readonly static ulong row_5_8x8 = 0xff0000000000;
        //private readonly static ulong row_5_below_8x8 = 0xffff000000000000;

        //private readonly static ulong row_6_8x8 = 0xff000000000000;
        //private readonly static ulong row_6_below_8x8 = 0xff00000000000000;

        //private readonly static ulong row_7_8x8 = 0xff00000000000000;

        //// ###################
        //// # Columns for 8x8 #
        //// ###################
        //private readonly static ulong col_0_8x8 = 0x101010101010101;
        //private readonly static ulong col_0_right_8x8 = 0xfefefefefefefefe;

        //private readonly static ulong col_1_8x8 = 0x202020202020202;
        //private readonly static ulong col_1_right_8x8 = 0xfcfcfcfcfcfcfcfc;

        //private readonly static ulong col_2_8x8 = 0x404040404040404;
        //private readonly static ulong col_2_right_8x8 = 0xf8f8f8f8f8f8f8f8;

        //private readonly static ulong col_3_8x8 = 0x808080808080808;
        //private readonly static ulong col_3_right_8x8 = 0xf0f0f0f0f0f0f0f0;

        //private readonly static ulong col_4_8x8 = 0x1010101010101010;
        //private readonly static ulong col_4_right_8x8 = 0xe0e0e0e0e0e0e0e0;

        //private readonly static ulong col_5_8x8 = 0x2020202020202020;
        //private readonly static ulong col_5_right_8x8 = 0xc0c0c0c0c0c0c0c0;

        //private readonly static ulong col_6_8x8 = 0x4040404040404040;
        //private readonly static ulong col_6_right_8x8 = 0x8080808080808080;

        //private readonly static ulong col_7_8x8 = 0x8080808080808080;

        //public static readonly ulong[] rows_8x8 = [row_0_8x8, row_1_8x8, row_2_8x8, row_3_8x8, row_4_8x8, row_5_8x8, row_6_8x8, row_7_8x8];
        //public static readonly ulong[] rowsBelow_8x8 = [row_0_below_8x8, row_1_below_8x8, row_2_below_8x8, row_3_below_8x8, row_4_below_8x8, row_5_below_8x8, row_6_below_8x8];

        //public static readonly ulong[] cols_8x8 = [col_0_8x8, col_1_8x8, col_2_8x8, col_3_8x8, col_4_8x8, col_5_8x8, col_6_8x8, col_7_8x8];
        //public static readonly ulong[] colsRight_8x8 = [col_0_right_8x8, col_1_right_8x8, col_2_right_8x8, col_3_right_8x8, col_4_right_8x8, col_5_right_8x8, col_6_right_8x8];





        // Deletes empty rows and columns and shifts the remaining up and left
        // Note: This removes empty rows and columns BETWEEN the board too



        /// <summary>
        /// This currently doesn't work but that's ok because we aren't using it at the moment.
        /// Loop over each row and removes any empty rows by shifting everything to below the empty row.
        /// 
        /// </summary>
        /// <param name="bitBoard"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static ulong ShiftAndRemoveEmpty(ulong bitBoard)
        {
            throw new NotImplementedException();
            // These would have to be recreated as static arrays 
            //ulong[] rows = new ulong[8];
            //ulong[] rowsBelow = new ulong[size - 1];
            //ulong[] cols = new ulong[size];
            //ulong[] colsRight = new ulong[size - 1];


            //for (int i = 0; i < rows.Length; i++)
            //{
            //    while ((bitboard & rows[i]) == 0 && (bitboard & rowsBelow[i]) != 0)
            //    {
            //        bitboard = (bitboard & rowsBelow[i]) >> 8 | (bitboard & ~rowsBelow[i]);
            //    }
            //}

            //for (int i = 0; i < cols.Length - 1; i++)
            //{
            //    while ((bitboard & cols[i]) == 0 && (bitboard & colsRight[i]) != 0)
            //    {
            //        bitboard = (bitboard & colsRight[i]) >> 1 | (bitboard & ~colsRight[i]);
            //    }
            //}

            //return bitboard;
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
        /// Takes in a bitboard and checks it against all 8 symmetries (4 rotations and their mirrors)<br></br>
        /// Outputs the smallest one
        /// </summary>
        /// <param name="bitBoard">The bitboard to canonicalize</param>
        /// <param name="verboseLogging">If true, output inforamtion about the process to the console for debugging purposes</param>
        /// <returns>Returns the min hash from the puzzle</returns>
        public static ulong CanonicalizeBitboard(ulong bitBoard)
        {
            ulong minValue = ShiftUpAndLeft(bitBoard);
            ulong current;

            // Rotation 90
            bitBoard = Rotate90CC(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            // Rotation 180
            bitBoard = Rotate90CC(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            // Rotation 270
            bitBoard = Rotate90CC(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            // Flip and do all rotations again
            bitBoard = FlipHorizontally(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            bitBoard = Rotate90CC(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            bitBoard = Rotate90CC(bitBoard);
            current = ShiftUpAndLeft(bitBoard);
            minValue = Math.Min(minValue, current);

            bitBoard = Rotate90CC(bitBoard);
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

        public static ulong ConvertPatternToBitboard(string pattern, int width)
        {
            var bitboard = 0UL;
            var position = (col: 0, row: 0);

            // Set initial position
            bitboard = SetBitboardCell(bitboard, position.col, position.row, true, width);

            for (int i = 0; i < pattern.Length; i++)
            {
                position = GetNextPosition(position, pattern[i]);

                // Set bit if it's the last character or different from next character
                if (i == pattern.Length - 1 || pattern[i] != pattern[i + 1])
                {
                    bitboard = SetBitboardCell(bitboard, position.col, position.row, true, width);
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

        public static HashSet<ulong> ReduceToCanonical(HashSet<string> set, int width, bool verboseLogging = false)
        {
            HashSet<ulong> result = new HashSet<ulong>();

            foreach (string a in set)
            {
                result.Add(CanonicalizeBitboard(ConvertPatternToBitboard(a, width)));

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
                if (visited.Contains(temp) == false)
                {
                    permutations.Add(temp);
                }
            }

            return ReduceToCanonical(permutations);
        }


        //// These are used to mask out the GetPermutations method
        ////private static ulong[] boardMask = new ulong[8] { 0x1, 0x303, 0x70707, 0xf0f0f0f, 0x1f1f1f1f1f, 0x3f3f3f3f3f3f, 0x7f7f7f7f7f7f7f, 0xffffffffffffffff };

        ///// <summary>
        ///// Given a bitboard, returns a mask of the where the rows and columns intersect
        ///// It shouldn't include the input
        ///// It should factor in the puzzle size
        ///// This was for tile slayer, no need for it here.
        ///// </summary>
        ///// <param name="bitboard"></param>
        ///// <returns></returns>
        //public static ulong GetMask(ulong bitboard, int size)
        //{
        //    // Example input and size = 7
        //    // 87960930238976
        //    // 0 0 0 0 0 0 0 0
        //    // 0 1 0 0 0 0 1 0
        //    // 0 0 0 0 0 0 0 0
        //    // 0 0 0 0 0 0 0 0
        //    // 0 0 0 0 0 0 0 0
        //    // 0 0 0 0 1 0 1 0
        //    // 0 0 0 0 0 0 0 0
        //    // 0 0 0 0 0 0 0 0

        //    // Example output
        //    // 0 1 0 0 1 0 1 0
        //    // 1 0 1 1 1 1 0 0
        //    // 0 1 0 0 1 0 1 0
        //    // 0 1 0 0 1 0 1 0
        //    // 0 1 0 0 1 0 1 0
        //    // 1 0 1 1 1 1 0 0
        //    // 0 1 0 0 1 0 1 0
        //    // 0 0 0 0 0 0 0 0

        //    (HashSet<int> x, HashSet<int> y) pairs = new();
        //    pairs = FindXYofIndices(bitboard);

        //    ulong mask = 0UL;
        //    ulong[] rows = new ulong[size];
        //    ulong[] cols = new ulong[size];
        //    switch (size)
        //    {
        //        case 1:
        //            rows = rows_1x1;
        //            cols = cols_1x1;
        //            break;
        //        case 2:
        //            rows = rows_2x2;
        //            cols = cols_2x2;
        //            break;
        //        case 3:
        //            rows = rows_3x3;
        //            cols = cols_3x3;
        //            break;
        //        case 4:
        //            rows = rows_4x4;
        //            cols = cols_4x4;
        //            break;
        //        case 5:
        //            rows = rows_5x5;
        //            cols = cols_5x5;
        //            break;
        //        case 6:
        //            rows = rows_6x6;
        //            cols = cols_6x6;
        //            break;
        //        case 7:
        //            rows = rows_7x7;
        //            cols = cols_7x7;
        //            break;
        //        case 8:
        //            rows = rows_8x8;
        //            cols = cols_8x8;
        //            break;
        //        default:
        //            break;
        //    }
        //    foreach (var x in pairs.x)
        //    {
        //        mask |= cols[x];
        //    }

        //    foreach (var y in pairs.y)
        //    {
        //        mask |= rows[y];
        //    }

        //    return (mask ^ bitboard) & boardMask[size];
        //}





        // These methods are for saving and loading files


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="processAction"></param>
        public static void ProcessBinaryFileRead(string filePath, Action<BinaryReader> processAction)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    processAction(binaryReader);
                }
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="processAction"></param>
        public static void ProcessBinaryFileWrite(string filePath, Action<BinaryWriter> processAction)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    processAction(binaryWriter);
                }
            }
        }




        /// <summary>
        /// Takes an 8x8 bitboard with a polyomino and finds the smallest width and height surrounding it
        /// </summary>
        /// <param name="bitboard"></param>
        /// <returns></returns>
        public static (int Width, int Height) GetSize(ulong bitboard)
        {
            (int Width, int Height) dimensionStart = (0, 0);
            (int Width, int Height) dimensionEnd = (0, 0);
            // Get the width start (first col with at least 1 empty cell)
            for (int i = 0; i < 8; i++)
            {
                if ((~bitboard & CreateColMask(i, 8)) != 0)
                {
                    dimensionStart.Width = i;
                    break;
                }
            }

            // Get the width end (first full col)
            for (int i = dimensionStart.Width + 1; i < 8; i++)
            {
                if ((~bitboard & CreateColMask(i, 8)) == 0)
                {
                    dimensionEnd.Width = i;
                    break;
                }
            }

            if ((~bitboard & CreateColMask(7, 8)) > 0)
            {
                dimensionEnd.Width = 8;
            }


            // Get the height (first col with at least 1 empty cell)
            for (int i = 0; i < 8; i++)
            {
                if ((~bitboard & CreateRowMask(i, 8)) != 0)
                {
                    dimensionStart.Height = i;
                    break;
                }
            }

            for (int i = dimensionStart.Height + 1; i < 8; i++)
            {
                if ((~bitboard & CreateRowMask(i, 8)) == 0)
                {
                    dimensionEnd.Height = i;
                    break;
                }
            }

            if ((~bitboard & CreateRowMask(7, 8)) > 0)
            {
                dimensionEnd.Height = 8;
            }

            return (dimensionEnd.Width - dimensionStart.Width, dimensionEnd.Height - dimensionStart.Height);
        }

        /// <summary>
        /// Converts a bitboard of size 8 by 8 to a bitboard of size n by m.<br></br>
        /// It does this by finding the dimensions dynamically using GetSize().<br></br>
        /// And then shifting the bits so they are in the right place when called with Bitboard(wallData, width, height)<br></br>
        /// This will be used mostly to do a one-time conversion of the file<br></br>
        /// </summary>
        public static ulong Convert8x8ToNxM(ulong bitboard)
        {
            int width = GetSize(bitboard).Width;
            int height = GetSize(bitboard).Height;

            bitboard = ~bitboard;
            ulong result = 0UL;

            for (int i = 0; i < height; i++)
            {
                // Get the current row's bits
                ulong rowBits = (bitboard >> (i * (8 - width))) & CreateRowMask(i, width);

                result |= rowBits;
            }
            return result;
        }




        /// <summary>
        /// Goes through each bitboard and determines it's sizeX and sizeY and then saves them all to file
        /// </summary>
        /// <param name="filePath"></param>
        public static void SeparatePuzzlesBySize(string inputFilePath, string outputFilePath, bool verboseLogging = false)
        {
            // We probably could have used another approach
            // For example, read a byte, write a byte
            // Use file I/O to check if the file exists
            // Don't even worry about the dictionary
            Dictionary<(int Width, int Height), List<ulong>> bitboards = new Dictionary<(int a, int), List<ulong>>();

            ProcessBinaryFileRead(inputFilePath, reader =>
            {
                if (reader.BaseStream.Length % 8 != 0)
                {
                    throw new ArgumentException($"File length {reader.BaseStream.Length} is not a multiple of 8 bytes");
                }
                Console.WriteLine("Reading file into dictionary");
                ulong bitboard = 0UL;
                int Width = 0;
                int Height = 0;
                ulong converted = 0UL;
                int i = 0; // For a quick test
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                //while (reader.BaseStream.Position < reader.BaseStream.Length && i < 1000)
                {
                    // For each one, get it's width and height
                    // Use the width and height as the key
                    // Use the ulong as the value
                    bitboard = reader.ReadUInt64();
                    Width = GetSize(bitboard).Width;
                    Height = GetSize(bitboard).Height;
                    converted = Convert8x8ToNxM(bitboard);
                    //converted = Convert8x8ToNxM(bitboard);

                    if (bitboards.ContainsKey((Width, Height)) == false)
                    {
                        bitboards[(Width, Height)] = new List<ulong>();
                        bitboards[(Width, Height)].Add(converted);
                    }
                    else
                    {
                        bitboards[(Width, Height)].Add(converted);
                    }
                    i++;
                }
            });

            // At this point the dictionary should be full
            // For each key (width, height) create a new file named  width_height_bitboards.bin and add the ulong to the file
            Console.WriteLine("Writing data into file");
            foreach (var bitboard in bitboards)
            {
                string outputPath = outputFilePath + @$"\Bitboards {bitboard.Key.Width} x {bitboard.Key.Height}.bin";
                ProcessBinaryFileWrite(outputPath, writer =>
                {
                    int byteCount = GetByteCount(bitboard.Key.Width, bitboard.Key.Height);
                    Console.WriteLine($"Writing {byteCount}");
                    foreach (var value in bitboard.Value)
                    {
                        ulong temp = value;
                        for (int i = 0; i < byteCount; i++)
                        {
                            writer.Write((byte)(temp & 0xFF));
                            temp >>= 8;
                        }
                    }
                });
            }
        }




        public static int GetByteCount(int width, int height)
        {
            return (int)Math.Ceiling((float)(width * height) / 8.0);
        }







        /// <summary>
        /// Assumes each puzzle is 8 by 8
        /// </summary>
        /// <param name="puzzles"></param>
        /// <param name="filePath"></param>
        public static void SavePuzzlesToBinaryFile(List<ulong> puzzles, string filePath)
        {
            if (filePath.EndsWith(".bin") == false)
            {
                filePath += ".bin";
            }

            ProcessBinaryFileWrite(filePath, writer =>
            {
                byte[] sbytes = new byte[8];
                foreach (ulong puzzle in puzzles)
                {
                    sbytes = BitConverter.GetBytes(puzzle);
                    writer.Write(sbytes, 0, sbytes.Length);
                }
            });
        }





        //public static void PrintBitboardFromFile(string filePath, int index)
        //{
        //    ProcessBinaryFileRead(filePath, reader =>
        //    {
        //        reader.BaseStream.Seek(index * sizeof(ulong), SeekOrigin.Begin);
        //        ulong value = reader.ReadUInt64();
        //        (int width, int height) dimensions = GetSize(value);
        //        value = Convert8x8ToNxM(value);
        //        //(int width, int height) dimensions = (8, 8);
        //        Console.WriteLine("Original");
        //        PrintBitboard(value, dimensions.width, dimensions.height);
        //        Console.WriteLine("Inverted");
        //        PrintBitboard(~value, dimensions.width, dimensions.height);
        //    });
        //}



        public static void PrintBitboardFromFile(string filePath, int index, bool inverted = false)
        {
            ProcessBinaryFileRead(filePath, reader =>
            {
                reader.BaseStream.Seek(index * sizeof(ulong), SeekOrigin.Begin);
                ulong value = reader.ReadUInt64();
                (int width, int height) dimensions = GetSize(value);
                if (inverted == true)
                {
                    Console.WriteLine("Inverted");
                    PrintBitboard(~value, 8, 8);
                    //PrintBitboard(~value, dimensions.width, dimensions.height);

                }
                else
                {
                    Console.WriteLine("Original");
                    PrintBitboard(value, 6, 6);
                    //PrintBitboard(value, dimensions.width, dimensions.height);

                }
            });
        }




        public static (int, int) ProcessBitboardFromFile(string filePath, int index)
        {
            (int, int) dimensions = (0, 0);

            ProcessBinaryFileRead(filePath, reader =>
            {
                reader.BaseStream.Seek(index * sizeof(ulong), SeekOrigin.Begin);
                ulong value = reader.ReadUInt64();
                dimensions = GetSize(~value);
                if (dimensions.Item1 == 0)
                {
                    Console.WriteLine("Found a bitboard with '0' width -_-");
                    PrintBitboard(~value, 8, 8);
                }

            });

            return dimensions;
        }



        public static void ReverseBinaryFile(string inputFilePath, string outputFilePath)
        {
            List<ulong> bitboards = new List<ulong>();
            ProcessBinaryFileRead(inputFilePath, reader =>
            {
                if (reader.BaseStream.Length % 8 != 0)
                {
                    throw new ArgumentException($"File length {reader.BaseStream.Length} is not a multiple of 8 bytes");
                }
                Console.WriteLine("Reading file");
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    bitboards.Add(reader.ReadUInt64());
                }
            });
            Console.WriteLine("Sorting file");
            bitboards.Sort();
            Console.WriteLine("Reversing file");
            bitboards.Reverse();

            ProcessBinaryFileWrite(outputFilePath, writer =>
            {
                foreach (ulong bitboard in bitboards)
                {
                    writer.Write(bitboard);
                }
            });
        }




        public static void InvertFile(string inputFilePath, string outputFilePath, int byteCount)
        {
            if (byteCount < 1 || byteCount > 5)
            {
                throw new ArgumentException($"Byte count must be between 1 and 5, got {byteCount}");
            }

            List<ulong> values = new List<ulong>();
            ProcessBinaryFileRead(inputFilePath, reader =>
            {
                if (reader.BaseStream.Length % byteCount != 0)
                {
                    throw new ArgumentException($"File length {reader.BaseStream.Length} is not a multiple of {byteCount} bytes");
                }

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    values.Add(~ReadPartialUlong(reader, byteCount));
                }
            });

            values.Sort();

            ProcessBinaryFileWrite(outputFilePath, writer =>
            {
                foreach (ulong value in values)
                {
                    WritePartialUlong(writer, value, byteCount);
                }
            });
        }

        private static ulong ReadPartialUlong(BinaryReader reader, int byteCount)
        {
            ulong result = 0;
            for (int i = 0; i < byteCount; i++)
            {
                result |= (ulong)reader.ReadByte() << (i * 8);
            }
            return result;
        }

        private static void WritePartialUlong(BinaryWriter writer, ulong value, int byteCount)
        {
            // Create a mask for the specified number of bytes
            ulong mask = (1UL << (byteCount * 8)) - 1;
            // Apply mask to ensure we only write the relevant bytes
            value &= mask;

            for (int i = 0; i < byteCount; i++)
            {
                writer.Write((byte)(value >> (i * 8)));
            }
        }





        /// <summary>
        /// Prints all the puzzles from filePath starting from startIndex and going for range.
        /// If fromStart is set to false, it will loop over the range backwards.
        /// If it range is too large, it will end without error.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="startIndex"></param>
        /// <param name="range"></param>
        /// <param name="fromStart"></param>
        public static void PrintBitboardRange(string filePath, int startIndex, int range, bool fromStart = true)
        {
            //Puzzle puzzle = new Puzzle();

            ProcessBinaryFileRead(filePath, reader =>
            {
                if (fromStart == true)
                {
                    reader.BaseStream.Seek(startIndex * sizeof(ulong), SeekOrigin.Begin);
                }
                else
                {
                    reader.BaseStream.Seek(-range * sizeof(ulong), SeekOrigin.End);
                }
                for (long i = 0; i < range; i++)
                {
                    if (reader.BaseStream.Position >= reader.BaseStream.Length)
                    {
                        break; // End of file reached
                    }
                    ulong value = reader.ReadUInt64();
                    //ulong convert = Convert8x8ToNxM(value);
                    PrintBitboard(value, 8, 8);
                    //PrintBitboard(convert, 6, 6);
                }
            });
        }





        /// <summary>
        /// Determines how many 64-bit integers are present in the given file.
        /// </summary>
        public static int CountPuzzlesInFile(string filePath)
        {
            int puzzleCount = 0;
            ProcessBinaryFileRead(filePath, reader =>
            {
                // Ensure the file length is a multiple of 8
                if (reader.BaseStream.Length % 8 != 0)
                {
                    throw new InvalidOperationException("File size is not a multiple of 8 bytes.");
                }

                // Calculate and return the number of 64-bit integers
                puzzleCount = (int)(reader.BaseStream.Length / 8);
            });
            return puzzleCount;
        }




        /// <summary>
        /// Accepts a bitboard with a single bit and returns its index
        /// </summary>
        /// <param name="bitboard"></param>
        public static int BitboardToIndex(ulong bitboard, int width, int height)
        {
            if (BitOperations.PopCount(bitboard) != 1)
            {
                throw new ArgumentOutOfRangeException("Bitboard must contain exactly 1 bit.");
            }
            // Convert single bit back into an index
            int bitPosition = BitOperations.TrailingZeroCount(bitboard);
            int row = bitPosition / width;
            int col = bitPosition % width;
            return row * width + col;
        }




        /// <summary>
        /// Loops through each bitboard in file. Finds how 
        /// It needs to working under a variable byte count using GetByteCount and the width and height from the filename.
        /// </summary>
        /// <param name="filePath">The name of the file we are processing</param>
        public static void GetStatistics(string filePath)
        {
            ProcessBinaryFileRead(filePath, reader =>
            {
                Console.WriteLine($"The filePath we are processing right now is {filePath}");
                (int width, int height) dimensions = FilePathToDimensions(filePath);
                int byteCount = GetByteCount(dimensions.width, dimensions.height);
                int solutions;
                Bitboard bitboard;
                Dictionary<int, int> solutionCount = new Dictionary<int, int>();
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {

                    ulong value = BytesToUlong(reader.ReadBytes(byteCount));
                    PrintBitboard(~value, dimensions.width, dimensions.height);
                    bitboard = new Bitboard(~value, dimensions.width, dimensions.height);
                    solutions = bitboard.Solutions().Count;
                    if (solutionCount.ContainsKey(solutions) == false)
                    {
                        solutionCount[solutions] = 1;
                    }
                    else
                    {
                        solutionCount[solutions] += 1;
                    }
                }

                var sol = solutionCount.ToList();

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Solutions | Solution total\n");

                foreach (var solution in solutionCount)
                {
                    stringBuilder.AppendLine(solution.Key + " | " + solution.Value);
                }
                // @"C:\Users\Rober\Documents\Solution Counts\"
                using (FileStream fileStream = new FileStream($@"C:\Users\Rober\Documents\Solution Counts\{dimensions.width} x {dimensions.height}.txt", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        writer.Write(stringBuilder.ToString());
                    }
                }
            });
        }


        private static ulong BytesToUlong(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if (bytes.Length > 8)
            {
                throw new ArgumentException("Must be 8 elements or fewer");
            }

            ulong result = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                result |= (ulong)bytes[i] << (i * 8);
            }
            return result;
        }




        public static decimal Mean(List<int> ints)
        {

            throw new NotImplementedException();
        }


        /// <summary>
        /// The filePath is always Bitboards n x m.bit<br></br>
        /// n and m fall on the 10th and 14th character
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static (int width, int height) FilePathToDimensions(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            return ((int)char.GetNumericValue(fileName.ElementAt(10)), (int)char.GetNumericValue(fileName.ElementAt(14)));
        }




        /// <summary>
        /// Checks if the bitboard contains one continuous snake and no intersections.
        /// </summary>
        /// <param name="bitboard"></param>
        /// <returns>The number of 2x2 regions it contains</returns>
        public static int TwoByTwo(ulong bitboard)
        {
            ulong cube = 771;
            int count = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if ((bitboard & cube) == 0)
                    {
                        count++;
                    }
                    cube <<= 1;
                }
                cube <<= 3;
            }

            return count;
        }

        /// <summary>
        /// Checks if bitboard contains a three-way intersection
        /// </summary>
        /// <param name="bitboard"></param>
        /// <returns>true if it does; false if it doesn't</returns>
        public static bool ThreeWayIntersection(ulong bitboard)
        {
            ulong threeWay = 1794; // Pointing up

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if ((bitboard & threeWay) == 0)
                    {
                        return true;
                    }
                    threeWay <<= 1;
                }
                threeWay <<= 4;
            }




            threeWay = 519; // Pointing down

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if ((bitboard & threeWay) == 0)
                    {
                        return true;
                    }
                    threeWay <<= 1;
                }
                threeWay <<= 4;
            }

            threeWay = 66305; // Pointing right

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if ((bitboard & threeWay) == 0)
                    {
                        return true;
                    }
                    threeWay <<= 1;
                }
                threeWay <<= 3;
            }



            threeWay = 131842; // Pointing left
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if ((bitboard & threeWay) == 0)
                    {
                        return true;
                    }
                    threeWay <<= 1;
                }
                threeWay <<= 3;
            }

            return false;
        }

        public static bool PuzzlesToExclude(ulong bitboard)
        {
            List<ulong> emptyGrids = [
                // Empty n by m puzzles
                // Example: This is a 6 by 3.
                // 0 0 0 0 0 0
                // 0 0 0 0 0 0
                // 0 0 0 0 0 0

                // Empty grids
                0xfffffffffffffffe, //1 x 1
                0xfffffffffffffffc, //2 x 1
                0xfffffffffffffcfc, //2 x 2
                0xfffffffffffffff8, //3 x 1
                0xfffffffffffff8f8, //3 x 2
                0xfffffffffff8f8f8, //3 x 3
                0xfffffffffffffff0, //4 x 1
                0xfffffffffffff0f0, //4 x 2
                0xfffffffffff0f0f0, //4 x 3
                0xfffffffff0f0f0f0, //4 x 4
                0xffffffffffffffe0, //5 x 1
                0xffffffffffffe0e0, //5 x 2
                0xffffffffffe0e0e0, //5 x 3
                0xffffffffe0e0e0e0, //5 x 4
                0xffffffe0e0e0e0e0, //5 x 5
                0xffffffffffffffc0, //6 x 1
                0xffffffffffffc0c0, //6 x 2
                0xffffffffffc0c0c0, //6 x 3
                0xffffffffc0c0c0c0, //6 x 4
                0xffffffc0c0c0c0c0, //6 x 5
                0xffffc0c0c0c0c0c0, //6 x 6

                ];

            List<ulong> stairs = [
                // inverted stairs: Makes sure these are the canonical ones
                0xfffffffffffffcfd, // 2x2: default rotation
                0xfffffffffffffcfe, // +90� rotation
                0xfffffffffffffefc, // +90� rotation
                0xfffffffffffffdfc, // +90� rotation

                0xfffffffffff8f9fb, // 3x3
                0xfffffffffff8fcfe, // +90� rotation
                0xfffffffffffefcf8, // +90� rotation
                0xfffffffffffbf9f8, // +90� rotation

                0xfffffffff0f1f3f7, // 4x4
                0xfffffffff0f8fcfe, // +90� rotation
                0xfffffffffefcf8f0, // +90� rotation
                0xfffffffff7f3f1f0, // +90� rotation

                0xffffffe0e1e3e7ef, // 5x5
                0xffffffe0f0f8fcfe, // +90� rotation
                0xfffffffefcf8f0e0, // +90� rotation
                0xffffffefe7e3e1e0, // +90� rotation

                0xffffc0c1c3c7cfdf, // 6x6
                0xffffc0e0f0f8fcfe, // +90� rotation
                0xfffffefcf8f0e0c0, // +90� rotation
                0xffffdfcfc7c3c1c0, // +90� rotation
                ];


            foreach (ulong puzzle in emptyGrids)
            {
                if (puzzle == bitboard)
                {
                    return true;
                }
            }

            foreach (ulong puzzle in stairs)
            {
                if (puzzle == bitboard)
                {
                    Console.WriteLine("Found an inverted staircase!");
                    PrintBitboard(puzzle);
                    Console.WriteLine("\n\n\n");
                    return true;
                }
            }

            return false;
        }

        public static void SeparatePuzzles(string inputFilePath, string outputFilePath)
        {
            List<ulong> values = new List<ulong>();
            List<ulong> bad = new List<ulong>();
            ProcessBinaryFileRead(inputFilePath, reader =>
            {
                if (reader.BaseStream.Length % 8 != 0)
                {
                    throw new ArgumentException($"File length {reader.BaseStream.Length} is not a multiple of {8} bytes");
                }

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    ulong value = reader.ReadUInt64();
                    // This should have the highest priority
                    if (PuzzlesToExclude(value) == true)
                    {
                        //Console.WriteLine("We're not keeping this. It's a puzzle to exclude.");
                        //PrintBitboard(value);
                        bad.Add(value);
                        continue;
                    }
                    // This should have 2nd priority
                    if (TwoByTwo(value) > 0)
                    {
                        //Console.WriteLine("Keep anything with a 2x2.");
                        //PrintBitboard(value);
                        values.Add(value);
                    }
                    else if (ThreeWayIntersection(value) == true)
                    {
                        //Console.WriteLine("This should have a three-way intersection and no 2x2 regions.");
                        //PrintBitboard(value);
                        values.Add(value);
                    }
                    else if (BitOperations.PopCount(~value) > 11)
                    {
                        // This bitboard doesn't have any 2x2 regions
                        // This bitboard doesn't have any 3-way intersections
                        // This bitobard only contains snakes
                        // Let's only keep ones that have more than 11 empty cells
                        // This ensures that every bitboard contains at least 2 bends, with all but 1 having 3 or more
                        //Console.WriteLine("We're not keeping this because it doesn't contain an intersection nor 2x2 (snake)");
                        //PrintBitboard(value);
                        values.Add(value);
                    }
                    else
                    {
                        //Console.WriteLine("This bitboard only contains a snake, but it has 9 empty cells or less.");
                        //PrintBitboard(value);
                        //Console.WriteLine();
                        bad.Add(value);
                    }
                }
            });

            Console.WriteLine($"Values: {values.Count}");
            Console.WriteLine($"Bad: {bad.Count}");
            foreach (ulong badPuzzle in bad)
            {
                PrintBitboard(badPuzzle);
            }
            ProcessBinaryFileWrite(outputFilePath, writer =>
            {
                foreach (ulong value in values)
                {
                    writer.Write(value);
                }
            });
        }


        /// <summary>
        /// Loops over each ulong in good puzzle list from startIndex<br></br>
        /// Get a solution list for the puzzle at startIndex<br></br>
        /// Then, seach the next puzzle for any matching states (agnostics = color order doesn't matter)<br></br>
        /// It should be n steps away from the previous one<br></br>
        /// Keep going until you find count of them<br></br>
        /// Return a list of them
        /// </summary>
        /// <param name="startIndex">The starting bitboard</param>
        /// <param name="count">How many times we doing this?</param>
        /// <param name="difficulty">Minimum number of moves</param>
        public static void SimilarStateSearch(int startIndex, int count, int difficulty)
        {

        }

        public static sbyte down = 8;
        public static sbyte right = 1;


        /// <summary>
        /// Very simple PolyominoChecker<br></br>
        /// Scans for the first bit of a polyomino. Then does a breath-first search<br></br>
        /// If the number of visited cells = the number of empty cells, then it was a polyomino.
        /// </summary>
        /// <param name="bitboard"></param>

        public static bool PolyominoChecker(ulong bitboard)
        {
            // This should count the zeros
            int population = 64 - BitOperations.PopCount(bitboard);

            HashSet<int> visited = new HashSet<int>();
            Queue<int> queue = new Queue<int>();

            // Gets the first empty cell
            visited.Add(BitOperations.TrailingZeroCount(~bitboard));
            queue.Enqueue(BitOperations.TrailingZeroCount(~bitboard));

            // This is a basic BFS
            while (queue.Count > 0)
            {
                int index = queue.Dequeue();
                // Left
                if (index % 8 > 0 && GetBitboardCell(bitboard, index - 1) == false)
                {
                    if (!visited.Contains(index - 1))
                    {
                        visited.Add(index - 1);
                        queue.Enqueue(index - 1);
                    }
                }
                // Up
                if (index >= 8 && GetBitboardCell(bitboard, index - 8) == false)
                {
                    if (!visited.Contains(index - 8))
                    {
                        visited.Add(index - 8);
                        queue.Enqueue(index - 8);
                    }
                }
                // Down
                if (index + 8 < 64 && GetBitboardCell(bitboard, index + 8) == false)
                {
                    if (!visited.Contains(index + 8))
                    {
                        visited.Add(index + 8);
                        queue.Enqueue(index + 8);
                    }
                }
                // Right
                if (index % 8 < 7 && GetBitboardCell(bitboard, index + 1) == false)
                {
                    if (!visited.Contains(index + 1))
                    {
                        visited.Add(index + 1);
                        queue.Enqueue(index + 1);
                    }
                }
            }

            // If we visit all the empty cells, then it will equal the population of the empty cells
            return visited.Count == population;
        }

        /// <summary>
        /// Map from the long-form state code for each state to the state number
        /// see expandState.
        /// 
        /// This is only used during state table construction.
        /// </summary>
        public static Dictionary<string, ushort> stateNumbers = new Dictionary<string, ushort>();

        /// <summary>
        /// Packed map from (state*256 + next_row_byte) -> transition
        /// 
        /// transition is next_state + (island_count<<12), where island_count is the
        /// number of islands cut off from the further rows
        /// </summary>
        public static List<ushort> transitions = new List<ushort>();
        
        
        /// <summary>
        /// checker_state + next_row_number * 4096 + have_islands*65536 -> generator_state
        /// </summary>
        public static Dictionary<int, ushort> genStateNumbers = new Dictionary<int, ushort>();
        
        
        /// <summary>
        /// Generator states.  These for a DFA that accepts all 8-row polyminoes.
        /// State 0 is used as both the unique start state and the unique accept state
        /// </summary>
        public static List<List<GenTransitionInfo>> genStates = new List<List<GenTransitionInfo>>();


        /// <summary>
        /// The byte representing a row of all water.  Note that this code counts
        /// 0-islands, not 1-islands
        /// </summary>
        public const byte ALL_WATER = (byte)0xFF;

        #region Disjoint Set
        /*
         * Disjoint set the proper way.  The sets are integers in an array:
         * For each integer i
         *   - i === 0 => set is uninitialized (not yet a set)
         *   - i < 0 => set is a link to ~i
         *   - i >= 0 => set is of size i
         */

        // find with path compression.
        private static int find(int[] sets, int s)
        {
            int parent = sets[s];
            if (parent > 0)
            {
                return s;
            }
            else if (parent < 0)
            {
                parent = find(sets, ~parent);
                sets[s] = ~parent;
                return parent;
            }
            else
            {
                sets[s] = 1;
                return s;
            }
        }

        // union by size
        private static bool union(int[] sets, int x, int y)
        {
            x = find(sets, x);
            y = find(sets, y);
            if (x == y)
            {
                return false;
            }
            int szx = sets[x];
            int szy = sets[y];
            if (szx < szy)
            {
                sets[y] += szx;
                sets[x] = ~y;
            }
            else
            {
                sets[x] += szy;
                sets[y] = ~x;
            }
            return true;
        }

        #endregion


        /// <summary>
        /// Expands the specified state code.
        /// 
        /// A state code is a string of digits.
        ///  0 => water
        ///  x => island number x.  new islands are numbered from left to right
        /// </summary>
        /// <param name="stateCode">The state code to expand.</param>
        /// <param name="nextrow">the lower 8 bits represent the next row.  0-bits are land</param>
        /// <returns>The transition code for the transition from stateCode to nextrow</returns>
        public static ushort ExpandState(string stateCode, int nextrow)
        {
            // convert the next row into a disjoint set array
            // if you want to count 1-islands instead of 0-islands, change `~nextrow` into `nextrow` below,
            // and fix the ALL_WATER constant
            int[] sets = new int[8];
            for (int i = 0; i < 8; ++i)
            {
                sets[i] = (~nextrow >> i) & 1;
            }
            for (int i = 0; i < 7; ++i)
            {
                if (((~nextrow >> i) & 3) == 3)
                {
                    union(sets, i, i + 1);
                }
            }
            // map from state code island to first attached set in sets
            int[] links = [-1, -1, -1, -1, -1, -1, -1, -1];
            int topIslandCount = 0;
            for (int i = 0; i < 8; ++i)
            {
                char digit = stateCode[i];
                int topisland = digit - '1';
                topIslandCount = Math.Max(topIslandCount, topisland + 1);
                if (sets[i] != 0 && topisland >= 0)
                {
                    // connection from prev row to nextrow
                    int bottomSet = links[topisland];
                    if (bottomSet < 0)
                    {
                        // this island is not yet connected
                        links[topisland] = i;
                    }
                    else
                    {
                        // the top island is already connected. union bottom sets
                        union(sets, bottomSet, i);
                    }
                }
            }

            // count the number of top-row islands that don't connect to the next row.
            int cutOffCount = 0;
            for (int i = 0; i < topIslandCount; ++i)
            {
                if (links[i] < 0)
                {
                    ++cutOffCount;
                }
            }

            // turn the new union-find array into a state code
            char nextSet = '1';
            char[] newChars = "00000000".ToCharArray();
            for (int i = 0; i < 8; ++i)
            {
                links[i] = -1;
            }
            for (int i = 0; i < 8; ++i)
            {
                if (sets[i] != 0)
                {
                    int set = find(sets, i);
                    int link = links[set];
                    if (link >= 0)
                    {
                        newChars[i] = newChars[link];
                    }
                    else
                    {
                        newChars[i] = nextSet++;
                        links[set] = i;
                    }
                }
            }
            string newStateCode = new string(newChars);

            // get the state number
            if (stateNumbers.ContainsKey(newStateCode))
            {
                // state already exists and is/will be expanded
                return (ushort)(stateNumbers[newStateCode] | (cutOffCount << 12));
            }
            ushort newState = (ushort)stateNumbers.Count;
            stateNumbers.Add(newStateCode, newState);

            // fill out the state table
            while (transitions.Count <= (newState + 1) * 256)
            {
                transitions.Add(0);
            }
            for (int i = 0; i < 256; ++i)
            {
                transitions[newState * 256 + i] = ExpandState(newStateCode, i);
            }
            return (ushort)(newState | (cutOffCount << 12));
        }


        /// <summary>
        /// Fill out a state in the generator table if it doesn't exist
        /// Return the state number
        /// </summary>
        public static ushort MakeGenState(int nextRowNumber, int checkerState, int haveIslands)
        {
            int stateKey = checkerState + nextRowNumber * 4096 + haveIslands * 65536;
            if (genStateNumbers.ContainsKey(stateKey))
            {
                return genStateNumbers[stateKey];
            }
            ushort newGenState = (ushort)genStates.Count;
            genStateNumbers.Add(stateKey, newGenState);
            var tlist = new List<GenTransitionInfo>();
            genStates.Add(tlist);
            int transitionsOffset = checkerState * 256;
            ulong totalPaths = 0;
            for (int i = 0; i < 256; ++i)
            {
                var transition = transitions[transitionsOffset + i];
                int nextCheckerState = transition & 0x0FFF;
                var newIslands = (transition >> 12) + haveIslands;
                if (newIslands > (i == ALL_WATER ? 1 : 0))
                {
                    // we are destined to get too many islands this way.
                    continue;
                }
                if (nextRowNumber == 7)
                {
                    // all transitions for row 7 have to to the accept state
                    // calculate total number of islands
                    newIslands += transitions[nextCheckerState * 256 + ALL_WATER] >> 12;
                    if (newIslands == 1)
                    {
                        totalPaths += 1;
                        tlist.Add(new GenTransitionInfo { nextRow = (byte)i, nextState = 0, cumulativePaths = totalPaths });
                    }
                }
                else
                {
                    ushort nextGenState = MakeGenState(nextRowNumber + 1, nextCheckerState, newIslands);
                    ulong newPaths = genStates[nextGenState].LastOrDefault().cumulativePaths;
                    if (newPaths > 0)
                    {
                        totalPaths += newPaths;
                        tlist.Add(new GenTransitionInfo { nextRow = (byte)i, nextState = nextGenState, cumulativePaths = totalPaths });
                    }
                }
            }
            return newGenState;
        }

        // generate the DFA
        

        public static ulong GetNthPolimyno(ulong n)
        {
            int state = 0;
            ulong poly = 0;
            for (int row = 0; row < 8; ++row)
            {
                var tlist = genStates[state];
                // binary search to find the transition that contains the nth path
                int hi = tlist.Count - 1;
                int lo = 0;
                while (hi > lo)
                {
                    int test = (lo + hi) >> 1;
                    if (n >= tlist[test].cumulativePaths)
                    {
                        // test is too low
                        lo = test + 1;
                    }
                    else
                    {
                        // test is high enough
                        hi = test;
                    }
                }
                if (lo > 0)
                {
                    n -= tlist[lo - 1].cumulativePaths;
                }
                var transition = tlist[lo];
                poly = (poly << 8) | transition.nextRow;
                state = transition.nextState;
            }
            return poly;
        }

        public static ulong GetNValue(ulong poly)
        {
            int state = 0;
            ulong n = 0;

            for (int row = 0; row < 8; ++row)
            {
                byte currentRow = (byte)((poly >> ((7 - row) * 8)) & 0xFF);
                var tlist = genStates[state];

                // Find the transition that matches the current row
                int transitionIndex = tlist.FindIndex(t => t.nextRow == currentRow);

                if (transitionIndex < 0)
                    throw new ArgumentException("Invalid polyomino");

                // Add cumulative paths from previous transitions
                if (transitionIndex > 0)
                {
                    n += tlist[transitionIndex - 1].cumulativePaths;
                }

                // Update state for next iteration
                state = tlist[transitionIndex].nextState;
            }

            return n;
        }


        public struct GenTransitionInfo
        {
            public byte nextRow;
            public ushort nextState;
            public ulong cumulativePaths;
        }

        public static void SaveGenStates(string filePath, List<List<GenTransitionInfo>> genStates)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                Console.WriteLine($"About to write {(short)genStates.Count} lists.");
                writer.Write((short)genStates.Count);
                foreach (var tlist in genStates)
                {
                    Console.WriteLine($"This list has {(short)tlist.Count} elements.");
                    writer.Write((short)tlist.Count);
                    foreach(var state in tlist)
                    {
                        writer.Write(state.nextRow);
                        writer.Write(state.nextState);
                        writer.Write(state.cumulativePaths);
                    }
                }
            }
        }

        //public static List<List<GenTransitionInfo>> LoadGenStates(string filePath)
        public static void LoadGenStates(string filePath)
        {
            ProcessBinaryFileRead(filePath, reader =>
            {
                Console.WriteLine(reader.BaseStream.Length); 
            });

            //return genStates;
        }

        ////   print("Saved gen_states successfully.")
        //       public static void SaveGenStates(string filePath, List<List<GenTransitionInfo>> genStates)
        //       {
        //           using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        //           {
        //               foreach (var list in genStates)
        //               {
        //                   writer.Write((byte)list.Count); // Outer array size
        //                   foreach(var state in list)
        //                   {
        //                       writer.Write(state.nextRow);          // 1 byte
        //                       writer.Write(state.nextState);        // 2 bytes (ushort)
        //                       writer.Write(state.cumulativePaths);  // 8 bytes (ulong)
        //                   }
        //               }
        //           }
        //           Console.WriteLine($"Saved {genStates.Count} gen_states successfully.");
        //       }
    }
}
