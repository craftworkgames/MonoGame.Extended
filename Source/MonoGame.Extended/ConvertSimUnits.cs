using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class ConvertSimUnits
    {
        private static float _displayToSimRatio = 100f;
        private static float _simToDisplayRatio = 1 / _displayToSimRatio;

        public static void SetDisplayToSimRatio(float ratio)
        {
            _displayToSimRatio = ratio;
            _simToDisplayRatio = 1 / ratio;
        }

        public static float ToDisplayUnits(int simUnits)
        {
            return simUnits * _displayToSimRatio;
        }

        public static float ToDisplayUnits(float simUnits)
        {
            return simUnits * _displayToSimRatio;
        }

        public static double ToDisplayUnits(double simUnits)
        {
            return simUnits * _displayToSimRatio;
        }

        public static Vector2 ToDisplayUnits(Vector2 simUnits)
        {
            return simUnits * _displayToSimRatio;
        }

        public static Vector3 ToDisplayUnits(Vector3 simUnits)
        {
            return simUnits * _displayToSimRatio;
        }

        public static float ToSimUnits(int displayUnits)
        {
            return displayUnits * _simToDisplayRatio;
        }

        public static float ToSimUnits(float displayUnits)
        {
            return displayUnits * _simToDisplayRatio;
        }

        public static double ToSimUnits(double displayUnits)
        {
            return displayUnits * _simToDisplayRatio;
        }

        public static Vector2 ToSimUnits(Vector2 displayUnits)
        {
            return displayUnits * _simToDisplayRatio;
        }

        public static Vector3 ToSimUnits(Vector3 displayUnits)
        {
            return displayUnits * _simToDisplayRatio;
        }
    }
}
