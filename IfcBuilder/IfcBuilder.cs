using BimBuilder.Classes;
using GeometryGym.Ifc;
using System.Numerics;

namespace BimBuilder
{
    public class IfcBuilder
    {
        private DatabaseIfc db;
        private IfcBuilding building;
        private IfcProject project;

        public IfcBuilder(string description = "Sample")
        {
            db = new DatabaseIfc(ModelView.Ifc4NotAssigned);
            building = new IfcBuilding(db, $"{description} Building");
            project = new IfcProject(building, $"{description} Project");
        }

        public void CreateSample(BimContainer bimContainer)
        {
            CreateSlabMember(bimContainer.Slab.Outline, bimContainer.Slab.Depth);
            CreateFootings(bimContainer.Footings);
            bimContainer.FrameMembers.ForEach(m => CreateStructuralMember(m.FrameMaterial, m.StartPt, m.EndPt, m.Rotation));
            bimContainer.Roofs.ForEach(r => CreateCladdingMember(r.CladdingMaterial, r.StartPt, r.EndPt, 0));
        }

        public void CreateSample_Columns()
        {
            double memberLength = 3000;
            Vector2 position = new Vector2(0, 0);
            Vector2 offset = new Vector2(3000, 0);

            IEnumerable<FrameMaterial> frameMaterials = FrameMaterialHelper.GetAllFrameMaterialSizes();
            for (int i = 0; i < frameMaterials.Count(); i++)
            {
                CreateStructuralMember(frameMaterials.ElementAt(i), position + offset * i, memberLength);
            }
        }

        public void CreateSample_Cladding()
        {
            double claddingLength = 6000.0;
            Vector2 position = Vector2.Zero;

            CreateCladdingMember(CladdingMaterialHelper.Corrugated, position, claddingLength);
        }

        public void CreateSample_Cladding2()
        {
            Vector3 startPt = Vector3.Zero;
            Vector3 endPt = new Vector3(1000, 1000, 1000);

            CreateCladdingMember(CladdingMaterialHelper.Corrugated, startPt, endPt, 0);
        }

        public string Save()
        {
            string tempFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".ifc");

            using (FileStream stream = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                db.WriteStream(stream, project.Name);
            }

            return tempFilePath;
        }

        #region Industry Foundation Classes Domain-specific

        private IfcProfileDef Create2DProfile(CladdingMaterial claddingMaterial)
        {
            float segmentWidth = claddingMaterial.Profile.Last().X;
            double numOfSegments = claddingMaterial.CoverWidth / segmentWidth;

            List<IfcCartesianPoint> polyPts = new List<IfcCartesianPoint>();

            for (int i = 0; i < numOfSegments; i++)
            {
                float offsetX = i * segmentWidth;

                IEnumerable<IfcCartesianPoint> profilePts = claddingMaterial.Profile.Select(p => new Vector2(p.X + offsetX, p.Y).ToCartesianPt(db));

                polyPts.AddRange(profilePts);
            }

            IfcPolyline polyline = new IfcPolyline(polyPts);

            IfcCenterLineProfileDef openProfileDef = new IfcCenterLineProfileDef(claddingMaterial.Name, polyline, claddingMaterial.Thickness);

            return openProfileDef;
        }

        private IfcProfileDef Create2DProfile(FrameMaterial frameMaterial)
        {
            switch (frameMaterial.FrameMaterialType)
            {
                case FrameMaterialType.C:
                    return new IfcCShapeProfileDef(
                    db,
                    "C" + $"{frameMaterial.Web}{frameMaterial.Flange}{frameMaterial.Lip}",
                    frameMaterial.Web,
                    frameMaterial.Flange,
                    frameMaterial.WallThickness,
                    frameMaterial.Lip);

                case FrameMaterialType.Z:
                    return new IfcZShapeProfileDef(
                        db,
                        "Z" + $"{frameMaterial.Web}{frameMaterial.Flange}{frameMaterial.Lip}",
                        frameMaterial.Web,
                        frameMaterial.Flange,
                        frameMaterial.WallThickness,
                        frameMaterial.WallThickness);

                case FrameMaterialType.I:
                    return new IfcIShapeProfileDef(
                        db,
                        "I" + $"{frameMaterial.Web}{frameMaterial.Flange}{frameMaterial.Lip}",
                        frameMaterial.Flange,
                        frameMaterial.Web,
                        frameMaterial.WallThickness,
                        frameMaterial.WallThickness);
                default:
                    throw new NotImplementedException("Invalid frame material type");
            }
        }

