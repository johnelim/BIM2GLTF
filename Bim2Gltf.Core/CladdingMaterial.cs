using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Bim2Gltf.Core
{
    public class CladdingMaterial
    {
        public string Name { get; set; }
        public double CoverWidth { get; set; }
        public double RibHeight { get; set; }
        public double Thickness { get; set; }
        public Vector2[] Profile { get; set; }
    }

    public static class CladdingMaterialHelper
    {
        public static CladdingMaterial Corrugated = new CladdingMaterial
        {
            Name = "Corrugated",
            CoverWidth = 800,
            RibHeight = 20,
            Thickness = 1,
            Profile = CladdingProfileHelper.CreateCorrugated(20)
        };
    }

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
