using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHue
{
    class ColorExporter
    {
        public static void ExportToHTML(string hexColor, string outputPath)
        {
            hexColor = hexColor.ToUpper();
            Color rgbColor = ColorTranslator.FromHtml(hexColor);
            string rgbColorString = rgbColor.R + ", " + rgbColor.G + ", " + rgbColor.B;

            var (cmyk_c, cmyk_m, cmyk_y, cmyk_k) = ColorConverter.ConvertRGBToCMYK(rgbColor.R, rgbColor.G, rgbColor.B);
            string cmykColorString = $"{Math.Round(cmyk_c * 100)}%, {Math.Round(cmyk_m * 100)}%, {Math.Round(cmyk_y * 100)}%, {Math.Round(cmyk_k * 100)}%";

            var (hsl_h, hsl_s, hsl_l) = ColorConverter.ConvertRGBToHSL(rgbColor.R, rgbColor.G, rgbColor.B);
            string hslColorString = $"{Math.Round(hsl_h)}, {Math.Round(hsl_s * 100)}%, {Math.Round(hsl_l * 100)}%";

            var (hsv_h, hsv_s, hsv_v) = ColorConverter.ConvertRGBToHSV(rgbColor.R, rgbColor.G, rgbColor.B);
            string hsvColorString = $"{Math.Round(hsv_h)}, {Math.Round(hsv_s * 100)}%, {Math.Round(hsv_v * 100)}%";

            string pageFavicon = GenerateFavicon(hexColor);

            string htmlContent = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{hexColor} - StarHue</title>
    <link rel='icon' type='image/png' href='data:image/png;base64,{pageFavicon}'>
    <style>
        body{{background:#101010;color:#fff;font-family:Arial,sans-serif;display:flex;justify-content:center;align-items:center;min-height:100vh;margin:0;text-align:center}}
        .container{{display:flex;flex-direction:column;align-items:center;margin:30px 0}}
        .color-block{{background:{hexColor};width:150px;height:150px;border-radius:4px;border:1px solid #505050}}
        .color-values{{display:grid;gap:10px;border-radius:8px;background:#141414;padding:1em;border:1px solid #505050;margin:20px 0 20px}}
        .value-line{{display:flex;align-items:center;height:1.5em;padding:0 1em}}
        .value-title{{font-weight:bold;margin-right:1em;width:3em;text-align:left;color:#909090}}
        .footer{{color:#707070;font-size:small}}
        a{{color:inherit;text-decoration:none}}
        a:hover{{color:#909090}}
        hr{{width:100%;margin:0;padding:0;border-color:#505050}}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""color-block""></div>
        <div class=""color-values"">
            <div class=""value-line""><span class=""value-title"">HEX</span>{hexColor}</div><hr>
            <div class=""value-line""><span class=""value-title"">RGB</span>{rgbColorString}</div><hr>
            <div class=""value-line""><span class=""value-title"">CMYK</span>{cmykColorString}</div><hr>
            <div class=""value-line""><span class=""value-title"">HSL</span>{hslColorString}</div><hr>
            <div class=""value-line""><span class=""value-title"">HSV</span>{hsvColorString}</div>
        </div>
        <div class=""footer"">
            <a href=""https://github.com/justinnas/StarHue-Color-Picker"" target=""_blank""><strong>{MainWindow.appName} v{MainWindow.appVersion}</strong> © {MainWindow.appDeveloper}</a>
        </div>
    </div>
</body>
</html>
";
            File.WriteAllText(outputPath, htmlContent);
        }

        private static string GenerateFavicon(string hexColor)
        {
            Color color = ColorTranslator.FromHtml(hexColor);

            using (var bitmap = new Bitmap(64, 64))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(color);
                }
                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }
}
