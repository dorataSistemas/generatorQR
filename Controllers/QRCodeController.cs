using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using ZXing.Windows.Compatibility;

[ApiController]
[Route("api/[controller]")]
public class QRCodeController : ControllerBase
{
    [HttpPost("generate")]
    public IActionResult GenerateQRCode([FromBody] QRCodeRequest request)
    {
        if (string.IsNullOrEmpty(request.Link))
        {
            return BadRequest("Es obligatorio completar el campo 'Link'.");
        }

        // Convert hex color to RGB
        Color qrColor = ColorTranslator.FromHtml(request.ColorHex);

        // Create a QR code writer
        var writer = new QRCodeWriter();
        var matrix = writer.encode(request.Link, BarcodeFormat.QR_CODE, 300, 300);

        // Create a bitmap to draw the QR code
        using (var qrCodeImage = new Bitmap(300, 300))
        {
            for (int x = 0; x < matrix.Width; x++)
            {
                for (int y = 0; y < matrix.Height; y++)
                {
                    qrCodeImage.SetPixel(x, y, matrix[x, y] ? Color.Black : Color.White);
                }
            }

            // Convert the image to a byte array
            using (MemoryStream ms = new MemoryStream())
            {
                qrCodeImage.Save(ms, ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();

                // Return the image as a FileContentResult
                return File(imageBytes, "image/png", $"qrcode-{Guid.NewGuid()}.png");
            }
        }
    }
}

public class QRCodeRequest
{
    public string Link { get; set; } = "";
    public string ColorHex { get; set; } = "#000000";
}