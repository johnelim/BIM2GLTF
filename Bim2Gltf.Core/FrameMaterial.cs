namespace Bim2Gltf.Core
{
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
