using System;

namespace Utils
{
    public static class RandomString
    {
        private static readonly Random Random = new Random();
        public static string CreateString(int stringLength)
        {
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var chars = new char[stringLength];

            for (var i = 0; i < stringLength; i++)
            {
                chars[i] = allowedChars[Random.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
    }
}
