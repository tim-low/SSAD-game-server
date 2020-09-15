using Game_Server.Model;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Game_Server.Util
{
    public static class Utilities
    {
        public const long TickInSec = 10000000;
        public static string GenerateCode(int size)
        {
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return BitConverter.ToString(buff).Replace("-", "");
        }

        /// <summary>
        /// Exclusive of maxValue
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Random(int maxValue)
        {
            Random random = new Random();
            return random.Next(maxValue);
        }

        public static int RandomItemEnum()
        {
            Random random = new Random();
            Random random2 = new Random();
            return (random.Next(0, 12)+random2.Next(0, 12))%12;
        }

        public static string GenerateToken(string username, string salt)
        {
            return Convert.ToBase64String(Password.GenerateSaltedHash(Encoding.UTF8.GetBytes(username),
                Encoding.UTF8.GetBytes(salt))).Replace('/', '-');
        }

        public static byte[] DownloadImage(string imageUrl)
        {
            byte[] data;
            using (var webClient = new WebClient())
            {
                data = webClient.DownloadData(imageUrl);
            }
            return data;
        }

        public static BoardDirection Opposite(BoardDirection direction)
        {
            switch(direction)
            {
                case BoardDirection.UP:
                    return BoardDirection.DOWN;
                case BoardDirection.DOWN:
                    return BoardDirection.UP;
                case BoardDirection.LEFT:
                    return BoardDirection.RIGHT;
                case BoardDirection.RIGHT:
                    return BoardDirection.LEFT;
                default:
                    return BoardDirection.DOWN;
            }
        }

        /// <summary>
        /// Compute Level based on the user total experience
        /// </summary>
        /// <param name="experience"></param>
        /// <returns></returns>
        public static int ComputeLevel(int totalExperience, out int currentLevelExperience)
        {
            for(int i = 1; i < 10; i++)
            {
                if(totalExperience - (i*1000) > 0)
                {
                    totalExperience -= (i * 1000);
                }
                else
                {
                    currentLevelExperience = totalExperience;
                    return i;
                }
            }
            // shouldn't reach this state;
            currentLevelExperience = 0;
            return 10;
        }

        // Set bit at position itemNumber
        public static byte AddAttributeItem(byte attribute, int pos)
        {
            return attribute |= (byte)(1 << pos);
        }


        public static BoardTile ProcessDPad(BoardTile boardTile, BoardDirection direction, ItemType item)
        {
            switch(direction)
            {
                case BoardDirection.UP:
                    if (item == ItemType.Sliding)
                    {
                        return new BoardTile(boardTile.X, 7);
                    }
                    else
                    {
                        return new BoardTile(boardTile.X, boardTile.Y + 1);
                    }
                case BoardDirection.DOWN:
                    if (item == ItemType.Sliding)
                    {
                        return new BoardTile(boardTile.X, 0);
                    }
                    else
                    {
                        return new BoardTile(boardTile.X, boardTile.Y - 1);
                    }
                case BoardDirection.LEFT:
                    if (item == ItemType.Sliding)
                    {
                        return new BoardTile(0, boardTile.Y);
                    }
                    else
                    {
                        return new BoardTile(boardTile.X - 1, boardTile.Y);
                    }
                case BoardDirection.RIGHT:
                    if (item == ItemType.Sliding)
                    {
                        return new BoardTile(7, boardTile.Y);
                    }
                    else
                    {
                        return new BoardTile(boardTile.X + 1, boardTile.Y);
                    }
                default:
                    return new BoardTile(-1, -1);
            }
        }

        public static Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }

    }

    public static class Password
    {
        public const int SaltSize = 40;

        public static string CreateSalt(int size = SaltSize)
        {
            //Generate a cryptographic random number.
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return BitConverter.ToString(buff).ToLower().Replace("-", "").Substring(0, 40);
        }

        public static string GenerateSaltedHash(string plainText, string salt)
        {
            return BitConverter.ToString(GenerateSaltedHash(Encoding.UTF8.GetBytes(plainText),
                Encoding.UTF8.GetBytes(salt))).ToLower().Replace("-", "");
        }

        internal static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            var plainTextWithSaltBytes =
                new byte[plainText.Length + salt.Length];

            for (var i = 0; i < plainText.Length; i++)
                plainTextWithSaltBytes[i] = plainText[i];
            for (var i = 0; i < salt.Length; i++)
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }
    }
}
