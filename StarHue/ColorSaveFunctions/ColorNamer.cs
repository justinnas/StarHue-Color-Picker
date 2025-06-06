using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHue
{
    class ColorNamer
    {
        public static Dictionary<string, string> LoadColorsFromCsv(string filePath)
        {
            var colors = new Dictionary<string, string>();

            foreach (var line in File.ReadLines(filePath).Skip(1))
            {
                var parts = line.Split(',');
                string colorName = parts[0].Trim();
                string hexValue = parts[1].Trim();
                colors[colorName] = hexValue;
            }

            return colors;
        }

        public static string FindClosestColorName(string hex, Dictionary<string, string> colors)
        {
            var inputColor = ColorTranslator.FromHtml(hex);
            string closestColorName = null;
            double closestDistance = double.MaxValue;

            foreach (var color in colors)
            {
                var currentColor = ColorTranslator.FromHtml(color.Value);
                double distance = ColorDistance(currentColor, inputColor);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestColorName = color.Key;
                }
            }

            return closestColorName;
        }

        static double ColorDistance(Color c2, Color c1)
        {
            // Euclidean distance between two colors
            return Math.Sqrt(
                Math.Pow(c1.R - c2.R, 2) +
                Math.Pow(c1.G - c2.G, 2) +
                Math.Pow(c1.B - c2.B, 2));
        }
    }
}
