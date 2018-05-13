using System;
using System.Collections;

namespace Lab3
{
    class DES
    {
        private byte[] IP =
        {
            58, 50, 42, 34, 26, 18, 10, 02,
            60, 52, 44, 36, 28, 20, 12, 04,
            62, 54, 46, 38, 30, 22, 14, 06,
            64, 56, 48, 40, 32, 24, 16, 08,
            57, 49, 41, 33, 25, 17, 09, 01,
            59, 51, 43, 35, 27, 19, 11, 03,
            61, 53, 45, 37, 29, 21, 13, 05,
            63, 55, 47, 39, 31, 23, 15, 07
        };

        private byte[] IP_INV =
        {
            40, 08, 48, 16, 56, 24, 64, 32,
            39, 07, 47, 15, 55, 23, 63, 31,
            38, 06, 46, 14, 54, 22, 62, 30,
            37, 05, 45, 13, 53, 21, 61, 29,
            36, 04, 44, 12, 52, 20, 60, 28,
            35, 03, 43, 11, 51, 19, 59, 27,
            34, 02, 42, 10, 50, 18, 58, 26,
            33, 01, 41, 09, 49, 17, 57, 25
        };

        private byte[] EXP =
        {
            32, 01, 02, 03, 04, 05,
            04, 05, 06, 07, 08, 09,
            08, 09, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32, 01
        };

        private byte[,,] S =
        {
            {
                { 14, 04, 13, 01, 02, 15, 11, 08, 03, 10, 06, 12, 05, 09, 00, 07 },
                { 00, 15, 07, 04, 14, 02, 13, 01, 10, 06, 12, 11, 09, 05, 03, 08 },
                { 04, 01, 14, 08, 13, 06, 02, 11, 15, 12, 09, 07, 03, 10, 05, 00 },
                { 15, 12, 08, 02, 04, 09, 01, 07, 05, 11, 03, 14, 10, 00, 06, 13 }
            },
            {
                { 15, 01, 08, 14, 06, 11, 03, 04, 09, 07, 02, 13, 12, 00, 05, 10 },
                { 03, 13, 04, 07, 15, 02, 08, 14, 12, 00, 01, 10, 06, 09, 11, 05 },
                { 00, 14, 07, 11, 10, 04, 13, 01, 05, 08, 12, 06, 09, 03, 02, 15 },
                { 13, 08, 10, 01, 03, 15, 04, 02, 11, 06, 07, 12, 00, 05, 14, 09 }
            },
            {
                { 10, 00, 09, 14, 06, 03, 15, 05, 01, 13, 12, 07, 11, 04, 02, 08 },
                { 13, 07, 00, 09, 03, 04, 06, 10, 02, 08, 05, 14, 12, 11, 15, 01 },
                { 13, 06, 04, 09, 08, 15, 03, 00, 11, 01, 02, 12, 05, 10, 14, 07 },
                { 01, 10, 13, 00, 06, 09, 08, 07, 04, 15, 14, 03, 11, 05, 02, 12 }
            },
            {
                { 07, 13, 14, 03, 00, 06, 09, 10, 01, 02, 08, 05, 11, 12, 04, 15 },
                { 13, 08, 11, 05, 06, 15, 00, 03, 04, 07, 02, 12, 01, 10, 14, 09 },
                { 10, 06, 09, 00, 12, 11, 07, 13, 15, 01, 03, 14, 05, 02, 08, 04 },
                { 03, 15, 00, 06, 10, 01, 13, 08, 09, 04, 05, 11, 12, 07, 02, 14 }
            },
            {
                { 02, 12, 04, 01, 07, 10, 11, 06, 08, 05, 03, 15, 13, 00, 14, 09 },
                { 14, 11, 02, 12, 04, 07, 13, 01, 05, 00, 15, 10, 03, 09, 08, 06 },
                { 04, 02, 01, 11, 10, 13, 07, 08, 15, 09, 12, 05, 06, 03, 00, 14 },
                { 11, 08, 12, 07, 01, 14, 02, 13, 06, 15, 00, 09, 10, 04, 05, 03 }
            },
            {
                { 12, 01, 10, 15, 09, 02, 06, 08, 00, 13, 03, 04, 14, 07, 05, 11 },
                { 10, 15, 04, 02, 07, 12, 09, 05, 06, 01, 13, 14, 00, 11, 03, 08 },
                { 09, 14, 15, 05, 02, 08, 12, 03, 07, 00, 04, 10, 01, 13, 11, 06 },
                { 04, 03, 02, 12, 09, 05, 15, 10, 11, 14, 01, 07, 06, 00, 08, 13 }
            },
            {
                { 04, 11, 02, 14, 15, 00, 08, 13, 03, 12, 09, 07, 05, 10, 06, 01 },
                { 13, 00, 11, 07, 04, 09, 01, 10, 14, 03, 05, 12, 02, 15, 08, 06 },
                { 01, 04, 11, 13, 12, 03, 07, 14, 10, 15, 06, 08, 00, 05, 09, 02 },
                { 06, 11, 13, 08, 01, 04, 10, 07, 09, 05, 00, 15, 14, 02, 03, 12 }
            },
            {
                { 13, 02, 08, 04, 06, 15, 11, 01, 10, 09, 03, 14, 05, 00, 12, 07 },
                { 01, 15, 13, 08, 10, 03, 07, 04, 12, 05, 06, 11, 00, 14, 09, 02 },
                { 07, 11, 04, 01, 09, 12, 14, 02, 00, 06, 10, 13, 15, 03, 05, 08 },
                { 02, 01, 14, 07, 04, 10, 08, 13, 15, 12, 09, 00, 03, 05, 06, 11 }
            }
        };

