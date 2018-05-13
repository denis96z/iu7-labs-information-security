using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace Lab6
{
    class FrequencyData : IComparable<FrequencyData>
    {
        public byte? Byte { get; private set; }
        public long Frequency { get; private set; }
        
        public FrequencyData(long freq, byte? b = null)
        {
            Byte = b;
            Frequency = freq;
        }

        public int CompareTo(FrequencyData other)
        {
            return Math.Sign(this.Frequency - other.Frequency);
        }
    }

    class BitCode
    {
        public byte? Byte { get; private set; }
        public BitArray Bits { get; private set; }

        public BitCode(byte? b, bool[] bits)
        {
            Byte = b;

            Bits = new BitArray(bits.Length);
            for (int i = 0; i < bits.Length; i++)
            {
                Bits[i] = bits[bits.Length - i - 1];
            }
        }

        public byte[] Export()
        {
            int numBytes = Bits.Length / 8 +
                (Bits.Length % 8 == 0 ? 0 : 1);
            var bytes = new byte[numBytes];
            Bits.CopyTo(bytes, 0);

            var blob = new byte[numBytes + 2];
            if (Byte != null) blob[0] = (byte) Byte;
            blob[1] = (byte)Bits.Length;
            Array.Copy(bytes, 0, blob, 2, numBytes);

            return blob;
        }

        public void Import(byte[] blob, int index)
        {
            Byte = blob[index];
            Bits = new BitArray(blob[index + 1]);
            for (int i = 0; i < Bits.Count; i++)
            {
                Bits[i] = (blob[(i / 8) + index + 2] & (1 << (i % 8))) > 0;
            }
        }
    }

    class BitCodesTable
    {
        private List<BitCode> bitCodes = new List<BitCode>();

        public void Add(BitCode bitCode)
        {
            bitCodes.Add(bitCode);
            bitCodes.Sort((x, y) => x.Bits.Length - y.Bits.Length);
        }

        public BitArray Match(byte b)
        {
            foreach (var bitCode in bitCodes)
            {
                if (bitCode.Byte == b)
                {
                    return bitCode.Bits;
                }
            }
            return null;
        }

        public byte? Match (BitArray bits)
        {
            foreach (var bitCode in bitCodes)
            {
                if (bitCode.Bits.Count != bits.Count)
                {
                    continue;
                }

                for (int i = 0; i < bits.Count; i++)
                {
                    if (bits[i] != bitCode.Bits[i])
                    {
                        goto LabelMismatch;
                    }
                }
                return bitCode.Byte;

                LabelMismatch:;
            }
            return null;
        }

        public byte[] Export()
        {
            var list = new List<byte[]>();

            int blobLength = 0;
            foreach (var bitCode in bitCodes)
            {
                var bytes = bitCode.Export();
                blobLength += bytes.Length;
                list.Add(bytes);
            }

            var blob = new byte[blobLength + 2];
            var lengthBytes = BitConverter.GetBytes((short)blobLength);
            blob[0] = lengthBytes[0];
            blob[1] = lengthBytes[1];

            int index = 2;
            foreach (var bits in list)
            {
                Array.Copy(bits, 0, blob, index, bits.Length);
                index += bits.Length;
            }

            return blob;
        }

        public void Import(byte[] blob, int startIndex, int blobLength)
        {
            bitCodes.Clear();

            int index = startIndex;
            while (index < blobLength)
            {
                int length = (blob[index + 1] / 8) +
                    ((blob[index + 1] % 8 == 0) ? 0 : 1);
                bitCodes.Add(new BitCode(null, new bool[] { }));
                bitCodes.Last().Import(blob, index);
                index += length + 2;
            }
        }
    }

    class Huffman
    {
        public byte[] Compress(byte[] data)
        {
            var freq = CountFrequencies(data);
            var freqList = GetFreqList(freq);
            var tree = MakeFrequencyTree(freqList);
            var codes = SeekCodes(freqList, tree);

            var blob = codes.Export();
            var cData = CompressBytes(data, codes);

            var compressed = new byte[blob.Length + cData.Length];
            Array.Copy(blob, 0, compressed, 0, blob.Length);
            Array.Copy(cData, 0, compressed, blob.Length, cData.Length);

            #if DEBUG
            VisualizeTree(tree);
            #endif

            return compressed;
        }

        public byte[] Decompress(byte[] cData)
        {
            var tableLength = BitConverter.ToInt16(cData, 0);
            var bitCodesTable = new BitCodesTable();
            bitCodesTable.Import(cData, 2, tableLength);

            var dataCount = BitConverter.ToInt64(cData, tableLength + 2);
            var data = DecompressBytes(cData, tableLength + 10, dataCount, bitCodesTable);

            return data;
        }

        private long[] CountFrequencies(byte[] data)
        {
            long[] freq = new long[byte.MaxValue + 1];
            foreach (byte b in data)
            {
                freq[b]++;
            }
            return freq;
        }

        private List<FrequencyData> GetFreqList(long[] freq)
        {
            var freqList = new List<FrequencyData>();
            for (int i = 0; i < freq.Length; i++)
            {
                if (freq[i] > 0)
                {
                    freqList.Add(new FrequencyData(freq[i], (byte)i));
                }
            }
            return freqList;
        }

        private BinaryTree<FrequencyData> MakeFrequencyTree(List<FrequencyData> freqList)
        {
            var freqNodesList = new List<BinaryTreeNode<FrequencyData>>();
            foreach (var freq in freqList)
            {
                freqNodesList.Add(new BinaryTreeNode<FrequencyData>(freq));
            }

            int n = freqNodesList.Count;
            for (int i = 1; i < n; i++)
            {
                freqNodesList.Sort((x, y) => Math.Sign(x.Value.Frequency - y.Value.Frequency));

                var newFreq = freqNodesList[0].Value.Frequency + freqNodesList[1].Value.Frequency;
                var freqData = new FrequencyData(newFreq);

                var treeNode = new BinaryTreeNode<FrequencyData>(freqData, freqNodesList[0], freqNodesList[1]);

                freqNodesList.RemoveRange(0, 2);
                freqNodesList.Insert(0, treeNode);
            }

            var tree = new BinaryTree<FrequencyData>();
            tree.ReplaceRoot(freqNodesList.First());

            return tree;
        }

        private BitCodesTable SeekCodes(List<FrequencyData> freqList,
            BinaryTree<FrequencyData> tree)
        {
            var bitCodes = new BitCodesTable();

            var bitsStack = new Stack<bool>();
            var freqStack = new Stack<FrequencyData>();

            var bits = new Stack<bool>();
            tree.Search((value, actType) =>
            {
                switch (actType)
                {
                    case SearchAction.TurnLeft:
                        bits.Push(false);
                        break;

                    case SearchAction.TurnRight:
                        bits.Push(true);
                        break;

                    case SearchAction.RollBack:
                        if (bits.Count > 0)
                        {
                            if (value == null)
                            {
                                bits.Pop();
                            }
                            else
                            {
                                var curVal = (FrequencyData)value;
                                if (curVal.Byte != null)
                                {
                                    bitCodes.Add(new BitCode(curVal.Byte, bits.ToArray()));
                                }
                                bits.Pop();
                            }
                        }
                        break;
                }
            });
            return bitCodes;
        }

        private byte[] CompressBytes(byte[] data, BitCodesTable bitCodesTable)
        {
            var bits = new List<bool>();
            long dataLength = 0;
            foreach (byte b in data)
            {
                var bitCode = bitCodesTable.Match(b);
                for (int i = 0; i < bitCode.Count; i++)
                {
                    bits.Add(bitCode[i]);
                }
                dataLength++;
            }

            long length = (bits.Count / 8) + (bits.Count % 8 == 0 ? 0 : 1);
            var compressed = new byte[length + 8];

            var lengthBytes = BitConverter.GetBytes(dataLength);
            for (int index = 0; index < 8; index++)
            {
                compressed[index] = lengthBytes[index];
            }

            for (int i = 0; i < bits.Count; i++)
            {
                compressed[(i / 8) + 8] |= (byte)((bits[i] ? 1 : 0) << (i % 8));
            }

            return compressed;
        }

        private byte[] DecompressBytes(byte[] cData, int startIndex,
            long count, BitCodesTable table)
        {
            var data = new byte[count];
            long counter = 0;

            var bits = new List<bool>();
            for (int i = startIndex; i < cData.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bool value = (cData[i] & (1 << (j % 8))) > 0;
                    bits.Add(value);

                    var b = table.Match(new BitArray(bits.ToArray()));
                    if (b != null)
                    {
                        data[counter++] = (byte)b;
                        bits.Clear();
                    }

                    if (counter == count)
                    {
                        return data;
                    }
                }
            }

            throw new Exception("Файл поврежден.");
        }

        #if DEBUG
        private void VisualizeTree(BinaryTree<FrequencyData> tree, bool useChar = false)
        {
            var graph = new StringBuilder("digraph frequencies");
            graph.AppendLine(" {");

            var freqSet = new HashSet<object>();
            tree.InOrderBypass((current, left, right) =>
            {
                freqSet.Add(current);
                if (left != null)
                {
                    freqSet.Add(left);
                }
                if (right != null)
                {
                    freqSet.Add(right);
                }
            });

            foreach (var f in freqSet)
            {
                graph.Append("\t");
                graph.Append(f.GetHashCode());
                graph.Append(" [label=\"");
                graph.Append(ValueToString(f));
                graph.AppendLine("\"];");
            }

            string ValueToString(object nodeValue)
            {
                var value = (FrequencyData)nodeValue;
                if (value.Byte != null)
                {
                    string b = useChar ?
                        ((char)value.Byte).ToString() :
                        value.Byte.ToString();
                    return value.Frequency + " (" + b + ")";
                }
                return value.Frequency.ToString();
            }

            void AppendEdge(int first, int second, byte label)
            {
                graph.Append("\t");
                graph.Append(first);
                graph.Append(" -> ");
                graph.Append(second);
                graph.Append(" [label=");
                graph.Append(label);
                graph.AppendLine("];");
            }

            tree.InOrderBypass((currentValue, leftValue, rightValue) =>
            {
                if (leftValue != null)
                {
                    AppendEdge(currentValue.GetHashCode(), leftValue.GetHashCode(), 0);
                }
                if (rightValue != null)
                {
                    AppendEdge(currentValue.GetHashCode(), rightValue.GetHashCode(), 1);
                }
            });

            graph.AppendLine("}");
            File.WriteAllText("tree.gv", graph.ToString());

            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "viztree.bat";
            p.Start();
            p.WaitForExit();
        }
        #endif
    }
}
