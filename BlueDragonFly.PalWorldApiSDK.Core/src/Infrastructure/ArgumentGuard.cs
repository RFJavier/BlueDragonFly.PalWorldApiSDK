namespace BlueDragonFly.PalWorldApiSDK.Core;

internal static class ArgumentGuard
{
    public static string RequireValue(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value;
    }
}
