namespace WebServer.Auth;

[Flags]
public enum PasswordFlags
{
    Lowercase = 1,
    Uppercase = 2,
    Digits = 4,
    NonAlphanumeric = 8
}

/// <summary>
/// Used for random file name generation (mostly)
/// </summary>
public class PasswordGenerator
{
    public static string Generate(int length = 16,
                                  PasswordFlags flags = PasswordFlags.Lowercase |
                                                        PasswordFlags.Uppercase |
                                                        PasswordFlags.Digits)
    {
        var password = new StringBuilder();
        var random = new Random();

        while (password.Length < length)
        {
            if (flags.HasFlag(PasswordFlags.NonAlphanumeric))
                password.Append((char)random.Next(33, 48));

            if (flags.HasFlag(PasswordFlags.Digits))
                password.Append((char)random.Next(48, 58));

            if (flags.HasFlag(PasswordFlags.Lowercase))
                password.Append((char)random.Next(97, 123));

            if (flags.HasFlag(PasswordFlags.Uppercase))
                password.Append((char)random.Next(65, 91));
        }

        return password.ToString();
    }
}