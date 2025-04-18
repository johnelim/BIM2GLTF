using System.Numerics;

namespace BimBuilder
{
    public static class CladdingProfileHelper
    {
        public static Vector2[] CreateCorrugated(double profileHeight, int segments = 8)
        {
            var numOfPts = segments + 1;
            var profile = new Vector2[numOfPts];
            var step = Math.PI * 2 / segments;

            for (int i = 0; i < numOfPts; i++)
            {
                var a = step * i;
                var x = a * profileHeight;
                var y = (1 + Math.Sin(a)) * profileHeight / 2;
                profile[i] = new Vector2((float)x, (float)y);
            }

            return profile;
        }
    }
}
