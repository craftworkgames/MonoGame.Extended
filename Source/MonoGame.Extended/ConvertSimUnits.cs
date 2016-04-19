using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class ConvertSimUnits
    {
        public static float DisplayToSimRatio { get; private set; } = 100f;
        public static float SimToDisplayRatio { get; private set; } = 1 / DisplayToSimRatio;

        public static void SetDisplayToSimRatio(float ratio)
        {
            DisplayToSimRatio = ratio;
            SimToDisplayRatio = 1 / ratio;
        }

        public static float ToDisplayUnits(int simUnits)
        {
            return simUnits * DisplayToSimRatio;
        }

        public static float ToDisplayUnits(float simUnits)
        {
            return simUnits * DisplayToSimRatio;
        }

        public static double ToDisplayUnits(double simUnits)
        {
            return simUnits * DisplayToSimRatio;
        }

        public static Vector2 ToDisplayUnits(Vector2 simUnits)
        {
            return simUnits * DisplayToSimRatio;
        }

        public static Vector3 ToDisplayUnits(Vector3 simUnits)
        {
            return simUnits * DisplayToSimRatio;
        }

        public static float ToSimUnits(int displayUnits)
        {
            return displayUnits * SimToDisplayRatio;
        }

        public static float ToSimUnits(float displayUnits)
        {
            return displayUnits * SimToDisplayRatio;
        }

        public static double ToSimUnits(double displayUnits)
        {
            return displayUnits * SimToDisplayRatio;
        }

        public static Vector2 ToSimUnits(Vector2 displayUnits)
        {
            return displayUnits * SimToDisplayRatio;
        }

        public static Vector3 ToSimUnits(Vector3 displayUnits)
        {
            return displayUnits * SimToDisplayRatio;
        }
    }
}
