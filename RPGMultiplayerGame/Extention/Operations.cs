using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Ui.UiComponent;

namespace RPGMultiplayerGame.Extention
{
    public class Operations
    {
        public static Vector2 GetTopLeftPositionByPorsitionType(PositionType positionType, Vector2 origin, Vector2 size)
        {
            return positionType switch
            {
                PositionType.Centered => origin - size / 2,
                PositionType.ButtomLeft => origin - new Vector2(0, size.Y),
                PositionType.ButtomCentered => origin - new Vector2(size.X / 2, size.Y),
                PositionType.TopLeft => origin,
                PositionType.TopRight => origin - new Vector2(size.X, 0),
                PositionType.CenteredLeft => origin - new Vector2(0, size.Y / 2),
                PositionType.ButtomRight => origin - new Vector2(size.X, size.Y),
                PositionType.CenteredRight => origin - new Vector2(size.X, size.Y / 2),
                PositionType.TopCentered => origin - new Vector2(size.X / 2, 0),
                PositionType.None => origin,
                _ => throw new NotImplementedException("Unknown case in: " + nameof(GetTopLeftPositionByPorsitionType)),
            };
        }

        public static Vector2 GetPositionByTopLeftPosition(PositionType positionType, Vector2 origin, Vector2 size)
        {
            return positionType switch
            {
                PositionType.Centered => origin + size / 2,
                PositionType.ButtomLeft => origin + new Vector2(0, size.Y),
                PositionType.ButtomCentered => origin + new Vector2(size.X / 2, size.Y),
                PositionType.TopLeft => origin,
                PositionType.TopRight => origin + new Vector2(size.X, 0),
                PositionType.CenteredLeft => origin + new Vector2(0, size.Y / 2),
                PositionType.ButtomRight => origin + new Vector2(size.X, size.Y),
                PositionType.CenteredRight => origin + new Vector2(size.X, size.Y / 2),
                PositionType.TopCentered => origin + new Vector2(size.X / 2, 0),
                PositionType.None => origin,
                _ => throw new NotImplementedException("Unknown case in: " + nameof(GetPositionByTopLeftPosition)),
            };
        }

        public static Texture2D TintTextureByColor(GraphicsDevice graphicsDevice, Texture2D texture, Color color, float tintingAlpah)
        {
            if (color == Color.White)
            {
                return texture;
            }

            int pixelCount = texture.Width * texture.Height;
            Color[] pixels = new Color[pixelCount];
            texture.GetData(pixels);
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].A < 10)
                {
                    continue;
                }
                byte r = (byte)Math.Min(pixels[i].R * tintingAlpah + color.R * (1 - tintingAlpah), 255);
                byte g = (byte)Math.Min(pixels[i].G * tintingAlpah + color.G * (1 - tintingAlpah), 255);
                byte b = (byte)Math.Min(pixels[i].B * tintingAlpah + color.B * (1 - tintingAlpah), 255);
                pixels[i] = new Color(r, g, b, pixels[i].A);
            }
            Texture2D outTexture = new Texture2D(graphicsDevice, texture.Width, texture.Height, false, SurfaceFormat.Color);
            outTexture.SetData<Color>(pixels);
            return outTexture;
        }


        public static void DoTaskWithDelay(Action action, float delaySec)
        {
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep((int) (delaySec * 1000));
                action();
            })).Start();
        }
    }
}
