using System;
using System.Globalization;
using System.Text;

namespace SequenceSharp.ABI
{
    public class SequenceCoder
    {
        public static byte[] EncodeParameter(string abi, object parameter)
        {
            //Switch

            return new byte[] { };
        }

        public static object DecodeParameter(string abi, byte[] encoded)
        {
            return new object{ };
        }


        // TODO : Hex string to byte array and vice versa
        //https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa

        public static byte[] HexStringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}