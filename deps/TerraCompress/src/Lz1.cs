using System;
using System.Collections.Generic;

namespace Smallhacker.TerraCompress
{
    /// <summary>
    /// Modified Lz2 class to do Lz1 instead
    /// </summary>
    public class Lz1 : ICompressor, IDecompressor
    {
        private const byte DirectCopy = 0;
        private const byte ByteFill = 1;
        private const byte WordFill = 2;
        private const byte IncreaseFill = 3;
        private const byte Repeat = 4;
        private const byte LongCommand = 7;

        // How many bytes each command must encode to outdo Direct Copy
        private readonly byte[] _commandWeight =
            {
                0,  // Direct Copy
                3,  // Byte Fill
                4,  // Word Fill
                3,  // Increasing Fill
                4   // Repeat
            };

        // Evaluate Byte Fill
        public static void EvalByteFill(ref byte[] data, ref int position, ref int[] byteCount, ref byte currentByte)
        {
            byteCount[ByteFill] = 1;
            {
                for (int i = position; i < data.Length; i++)
                {
                    if (data[i] != currentByte)
                    {
                        break;
                    }
                    byteCount[ByteFill]++;
                }
            }
        }

        // Evalue Word Fill
        public static void EvalWordFill(ref byte[] data, ref int position, ref int[] byteCount, ref byte currentByte, ref byte nextByte)
        {
            byteCount[WordFill] = 1;
            {
                if (position < data.Length)
                {
                    byteCount[WordFill]++;
                    nextByte = data[position];
                    int oddEven = 0;
                    for (int i = position + 1; i < data.Length; i++, oddEven++)
                    {
                        byte currentOddEvenByte = (oddEven & 1) == 0 ? currentByte : nextByte;
                        if (data[i] != currentOddEvenByte)
                        {
                            break;
                        }
                        byteCount[WordFill]++;
                    }
                }
            }
        }

        // Evaluate Increasing Fill
        public static void EvalIncFill(ref byte[] data, ref int position, ref int[] byteCount, ref byte currentByte)
        {
            byteCount[IncreaseFill] = 1;
            {
                byte increaseByte = (byte)(currentByte + 1);
                for (int i = position; i < data.Length; i++)
                {
                    if (data[i] != increaseByte++)
                    {
                        break;
                    }
                    byteCount[IncreaseFill]++;
                }
            }
        }

        // Evaluate Repeat
        public static void EvalRepeat(ref byte[] data, ref int position, ref int[] byteCount, ref ushort repeatAddress)
        {
            byteCount[Repeat] = 0;
            {
                //Slow O(n^2) brute force algorithm for now
                int maxAddressInt = Math.Min(0xFFFF, position - 2);
                if (maxAddressInt >= 0)
                {
                    ushort maxAddress = (ushort)maxAddressInt;
                    for (int start = 0; start <= maxAddress; start++)
                    {
                        int chunkSize = 0;

                        for (int pos = position - 1; pos < data.Length && chunkSize < 1023; pos++)
                        {
                            if (data[pos] != data[start + chunkSize])
                            {
                                break;
                            }
                            chunkSize++;
                        }

                        if (chunkSize > byteCount[Repeat])
                        {
                            repeatAddress = (ushort)start;
                            byteCount[Repeat] = chunkSize;
                        }

                    }

                }
            }
        }

