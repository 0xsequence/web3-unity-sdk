using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;

namespace SequenceSharp.ABI
{
    /// <summary>
    /// https://docs.soliditylang.org/en/v0.8.13/abi-spec.html
    /// </summary>
    public enum ABIType
    {
        TUPLE,
        FIXEDARRAY,
        DYNAMICARRAY,
        BYTES,
        STRING,
        NUMBER,
        ADDRESS,
        BOOLEAN,
        NONE

    }
    public class SequenceCoder
    {
        /// <summary>
        /// https://docs.soliditylang.org/en/v0.8.13/abi-spec.html
        /// For any ABI value X, we recursively define enc(X), depending on the type of X being different types
        /// </summary>
        /// <param name="abi"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static byte[] EncodeParameter(string abi, object parameter)
        {
            //Switch

            return new byte[] { };
        }

        public static object DecodeParameter(string abi, byte[] encoded)
        {
            return new object{ };
        }

        //https://github.com/ethereum/EIPs/blob/master/EIPS/eip-55.md
        public static string AddressChecksum(string address)
        {

            return "";
        }


        // Hex string to byte array and vice versa
        // Ref:
        //https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa

        public static byte[] HexStringToByteArray(string hexString)
        {

            if (hexString.StartsWith("0x"))
            {
                hexString = hexString.Remove(0, 2);
            }

            byte firstByte = Convert.ToByte(hexString.Substring(0, 2), 16);
            int firstInt = Convert.ToInt32(firstByte);
            if (firstInt < 0)
            {
                int numberChars = hexString.Length;
                byte[] bytes = new byte[numberChars / 2];
                int curr = 0;
                for (int i = 0; i < numberChars-1; i += 2)
                {
                    curr = Convert.ToInt32(hexString.Substring(i, 2), 16);
                    bytes[i / 2] = Convert.ToByte(~curr);
                }
                curr = Convert.ToInt32(hexString.Substring(numberChars - 1, 2), 16);
                bytes[(numberChars - 1) / 2] = Convert.ToByte(~curr + 1);
                return bytes;
            }
            else
            {
                int numberChars = hexString.Length;
                byte[] bytes = new byte[numberChars / 2];
                for (int i = 0; i < numberChars; i += 2)
                    bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
                return bytes;
            }
            
        }
         
        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();

        }

        public static ABIType GetParameterType(object param)
        {
            //Start with static type
            //Boolean, Integer, Unsigned integer, Address
            if (param.GetType() == typeof(bool))
            {
                return ABIType.BOOLEAN;
            }
            else if (param.GetType() == typeof(BigInteger)||param.GetType() == typeof(int)|| param.GetType() == typeof(uint))
            {
                return ABIType.NUMBER;
            }
            else if (param.GetType() == typeof(string))
            {
                //TODO: make address a custom type  
                if(((string)param).Length == 42 && ((string)param).StartsWith("0x"))
                {
                    return ABIType.ADDRESS;
                }
                return ABIType.STRING;
            }
            else if (param is System.Runtime.CompilerServices.ITuple)
            {
                
                return ABIType.TUPLE;
            }
            else
            {
                IEnumerable paramEnumerable = param as IEnumerable;
                if (paramEnumerable != null)
                {
                    //IEnumerable types
                    var type = param.GetType();
                    string name = type.Name;
                    //Support ArrayList, List and Array (Bytes is considered byte array) as of now
                    if (param is IList &&
           param.GetType().IsGenericType &&
           param.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
                    {
                        return ABIType.DYNAMICARRAY;
                    }
                    else if (param.GetType().IsArray)
                    {
                        foreach(var item in paramEnumerable)
                        {
                            if (item.GetType() == typeof(System.Byte))
                                return ABIType.BYTES;
                            break;
                        }
                        return ABIType.FIXEDARRAY;
                    }

                }

                return ABIType.NONE;
                
            }
        }

        public static bool IsStaticType(ABIType paramType)
        {
            //Boolean, Integer, Unsigned integer, Address
            if ((paramType == ABIType.BOOLEAN) || (paramType == ABIType.NUMBER) || (paramType == ABIType.ADDRESS))
                return true;
            return false;
        }


        public static bool IsDynamicType(ABIType paramType)
        {
            /*
             Definition: The following types are called “dynamic”:

                bytes

                string

                T[] for any T

                T[k] for any dynamic T and any k >= 0

                (T1,...,Tk) if Ti is dynamic for some 1 <= i <= k
            */

            if ((paramType == ABIType.BYTES) || (paramType == ABIType.STRING) || (paramType == ABIType.DYNAMICARRAY) || (paramType == ABIType.FIXEDARRAY) || (paramType == ABIType.TUPLE))
                return true;
            return false;
        }
    }
}