        private byte[] P =
        {
            16, 07, 20, 21, 29, 12, 28, 17,
            01, 15, 23, 26, 05, 18, 31, 10,
            02, 08, 24, 14, 32, 27, 03, 09,
            19, 13, 30, 06, 22, 11, 04, 25
        };

        private byte[] G =
        {
            57, 49, 41, 33, 25, 17, 09,
            01, 58, 50, 42, 34, 26, 18,
            10, 02, 59, 51, 43, 35, 27,
            19, 11, 03, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15,
            07, 62, 54, 46, 38, 30, 22,
            14, 06, 61, 53, 45, 37, 29,
            21, 13, 05, 28, 20, 12, 04
        };

        private byte[] H =
        {
            14, 17, 11, 24, 01, 05, 03, 28,
            15, 06, 21, 10, 23, 19, 12, 04,
            26, 08, 16, 07, 27, 20, 13, 02,
            41, 52, 31, 37, 47, 55, 30, 40,
            51, 45, 33, 48, 44, 49, 39, 56,
            34, 53, 46, 42, 50, 36, 29, 32
        };

        private byte[] SHL_NUMS =
        {
            1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
        };

        public DES()
        {
            for (int i = 0; i < IP.Length; i++)
            {
                IP[i]--;
                IP_INV[i]--;
            }

            for (int i = 0; i < EXP.Length; i++)
            {
                EXP[i]--;
            }

            for (int i = 0; i < P.Length; i++)
            {
                P[i]--;
            }

            for (int i = 0; i < G.Length; i++)
            {
                G[i]--;
            }

            for (int i = 0; i < H.Length; i++)
            {
                H[i]--;
            }
        }

        public byte[] EncryptData(byte[] data, byte[] key)
        {
            int n = data.Length % 8;

            byte[] newData = new byte[data.Length];

            int index = 0;
            for (int i = 0; i < n; i++)
            {
                byte[] block = new byte[8];
                for (int j = 0; j < 8; j++)
                {
                    block[j] = data[i * 8 + j];
                }
                block = EncryptDataBlock(block, key);
                for (int j = 0; j < 8; j++)
                {
                    newData[index++] = block[j];
                }
            }
            
            for ( ; index < data.Length; index++)
            {
                newData[index] = data[index];
            }

            return newData;
        }

        public byte[] DecryptData(byte[] data, byte[] key)
        {
            int n = data.Length % 8;

            byte[] newData = new byte[data.Length];

            int index = 0;
            for (int i = 0; i < n; i++)
            {
                byte[] block = new byte[8];
                for (int j = 0; j < 8; j++)
                {
                    block[j] = data[i * 8 + j];
                }
                block = DecryptDataBlock(block, key);
                for (int j = 0; j < 8; j++)
                {
                    newData[index++] = block[j];
                }
            }

            for (; index < data.Length; index++)
            {
                newData[index] = data[index];
            }

            return newData;
        }

        public byte[] EncryptDataBlock(byte[] data, byte[] key)
        {
            if (data.Length != 8 || key.Length != 8)
            {
                throw new ArgumentException();
            }

            BitArray dataBits = new BitArray(data);
            (BitArray left, BitArray right) = Divide(dataBits);

            BitArray[] keys = MakeKeys(key);

            for (int i = 0; i < 16; i++)
            {

                (left, right) = Round(left, right, keys[i]);
            }

            dataBits = Concat(left, right);

            byte[] result = new byte[data.Length];
            dataBits.CopyTo(result, 0);
            return result;
        }

        public byte[] DecryptDataBlock(byte[] data, byte[] key)
        {
            if (data.Length != 8 || key.Length != 8)
            {
                throw new ArgumentException();
            }

            BitArray dataBits = new BitArray(data);
            (BitArray left, BitArray right) = Divide(dataBits);

            BitArray[] keys = MakeKeys(key);

            for (int i = 0; i < 16; i++)
            {

                (left, right) = RoundRev(left, right, keys[15 - i]);
            }

            dataBits = Concat(left, right);

            byte[] result = new byte[data.Length];
            dataBits.CopyTo(result, 0);
            return result;
        }

