/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.oM.Structure;
using BH.Engine.Geometry;
//using BH.Engine.Structure;
using BH.oM.Dimensional;
using BH.oM.Physical.Elements;
using BH.Engine.Physical;
using BH.oM.Structure.Calculations;
using BH.oM.Structure.SectionProperties;
using BH.oM.Physical.Constructions;
using BH.Engine.Base;
using BH.Engine.Spatial;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Loads;

namespace BH.Engine.Adapters.WindLoad
{
    public static partial class Compute
    {


        public static List<PlanarSurface> InclinedTributaryAreas(Polyline inputOutline, List<Polyline> wallsLines/*, Construction construc*/)
        {
            if (!inputOutline.IsClosed()) { BH.Engine.Base.Compute.RecordWarning($"The Polyline {inputOutline} is not closed! If possible the curve will be closed!"); };
            inputOutline = inputOutline.Close();

            var joindWallLines = wallsLines.Join();

            //Definition Rotation parameters
            BH.oM.Geometry.Vector zAxis = new Vector() { X = 0, Y = 0, Z = 1 };
            var normalOutline = BH.Engine.Geometry.Query.Normal(inputOutline);
            var angle = BH.Engine.Geometry.Query.Angle(normalOutline, zAxis);
            var rotationOrigin = BH.Engine.Geometry.Query.Centroid(inputOutline);
            var rotationAxis = BH.Engine.Geometry.Query.CrossProduct(normalOutline, zAxis);

            //Geometry Rotated into the XY Plane
            var outlineXY = BH.Engine.Geometry.Modify.Rotate(inputOutline, rotationOrigin, rotationAxis, angle);
            var wallsXYLines = wallsLines.Select(x => BH.Engine.Geometry.Modify.Rotate(x, rotationOrigin, rotationAxis, angle)).ToList();

            //Parameter preparation for TributaryArea calculation
            IElement2D outlineXY2D = Engine.Geometry.Create.PlanarSurface(outlineXY);

            //Generate Wall and extract Element2d and Flatten to list
            var construction = BH.Engine.Library.Query.Match("BuildingEnvironments", "generic_partition"
, true, true).DeepClone() as Construction;
            var wallList = wallsXYLines.Select(l => BH.Engine.Physical.Create.Wall(new Line() { Start = l.ControlPoints().First(), End = l.ControlPoints().Last() }, -1, construction, Offset.Undefined, "")).ToList();
            var wall2DElements = wallList.Select(k => (IElement2D)k).ToList();


            var tributaryAreasXY = Structure.Compute.TributaryAreas(new List<IElement2D> { outlineXY2D }, new List<IElement1D>(), new List<IElement1D>(), wall2DElements, true, true, 50, TributaryAreaMethod.Voronoi, Tolerance.Distance, Tolerance.MacroDistance, Math.PI / 180 * 3);

            //Process TributaryAreas into Surface Outlines
            List<PlanarSurface> tributaryAreaXYRegions = tributaryAreasXY.Item1[0].Select(x => (x as TributaryRegion).Regions[0]).ToList();

            //Rotate back to original position
            tributaryAreaXYRegions = tributaryAreaXYRegions.Select(a => BH.Engine.Geometry.Modify.Rotate(a, rotationOrigin, rotationAxis, -angle)).ToList();

            return tributaryAreaXYRegions;
        }

    }
}