        private void CreateStructuralMember(FrameMaterial frameMaterial, Vector2 position, double length)
        {
            // Define profile 2D
            IfcProfileDef profile2D = Create2DProfile(frameMaterial);

            // Define profile 3D by extrusion
            IfcExtrudedAreaSolid profile3D = new IfcExtrudedAreaSolid(profile2D, length);

            // Use 3D profile as shape representation from extrusion
            IfcShapeRepresentation shapeRep = new IfcShapeRepresentation(profile3D);

            // Create a product definition shape
            IfcProductDefinitionShape productRep = new IfcProductDefinitionShape(shapeRep);

            // Create local object placement
            IfcLocalPlacement placement = new IfcLocalPlacement(new IfcAxis2Placement2D(new IfcCartesianPoint(db, position.X, position.Y)));

            // Create a member
            IfcMember ifcMember = new IfcMember(building, placement, productRep);
        }

        private void CreateStructuralMember(FrameMaterial frameMaterial, Vector3 startPt, Vector3 endPt, double rotDeg)
        {
            // Define profile 2D
            IfcProfileDef profile2D = Create2DProfile(frameMaterial);

            // Define profile 3D by extrusion
            IfcExtrudedAreaSolid profile3D = new IfcExtrudedAreaSolid(profile2D, (endPt - startPt).Length());

            // Use 3D profile as shape representation from extrusion
            IfcShapeRepresentation shapeRep = new IfcShapeRepresentation(profile3D);

            // Create a product definition shape
            IfcProductDefinitionShape productRep = new IfcProductDefinitionShape(shapeRep);

            // Create local object placement
            IfcLocalPlacement placement = new IfcLocalPlacement(CreateAxis2Placement(startPt, endPt, rotDeg));

            // Create a member
            IfcMember ifcMember = new IfcMember(building, placement, productRep);
        }

        private void CreateCladdingMember(CladdingMaterial cladding, Vector2 position, double length)
        {
            IfcProfileDef profile2D = Create2DProfile(cladding);

            IfcExtrudedAreaSolid profile3D = new IfcExtrudedAreaSolid(profile2D, length);

            IfcShapeRepresentation shapeRep = new IfcShapeRepresentation(profile3D);

            IfcProductDefinitionShape productRep = new IfcProductDefinitionShape(shapeRep);

            // Place horizontally
            IfcCartesianPoint localPosition = new IfcCartesianPoint(db, position.X, position.Y);
            IfcDirection refDirection = new IfcDirection(db, 0, -1, 0);
            IfcDirection axis = new IfcDirection(db, 1, 0, 0);
            IfcAxis2Placement3D placement = new IfcAxis2Placement3D(localPosition, axis, refDirection);
            IfcLocalPlacement objectPlacement = new IfcLocalPlacement(placement);

            IfcCovering ifcCovering = new IfcCovering(building, objectPlacement, productRep);
        }

        private void CreateCladdingMember(CladdingMaterial cladding, Vector3 startPt, Vector3 endPt, double rotDeg)
        {
            IfcProfileDef profile2D = Create2DProfile(cladding);

            IfcExtrudedAreaSolid profile3D = new IfcExtrudedAreaSolid(profile2D, (endPt - startPt).Length());

            IfcShapeRepresentation shapeRep = new IfcShapeRepresentation(profile3D);

            IfcProductDefinitionShape productRep = new IfcProductDefinitionShape(shapeRep);

            IfcAxis2Placement3D axis2Placement = CreateAxis2Placement(startPt, endPt, rotDeg);

            IfcLocalPlacement objectPlacement = new IfcLocalPlacement(axis2Placement);

            IfcCovering ifcCovering = new IfcCovering(building, objectPlacement, productRep);
        }

