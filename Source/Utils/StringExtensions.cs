namespace CarbonCore.Utils
{
    using System;
    using System.Text;

    public static class StringExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static string Capitalize(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            char[] characters = source.ToCharArray();
            characters[0] = char.ToUpper(characters[0]);
            return new string(characters);
        }

        public static string ToFormattedString(this float source, string format)
        {
            string formatted = source.ToString(format);
            return formatted;
        }

        public static string ToReverse(this string source)
        {
            char[] charArray = source.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static StringBuilder AppendFormatLine(this StringBuilder target, string format, params object[] args)
        {
            return target.AppendLine(string.Format(format, args));
        }
        
        public static StringBuilder AppendCommaSeparated(this StringBuilder target, params object[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                target.Append(columns[i]);
                if (i < columns.Length - 1)
                {
                    target.Append(",");
                }
            }

            return target;
        }

        public static StringBuilder AppendCommaSeparatedLine(this StringBuilder target, params object[] columns)
        {
            return target.AppendCommaSeparated(columns).Append(Environment.NewLine);
        }

        public static string SubstringUntil(this string source, char target, int startIndex = 0)
        {
            if (startIndex == source.Length - 1 || source[startIndex] == target)
            {
                return null;
            }

            int targetIndex = -1;
            for (var i = startIndex; i < source.Length; i++)
            {
                if (source[i] == target)
                {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex < 0)
            {
                return null;
            }

            return source.Substring(startIndex, targetIndex - startIndex);
        }

        public static string GetAgnosticPath(this string sourcePath)
        {
            return sourcePath.Replace('/', System.IO.Path.DirectorySeparatorChar).Replace('\\', System.IO.Path.DirectorySeparatorChar);
        }
    }
}
