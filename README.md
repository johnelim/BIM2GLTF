# BIM2GLTF

**BIM2GLTF** is a .NET 8.0 toolkit for converting BIM (IFC) models to GLTF format.  
It leverages the **xBIM Toolkit** for parsing and tessellating IFC geometries, **SharpGLTF** for generating GLTF assets, and **GeometryGym** for creating sample IFC files.

---
## ⚠️ Platform Note

> ❗ **xBIM Toolkit relies on Windows-only components** (e.g., `System.Windows.Media.Media3D` for geometry processing).  
> As a result, this project **must be run on Windows** for full functionality.

---

## 📁 Projects

### `Bim2Gltf.Core`
> Core conversion library using:

- ✅ [.NET 8.0](https://dotnet.microsoft.com/)
- 🧰 [xBIM Toolkit](https://github.com/xBimTeam/XbimEssentials) for IFC parsing and geometry tessellation
- 🧩 [SharpGLTF](https://github.com/vpenades/SharpGLTF) for GLTF mesh generation
- 🏗️ [GeometryGym](https://github.com/jmirtsch/GeometryGymIFC) for generating and manipulating IFC files (test/demo data)

### `Bim2Gltf.Tests`
> MSTest-based unit tests and sample use-cases for validating conversion logic.

---

## 📦 Open Source NuGet Dependencies

- `Xbim.Essentials`
- `Xbim.Geometry`
- `SharpGLTF.Runtime`
- `GeometryGym.Ifc`

---

## 🚀 Getting Started

### Clone the Repo

```bash
git clone https://github.com/your-username/BIM2GLTF.git
cd BIM2GLTF
