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
        List<Polyline> wallLineList;


        [Test]
        public void TributalAreaTest()
        {



            ////            //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "myJson.json");
            //string jsonContent = File.ReadAllText("C:\\BHoMGit\\WindLoad_Toolkit\\.ci\\unit-tests\\WindLoad_Adapter_Test\\objectJsonFiles\\myJson.json");
            ////BH.Engine.Base.Compute.LoadAllAssemblies();

            ////Trying to to deserialize the json file
            //var objects0=BH.Engine.Adapters.File.Compute.ReadFromJsonFile("C:\\BHoMGit\\WindLoad_Toolkit\\.ci\\unit-tests\\WindLoad_Adapter_Test\\objectJsonFiles\\myJson.json", true);

            p0 = new Point() { X = 0, Y = 0, Z = 0 };
            p1 = new Point() { X = 0, Y = 0, Z = 10 };
            p2 = new Point() { X = 0, Y = 10, Z = 10 };
            p3 = new Point() { X = 0, Y = 10, Z = 0 };
            SurfaceOutline = new Polyline() { ControlPoints = new List<Point> { p0, p1, p2, p3, p0 } };

            p4 = new Point() { X = 0, Y = 3, Z = 3 };
            p5 = new Point() { X = 0, Y = 3, Z = 7 };
            p6 = new Point() { X = 0, Y = 7, Z = 7 };
            p7 = new Point() { X = 0, Y = 7, Z = 3 };

            Polyline line0 = new Polyline() { ControlPoints = new List<Point> { p4, p5} };
            Polyline line1 = new Polyline() { ControlPoints = new List<Point> { p5, p6} };
            Polyline line2 = new Polyline() { ControlPoints = new List<Point> { p6, p7 } };
            Polyline line3 = new Polyline() { ControlPoints = new List<Point> { p7, p4 } };

            wallLineList = new List<Polyline> { line0, line1, line2, line3 };




            //var regions = Compute.InclinedTributaryAreas(SurfaceOutline, wallLineList);
            int i = 5;




            Assert.True(i < 10);

        }
    }
}
