using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework {
    public static class RandomUtility {
        private const string NUMBERS_AND_LETTERS = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static int g_seedOffset = 1;

        public static string GetString(int length) {
            int position = 0;
            char[] pattern = new char[NUMBERS_AND_LETTERS.Length];
            StringBuilder numbersAndLetters = new StringBuilder(NUMBERS_AND_LETTERS);
            Random random = new Random(Environment.TickCount + (g_seedOffset++) << length);

            for(int i = 0; i < pattern.Length; i++) {
                position = random.Next(numbersAndLetters.Length);
                pattern[i] = numbersAndLetters[position];
                numbersAndLetters.Remove(position, 1);
            }

            char[] result = new char[length];
            for(int i = 0; i < length; i++) {
                position = random.Next(pattern.Length);
                result[i] = pattern[position];
            }

            return new string(result);
        }
    }
}
