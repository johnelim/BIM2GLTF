using GeometryGym.Ifc;
using System.Numerics;

namespace Bim2Gltf.Core
{
    public class IfcBuilder
    {
        private static DatabaseIfc db;
        private static IfcBuilding building;
        private static IfcProject project;

        public IfcBuilder(string description = "Sample")
        {
            db = new DatabaseIfc(ModelView.Ifc4NotAssigned);
            building = new IfcBuilding(db, $"{description} Building");
            project = new IfcProject(building, $"{description} Project");
        }

        public static string CreateSample_Columns()
        {
            IfcBuilder ifcBuilder = new IfcBuilder();

            double memberLength = 3000;
            Vector2 position = new Vector2(0, 0);
            Vector2 offset = new Vector2(3000, 0);

            IEnumerable<FrameMaterial> frameMaterials = FrameMaterialHelper.GetAllFrameMaterialSizes();
            for (int i = 0; i < frameMaterials.Count(); i++)
            {
                ifcBuilder.CreateStructuralMember(frameMaterials.ElementAt(i), position + offset * i, memberLength);
            }

            string outputFileName = "Sample Ifc - Structural Columns.ifc";
            db.WriteFile(outputFileName);
            return outputFileName;
        }

        public static string CreateSample_Cladding()
        {
            IfcBuilder ifcBuilder = new IfcBuilder();

            double claddingLength = 6000.0;
            Vector2 position = Vector2.Zero;

            ifcBuilder.CreateCladdingMember(CladdingMaterialHelper.Corrugated, position, claddingLength);

            string outputFileName = "Sample Ifc - Cladding.ifc";
            db.WriteFile(outputFileName);
            return outputFileName;
        }

        public static string Save()
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
            var segmentWidth = claddingMaterial.Profile.Last().X;
            var numOfSegments = claddingMaterial.CoverWidth / segmentWidth;

            List<IfcCartesianPoint> polyPts = new List<IfcCartesianPoint>();

            for (int i = 0; i < numOfSegments; i++)
            {
                var offsetX = i * segmentWidth;

                var profilePts = claddingMaterial.Profile.Select(p => new Vector2(p.X + offsetX, p.Y).ToCartesianPt(db));

                polyPts.AddRange(profilePts);
            }

            IfcPolyline polyCurve = new IfcPolyline(polyPts);

            IfcCenterLineProfileDef openProfileDef = new IfcCenterLineProfileDef(claddingMaterial.Name, polyCurve, claddingMaterial.Thickness);

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
                        frameMaterial.Web,
                        frameMaterial.Flange,
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
        #endregion
    }

    public static class GeometryGymExtensions
    {
        public static IfcCartesianPoint ToCartesianPt(this Vector2 p, DatabaseIfc db)
        {
            return new IfcCartesianPoint(db, p.X, p.Y);
        }
    }
}
