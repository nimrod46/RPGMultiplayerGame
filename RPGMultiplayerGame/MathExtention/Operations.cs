using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Ui.UiComponent;

namespace RPGMultiplayerGame.MathExtention
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
    }
}
