using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Xphter.Framework.Text {
    /// <summary>
    /// Provides encrypt function use a hash algorithm.
    /// </summary>
    public class HashEncryptor {
        /// <summary>
        /// Initialize a new instance of Encryptor class.
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="algorithm"></param>
        /// <exception cref="System.ArgumentException"><paramref name="encoding"/> or <paramref name="algorithm"/> is null.</exception>
        public HashEncryptor(Encoding encoding, HashAlgorithm algorithm) {
            if(encoding == null) {
                throw new ArgumentException("Text encoding is null.", "encoding");
            }
            if(algorithm == null) {
                throw new ArgumentException("Hash algorithm is null.", "algorithm");
            }

            this.Encoding = encoding;
            this.Algorithm = algorithm;
        }

        private byte[] m_emptyCipherText = new byte[0];

        /// <summary>
        /// Gets the Encoding used to decode string to the binary data.
        /// </summary>
        public Encoding Encoding {
            get;
            private set;
        }

        /// <summary>
        /// Gets the has algorithm.
        /// </summary>
        public HashAlgorithm Algorithm {
            get;
            private set;
        }

        /// <summary>
        /// Compares the specified two binary data, return true if they are the same.
        /// </summary>
        /// <param name="data0"></param>
        /// <param name="data1"></param>
        /// <returns></returns>
        public static bool CompareBytes(byte[] data0, byte[] data1) {
            if(object.ReferenceEquals(data0, data1)) {
                return true;
            }
            if(data0 == null || data1 == null) {
                return false;
            }
            if(data0.Length != data1.Length) {
                return false;
            }

            for(int i = 0; i < data0.Length; i++) {
                if(data0[i] != data1[i]) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Encrypts the specified plaintext.
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public byte[] Encrypt(string plaintext) {
            if(plaintext == null || plaintext.Length == 0) {
                return this.m_emptyCipherText;
            }
            byte[] buffer = this.Encoding.GetBytes(plaintext);
            byte[] ciphertext = this.Algorithm.ComputeHash(buffer);
            return ciphertext;
        }

        /// <summary>
        /// Validates whether the specified plantext can encrypt to the specified ciphertext.
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        public bool Validate(string plaintext, byte[] ciphertext) {
            return CompareBytes(this.Encrypt(plaintext), ciphertext);
        }
    }
}
