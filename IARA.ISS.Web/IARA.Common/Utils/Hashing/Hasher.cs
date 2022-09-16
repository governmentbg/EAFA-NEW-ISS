using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IARA.Common.Constants;
using Microsoft.Extensions.ObjectPool;

namespace IARA.Common.Utils.Hashing
{
    public static class Hasher
    {
        private const double SEP_DIV = 3.5;
        private const double GUARD_DIV = 12.0;
        private const int MAX_NUMBER_HASH_LENGTH = 6;

        private static char[] alphabet;
        private static long alphabetLength;
        private static char[] seps;
        private static char[] guards;
        private static char[] salt;
        private static readonly int minHashLength;
        private static readonly ObjectPool<StringBuilder> sbPool = new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy());

        // Creates the Regex in the first usage, speed up first use of non-hex methods
        private static readonly Lazy<Regex> hexValidator = new Lazy<Regex>(() => new Regex("^[0-9-fa-F]+$", RegexOptions.Compiled));

        private static readonly Lazy<Regex> hexSplitter = new Lazy<Regex>(() => new Regex(@"[\w\W]{1,12}", RegexOptions.Compiled));

        static Hasher()
        {
            if (DefaultConstants.HASHER_SALT == null)
            {
                throw new ArgumentNullException(nameof(DefaultConstants.HASHER_SALT));
            }
            if (string.IsNullOrWhiteSpace(DefaultConstants.HASHER_ALPHABET))
            {
                throw new ArgumentNullException(nameof(DefaultConstants.HASHER_ALPHABET));
            }
            if (DefaultConstants.HASHER_MIN_HASH_LENGTH < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(DefaultConstants.HASHER_MIN_HASH_LENGTH), "Value must be zero or greater");
            }
            if (string.IsNullOrWhiteSpace(DefaultConstants.HASHER_SEPS))
            {
                throw new ArgumentNullException(nameof(DefaultConstants.HASHER_SEPS));
            }

            salt = DefaultConstants.HASHER_SALT.Trim().ToCharArray();
            alphabet = DefaultConstants.HASHER_ALPHABET.ToCharArray().Distinct().ToArray();
            seps = DefaultConstants.HASHER_SEPS.ToCharArray();
            minHashLength = DefaultConstants.HASHER_MIN_HASH_LENGTH;

            if (alphabet.Length < DefaultConstants.HASHER_MIN_ALPHABET_LENGTH)
            {
                throw new ArgumentException($"Alphabet must contain at least {DefaultConstants.HASHER_MIN_ALPHABET_LENGTH} unique characters",
                    nameof(DefaultConstants.HASHER_ALPHABET));
            }

            SetupSeps();
            SetupGuards();

