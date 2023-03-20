using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SequenceSharp.ABI
{
    /// <summary>
    /// (T1,...,Tk) for k >= 0 and any types T1, …, Tk
    ///enc(X) = head(X(1)) ... head(X(k)) tail(X(1)) ... tail(X(k))
    ///where X = (X(1), ..., X(k)) and head and tail are defined for Ti as follows:
    ///if Ti is static:
    ///head(X(i)) = enc(X(i)) and tail(X(i)) = "" (the empty string)
    ///otherwise, i.e. if Ti is dynamic:
    ///head(X(i)) = enc(len(head(X(1)) ... head(X(k)) tail(X(1)) ... tail(X(i-1)) )) tail(X(i)) = enc(X(i))

    ///Note that in the dynamic case, head(X(i)) is well-defined since the lengths of the head parts only depend on the types and not the values.The value of head(X(i)) is the offset of the beginning of tail(X(i)) relative to the start of enc(X).
    /// 
    /// </summary>
    public class TupleCoder : ICoder
    {
        //TODO: mmmmmmm,  will refactor these coders, maybe put them in sequencecoders and make some static methods to call
        AddressCoder _addressCoder = new AddressCoder();
        //ArrayCoder _arrayCoder = new ArrayCoder();
        BooleanCoder _booleanCoder = new BooleanCoder();
        FixedBytesCoder _fixedBytesCoder = new FixedBytesCoder();
        BytesCoder _bytesCoder = new BytesCoder();
        IntCoder _intCoder = new IntCoder();
        StringCoder _stringCoder = new StringCoder();

        public object Decode(byte[] encoded)
        {
            throw new System.NotImplementedException();
        }

        public T DefaultValue<T>()
        {
            throw new System.NotImplementedException();
        }


        public byte[] Encode(object value)
        {
            string encodedStr = EncodeToString(value);

            return SequenceCoder.HexStringToByteArray(encodedStr);

        }



        public string EncodeToString(object value)
        {

            List<object> valueTuple = (List<object>)value;

            int tupleLength = valueTuple.Count;
            int headerTotalByteLength = tupleLength * 32;
            List<string> headList = new List<string>();
            List<string> tailList = new List<string>();
            int tailLength = 0;
            for (int i = 0; i < tupleLength; i++)
            {
                string head_i = "", tail_i = "";
                ABIType type = SequenceCoder.GetParameterType(valueTuple[i]);


                switch (type)
                {
                    //Statics: head(X(i)) = enc(X(i) and tail(X(i)) = "" (the empty string)
                    case ABIType.BOOLEAN:
                        head_i = _booleanCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.NUMBER:
                        head_i = _intCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.ADDRESS:
                        head_i = _addressCoder.EncodeToString(valueTuple[i]);
                        break;
                    //Dynamics: head(X(i)) = enc(len( head(X(1)) ... head(X(k)) tail(X(1)) ... tail(X(i-1)) )) tail(X(i)) = enc(X(i))
                    case ABIType.BYTES:
                        head_i = _intCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = _fixedBytesCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.STRING:
                        Encoding utf8 = Encoding.UTF8;
                        head_i = _intCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = _fixedBytesCoder.EncodeToString(utf8.GetBytes((string)valueTuple[i]));
                        break;
                    case ABIType.DYNAMICARRAY:
                        head_i = _intCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = _intCoder.EncodeToString(((List<object>)(valueTuple[i])).Count) + EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.FIXEDARRAY:
                        head_i = _intCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.TUPLE:
                        head_i = _intCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = EncodeToString(valueTuple[i]);
                        break;
                    default:
                        break;
                }
                tailLength += tail_i.Length/2; // 64 hex str-> 32 bytes

                headList.Add(head_i);
                tailList.Add(tail_i);


            }

            //concat head list and tail list
            string encoded = "";
            foreach (string headstr in headList)
            {
                encoded += headstr;
            }
            foreach (string tailstr in tailList)
            {
                encoded += tailstr;
            }

            return encoded;
        }

        public string DecodeToString(byte[] encoded)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSupportedType()
        {
            throw new System.NotImplementedException();
        }
    }

        
}