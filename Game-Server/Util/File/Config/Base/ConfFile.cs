using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game_Server.Util
{
    public class ConfFile
    {
        protected readonly List<Config> Values;

        protected ConfFile()
        {
            Values = new List<Config>();
        }

        /// <summary>
        ///     Loads all options in the file, and included files.
        ///     Does nothing if file doesn't exist.
        /// </summary>
        /// <param name="filePath"></param>
        public void Include(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            LoadFile(filePath);
        }

        /// <summary>
        ///     Loads all options in the file, and included files.
        ///     Throws FileNotFoundException if file couldn't be found.
        /// </summary>
        /// <param name="filePath"></param>
        protected void Require(string filePath)
        {
            LoadFile(filePath);
        }

        /// <summary>
        ///     Loads all options in the file, and included files.
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadFile(string filePath)
        {
            using (var fr = new FileReader(filePath))
            {
                foreach (var line in fr)
                {
                    var pos = -1;

                    // Check for seperator
                    if ((pos = line.Value.IndexOf(':')) < 0)
                        return;

                    string key = line.Value.Substring(0, pos).Trim();
                    string value = line.Value.Substring(pos + 1).Trim();

                    Config config = new Config()
                    {
                        Key = key,
                        Value = value
                    };

                    Config found = Values.SingleOrDefault(value => value.Key == key);
                    if (found != null)
                        Values.Remove(found);
                    Values.Add(config);
                }
            }
        }

        /// <summary>
        ///     Returns the option as bool, or the default value, if the option
        ///     doesn't exist.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected bool GetBool(string option, bool defaultValue = false)
        {
            string value;
            Config file = Values.SingleOrDefault(value => value.Key == option);
            if (file == null)
                return defaultValue;

            value = file.Value.ToLower().Trim();

            return value == "1" || value == "yes" || value == "true";
        }

        /// <summary>
        ///     Returns the option as byte, or the default value, if the option
        ///     doesn't exist.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected byte GetByte(string option, byte defaultValue = 0)
        {
            Config file = Values.SingleOrDefault(value => value.Key == option);
            if (file == null)
                return defaultValue;

            byte ret;
            if (byte.TryParse(file.Value, out ret))
                return ret;

            Log.Warning("Invalid value for '{0}', defaulting to '{1}'", option, defaultValue);
            return defaultValue;
        }

        /// <summary>
        ///     Returns the option as short, or the default value, if the option
        ///     doesn't exist.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected short GetShort(string option, short defaultValue = 0)
        {
            Config file = Values.SingleOrDefault(value => value.Key == option);
            if (file == null)
                return defaultValue;

            short ret;
            if (short.TryParse(file.Value, out ret))
                return ret;

            Log.Warning("Invalid value for '{0}', defaulting to '{1}'", option, defaultValue);
            return defaultValue;
        }

        /// <summary>
        ///     Returns the option as int, or the default value, if the option
        ///     doesn't exist.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected int GetInt(string option, int defaultValue = 0)
        {
            Config file = Values.SingleOrDefault(value => value.Key == option);
            if (file == null)
                return defaultValue;

            int ret;
            if (int.TryParse(file.Value, out ret))
                return ret;

            Log.Warning("Invalid value for '{0}', defaulting to '{1}'", option, defaultValue);
            return defaultValue;
        }

        /// <summary>
        ///     Returns the option as long, or the default value, if the option
        ///     doesn't exist.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected long GetLong(string option, long defaultValue = 0)
        {
            Config file = Values.SingleOrDefault(value => value.Key == option);
            if (file == null)
                return defaultValue;

            long ret;
            if (long.TryParse(file.Value, out ret))
                return ret;

            Log.Warning("Invalid value for '{0}', defaulting to '{1}'", option, defaultValue);
            return defaultValue;
        }

        /// <summary>
        ///     Returns the option as string, or the default value, if the option
        ///     doesn't exist.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected string GetString(string option, string defaultValue = "")
        {
            Config file = Values.SingleOrDefault(value => value.Key == option);
            if (file == null)
                return defaultValue;
            return file.Value;
        }

        /// <summary>
        ///     Returns the option as float, or the default value, if the option
        ///     doesn't exist.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected float GetFloat(string option, float defaultValue = 0)
        {
            Config file = Values.SingleOrDefault(value => value.Key == option);
            if (file == null)
                return defaultValue;
            float ret;
            if (float.TryParse(file.Value, out ret))
                return ret;

            Log.Warning("Invalid value for '{0}', defaulting to '{1}'", option, defaultValue);
            return defaultValue;
        }

        /// <summary>
        ///     Returns the option as a DateTime, or the default value, if the
        ///     option doesn't exist.
        /// </summary>
        /// <remarks>
        ///     For acceptable value formatting, see
        ///     <see href="http://msdn.microsoft.com/en-us/library/system.datetime.parse(v=vs.110).aspx">MSDN</see>.
        /// </remarks>
        /// <param name="option"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected DateTime GetDateTime(string option, DateTime defaultValue = default(DateTime))
        {
            Config file = Values.SingleOrDefault(value => value.Key == option);
            if (file == null)
                return defaultValue;

            DateTime ret;
            if (DateTime.TryParse(file.Value, out ret))
                return ret;

            Log.Warning("Invalid value for '{0}', defaulting to '{1}'", option, defaultValue);
            return defaultValue;
        }

        /// <summary>
        ///     Returns the option as a TimeSpan, or the default value, if the
        ///     option doesn't exist.
        /// </summary>
        /// <remarks>
        ///     Value must be formatted as [-]{ d | [d.]hh:mm[:ss[.ff]] }
        ///     For more details, see <see href="http://msdn.microsoft.com/en-us/library/se73z7b9(v=vs.110).aspx">MSDN</see>.
        /// </remarks>
        /// <param name="option"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected TimeSpan GetTimeSpan(string option, TimeSpan defaultValue = default(TimeSpan))
        {
            Config file = Values.SingleOrDefault(value => value.Key == option);
            if (file == null)
                return defaultValue;

            TimeSpan ret;
            if (TimeSpan.TryParse(file.Value, out ret))
                return ret;

            Log.Warning("Invalid value for '{0}', defaulting to '{1}'", option, defaultValue);
            return defaultValue;
        }

        /// <summary>
        ///     Returns the option as an enum, or the default value, if the option
        ///     doesn't exist.
        /// </summary>
        /// <typeparam name="T">The type of the enum</typeparam>
        /// <param name="option"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected T GetEnum<T>(string option, T defaultValue = default(T)) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new NotSupportedException("Type " + typeof(T) + " is not an enum.");

            Config file = Values.SingleOrDefault(value => value.Key == option);
            if (file == null)
                return defaultValue;

            T ret;

            if (Enum.TryParse(file.Value, true, out ret))
                return ret;

            Log.Warning("Invalid value for '{0}', defaulting to '{1}'", option, defaultValue);
            return defaultValue;

        }

        /**
         *
         *
         *
         *
         *
         *
         *
         *
         *
         *
         *
         *
         **/

    }
}
