using UnityEngine;
using SequenceSharp.ABI;
using System.Numerics;

public class CoderTest : MonoBehaviour
{
    private void Start()
    {
        AddressCoderTest();
        ArrayCoderTest();
        BooleanCoderTest();
        BytesCoderTest();
        IntCoderTest();
        StringCoderTest();
        TupleCoderTest();
    }


    void AddressCoderTest()
    {
        
        {
            //Param Account Address
            //Encode 
            string abi = @"{ name: 'sequence', type: 'address'} ";
            string parameter = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
            byte[] expected = SequenceCoder.HexStringToByteArray("0000000000000000000000008e3e38fe7367dd3b52d1e281e4e8400447c8d8b9");
            byte[] encoded = SequenceCoder.EncodeParameter(abi, parameter);
            Debug.Assert(expected == encoded);

            //Decode
            string decoded =(string) SequenceCoder.DecodeParameter(abi, encoded);
            Debug.Assert(decoded == parameter);

            
        }

        {
            //Param ContractS Address
            string abi = @"{ name: 'sequence', type: 'address'} ";
            string parameter = "0xfCFdE38A1EeaE0ee7e130BbF66e94844Bc5D5B6B";
            byte[] expected = SequenceCoder.HexStringToByteArray("000000000000000000000000fcfde38a1eeae0ee7e130bbf66e94844bc5d5b6b");
            byte[] encoded = SequenceCoder.EncodeParameter(abi, parameter);
            Debug.Assert(expected == encoded);

            //Decode
            string decoded = (string)SequenceCoder.DecodeParameter(abi, encoded);
            Debug.Assert(decoded == parameter);
        }

        //TODO Case: Address length surpasses the maximum allowed limit
        //TODO Case: Address length insufficient
        //TODO Case: Address

 

    }
    void ArrayCoderTest()
    {

        {
            //Encode 
            //Param Contract Address
            string abi = @"{ name: 'sequence', type: 'address'} ";
            string parameter = "0xfCFdE38A1EeaE0ee7e130BbF66e94844Bc5D5B6B";
            byte[] expected = SequenceCoder.HexStringToByteArray("0x0000000000000000000000000000000000000000000000000000000000000001");
            byte[] encoded = SequenceCoder.EncodeParameter(abi, parameter);
            Debug.Assert(expected == encoded);

            //Decode
            string decoded = (string)SequenceCoder.DecodeParameter(abi, encoded);
            Debug.Assert(decoded == parameter);
        }
    }

    void BooleanCoderTest()
    {
        Debug.Log("Boolean Coder Test:");

        {
            //Encode
            //Param True
            string abi = @"{ name: 'sequence', type: 'bool'} ";
            bool parameter = true;
            byte[] expected = SequenceCoder.HexStringToByteArray("0x0000000000000000000000000000000000000000000000000000000000000001");
            byte[] encoded = SequenceCoder.EncodeParameter(abi, parameter);
            Debug.Assert(expected == encoded);

            //Decode
            bool decoded = (bool)SequenceCoder.DecodeParameter(abi, encoded);
            Debug.Assert(decoded == parameter);
        }

        {
            //Param False
            string abi = @"{ name: 'sequence', type: 'bool'} ";
            bool parameter = false;
            byte[] expected = SequenceCoder.HexStringToByteArray("0x0000000000000000000000000000000000000000000000000000000000000000");
            byte[] encoded = SequenceCoder.EncodeParameter(abi, parameter);
            Debug.Assert(expected == encoded);

            //Decode
            bool decoded = (bool)SequenceCoder.DecodeParameter(abi, encoded);
            Debug.Assert(decoded == parameter);
        }

    }
    void BytesCoderTest()
    {
        //Encode
        string abi = @"{name: 'sequence', type: 'bytes'} ";
        byte[] parameter = SequenceCoder.HexStringToByteArray("0xaabbccdd"); //byte array as input
        byte[] expected = SequenceCoder.HexStringToByteArray("00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000004aabbccdd00000000000000000000000000000000000000000000000000000000");
        byte[] encoded = SequenceCoder.EncodeParameter(abi, parameter);
        Debug.Assert(expected == encoded);

        //Decode
        byte[] decoded = (byte[])SequenceCoder.DecodeParameter(abi, encoded);
        Debug.Assert(decoded == parameter);
    }

    void IntCoderTest()
    {
        {
            //Param Positive
            string abi = @"{name: 'sequence', type: 'uint8'} ";
            BigInteger parameter = 5;
            byte[] expected = SequenceCoder.HexStringToByteArray("0000000000000000000000000000000000000000000000000000000000000005");
            byte[] encoded = SequenceCoder.EncodeParameter(abi, parameter);
            Debug.Assert(expected == encoded);

            //Decode
            BigInteger decoded = (BigInteger)SequenceCoder.DecodeParameter(abi, encoded);
            Debug.Assert(decoded == parameter);
        }

        {
            //Param Positive
            string abi = @"{name: 'sequence', type: 'uint8'} ";
            BigInteger parameter = -5;
            byte[] expected = SequenceCoder.HexStringToByteArray("fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffb");
            byte[] encoded = SequenceCoder.EncodeParameter(abi, parameter);
            Debug.Assert(expected == encoded);

            //Decode
            BigInteger decoded = (BigInteger)SequenceCoder.DecodeParameter(abi, encoded);
            Debug.Assert(decoded == parameter);
        }
        //TODO Case: distinguish between unit8, unit32, unit40 and unit256
    }

    void StringCoderTest()
    {
        {
            //Param string
            string abi = @"{name: 'sequence', type: 'string'} ";
            string parameter = "sequence";
            byte[] expected = SequenceCoder.HexStringToByteArray("0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000873657175656e6365000000000000000000000000000000000000000000000000");
            byte[] encoded = SequenceCoder.EncodeParameter(abi, parameter);
            Debug.Assert(expected == encoded);

            //Decode
            string decoded = (string)SequenceCoder.DecodeParameter(abi, encoded);
            Debug.Assert(decoded == parameter);
        }
    }

    void TupleCoderTest()
    {

    }



}
