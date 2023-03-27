using UnityEngine;
using SequenceSharp.ABI;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Text;


public class CoderTest : MonoBehaviour
{
    AddressCoder _addressCoder = new AddressCoder();
    ArrayCoder _arrayCoder = new ArrayCoder();
    BooleanCoder _booleanCoder = new BooleanCoder();
    BytesCoder _bytesCoder = new BytesCoder();
    //FixedBytesCoder _fixedBytesCoder = new FixedBytesCoder();
    NumberCoder _numberCoder = new NumberCoder();
    StringCoder _stringCoder = new StringCoder();
    private void Start()
    {
        //AddressCoderTest();//TODO: Checksum
        ArrayCoderTest();
        BooleanCoderTest();
        BytesCoderTest();
        IntCoderTest();
        StringCoderTest();
        TupleCoderTest();


    }

    void CheckSumTest()
    {

    }


    void AddressCoderTest()
    {
        
        {
            //Param Account Address
            //Encode 
            string parameter = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
            byte[] expected = SequenceCoder.HexStringToByteArray("0000000000000000000000008e3e38fe7367dd3b52d1e281e4e8400447c8d8b9");
            byte[] encoded = _addressCoder.Encode(parameter);
            Debug.Assert(expected.SequenceEqual(encoded));

            //Decode
            string decoded = (string)_addressCoder.Decode(encoded);
            string decodedWithCheckSum = SequenceCoder.AddressChecksum(decoded);
            Debug.Log(decoded);
            Debug.Assert(decodedWithCheckSum == parameter);

            
        }

        {
            //Param ContractS Address
            string parameter = "0xfCFdE38A1EeaE0ee7e130BbF66e94844Bc5D5B6B";
            byte[] expected = SequenceCoder.HexStringToByteArray("000000000000000000000000fcfde38a1eeae0ee7e130bbf66e94844bc5d5b6b");
            byte[] encoded = _addressCoder.Encode(parameter);
            Debug.Assert(expected.SequenceEqual(encoded));

            //Decode
            string decoded = (string)_addressCoder.Decode(encoded);
            string decodedWithCheckSum = SequenceCoder.AddressChecksum(decoded);
            Debug.Log(decoded);
            Debug.Assert(decodedWithCheckSum == parameter);
        }

        //TODO Case: Address length surpasses the maximum allowed limit
        //TODO Case: Address length insufficient
        //TODO Case: Address

 

    }
    void ArrayCoderTest()
    {

        {
            //Encode 

            List<BigInteger> parameter = new List<BigInteger> { 1, 2, 3 };
            byte[] expected = SequenceCoder.HexStringToByteArray("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000003");

            byte[] encoded = _arrayCoder.Encode(parameter);
            Debug.Assert(expected.SequenceEqual(encoded));

            //Decode
            List<object> types = new List<object> { new List<object> { ABIType.NUMBER, ABIType.NUMBER, ABIType.NUMBER } };
            List<object> decodedRaw = _arrayCoder.Decode(encoded,types);
            List<object> decodedBigInt = (List<object>)(decodedRaw[0]);
            List<BigInteger> decoded = new List<BigInteger>();

            foreach (BigInteger d in decodedBigInt)
            {
                decoded.Add(d);
            }
            Debug.Assert((decoded).SequenceEqual(parameter));
        }
    }

    void BooleanCoderTest()
    {


        {
            //Encode
            //Param True
            bool parameter = true;
            byte[] expected = SequenceCoder.HexStringToByteArray("0x0000000000000000000000000000000000000000000000000000000000000001");
            byte[] encoded = _booleanCoder.Encode(parameter);
            Debug.Assert(expected.SequenceEqual(encoded));

            //Decode
            bool decoded = (bool)_booleanCoder.Decode(encoded);
            Debug.Assert(decoded == parameter);
        }

        {
            //Param False
            bool parameter = false;
            byte[] expected = SequenceCoder.HexStringToByteArray("0x0000000000000000000000000000000000000000000000000000000000000000");
            byte[] encoded = _booleanCoder.Encode(parameter);
            Debug.Assert(expected.SequenceEqual(encoded));

            //Decode
            bool decoded = (bool)_booleanCoder.Decode(encoded);
            Debug.Assert(decoded == parameter);
        }

    }
    void BytesCoderTest()
    {
        //Encode
        byte[] parameter = SequenceCoder.HexStringToByteArray("0xaabbccdd"); //byte array as input
        byte[] expected = SequenceCoder.HexStringToByteArray("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000004aabbccdd00000000000000000000000000000000000000000000000000000000");

        byte[] encoded = _bytesCoder.Encode(parameter);

        Debug.Assert(expected.SequenceEqual(encoded));

        //Decode
        byte[] decoded = (byte[])_bytesCoder.Decode(encoded);
        
        Debug.Assert(decoded.SequenceEqual(parameter));
    }

    void IntCoderTest()
    {
        {
            //Param Positive
            BigInteger parameter = 5;
            byte[] expected = SequenceCoder.HexStringToByteArray("0000000000000000000000000000000000000000000000000000000000000005");
            byte[] encoded = _numberCoder.Encode(parameter);
            Debug.Assert(expected.SequenceEqual(encoded));


            //Decode
            //BigInteger decoded = (BigInteger)_numberCoder.Decode(encoded);
            BigInteger decoded = (BigInteger)_numberCoder.Decode(encoded);
            Debug.Assert(decoded == parameter);
        }

        {
            //Param Positive
            BigInteger parameter = -5;
            byte[] expected = SequenceCoder.HexStringToByteArray("fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffb");
            byte[] encoded = _numberCoder.Encode(parameter);
            Debug.Assert(expected.SequenceEqual(encoded));

            //Decode
            BigInteger decoded = (BigInteger)_numberCoder.Decode(encoded);
            Debug.Assert(decoded == parameter);
        }
        //TODO Case: unit8, unit32, unit40 and unit256
    }

    void StringCoderTest()
    {
        {
            //Param string
            string parameter = "sequence";
            byte[] expected = SequenceCoder.HexStringToByteArray("0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000873657175656e6365000000000000000000000000000000000000000000000000");
            byte[] encoded = _stringCoder.Encode(parameter);
            Debug.Assert(expected.SequenceEqual(encoded));

            //Decode
            string decoded = (string)_stringCoder.Decode(encoded);
            Debug.Log(decoded);
            Debug.Assert(decoded == parameter);
        }
    }

    void TupleCoderTest()
    {

    }



}
