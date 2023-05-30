namespace SayHenTai_WebApp.Infrastructure.Caching
{
    /// <summary>
    /// I'm take idea form https://github.com/ardalis/GuardClauses
    /// </summary>
    public static class GuardClauses
    {

        /// <summary>
        /// Check a object and throw ArgumentNullException if null
        /// </summary>
        /// <param name="input">Oject to check</param>
        /// <param name="parameterName">Name of parameter</param>
        public static void Null(object input, string parameterName)
        {
            if (null == input)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Check a string null or empty
        /// </summary>
        /// <param name="input">string input</param>
        /// <param name="parameterName">Name of parameter</param>
        public static void NullOrEmpty(string input, string parameterName)
        {
            Null(input, parameterName);
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Required input " + parameterName + " was empty.", parameterName);
            }
        }

        /// <summary>
        /// Check a collection
        /// </summary>
        /// <typeparam name="T">Type of object in collection</typeparam>
        /// <param name="input">Collection input</param>
        /// <param name="parameterName">Name of parameter</param>
        public static void NullOrEmpty<T>(IEnumerable<T> input, string parameterName)
        {
            Null(input, parameterName);
            if (!input.Any())
            {
                throw new ArgumentException("Required input " + parameterName + " was empty.", parameterName);
            }
        }

        /// <summary>
        /// Check string null or only contains space
        /// </summary>
        /// <param name="input">String input</param>
        /// <param name="parameterName">Name of parameter</param>
        public static void NullOrWhiteSpace(string input, string parameterName)
        {
            NullOrEmpty(input, parameterName);
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Required input " + parameterName + " was empty.", parameterName);
            }
        }

        /// <summary>
        /// Check range of int input
        /// </summary>
        /// <param name="input">Number input</param>
        /// <param name="parameterName">Name of parameter</param>
        /// <param name="rangeFrom">Range from</param>
        /// <param name="rangeTo">range to</param>
        public static void OutOfRange(int input, string parameterName, int rangeFrom, int rangeTo)
        {
            OutOfRange<int>(input, parameterName, rangeFrom, rangeTo);
        }

        /// <summary>
        /// Check range of long input
        /// </summary>
        /// <param name="input">Number input</param>
        /// <param name="parameterName">Name of parameter</param>
        /// <param name="rangeFrom">Range from</param>
        /// <param name="rangeTo">range to</param>
        public static void OutOfRange(long input, string parameterName, long rangeFrom, long rangeTo)
        {
            OutOfRange<long>(input, parameterName, rangeFrom, rangeTo);
        }

        /// <summary>
        /// Check range of date time input
        /// </summary>
        /// <param name="input">Date time input</param>
        /// <param name="parameterName">Name of parameter</param>
        /// <param name="rangeFrom">Date time from</param>
        /// <param name="rangeTo">Date time to</param>
        public static void OutOfRange(DateTime input, string parameterName, DateTime rangeFrom, DateTime rangeTo)
        {
            OutOfRange<DateTime>(input, parameterName, rangeFrom, rangeTo);
        }

        /// <summary>
        /// Check range of sql server range
        /// </summary>
        /// <param name="input">Date time input</param>
        /// <param name="parameterName">Name of parameter</param>
        public static void OutOfSQLDateRange(DateTime input, string parameterName)
        {
            OutOfRange<DateTime>(input, parameterName, new DateTime(552877920000000000L), new DateTime(3155378975999970000L));
        }

        /// <summary>
        /// Check range of input
        /// </summary>
        /// <typeparam name="T">Type of check</typeparam>
        /// <param name="input">Input to check</param>
        /// <param name="parameterName">Name of parameter</param>
        /// <param name="rangeFrom">Check value from</param>
        /// <param name="rangeTo">Check value to</param>
        private static void OutOfRange<T>(T input, string parameterName, T rangeFrom, T rangeTo)
        {
            Comparer<T> @default = Comparer<T>.Default;
            if (@default.Compare(rangeFrom, rangeTo) > 0)
            {
                throw new ArgumentException("rangeFrom should be less or equal than rangeTo");
            }
            if (@default.Compare(input, rangeFrom) < 0 || @default.Compare(input, rangeTo) > 0)
            {
                throw new ArgumentOutOfRangeException("Input " + parameterName + " was out of range", parameterName);
            }
        }

        /// <summary>
        /// Check input type int equals zero
        /// </summary>
        /// <param name="input">input type int</param>
        /// <param name="parameterName">Name of parameter</param>
        public static void Zero(int input, string parameterName)
        {
            Zero<int>(input, parameterName);
        }

        /// <summary>
        /// Check input type long equals zero
        /// </summary>
        /// <param name="input">input type long</param>
        /// <param name="parameterName">Name of parameter</param>
        public static void Zero(long input, string parameterName)
        {
            Zero<long>(input, parameterName);
        }

        /// <summary>
        /// Check input type decimal equals zero
        /// </summary>
        /// <param name="input">input type decimal</param>
        /// <param name="parameterName">Name of parameter</param>
        public static void Zero(decimal input, string parameterName)
        {
            Zero<decimal>(input, parameterName);
        }

        /// <summary>
        /// Check input type loat equals zero
        /// </summary>
        /// <param name="input">input type float</param>
        /// <param name="parameterName">Name of parameter</param>
        public static void Zero(float input, string parameterName)
        {
            Zero<float>(input, parameterName);
        }

        /// <summary>
        /// Check input type double equals zero
        /// </summary>
        /// <param name="input">input type double</param>
        /// <param name="parameterName">Name of parameter</param>
        public static void Zero(double input, string parameterName)
        {
            Zero<double>(input, parameterName);
        }

        /// <summary>
        /// Check ipunt equals zeror
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="parameterName">Name of parameter</param>
        private static void Zero<T>(T input, string parameterName)
        {
            if (EqualityComparer<T>.Default.Equals(input, default))
            {
                throw new ArgumentException("Required input " + parameterName + " cannot be zero.", parameterName);
            }
        }

        /// <summary>
        /// Check out of range for enum
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="input">Input value</param>
        /// <param name="parameterName">Name of parameter</param>
        public static void OutOfRange<T>(int input, string parameterName) where T : Enum
        {
            if (!Enum.IsDefined(typeof(T), input))
            {
                typeof(T).ToString();
                throw new ArgumentOutOfRangeException("Required input " + parameterName + " was not a valid enum value for " + typeof(T).ToString() + ".", parameterName);
            }
        }

        /// <summary>
        /// Check input is default value
        /// </summary>
        /// <typeparam name="T">Type of input</typeparam>
        /// <param name="input">input value</param>
        /// <param name="parameterName">Name of parameter</param>
        public static void Default<T>(T input, string parameterName)
        {
            if (EqualityComparer<T>.Default.Equals(input, default))
            {
                throw new ArgumentException("Parameter [" + parameterName + "] is default value for type " + typeof(T).Name);
            }
        }
    }
}
