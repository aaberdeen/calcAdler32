using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calcAdler32
{
    class Program
    {
        public static byte[] PortArray = new byte[200];
        public static int  PortArrayCount;
        static void Main(string[] args)
        {      
            //string frame = "10023F4D00000000003F000006160D001D9F2E00008D1500249F2E00008D150000000000000000000000000000000000000000000000FFC9000000070000A405E9AE"; //good
            string frame = "10023F3400000000003F000005110D002D9F2E00008D1500249F2E00008D150000000000000000000000000000000000000000000000FFCA0000000700009605E6AA";
            
            PortArray = StringToByteArrayFastest(frame);
            
            int PktLengthInt = PortArray[2] + 2;
            uint stripCount = DleStrip();
            uint chkSum = calcAdler32(3, (uint)PktLengthInt - 4 - 2 - stripCount);

            string chksum = string.Format( "{0:X2}", chkSum);
            byte[] chkSumByte = StringToByteArrayFastest(chksum);

            for (int i = 0; i < chkSumByte.Length; i++)
            {
                Console.Write("{0:X2}, ", chkSumByte[chkSumByte.Length -1 -i]);
            }
            Console.ReadLine();
        }



        private static uint calcAdler32(int startPoint, uint length)
        {
            uint ckSumCalc;
            //adler32 check sum
            {
                uint a = 1, b = 0;
                int index;
                const uint MOD_ADLER = 65521;

                /* Process each byte of the data in order */
                for (index = startPoint; index < (startPoint + length); ++index)
                {
                    a = (a + (uint)PortArray[index]) % MOD_ADLER;
                    b = (b + a) % MOD_ADLER;
                }

                ckSumCalc = (b << 16) | a;   // needs work !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }
            return ckSumCalc;
        }
        private static uint DleStrip()
        {
            uint count = 0;
            const int DLE = 0x10;

            for (int i = 1; i <= (PortArrayCount - 1); i++)
            {
                if (PortArray[i] == DLE)
                {
                    if (PortArray[i - 1] == DLE)   //so double DLE
                    {
                        // shift array left form this point
                        for (int j = i; j <= PortArrayCount - 1; j++)
                        {
                            PortArray[j] = PortArray[j + 1];

                        }
                        count++;
                    }


                }
            }
            return count;

        }

        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            PortArrayCount =hex.Length >> 1;
            byte[] arr = new byte[hex.Length >> 1];
           

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}
