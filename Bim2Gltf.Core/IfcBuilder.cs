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

        public static string CreateSample()
        {
            IfcBuilder builder = new IfcBuilder();

            var outputFilePath = Save();

            return outputFilePath;
        }

        public static string Save()
        {
            var tempFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".ifc");

            using (var stream = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                db.WriteStream(stream, project.Name);
            }

            return tempFilePath;
        }

        #region Industry Foundation Classes Domain-specific

        public IfcProfileDef Create2DProfile(FrameMaterial frameMaterial)
        {
            var frameType = frameMaterial.FrameMaterialType.ToString();

            if (frameType.StartsWith("c", StringComparison.OrdinalIgnoreCase))
            {
                return new IfcCShapeProfileDef(
                    db,
                    "C" + $"{frameMaterial.Web}{frameMaterial.Flange}{frameMaterial.Lip}",
                    frameMaterial.Web,
                    frameMaterial.Flange,
                    frameMaterial.WallThickness,
                    frameMaterial.Lip);
            }

            return new IfcZShapeProfileDef(
                db,
                "Z" + $"{frameMaterial.Web}{frameMaterial.Flange}{frameMaterial.Lip}",
                frameMaterial.Web,
                frameMaterial.Flange,
                frameMaterial.WallThickness,
                frameMaterial.WallThickness);
        }

        public void CreateMember(FrameMaterial frameMaterial, Vector2 position)
        {
            // Define profile 2D
            IfcProfileDef profile2D = Create2DProfile(frameMaterial);

            // Define profile 3D by extrusion
            var profile3D = new IfcExtrudedAreaSolid(profile2D, 5000);

            // Use 3D profile as shape representation from extrusion
            var shapeRep = new IfcShapeRepresentation(profile3D);

            // Create a product definition shape
            var productRep = new IfcProductDefinitionShape(shapeRep);

            // Create local object placement
            var placement = new IfcLocalPlacement(new IfcAxis2Placement2D(new IfcCartesianPoint(db, position.X, position.Y)));

            // Create a member
            var ifcMember = new IfcMember(building, placement, productRep);
        }
        #endregion
    }
}
