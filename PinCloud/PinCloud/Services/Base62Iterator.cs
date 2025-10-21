namespace APinI.Services
{
    public class Base62Iterator
    {
        private const string Base62Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private char[] currentString;

        public Base62Iterator(string startString)
        {
            currentString = startString.ToCharArray();
        }

        public string Next()
        {
            int index = currentString.Length - 1;
            while (index >= 0)
            {
                int charIndex = Base62Chars.IndexOf(currentString[index]);
                if (charIndex < Base62Chars.Length - 1)
                {
                    currentString[index] = Base62Chars[charIndex + 1];
                    break;
                }
                else
                {
                    currentString[index] = Base62Chars[0];
                    index--;
                }
            }

            if (index < 0) throw new InvalidOperationException("Sequence has reached its maximum value.");

            return new string(currentString);
        }
    }
}
