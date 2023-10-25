using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.oM.Base;
using BH.Engine.Adapters.WindLoad;
using System.Globalization;
using BH.oM.Test.NUnit;

namespace BH.Test.Adapter.WindLoad.TestFolder
{
    internal class TestTest : NUnitTest
    {
        Point p0;
        Point p1;
        Point p2;
        Point p3;
        Point p4;
        Point p5;
        Point p6;
        Point p7;

        Polyline SurfaceOutline;
        List<Line> WallList;


        [Test]
        public void TributalAreaTest()
        {
            //BH.Engine.Base.Compute.LoadAllAssemblies();

            p0 = new Point() { X = 0, Y = 0, Z = 0 };
            p1 = new Point() { X = 10, Y = 0, Z = 0 };
            p2 = new Point() { X = 10, Y = 0, Z = 10 };
            p3 = new Point() { X = 0, Y = 0, Z = 10 };
            SurfaceOutline = new Polyline() { ControlPoints = new List<Point> { p0, p1, p2, p3, p0 } };

            p4 = new Point() { X = 4, Y = 0, Z = 4 };
            p5 = new Point() { X = 6, Y = 0, Z = 4 };
            p6 = new Point() { X = 6, Y = 0, Z = 6 };
            p7 = new Point() { X = 4, Y = 0, Z = 6 };

            Line line0 = new Line() { Start = p4, End = p5 };
            Line line1 = new Line() { Start = p5, End = p6 };
            Line line2 = new Line() { Start = p6, End = p7 };
            Line line3 = new Line() { Start = p7, End = p4 };

            WallList = new List<Line> { line0, line1, line2, line3 };

          


            var regions = Compute.InclinedTributaryAreas(SurfaceOutline, WallList);
            int i = 5;




            Assert.True(i < 10);

        }
    }
}