        private void CreateSlabMember(List<Vector2> outline, double depth)
        {
            IfcArbitraryClosedProfileDef profile2D = new IfcArbitraryClosedProfileDef("Slab", outline.ToPolyline(db));

            // We extruded to -Z axis
            IfcExtrudedAreaSolid profile3D = new IfcExtrudedAreaSolid(profile2D, depth);
            profile3D.ExtrudedDirection = new IfcDirection(db, 0, 0, -1);

            IfcShapeRepresentation shape = new IfcShapeRepresentation(profile3D);

            IfcProductDefinitionShape product = new IfcProductDefinitionShape(shape);

            IfcSlab slab = new IfcSlab(building, null, product);
        }

        private void CreateFootings(List<BimFooting> footings)
        {
            // all footings are identical, use a single representation
            var profile2D = new IfcRectangleProfileDef(db, "footing", footings[0].Size, footings[0].Size);

            // We extruded to -Z axis
            IfcExtrudedAreaSolid profile3D = new IfcExtrudedAreaSolid(profile2D, footings[0].Depth);
            profile3D.ExtrudedDirection = new IfcDirection(db, 0, 0, -1);

            IfcShapeRepresentation shape = new IfcShapeRepresentation(profile3D);

            IfcProductDefinitionShape product = new IfcProductDefinitionShape(shape);

            // Use local placement to "copy" the footings
            for (int i = 0; i < footings.Count; i++)
            {
                var footingPosition = footings[i].Position;

                var placement = new IfcAxis2Placement2D(footingPosition.ToCartesianPt(db));

                IfcFooting footing = new IfcFooting(building, new IfcLocalPlacement(placement), product);
            }
        }

        /// <summary>
        /// Parametrically determine length from start and end points
        /// </summary>
        private IfcAxis2Placement3D CreateAxis2Placement(Vector3 startPt, Vector3 endPt, double rotDeg)
        {
            Vector3 extrusionVector = (endPt - startPt);
            float extrusionLength = extrusionVector.Length();

            Vector3 unitZ = Vector3.UnitZ;
            Vector3 newUnitZ = Vector3.Normalize(extrusionVector);
            var dot = Vector3.Dot(unitZ, newUnitZ);

            var refDirectionNormal = Vector3.UnitX;

            if (dot != 1)
            {
                double cosTheta = Math.Cos(rotDeg);
                double sinTheta = Math.Sin(rotDeg);
                Vector3 rotatedReferenceVector = Vector3.Multiply((float)cosTheta, unitZ) +
                    Vector3.Multiply((float)sinTheta, Vector3.Cross(newUnitZ, unitZ)) +
                    Vector3.Multiply((float)(Vector3.Dot(newUnitZ, unitZ) * (1 - cosTheta)), newUnitZ);

                Vector3 refDirection = Vector3.Cross(extrusionVector, Vector3.UnitZ);
                refDirectionNormal = Vector3.Normalize(refDirection);
            }

            IfcDirection axis = new IfcDirection(db, extrusionVector.X, extrusionVector.Y, extrusionVector.Z);
            IfcDirection refDir = new IfcDirection(db, refDirectionNormal.X, refDirectionNormal.Y, refDirectionNormal.Z);
            IfcCartesianPoint position = startPt.ToCartesianPt(db);

            return new IfcAxis2Placement3D(position, axis, refDir);
        }

        #endregion
    }

    public static class GeometryGymExtensions
    {
        public static IfcCartesianPoint ToCartesianPt(this Vector3 p, DatabaseIfc db)
        {
            return new IfcCartesianPoint(db, p.X, p.Y, p.Z);
        }

        public static IfcCartesianPoint ToCartesianPt(this Vector2 p, DatabaseIfc db)
        {
            return new IfcCartesianPoint(db, p.X, p.Y);
        }

        public static IfcPolyline ToPolyline(this List<Vector2> pts, DatabaseIfc db)
        {
            return new IfcPolyline(pts.Select(p => p.ToCartesianPt(db)));
        }
    }
}