        public byte[] Compress(byte[] data)
        {
            // Greedy implementation

            if (data == null)
            {
                throw new ArgumentException("Data is null.");
            }

            List<byte> output = new();
            int position = 0;
            int length = data.Length;

            List<byte> directCopyBuffer = null;

            while (position < length)
            {
                byte currentByte = data[position++];
                byte nextByte = 0;
                ushort repeatAddress = 0;

                int[] byteCount = new int[Repeat+1];

                EvalByteFill(ref data, ref position, ref byteCount, ref currentByte);
                EvalWordFill(ref data, ref position, ref byteCount, ref currentByte, ref nextByte);
                EvalIncFill(ref data, ref position, ref byteCount, ref currentByte);
                EvalRepeat(ref data, ref position, ref byteCount, ref repeatAddress);

                // Choose next command
                byte nextCommand = DirectCopy; // Default command unless anything better is found
                int nextCommandByteCount = 1;
                for (byte commandSuggestion = 1; commandSuggestion < byteCount.Length; commandSuggestion++)
                {
                    // Would this command save any space?
                    if (byteCount[commandSuggestion] >= _commandWeight[commandSuggestion])
                    {
                        // Is it better than what we already have?
                        if (byteCount[commandSuggestion] > nextCommandByteCount)
                        {
                            nextCommand = commandSuggestion;
                            nextCommandByteCount = byteCount[commandSuggestion];
                        }
                    }
                }

                // Direct Copy commands are incrementally built.
                // Output or add to as needed.
                if (nextCommand == DirectCopy)
                {
                    if (directCopyBuffer == null)
                    {
                        directCopyBuffer = new List<byte>();
                    }
                    directCopyBuffer.Add(currentByte);

                    if (directCopyBuffer.Count >= 1023)
                    {
                        // Direct Copy has a maximum length of 1023 bytes
                        OutputCommand(DirectCopy, directCopyBuffer.Count, output);
                        output.AddRange(directCopyBuffer);
                        directCopyBuffer = null;
                    }
                } else
                {
                    if (directCopyBuffer != null)
                    {
                        // Direct Copy command in progress. Write it to output before proceeding
                        OutputCommand(DirectCopy, directCopyBuffer.Count, output);
                        output.AddRange(directCopyBuffer);
                        directCopyBuffer = null;
                    }
                }

                // Output command
                switch(nextCommand)
                {
                    case DirectCopy:
                        // Already handled above
                        break;
                    case ByteFill:
                        OutputCommand(nextCommand, nextCommandByteCount, output);
                        output.Add(currentByte);
                        break;
                    case WordFill:
                        OutputCommand(nextCommand, nextCommandByteCount, output);
                        output.Add(currentByte);
                        output.Add(nextByte);
                        break;
                    case IncreaseFill:
                        OutputCommand(nextCommand, nextCommandByteCount, output);
                        output.Add(currentByte);
                        break;
                    case Repeat:
                        OutputCommand(nextCommand, nextCommandByteCount, output);
                        output.Add((byte)repeatAddress);
                        output.Add((byte)(repeatAddress >> 8));
                        break;
                    default:
                        throw new Exception("Internal error: Unknown command chosen.");
                }

                position += (nextCommandByteCount) - 1;
            }

            // Output Direct Copy buffer if it exists
            if (directCopyBuffer != null)
            {
                OutputCommand(DirectCopy, directCopyBuffer.Count, output);
                output.AddRange(directCopyBuffer);
            }

            output.Add(0xFF);
            return output.ToArray();
        }

        public byte[] Decompress(byte[] compressedData, uint start)
        {
            if (compressedData == null)
            {
                throw new ArgumentException("Compressed data is null.");
            }
            try
            {
                List<byte> output = new();
                uint position = start;

                while (true)
                {
                    byte commandLength = compressedData[position++];
                    if (commandLength == 0xFF)
                    {
                        break;
                    }

                    byte command = (byte)(commandLength >> 5);
                    int length;
                    if (command == LongCommand) // Long command
                    {
                        length = compressedData[position++];
                        length |= ((commandLength & 3) << 8);
                        length++;
                        command = (byte)((commandLength >> 2) & 7);

                    }
                    else
                    {
                        length = (commandLength & 0x1F) + 1;
                    }

                    switch (command)
                    {
                        case DirectCopy: // Direct Copy
                            for (int i = 0; i < length; i++)
                            {
                                output.Add(compressedData[position++]);
                            }
                            break;
                        case ByteFill: // Byte Fill
                            byte fillByte = compressedData[position++];
                            for (int i = 0; i < length; i++)
                            {
                                output.Add(fillByte);
                            }
                            break;
                        case WordFill: // Word Fill
                            byte fillByteEven = compressedData[position++];
                            byte fillByteOdd = compressedData[position++];
                            for (int i = 0; i < length; i++)
                            {
                                byte thisByte = (i & 1) == 0 ? fillByteEven : fillByteOdd;
                                output.Add(thisByte);
                            }
                            break;
                        case IncreaseFill: // Increasing Fill
                            byte increaseFillByte = compressedData[position++];
                            for (int i = 0; i < length; i++)
                            {
                                output.Add(increaseFillByte++);
                            }
                            break;
                        case Repeat: // Repeat
                            ushort origin = (ushort)(compressedData[position++] | (compressedData[position++] << 8));
                            for (int i = 0; i < length; i++)
                            {
                                output.Add(output[origin++]);
                            }
                            break;

                        default:
                            throw new Exception("Invalid Lz2 command: " + command);
                    }
                }

                return output.ToArray();
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Reached unexpected end of compressed data.");
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new Exception("Compressed data contains invalid Lz2 Repeat command.");
            }
        }

        private static void OutputCommand(int command, int length, List<byte> output)
        {
            if (length < 1 || length >= 1024)
            {
                throw new ArgumentException("Internal error: Length assertion failed.");
            }
            if (length > 32)
            {
                // Long command
                length--;
                byte firstByte = (byte)(0xE0 | (command << 2) | (length >> 8));
                byte secondByte = (byte)length;
                output.Add(firstByte);
                output.Add(secondByte);
            }
            else
            {
                // Short command
                length--;
                byte commandLength = (byte)(command << 5 | length);
                output.Add(commandLength);
            }
        }
    }
}
