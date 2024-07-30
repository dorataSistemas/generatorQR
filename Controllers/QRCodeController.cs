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

        // Set the path for the logo
        string logoPath = Path.Combine("C:\\Users\\USER\\OneDrive\\Documents", "logo.png");
        if (!System.IO.File.Exists(logoPath))
        {
            return BadRequest("El archivo de logo no se encuentra en la ruta especificada.");
        }

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

            // Load the logo
            using (var logo = new Bitmap(logoPath))
            {
                // Calculate the position and size of the logo
                int logoSize = qrCodeImage.Width / 5; // Adjust size as needed
                int logoX = (qrCodeImage.Width - logoSize) / 2;
                int logoY = (qrCodeImage.Height - logoSize) / 2;

                using (Graphics graphics = Graphics.FromImage(qrCodeImage))
                {
                    // Draw the logo on the QR code
                    graphics.DrawImage(logo, new Rectangle(logoX, logoY, logoSize, logoSize));
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