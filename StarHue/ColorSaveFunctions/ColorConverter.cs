using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHue
{
    class ColorConverter
    {
        public static (float c, float m, float y, float k) ConvertRGBToCMYK(int r, int g, int b)
        {
            float c = 1 - r / 255f;
            float m = 1 - g / 255f;
            float y = 1 - b / 255f;
            float k = Math.Min(c, Math.Min(m, y));

            if (k == 1) // Edge-case with pure black (#000000)
            {
                c = 0;
                m = 0;
                y = 0;
            }

            if (k < 1)
            {
                c = (c - k) / (1 - k);
                m = (m - k) / (1 - k);
                y = (y - k) / (1 - k);
            }
            return (c, m, y, k);
        }

        public static (float h, float s, float l) ConvertRGBToHSL(int r, int g, int b)
        {
            float rNorm = r / 255f;
            float gNorm = g / 255f;
            float bNorm = b / 255f;

            float max = Math.Max(rNorm, Math.Max(gNorm, bNorm));
            float min = Math.Min(rNorm, Math.Min(gNorm, bNorm));
            float delta = max - min;

            float h = 0, s = 0, l = (max + min) / 2;

            if (delta != 0)
            {
                if (max == rNorm)
                    h = (gNorm - bNorm) / delta + (gNorm < bNorm ? 6 : 0);
                else if (max == gNorm)
                    h = (bNorm - rNorm) / delta + 2;
                else
                    h = (rNorm - gNorm) / delta + 4;

                s = delta / (1 - Math.Abs(2 * l - 1));
                h *= 60;
            }

            return (h, s, l);
        }

        public static (float h, float s, float v) ConvertRGBToHSV(int r, int g, int b)
        {
            float rNorm = r / 255f;
            float gNorm = g / 255f;
            float bNorm = b / 255f;

            float max = Math.Max(rNorm, Math.Max(gNorm, bNorm));
            float min = Math.Min(rNorm, Math.Min(gNorm, bNorm));
            float delta = max - min;

            float h = 0, s = 0, v = max;

            if (delta != 0)
            {
                if (max == rNorm)
                    h = (gNorm - bNorm) / delta + (gNorm < bNorm ? 6 : 0);
                else if (max == gNorm)
                    h = (bNorm - rNorm) / delta + 2;
                else
                    h = (rNorm - gNorm) / delta + 4;

                s = delta / max;
                h *= 60;
            }

            return (h, s, v);
        }
    }
}