        private BitArray[] MakeKeys(byte[] key)
        {
            BitArray keyBits = ApplyG(new BitArray(key));

            BitArray[] keys = new BitArray[16];
            for (int i = 0; i < 16; i++)
            {
                (BitArray keyLeft, BitArray keyRight) = Divide(keyBits);

                keyLeft = ShlKey(i, keyLeft);
                keyRight = ShlKey(i, keyRight);

                keys[i] = ShlKey(i, ApplyH(Concat(keyLeft, keyRight)));
            }

            return keys;
        }

        private (BitArray, BitArray) Divide(BitArray bits)
        {
            int h = bits.Length / 2;
            BitArray left = new BitArray(h);
            BitArray right = new BitArray(h);
            for (int i = 0; i < h; i++)
            {
                left[i] = bits[i];
                right[i] = bits[i + h];
            }
            return (left, right);
        }

        private BitArray Concat(BitArray left, BitArray right)
        {
            BitArray bits = new BitArray(left.Length + right.Length);
            for (int i = 0; i < left.Length; i++)
            {
                bits[i] = left[i];
                bits[i + left.Length] = right[i];
            }
            return bits;
        }

        private (BitArray, BitArray) Round(BitArray left, BitArray right, BitArray key)
        {
            BitArray newLeft = new BitArray(right);
            BitArray newRight = left.Xor(FeistelFunction(right, key));
            return (newLeft, newRight);
        }

        private (BitArray, BitArray) RoundRev(BitArray left, BitArray right, BitArray key)
        {
            BitArray newRight = new BitArray(left);
            BitArray newLeft = right.Xor(FeistelFunction(left, key));
            return (newLeft, newRight);
        }

        private BitArray FeistelFunction(BitArray dataPart, BitArray key)
        {
            BitArray dataBits = Expand(dataPart);
            dataBits.Xor(key);
            return ApplyP(ApplyS(dataBits));
        }

        private BitArray Expand(BitArray dataPart)
        {
            BitArray newBits = new BitArray(EXP.Length);
            for (int i = 0; i < EXP.Length; i++)
            {
                newBits[i] = dataPart[EXP[i]];
            }
            return newBits;
        }

        public BitArray ApplyS(BitArray bits)
        {
            BitArray result = new BitArray(32);
            for (int i = 0; i < 8; i++)
            {
                BitArray b = new BitArray(6);
                for (int j = i * 6, index = 0; index < 6; j++, index++)
                {
                    b[index] = bits[j];
                }
                b = ApplySToBlock(i, b);
                for (int j = i * 4, index = 0; index < 4; j++, index++)
                {
                    result[j] = b[index];
                }
            }
            return result;
        }

        private BitArray ApplySToBlock(int index, BitArray bits)
        {
            int row = (bits[0] ? 1 : 0) << 1 | (bits[5] ? 1 : 0);
            int col = (bits[1] ? 1 : 0) << 3 | (bits[2] ? 1 : 0) << 2 |
                (bits[3] ? 1 : 0) << 1 | (bits[4] ? 1 : 0);
            byte value = S[index, row, col];
            BitArray result = new BitArray(4);
            result[0] = (value & 0b0001) > 0;
            result[1] = (value & 0b0010) > 0;
            result[2] = (value & 0b0100) > 0;
            result[3] = (value & 0b1000) > 0;
            return result;
        }

        private BitArray ApplyP(BitArray bits)
        {
            BitArray newBits = new BitArray(P.Length);
            for (int i = 0; i < P.Length; i++)
            {
                newBits[i] = bits[P[i]];
            }
            return newBits;
        }

        private BitArray ApplyG(BitArray keyBits)
        {
            BitArray newBits = new BitArray(G.Length);
            for (int i = 0; i < G.Length; i++)
            {
                newBits[i] = keyBits[G[i]];
            }
            return newBits;
        }

        private BitArray ApplyH(BitArray keyBits)
        {
            BitArray newBits = new BitArray(H.Length);
            for (int i = 0; i < H.Length; i++)
            {
                newBits[i] = keyBits[H[i]];
            }
            return newBits;
        }

        private BitArray ShlKey(int iterNum, BitArray key)
        {
            BitArray keyBits = new BitArray(key);
            byte shlNum = SHL_NUMS[iterNum];

            var bit0 = keyBits[0];
            if (shlNum == 1)
            {
                var bit1 = keyBits[1];
                for (int i = 3; i < keyBits.Length; i++)
                {
                    keyBits[i - 3] = keyBits[i - 1];
                    keyBits[i - 2] = keyBits[i];
                }
                keyBits[keyBits.Length - 2] = bit1;
            }
            else
            {
                for (int i = 1; i < keyBits.Length; i++)
                {
                    keyBits[i - 1] = keyBits[i];
                }
            }
            keyBits[keyBits.Length - 1] = bit0;

            return keyBits;
        }
    }
}
