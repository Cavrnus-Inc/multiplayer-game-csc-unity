
using System.Globalization;
using UnityEngine;

namespace CavrnusSdk.Experimental
{
    public static class CavrnusPropertyObjectHelpers
    {
        public static float ToFloat(this string obj)
        {
            if (float.TryParse(obj, out var result))
                return result;

            return result;
        }

        public static Vector4 ToVector4(this string obj)
        {
            return Vector4.one;
        }
        
        public static Color ToColor(this string obj)
        {
            if (obj.StartsWith("RGBA"))
            {
                obj = obj.Replace("RGBA(", "").Replace(")", "");
                var components = obj.Split(',');

                if (components.Length == 4)
                {
                    var r = float.Parse(components[0], CultureInfo.InvariantCulture);
                    var g = float.Parse(components[1], CultureInfo.InvariantCulture);
                    var b = float.Parse(components[2], CultureInfo.InvariantCulture);
                    var a = float.Parse(components[3], CultureInfo.InvariantCulture);

                    return new Color(r, g, b, a);
                }
            }

            if (ColorUtility.TryParseHtmlString(obj, out var color))
                return color;
            
            return Color.white;
        }
    }
}