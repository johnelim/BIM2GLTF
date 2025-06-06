﻿using System.Numerics;

namespace BimBuilder.Classes
{
    public class BimContainer
    {
        public BimSlab Slab { get; set; }
        public List<BimFooting> Footings { get; set; }
        public List<BimCladding> Walls { get; set; }
        public List<BimCladding> Roofs { get; set; }
        public List<BimFrameMember> FrameMembers { get; set; }

    }

    public class BimFrameMember
    {
        public double Rotation { get; set; }
        public Vector3 StartPt { get; set; }
        public Vector3 EndPt { get; set; }
        public FrameMaterial FrameMaterial { get; set; }
    }

    public class BimSlab
    {
        public double Depth { get; set; }
        public List<Vector2> Outline { get; set; }
    }

    public class BimFooting
    {
        public Vector2 Position { get; set; }
        public double Depth { get; set; }
        public double Size { get; set; }
        public FootingType Type { get; set; }
    }

    public enum FootingType
    {
        Block, // Square
        Bored // Circular
    }

    public class BimCladding
    {
        public Vector3 StartPt { get; set; }
        public Vector3 EndPt { get; set; }
        public CladdingMaterial CladdingMaterial { get; set; }
    }
}
