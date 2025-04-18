using System.Numerics;

namespace BimBuilder.Classes
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
}
