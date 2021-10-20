/*
    Terra Compress Lz2
    Version 1.0
*/

using System;
using System.Collections.Generic;

namespace Smallhacker.TerraCompress
{
    public class Lz2 : ICompressor, IDecompressor
    {
        private const byte DIRECT_COPY = 0;
        private const byte BYTE_FILL = 1;
        private const byte WORD_FILL = 2;
        private const byte INCREASE_FILL = 3;
        private const byte REPEAT = 4;
        private const byte LONG_COMMAND = 7;

        // How many bytes each command must encode to outdo Direct Copy
        private readonly byte[] COMMAND_WEIGHT =
            {
                0,  // Direct Copy
                3,  // Byte Fill
                4,  // Word Fill
                3,  // Increasing Fill
                4   // Repeat
            };

        public byte[] Compress(byte[] data)
        {
            // Greedy implementation

            if (data == null)
            {
                throw new Exception("Data is null.");
            }

            List<byte> output = new List<byte>();
            int position = 0;
            int length = data.Length;

            List<byte> directCopyBuffer = null;

            while (position < length)
            {
                byte currentByte = data[position++];
                byte nextByte = 0;
                ushort repeatAddress = 0;

                int[] byteCount = new int[REPEAT+1];

                // Evaluate Byte Fill
                byteCount[BYTE_FILL] = 1;
                { 
                    for (int i = position; i < length; i++)
                    {
                        if (data[i] != currentByte)
                        {
                            break;
                        }
                        byteCount[BYTE_FILL]++;
                    }
                }

                // Evaluate Word Fill
                byteCount[WORD_FILL] = 1;
                {
                    if (position < length)
                    {
                        byteCount[WORD_FILL]++;
                        nextByte = data[position];
                        int oddEven = 0;
                        for (int i = position + 1; i < length; i++, oddEven++)
                        {
                            byte currentOddEvenByte = (oddEven & 1) == 0 ? currentByte : nextByte;
                            if (data[i] != currentOddEvenByte)
                            {
                                break;
                            }
                            byteCount[WORD_FILL]++;
                        }
                    }
                }

                // Evaluate Increasing Fill
                byteCount[INCREASE_FILL] = 1;
                {
                    byte increaseByte = (byte)(currentByte + 1);
                    for (int i = position; i < length; i++)
                    {
                        if (data[i] != increaseByte++)
                        {
                            break;
                        }
                        byteCount[INCREASE_FILL]++;
                    }
                }

                // Evaluate Repeat
                byteCount[REPEAT] = 0;
                {
                    //Slow O(n^2) brute force algorithm for now
                    int maxAddressInt = Math.Min(0xFFFF, position - 2);
                    if (maxAddressInt >= 0) {
                        ushort maxAddress = (ushort)maxAddressInt;
                        for (int start = 0; start <= maxAddress; start++)
                        {
                            int chunkSize = 0;

                            for (int pos = position - 1; pos < length && chunkSize < 1023; pos++)
                            {
                                if (data[pos] != data[start + chunkSize])
                                {
                                    break;
                                }
                                chunkSize++;
                            }

                            if (chunkSize > byteCount[REPEAT])
                            {
                                repeatAddress = (ushort)start;
                                byteCount[REPEAT] = chunkSize;
                            }

                        }

                    }
                }

                // Choose next command
                byte nextCommand = DIRECT_COPY; // Default command unless anything better is found
                int nextCommandByteCount = 1;
                for (byte commandSuggestion = 1; commandSuggestion < byteCount.Length; commandSuggestion++)
                {
                    // Would this command save any space?
                    if (byteCount[commandSuggestion] >= COMMAND_WEIGHT[commandSuggestion])
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
                if (nextCommand == DIRECT_COPY)
                {
                    if (directCopyBuffer == null)
                    {
                        directCopyBuffer = new List<byte>();
                    }
                    directCopyBuffer.Add(currentByte);

                    if (directCopyBuffer.Count >= 1023)
                    {
                        // Direct Copy has a maximum length of 1023 bytes
                        OutputCommand(DIRECT_COPY, directCopyBuffer.Count, output);
                        output.AddRange(directCopyBuffer);
                        directCopyBuffer = null;
                    }
                } else
                {
                    if (directCopyBuffer != null)
                    {
                        // Direct Copy command in progress. Write it to output before proceeding
                        OutputCommand(DIRECT_COPY, directCopyBuffer.Count, output);
                        output.AddRange(directCopyBuffer);
                        directCopyBuffer = null;
                    }
                }

                // Output command
                switch(nextCommand)
                {
                    case DIRECT_COPY:
                        // Already handled above
                        break;
                    case BYTE_FILL:
                        OutputCommand(nextCommand, nextCommandByteCount, output);
                        output.Add(currentByte);
                        break;
                    case WORD_FILL:
                        OutputCommand(nextCommand, nextCommandByteCount, output);
                        output.Add(currentByte);
                        output.Add(nextByte);
                        break;
                    case INCREASE_FILL:
                        OutputCommand(nextCommand, nextCommandByteCount, output);
                        output.Add(currentByte);
                        break;
                    case REPEAT:
                        OutputCommand(nextCommand, nextCommandByteCount, output);
                        output.Add((byte)(repeatAddress >> 8));
                        output.Add((byte)repeatAddress);
                        break;
                    default:
                        throw new Exception("Internal error: Unknown command chosen.");
                }

                position += (nextCommandByteCount) - 1;
            }

            // Output Direct Copy buffer if it exists
            if (directCopyBuffer != null)
            {
                OutputCommand(DIRECT_COPY, directCopyBuffer.Count, output);
                output.AddRange(directCopyBuffer);
            }

            output.Add(0xFF);
            return output.ToArray();
        }

        public byte[] Decompress(byte[] compressedData, uint start)
        {
            if (compressedData == null)
            {
                throw new Exception("Compressed data is null.");
            }
            try
            {
                List<byte> output = new List<byte>();
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
                    if (command == LONG_COMMAND) // Long command
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
                        case DIRECT_COPY: // Direct Copy
                            for (int i = 0; i < length; i++)
                            {
                                output.Add(compressedData[position++]);
                            }
                            break;
                        case BYTE_FILL: // Byte Fill
                            byte fillByte = compressedData[position++];
                            for (int i = 0; i < length; i++)
                            {
                                output.Add(fillByte);
                            }
                            break;
                        case WORD_FILL: // Word Fill
                            byte fillByteEven = compressedData[position++];
                            byte fillByteOdd = compressedData[position++];
                            for (int i = 0; i < length; i++)
                            {
                                byte thisByte = (i & 1) == 0 ? fillByteEven : fillByteOdd;
                                output.Add(thisByte);
                            }
                            break;
                        case INCREASE_FILL: // Increasing Fill
                            byte increaseFillByte = compressedData[position++];
                            for (int i = 0; i < length; i++)
                            {
                                output.Add(increaseFillByte++);
                            }
                            break;
                        case REPEAT: // Repeat
                            ushort origin = (ushort)((compressedData[position++] << 8) | compressedData[position++]);
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
                throw new Exception("Internal error: Length assertion failed.");
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
    