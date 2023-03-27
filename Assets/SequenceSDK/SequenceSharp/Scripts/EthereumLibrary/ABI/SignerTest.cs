using UnityEngine;
using SequenceSharp.ABI;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Text;

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;


public class SignerTest : MonoBehaviour
{
    private void Start()
    {
        
    }

    static AsymmetricCipherKeyPair GenerateKeyPair()
    {
        var curve = ECNamedCurveTable.GetByName("secp256k1");
        var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());

        var secureRandom = new SecureRandom();
        var keyParams = new ECKeyGenerationParameters(domainParams, secureRandom);

        var generator = new ECKeyPairGenerator("ECDSA");
        generator.Init(keyParams);
        var keyPair = generator.GenerateKeyPair();

        var privateKey = keyPair.Private as ECPrivateKeyParameters;
        var publicKey = keyPair.Public as ECPublicKeyParameters;

        Debug.Log($"Private key: {ToHex(privateKey.D.ToByteArrayUnsigned())}");
        Debug.Log($"Public key: {ToHex(publicKey.Q.GetEncoded())}");

        return keyPair;
    }

    static string ToHex(byte[] data) => String.Concat(data.Select(x => x.ToString("x2")));





}
