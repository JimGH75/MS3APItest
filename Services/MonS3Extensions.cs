using MonS3API;

public static class MonS3Extensions
{
    public static int AsInt(this MonS3APIValue v)
        => Convert.ToInt32(v);

    public static decimal AsDecimal(this MonS3APIValue v)
        => Convert.ToDecimal(v);

    public static string AsString(this MonS3APIValue v)
        => v?.ToString();

    public static DateTime AsDate(this MonS3APIValue v)
        => Convert.ToDateTime(v);
}