            alphabetLength = alphabet.Length;
        }

        private static void SetupSeps()
        {
            // seps should contain only characters present in alphabet;
            seps = seps.Intersect(alphabet).ToArray();

            // alphabet should not contain seps;
            alphabet = alphabet.Except(seps).ToArray();

            ConsistentShuffle(seps, seps.Length, salt, salt.Length);

            if (seps.Length == 0 || ((float)alphabet.Length / seps.Length) > SEP_DIV)
            {
                var sepsLength = (int)Math.Ceiling((float)alphabet.Length / SEP_DIV);

                if (sepsLength == 1)
                {
                    sepsLength = 2;
                }

                if (sepsLength > seps.Length)
                {
                    var diff = sepsLength - seps.Length;
                    seps = seps.Append(alphabet, 0, diff);
                    alphabet = alphabet.SubArray(diff);
                }
                else
                {
                    seps = seps.SubArray(0, sepsLength);
                }
            }

            ConsistentShuffle(alphabet, alphabet.Length, salt, salt.Length);
        }

        private static void SetupGuards()
        {
            var guardCount = (int)Math.Ceiling(alphabet.Length / GUARD_DIV);

            if (alphabet.Length < 3)
            {
                guards = seps.SubArray(0, guardCount);
                seps = seps.SubArray(guardCount);
            }
            else
            {
                guards = alphabet.SubArray(0, guardCount);
                alphabet = alphabet.SubArray(guardCount);
            }
        }

        /// <summary>
        /// Encodes the provided numbers into a hash string.
        /// </summary>
        /// <param name="numbers">List of integers.</param>
        /// <returns>Encoded hash string.</returns>
        public static string Encode(params int[] numbers) => GenerateHashFrom(Array.ConvertAll(numbers, n => (long)n));

        /// <summary>
        /// Encodes the provided numbers into a hash string.
        /// </summary>
        /// <param name="numbers">Enumerable list of integers.</param>
        /// <returns>Encoded hash string.</returns>
        public static string Encode(IEnumerable<int> numbers) => Encode(numbers.ToArray());

        /// <summary>
        /// Encodes the provided numbers into a hash string.
        /// </summary>
        /// <param name="numbers">List of 64-bit integers.</param>
        /// <returns>Encoded hash string.</returns>
        public static string EncodeLong(params long[] numbers) => GenerateHashFrom(numbers);

        /// <summary>
        /// Encodes the provided numbers into a hash string.
        /// </summary>
        /// <param name="numbers">Enumerable list of 64-bit integers.</param>
        /// <returns>Encoded hash string.</returns>
        public static string EncodeLong(IEnumerable<long> numbers) => EncodeLong(numbers.ToArray());

        /// <summary>
        /// Decodes the provided hash into numbers.
        /// </summary>
        /// <param name="hash">Hash string to decode.</param>
        /// <returns>Array of integers.</returns>
        /// <exception cref="T:System.OverflowException">If the decoded number overflows integer.</exception>
        public static int[] Decode(string hash) => Array.ConvertAll(GetNumbersFrom(hash), n => (int)n);

        /// <summary>
        /// Decodes the provided hash into numbers.
        /// </summary>
        /// <param name="hash">Hash string to decode.</param>
        /// <returns>Array of 64-bit integers.</returns>
        public static long[] DecodeLong(string hash) => GetNumbersFrom(hash);

        /// <summary>
        /// Encodes the provided hex-string into a hash string.
        /// </summary>
        /// <param name="hex">Hex string to encode.</param>
        /// <returns>Encoded hash string.</returns>
        public static string EncodeHex(string hex)
        {
            if (!hexValidator.Value.IsMatch(hex))
            {
                return string.Empty;
            }

            var matches = hexSplitter.Value.Matches(hex);
            var numbers = new List<long>(matches.Count);

            foreach (Match match in matches)
            {
                var number = Convert.ToInt64(string.Concat("1", match.Value), 16);
                numbers.Add(number);
            }

            return EncodeLong(numbers.ToArray());
        }

        /// <summary>
        /// Decodes the provided hash into a hex-string.
        /// </summary>
        /// <param name="hash">Hash string to decode.</param>
        /// <returns>Decoded hex string.</returns>
        public static string DecodeHex(string hash)
        {
            var builder = sbPool.Get();
            var numbers = DecodeLong(hash);

            foreach (var number in numbers)
            {
                var s = number.ToString("X");

                for (var i = 1; i < s.Length; i++)
                {
                    builder.Append(s[i]);
                }
            }

            var result = builder.ToString();
            sbPool.Return(builder);

            return result;
        }

        private static string GenerateHashFrom(long[] numbers)
        {
            if (numbers == null || numbers.Length == 0 || numbers.Any(n => n < 0))
            {
                return string.Empty;
            }

            long numbersHashInt = 0;
            for (var i = 0; i < numbers.Length; i++)
            {
                numbersHashInt += numbers[i] % (i + 100);
            }

            var builder = sbPool.Get();

            char[] shuffleBuffer = null;
            var alphabet = Hasher.alphabet.CopyPooled();
            var hashBuffer = ArrayPool<char>.Shared.Rent(MAX_NUMBER_HASH_LENGTH);
            try
            {
                var lottery = alphabet[numbersHashInt % alphabetLength];
                builder.Append(lottery);
                shuffleBuffer = CreatePooledBuffer(Hasher.alphabet.Length, lottery);

                var startIndex = 1 + salt.Length;
                var length = Hasher.alphabet.Length - startIndex;

                for (var i = 0; i < numbers.Length; i++)
                {
                    var number = numbers[i];

                    if (length > 0)
                    {
                        Array.Copy(alphabet, 0, shuffleBuffer, startIndex, length);
                    }

                    ConsistentShuffle(alphabet, Hasher.alphabet.Length, shuffleBuffer, Hasher.alphabet.Length);
                    var hashLength = BuildReversedHash(number, alphabet, hashBuffer);

                    for (var j = hashLength - 1; j > -1; j--)
                    {
                        builder.Append(hashBuffer[j]);
                    }

                    if (i + 1 < numbers.Length)
                    {
                        number %= hashBuffer[hashLength - 1] + i;
                        var sepsIndex = number % seps.Length;

                        builder.Append(seps[sepsIndex]);
                    }
                }

                if (builder.Length < minHashLength)
                {
                    var guardIndex = (numbersHashInt + builder[0]) % guards.Length;
                    var guard = guards[guardIndex];

                    builder.Insert(0, guard);

                    if (builder.Length < minHashLength)
                    {
                        guardIndex = (numbersHashInt + builder[2]) % guards.Length;
                        guard = guards[guardIndex];

                        builder.Append(guard);
                    }
                }

                var halfLength = Hasher.alphabet.Length / 2;

                while (builder.Length < minHashLength)
                {
                    Array.Copy(alphabet, shuffleBuffer, Hasher.alphabet.Length);
                    ConsistentShuffle(alphabet, Hasher.alphabet.Length, shuffleBuffer, Hasher.alphabet.Length);
                    builder.Insert(0, alphabet, halfLength, Hasher.alphabet.Length - halfLength);
                    builder.Append(alphabet, 0, halfLength);

                    var excess = builder.Length - minHashLength;
                    if (excess > 0)
                    {
                        builder.Remove(0, excess / 2);
                        builder.Remove(minHashLength, builder.Length - minHashLength);
                    }
                }
            }
            finally
            {
                alphabet.ReturnToPool();
                shuffleBuffer.ReturnToPool();
                hashBuffer.ReturnToPool();
            }

            var result = builder.ToString();
            sbPool.Return(builder);

            return result;
        }

        private static int BuildReversedHash(long input, char[] alphabet, char[] hashBuffer)
        {
            var length = 0;
            do
            {
                hashBuffer[length++] = alphabet[input % alphabetLength];
                input /= alphabetLength;
            }
            while (input > 0);

            return length;
        }

        private static long Unhash(string input, char[] alphabet)
        {
            long number = 0;

            for (var i = 0; i < input.Length; i++)
            {
                var pos = Array.IndexOf(alphabet, input[i]);
                number = (number * alphabetLength) + pos;
            }

            return number;
        }

        private static long[] GetNumbersFrom(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                return Array.Empty<long>();
            }

            var hashArray = hash.Split(guards, StringSplitOptions.RemoveEmptyEntries);
            if (hashArray.Length == 0)
            {
                return Array.Empty<long>();
            }

            var i = 0;
            if (hashArray.Length == 3 || hashArray.Length == 2)
            {
                i = 1;
            }

            var hashBreakdown = hashArray[i];
            var lottery = hashBreakdown[0];

            if (lottery == default(char))
            {
                return Array.Empty<long>();
            }

            hashBreakdown = hashBreakdown.Substring(1);

            hashArray = hashBreakdown.Split(seps, StringSplitOptions.RemoveEmptyEntries);

            var result = new long[hashArray.Length];
            char[] buffer = null;
            var alphabet = Hasher.alphabet.CopyPooled();
            try
            {
                buffer = CreatePooledBuffer(Hasher.alphabet.Length, lottery);

                var startIndex = 1 + salt.Length;
                var length = Hasher.alphabet.Length - startIndex;

                for (var j = 0; j < hashArray.Length; j++)
                {
                    var subHash = hashArray[j];

                    if (length > 0)
                    {
                        Array.Copy(alphabet, 0, buffer, startIndex, length);
                    }

                    ConsistentShuffle(alphabet, Hasher.alphabet.Length, buffer, Hasher.alphabet.Length);
                    result[j] = Unhash(subHash, alphabet);
                }
            }
            finally
            {
                alphabet.ReturnToPool();
                buffer.ReturnToPool();
            }

            if (EncodeLong(result) == hash)
            {
                return result;
            }

            return Array.Empty<long>();
        }

        private static char[] CreatePooledBuffer(int alphabetLength, char lottery)
        {
            var buffer = ArrayPool<char>.Shared.Rent(alphabetLength);
            buffer[0] = lottery;
            Array.Copy(salt, 0, buffer, 1, Math.Min(salt.Length, alphabetLength - 1));

            return buffer;
        }

        private static void ConsistentShuffle(char[] alphabet, int alphabetLength, char[] salt, int saltLength)
        {
            if (salt.Length == 0)
            {
                return;
            }

            int n;
            for (int i = alphabetLength - 1, v = 0, p = 0; i > 0; i--, v++)
            {
                v %= saltLength;
                p += (n = salt[v]);
                var j = (n + v + p) % i;

                //swap characters at positions i and j
                var temp = alphabet[j];
                alphabet[j] = alphabet[i];
                alphabet[i] = temp;
            }
        }
    }
}
