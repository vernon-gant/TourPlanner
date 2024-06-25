using System.Text;

namespace TP.OpenRoute;

public static class PolylineEncoder
{
    public static string Encode(List<List<decimal>> coordinates)
    {
        var str = new StringBuilder();

        var encodeDiff = (Action<int>)(diff =>
        {
            int shifted = diff << 1;
            if (diff < 0)
                shifted = ~shifted;
            int rem = shifted;
            while (rem >= 0x20)
            {
                str.Append((char)((0x20 | (rem & 0x1f)) + 63));
                rem >>= 5;
            }

            str.Append((char)(rem + 63));
        });

        int lastLat = 0;
        int lastLng = 0;
        foreach (var point in coordinates)
        {
            int lat = (int)Math.Round(point[1] * 1E5M);
            int lng = (int)Math.Round(point[0] * 1E5M);
            encodeDiff(lat - lastLat);
            encodeDiff(lng - lastLng);
            lastLat = lat;
            lastLng = lng;
        }

        return str.ToString();
    }
}