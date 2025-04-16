using System.Reflection;

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

        public static class I
        {
            public static FrameMaterial I150 => CreateMaterial(FrameMaterialType.I, 150, 75, 5, 2);
            public static FrameMaterial I200 => CreateMaterial(FrameMaterialType.I, 200, 100, 8, 2);
            public static FrameMaterial I250 => CreateMaterial(FrameMaterialType.I, 250, 125, 10, 2);
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

        public static IEnumerable<FrameMaterial> GetAllFrameMaterialSizes()
        {
            // Using reflection to iterate from all frame material sizes
            Type frameMaterialHelper = typeof(FrameMaterialHelper);

            // Nested classes are the member cross section type (i.e. C or Z channels)
            Type[] crossSectionTypes = frameMaterialHelper.GetNestedTypes();

            foreach (Type crossSectionType in crossSectionTypes)
            {
                string memberType = crossSectionType.Name;

                // Inner loop are the exact sizes (100, 200, 300 etc.)
                foreach (PropertyInfo crossSectionSize in crossSectionType.GetProperties(BindingFlags.Public | BindingFlags.Static))
                {
                    if (crossSectionSize.GetValue(null) is FrameMaterial value)
                    {
                        yield return value;
                    }
                }
            }
        }
    }
}
