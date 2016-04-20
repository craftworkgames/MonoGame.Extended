using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class ConvertSimUnits
    {
        public static float SimToDisplayRatio { get; private set; } = 100f;
        public static float DisplayToSimRatio { get; private set; } = 1 / SimToDisplayRatio;

        public static void SetSimToDisplayRatio(float ratio)
        {
            SimToDisplayRatio = ratio;
            DisplayToSimRatio = 1 / ratio;
        }

        public static float ToDisplayUnits(int simUnits)
        {
            return simUnits * SimToDisplayRatio;
        }

        public static float ToDisplayUnits(float simUnits)
        {
            return simUnits * SimToDisplayRatio;
        }

        public static double ToDisplayUnits(double simUnits)
        {
            return simUnits * SimToDisplayRatio;
        }

        public static Vector2 ToDisplayUnits(Vector2 simUnits)
        {
            return simUnits * SimToDisplayRatio;
        }

        public static Vector3 ToDisplayUnits(Vector3 simUnits)
        {
            return simUnits * SimToDisplayRatio;
        }

        public static float ToSimUnits(int displayUnits)
        {
            return displayUnits * DisplayToSimRatio;
        }

        public static float ToSimUnits(float displayUnits)
        {
            return displayUnits * DisplayToSimRatio;
        }

        public static double ToSimUnits(double displayUnits)
        {
            return displayUnits * DisplayToSimRatio;
        }

        public static Vector2 ToSimUnits(Vector2 displayUnits)
        {
            return displayUnits * DisplayToSimRatio;
        }

        public static Vector3 ToSimUnits(Vector3 displayUnits)
        {
            return displayUnits * DisplayToSimRatio;
        }
    }
}
