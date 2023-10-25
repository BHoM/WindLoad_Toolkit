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

namespace BH.Engine.Adapters.WindLoad
{
    public static partial class Compute
    {


        public static List<PolyCurve> InclinedTributaryAreas(Polyline inputOutline, List<Line> wallsLines)
        {
            if (!inputOutline.IsClosed()) { BH.Engine.Base.Compute.RecordWarning($"The Polyline {inputOutline} is not closed! If possible the curve will be closed!"); };
            inputOutline = inputOutline.Close();

            var joindWallLines = wallsLines.Join();

            //Definition Rotation parameters
            BH.oM.Geometry.Vector zAxis = new Vector() { X = 0, Y = 0, Z = 1 };
            var normalOutline = BH.Engine.Geometry.Query.Normal(inputOutline);
            var angle = BH.Engine.Geometry.Query.Angle(normalOutline, zAxis);
            var rotationOrigin = BH.Engine.Geometry.Query.Centroid(inputOutline);
            var rotationAxis = BH.Engine.Geometry.Query.CrossProduct(zAxis, normalOutline);

            //Geometry Rotated into the XY Plane
            var outlineXY = BH.Engine.Geometry.Modify.Rotate(inputOutline, rotationOrigin, rotationAxis, angle);
            var wallsXY = joindWallLines.Select(x => BH.Engine.Geometry.Modify.Rotate(x, rotationOrigin, rotationAxis, angle)).ToList();


            //Parameter preparation for TributaryArea calculation
            IElement2D outlineXY2D = Engine.Geometry.Create.PlanarSurface(outlineXY);

            //TODO: Construction call not working yes --> fix this
            var concrete = BH.Engine.Library.Query.Match("Concrete", "C25/30", true, true).DeepClone() as IMaterialFragment;
            var construction = BH.Engine.Library.Query.Match("Constructions", "generic_partition", true, true) as Construction;
            Construction c = Engine.Physical.Create.Construction("");

            var ps = Engine.Geometry.Create.PlanarSurface(joindWallLines.First(), null);
            //PlanarSurface ps = new PlanarSurface() { ExternalBoundary = joindWallLines.First() };
            var ds2d = Engine.Geometry.Create.NewInternalElement2D(ps);


            //Generate Wall and extract Element2d and Flatten to list
            var wallsNested = wallsLines.Select(l => BH.Engine.Physical.Create.Wall(construction, l).InternalElements2D()).ToList();
            var wallsFlat = wallsNested.SelectMany(x => x).ToList();

            //var tributaryAreasXY = Structure.Compute.TributaryAreas(new List<IElement2D> { outlineXY2D }, new List<IElement1D>(), new List<IElement1D>(), wallsFlat, true, true, 50, TributaryAreaMethod.Voronoi, Tolerance.Distance, Tolerance.MacroDistance, Math.PI / 180 * 3);

            var tributaryAreasXY = Structure.Compute.TributaryAreas(new List<IElement2D> { outlineXY2D }, new List<IElement1D>(), new List<IElement1D>(), new List<IElement2D> { ds2d }, true, true, 50, TributaryAreaMethod.Voronoi, Tolerance.Distance, Tolerance.MacroDistance, Math.PI / 180 * 3);

            //Process TributaryAreas into Surface Outlines
            var tributaryAreaXYList = tributaryAreasXY.Item1.SelectMany(x => x).ToList();
            IEnumerable<IReadOnlyList<PlanarSurface>> k = tributaryAreaXYList.Select(t => t.Regions);
            List<PlanarSurface> flatList = k.SelectMany(x => x).ToList();
            var curveOutline = flatList.Select(x => x.OutlineCurve()).ToList();

            List<PolyCurve> resultCurves = curveOutline.Select(o => BH.Engine.Geometry.Modify.Rotate(o, rotationOrigin, rotationAxis, -angle)).ToList();

            //Rotate back to original position
            var RegionBackRot = BH.Engine.Geometry.Modify.Rotate(inputOutline, rotationOrigin, rotationAxis, -angle);




            return resultCurves;
        }

    }
}

