using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Primary_Puzzle_Solver
{

    class BitboardStorage
    {
        const string DataFile = @"C:\Users\rober\Documents\Primary Puzzles\Good ones\bitboards.dat";
        const string IndexFile = @"C:\Users\rober\Documents\Primary Puzzles\Good ones\index.dat";

        // Writes bitboards with index
        public static void WriteBitboards(ulong[] bitboards)
        {
            using var dataStream = new BinaryWriter(File.Open(DataFile, FileMode.Create));
            using var indexStream = new BinaryWriter(File.Open(IndexFile, FileMode.Create));
            int offset = 0;
            foreach (ulong bitboard in bitboards)
            {
                byte[] compressed = Compress(bitboard);
                dataStream.Write((byte)compressed.Length);  // Prefix with length
                dataStream.Write(compressed);
                indexStream.Write(offset);  // Save offset
                offset += 1 + compressed.Length;
            }
        }

        // Reads a bitboard by index
        public static ulong ReadBitboard(int index)
        {
            using var indexStream = new BinaryReader(File.OpenRead(IndexFile));
            using var dataStream = new BinaryReader(File.OpenRead(DataFile));
            indexStream.BaseStream.Seek(index * sizeof(long), SeekOrigin.Begin);
            long offset = indexStream.ReadInt64();

            dataStream.BaseStream.Seek(offset, SeekOrigin.Begin);
            int length = dataStream.ReadByte();
            byte[] compressed = dataStream.ReadBytes(length);

            return Decompress(compressed);
        }

        // Compress ulong to minimal bytes
        private static byte[] Compress(ulong value)
        {
            int bytesNeeded = 1;
            ulong temp = value;
            while ((temp >>= 8) != 0) bytesNeeded++;

            byte[] result = new byte[bytesNeeded];
            for (int i = 0; i < bytesNeeded; i++)
            {
                result[i] = (byte)(value & 0xFF);
                value >>= 8;
            }
            return result;
        }

        // Decompress bytes back to ulong
        private static ulong Decompress(byte[] data)
        {
            ulong result = 0;
            for (int i = data.Length - 1; i >= 0; i--)
            {
                result = (result << 8) | data[i];
            }
            return result;
        }
    }
}
