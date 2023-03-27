using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SequenceSharp.ABI;
using System.Linq;
using System.Numerics;

public class CoderTests
{
    AddressCoder _addressCoder = new AddressCoder();
    ArrayCoder _arrayCoder = new ArrayCoder();
    BooleanCoder _booleanCoder = new BooleanCoder();
    BytesCoder _bytesCoder = new BytesCoder();
    //FixedBytesCoder _fixedBytesCoder = new FixedBytesCoder();
    NumberCoder _numberCoder = new NumberCoder();
    StringCoder _stringCoder = new StringCoder();


    [Test]
    public void AddressCoderTest()
    {

        {
            //Param Account Address
            //Encode 
            string parameter = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
            byte[] expected = SequenceCoder.HexStringToByteArray("0000000000000000000000008e3e38fe7367dd3b52d1e281e4e8400447c8d8b9");
            byte[] encoded = _addressCoder.Encode(parameter);
            CollectionAssert.AreEqual(expected, encoded);

            //Decode
            string decoded = (string)_addressCoder.Decode(encoded);
            CollectionAssert.AreEqual(decoded, parameter);


        }

        {
            //Param ContractS Address
            string parameter = "0xfCFdE38A1EeaE0ee7e130BbF66e94844Bc5D5B6B";
            byte[] expected = SequenceCoder.HexStringToByteArray("000000000000000000000000fcfde38a1eeae0ee7e130bbf66e94844bc5d5b6b");
            byte[] encoded = _addressCoder.Encode(parameter);
            CollectionAssert.AreEqual(expected, encoded);

            //Decode
            string decoded = (string)_addressCoder.Decode(encoded);
            CollectionAssert.AreEqual(decoded, parameter);
        }

        //TODO Case: Address length surpasses the maximum allowed limit
        //TODO Case: Address length insufficient
        //TODO Case: Address



    }

    [Test]
    public void ArrayCoderTest()
    {

        {
            //Encode 

            List<BigInteger> parameter = new List<BigInteger> { 1, 2, 3 };
            byte[] expected = SequenceCoder.HexStringToByteArray("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000003");

            byte[] encoded = _arrayCoder.Encode(parameter);
            CollectionAssert.AreEqual(expected, encoded);

            //Decode
            List<object> types = new List<object> { new List<object> { ABIType.NUMBER, ABIType.NUMBER, ABIType.NUMBER } };
            List<object> decodedRaw = _arrayCoder.Decode(encoded, types);
            List<object> decodedBigInt = (List<object>)(decodedRaw[0]);
            List<BigInteger> decoded = new List<BigInteger>();

            foreach (BigInteger d in decodedBigInt)
            {
                decoded.Add(d);
            }
            CollectionAssert.AreEqual(decoded, parameter);
        }
    }


    [Test]
    public void BooleanCoderTest()
    {


        {
            //Encode
            //Param True
            bool parameter = true;
            byte[] expected = SequenceCoder.HexStringToByteArray("0x0000000000000000000000000000000000000000000000000000000000000001");
            byte[] encoded = _booleanCoder.Encode(parameter);
            CollectionAssert.AreEqual(expected, encoded);

            //Decode
            bool decoded = (bool)_booleanCoder.Decode(encoded);
            Assert.AreEqual(decoded, parameter);
        }

        {
            //Param False
            bool parameter = false;
            byte[] expected = SequenceCoder.HexStringToByteArray("0x0000000000000000000000000000000000000000000000000000000000000000");
            byte[] encoded = _booleanCoder.Encode(parameter);
            CollectionAssert.AreEqual(expected, encoded);

            //Decode
            bool decoded = (bool)_booleanCoder.Decode(encoded);
            Assert.AreEqual(decoded, parameter);
        }

    }

    [Test]
    public void BytesCoderTest()
    {
        //Encode
        byte[] parameter = SequenceCoder.HexStringToByteArray("0xaabbccdd"); //byte array as input
        byte[] expected = SequenceCoder.HexStringToByteArray("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000004aabbccdd00000000000000000000000000000000000000000000000000000000");

        byte[] encoded = _bytesCoder.Encode(parameter);

        CollectionAssert.AreEqual(expected, encoded);

        //Decode
        byte[] decoded = (byte[])_bytesCoder.Decode(encoded);

        CollectionAssert.AreEqual(decoded, parameter);
    }


    [Test]
    public void IntCoderTest()
    {
        {
            //Param Positive
            BigInteger parameter = 5;
            byte[] expected = SequenceCoder.HexStringToByteArray("0000000000000000000000000000000000000000000000000000000000000005");
            byte[] encoded = _numberCoder.Encode(parameter);
            CollectionAssert.AreEqual(expected, encoded);


            //Decode
            //BigInteger decoded = (BigInteger)_numberCoder.Decode(encoded);
            BigInteger decoded = (BigInteger)_numberCoder.Decode(encoded);
            Assert.AreEqual(decoded, parameter);
        }

        {
            //Param Positive
            BigInteger parameter = -5;
            byte[] expected = SequenceCoder.HexStringToByteArray("fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffb");
            byte[] encoded = _numberCoder.Encode(parameter);
            CollectionAssert.AreEqual(expected, encoded);

            //Decode
            BigInteger decoded = (BigInteger)_numberCoder.Decode(encoded);
            Assert.AreEqual(decoded, parameter);
        }
        //TODO Case: unit8, unit32, unit40 and unit256
    }

    [Test]
    public void StringCoderTest()
    {
        {
            //Param string
            string parameter = "sequence";
            byte[] expected = SequenceCoder.HexStringToByteArray("0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000873657175656e6365000000000000000000000000000000000000000000000000");
            byte[] encoded = _stringCoder.Encode(parameter);
            CollectionAssert.AreEqual(expected, encoded);

            //Decode
            string decoded = (string)_stringCoder.Decode(encoded);
            Assert.AreEqual(decoded, parameter);
        }
    }


}
