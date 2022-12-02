//Modified source file from https://https://github.com/Smallhacker/TerraCompress

//Copyright (c) 2014 Tomas Andreasson
//
//This software is provided 'as-is', without any express or implied
//warranty. In no event will the authors be held liable for any damages
//arising from the use of this software.
//
//Permission is granted to anyone to use this software for any purpose,
//including commercial applications, and to alter it and redistribute it
//freely, subject to the following restrictions:
//
//    1.The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
//
//    2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
//
//    3. This notice may not be removed or altered from any source
//    distribution.

namespace RetroMole.Core.Compression
{
    /// <summary>
    /// LZ1 Compression/Decompression class
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
        private static void EvalByteFill(ref byte[] data, ref int position, ref int[] byteCount, ref byte currentByte)
        {
            byteCount[ByteFill] = 1;
            {
                for (var i = position; i < data.Length; i++)
                {
                    if (data[i] != currentByte)
                        break;
                    byteCount[ByteFill]++;
                }
            }
        }

        // Evaluate Word Fill
        private static void EvalWordFill(ref byte[] data, ref int position, ref int[] byteCount, ref byte currentByte, ref byte nextByte)
        {
            byteCount[WordFill] = 1;
            {
                if (position >= data.Length) return;
                byteCount[WordFill]++;
                nextByte = data[position];
                var oddEven = 0;
                for (var i = position + 1; i < data.Length; i++, oddEven++)
                {
                    var currentOddEvenByte = (oddEven & 1) == 0 ? currentByte : nextByte;
                    if (data[i] != currentOddEvenByte)
                        break;
                    byteCount[WordFill]++;
                }
            }
        }

        // Evaluate Increasing Fill
        private static void EvalIncFill(ref byte[] data, ref int position, ref int[] byteCount, ref byte currentByte)
        {
            byteCount[IncreaseFill] = 1;
            {
                var increaseByte = (byte)(currentByte + 1);
                for (var i = position; i < data.Length; i++)
                {
                    if (data[i] != increaseByte++)
                        break;
                    byteCount[IncreaseFill]++;
                }
            }
        }

        // Evaluate Repeat
        private static void EvalRepeat(ref byte[] data, ref int position, ref int[] byteCount, ref ushort repeatAddress)
        {
            byteCount[Repeat] = 0;
            {
                //Slow O(n^2) brute force algorithm for now
                var maxAddressInt = Math.Min(0xFFFF, position - 2);
                if (maxAddressInt < 0) return;
                var maxAddress = (ushort)maxAddressInt;
                for (var start = 0; start <= maxAddress; start++)
                {
                    var chunkSize = 0;

                    for (var pos = position - 1; pos < data.Length && chunkSize < 1023; pos++)
                    {
                        if (data[pos] != data[start + chunkSize])
                            break;
                        chunkSize++;
                    }

                    if (chunkSize <= byteCount[Repeat]) continue;
                    repeatAddress = (ushort)start;
                    byteCount[Repeat] = chunkSize;

                }
            }
        }

        public byte[] Compress(byte[] data)
        {
            // Greedy implementation
            if (data == null)
                throw new ArgumentException("Data is null.");

            List<byte> output = new();
            var position = 0;
            var length = data.Length;

            List<byte> directCopyBuffer = new();

            while (position < length)
            {
                var currentByte = data[position++];
                byte nextByte = 0;
                ushort repeatAddress = 0;

                var byteCount = new int[Repeat + 1];

                EvalByteFill(ref data, ref position, ref byteCount, ref currentByte);
                EvalWordFill(ref data, ref position, ref byteCount, ref currentByte, ref nextByte);
                EvalIncFill(ref data, ref position, ref byteCount, ref currentByte);
                EvalRepeat(ref data, ref position, ref byteCount, ref repeatAddress);

                // Choose next command
                var nextCommand = DirectCopy; // Default command unless anything better is found
                var nextCommandByteCount = 1;
                for (byte commandSuggestion = 1; commandSuggestion < byteCount.Length; commandSuggestion++)
                {
                    // Would this command save any space?
                    if (byteCount[commandSuggestion] < _commandWeight[commandSuggestion]) continue;
                    // Is it better than what we already have?
                    if (byteCount[commandSuggestion] <= nextCommandByteCount) continue;
                    nextCommand = commandSuggestion;
                    nextCommandByteCount = byteCount[commandSuggestion];
                }

                // Direct Copy commands are incrementally built.
                // Output or add to as needed.
                if (nextCommand == DirectCopy)
                {
                    directCopyBuffer.Add(currentByte);

                    if (directCopyBuffer.Count >= 1023)
                    {
                        // Direct Copy has a maximum length of 1023 bytes
                        OutputCommand(DirectCopy, directCopyBuffer.Count, output);
                        output.AddRange(directCopyBuffer);
                        directCopyBuffer = new();
                    }
                }
                else
                {
                    if (directCopyBuffer.Count != 0)
                    {
                        // Direct Copy command in progress. Write it to output before proceeding
                        OutputCommand(DirectCopy, directCopyBuffer.Count, output);
                        output.AddRange(directCopyBuffer);
                        directCopyBuffer = new();
                    }
                }

                // Output command
                switch (nextCommand)
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

                position += nextCommandByteCount - 1;
            }

            // Output Direct Copy buffer if it exists
            if (directCopyBuffer.Count != 0)
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
                throw new ArgumentException("Compressed data is null.");
            try
            {
                List<byte> output = new();
                var position = start;

                while (true)
                {
                    var commandLength = compressedData[position++];
                    if (commandLength == 0xFF)
                        break;

                    var command = (byte)(commandLength >> 5);
                    int length;
                    if (command == LongCommand) // Long command
                    {
                        length = compressedData[position++];
                        length |= ((commandLength & 3) << 8);
                        length++;
                        command = (byte)((commandLength >> 2) & 7);

                    }
                    else
                        length = (commandLength & 0x1F) + 1;

                    switch (command)
                    {
                        case DirectCopy: // Direct Copy
                            for (var i = 0; i < length; i++)
                                output.Add(compressedData[position++]);
                            break;
                        case ByteFill: // Byte Fill
                            var fillByte = compressedData[position++];
                            for (var i = 0; i < length; i++)
                                output.Add(fillByte);
                            break;
                        case WordFill: // Word Fill
                            var fillByteEven = compressedData[position++];
                            var fillByteOdd = compressedData[position++];
                            for (var i = 0; i < length; i++)
                                output.Add((i & 1) == 0 ? fillByteEven : fillByteOdd);
                            break;
                        case IncreaseFill: // Increasing Fill
                            var increaseFillByte = compressedData[position++];
                            for (var i = 0; i < length; i++)
                                output.Add(increaseFillByte++);
                            break;
                        case Repeat: // Repeat
                            var origin = (ushort)(compressedData[position++] | (compressedData[position++] << 8));
                            for (var i = 0; i < length; i++)
                                output.Add(output[origin++]);
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

        private static void OutputCommand(int command, int length, ICollection<byte> output)
        {
            switch (length)
            {
                case < 1 or >= 1024:
                    throw new ArgumentException("Internal error: Length assertion failed.");
                case > 32:
                {
                    // Long command
                    length--;
                    var firstByte = (byte)(0xE0 | (command << 2) | (length >> 8));
                    var secondByte = (byte)length;
                    output.Add(firstByte);
                    output.Add(secondByte);
                    break;
                }
                default:
                {
                    // Short command
                    length--;
                    var commandLength = (byte)(command << 5 | length);
                    output.Add(commandLength);
                    break;
                }
            }
        }
    }
}
