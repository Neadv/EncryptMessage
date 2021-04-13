using System;
using System.Text;

namespace EncryptMessage.Models
{
    public class RandomString
    {
        private static readonly string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string lower = upper.ToLower();
        private static readonly string digits = "0123456789";
        private static readonly string alphanum = upper + lower + digits;

        private Random random;
        private string symbols;
        private int length;

        public RandomString(int length, Random random, string symbols)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (symbols == null || symbols.Length <= 1)
                throw new ArgumentNullException(nameof(symbols));
            if (random == null)
                throw new ArgumentNullException(nameof(Random));

            this.random = random;
            this.symbols = symbols;
            this.length = length;
        }

        public RandomString(int length, Random random) : this(length, random, alphanum) { }

        public RandomString(int length) : this(length, new Random(), alphanum) { }

        public string Next()
        {
            StringBuilder stringBuilder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(symbols[random.Next(symbols.Length)]);
            }
            return stringBuilder.ToString();
        }
    }
}
