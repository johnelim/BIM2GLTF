using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bim2Gltf.Core
{
    public static class FrameMaterialHelper
    {
        public static class C
        {
            public static FrameMaterial C100 => CreateMaterial(FrameMaterialType.C, 100, 50, 15, 2);
            public static FrameMaterial C200 => CreateMaterial(FrameMaterialType.C, 200, 75, 20, 2);
            public static FrameMaterial C250 => CreateMaterial(FrameMaterialType.C, 250, 75, 20, 2);
        }

        public static class Z
        {
            public static FrameMaterial Z100 => CreateMaterial(FrameMaterialType.Z, 100, 50, 15, 2);
            public static FrameMaterial Z200 => CreateMaterial(FrameMaterialType.Z, 200, 75, 20, 2);
            public static FrameMaterial Z250 => CreateMaterial(FrameMaterialType.Z, 250, 75, 20, 2);
        }

        public static FrameMaterial CreateMaterial(FrameMaterialType t, double web, double flange, double lip, double wallThickness)
        {
            return new FrameMaterial()
            {
                FrameMaterialType = t,
                Web = web,
                Flange = flange,
                Lip = lip,
                WallThickness = wallThickness,
            };
        }
    }

    public class FrameMaterial
    {
        public FrameMaterialType FrameMaterialType { get; set; }
        public double Web { get; set; }
        public double Flange { get; set; }
        public double Lip { get; set; }
        public double WallThickness { get; set; } = 2.0;
    }

    public enum FrameMaterialType
    {
        C,
        Z,
        I
    }
}
