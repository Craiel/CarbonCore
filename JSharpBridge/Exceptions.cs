namespace CarbonCore.JSharpBridge
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    public static class ExceptionExtensions
    {
        public static string GetLocalizedMessage(this Exception exception)
        {
            throw new NotImplementedException();
        }
    }

    public class InterruptedException : Exception
    {
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class NumberFormatException : FormatException
    {
    }

    public class RuntimeException : Exception
    {
        public RuntimeException(string message = null, Exception par2Exception = null)
        {
            throw new NotImplementedException();
        }

        public RuntimeException(Exception inner)
        {
            throw new NotImplementedException();
        }
    }

    public class IllegalStateException : Exception
    {
        public IllegalStateException(string message = null)
        {
            throw new NotImplementedException();
        }
    }

    public class NoSuchAlgorithmException : Exception
    {
    }

    public class UnsupportedEncodingException : Exception
    {
    }

    public class InvalidKeySpecException : Exception
    {
    }

    public class IllegalBlockSizeException : Exception
    {
    }

    public class BadPaddingException : Exception
    {
    }

    public class InvalidKeyException : Exception
    {
    }

    public class NoSuchPaddingException : Exception
    {
    }

    public class JsonParseException : Exception
    {
        public JsonParseException(string message = null, Exception inner = null)
        {
            throw new NotImplementedException();
        }
    }

    public class EOFException : Exception
    {
    }

    public class IllegalFormatException : Exception
    {
    }

    public class SocketException : Exception
    {
    }

    public class SocketTimeoutException : Exception
    {
    }

    public class SoundSystemException : Exception
    {
    }

    public class IllegalAccessException : Exception
    {
    }

    public class InvalidSyntaxException : Exception
    {
    }

    public class ParseException : Exception
    {
    }

    public class MalformedURLException : Exception
    {
    }

    public class UnknownHostException : Exception
    {
    }

    public class PortUnreachableException : Exception
    {
    }

    public class ConnectException : Exception
    {
    }

    public class DataFormatException : Exception
    {
    }

    public class URISyntaxException : Exception
    {
    }

    public class UnsupportedOperationException : Exception
    {
        public UnsupportedOperationException(string canTDyeNonLeather = null)
        {
            throw new NotImplementedException();
        }
    }
}
