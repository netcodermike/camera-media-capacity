using System.Text.Json;

try
{
    if (args.Length != 1) throw new ArgumentException("Pass one JSON input file");
    using var data = JsonDocument.Parse(File.ReadAllText(args[0]));
    Console.WriteLine(JsonSerializer.Serialize(Analyze(data.RootElement)));
    return 0;
}
catch (Exception error) when (error is IOException or JsonException or FormatException
                               or ArgumentException or InvalidOperationException
                               or KeyNotFoundException or OverflowException)
{
    Console.Error.WriteLine(error.Message);
    return 2;
}

static object Analyze(JsonElement input)
{
    decimal card = input.GetProperty("card_gb").GetDecimal();
    decimal reserve = input.GetProperty("reserve_percent").GetDecimal();
    decimal nextRate = input.GetProperty("next_mbps").GetDecimal();
    if (card <= 0 || reserve < 0 || reserve >= 100 || nextRate <= 0)
        throw new ArgumentException("Invalid capacity, reserve, or bitrate");
    decimal used = 0;
    foreach (var clip in input.GetProperty("clips").EnumerateArray())
    {
        if (clip.GetArrayLength() != 2) throw new ArgumentException("Each clip needs bitrate and seconds");
        decimal rate = clip[0].GetDecimal(), seconds = clip[1].GetDecimal();
        if (rate <= 0 || seconds < 0) throw new ArgumentException("Invalid clip bitrate or duration");
        used += rate * seconds / 8000m;
    }
    decimal usable = card * (1 - reserve / 100m);
    decimal remaining = Math.Max(0, usable - used);
    return new { used_gb = Math.Round(used, 5), usable_gb = Math.Round(usable, 5),
        remaining_seconds = Math.Floor(remaining * 8000m / nextRate), fits = used <= usable };
}
