using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class ScenarioTests
    {

        private int _defaultMagneticVariation = -20;

        [TestMethod]
        public void TestTrueVsMagnetic()
        {
            
            Runway runway = Runway.FromMagnetic(340, _defaultMagneticVariation);
            Wind wind = Wind.FromMagnetic(200, _defaultMagneticVariation, 10);
            
            Scenario scenario = new Scenario(runway, wind);

            // we said 340 magnetic for the runway, so with -20 mag var
            // the true direction for runway heading should be 0
            Assert.AreEqual(0, scenario.Runway.RunwayHeading.DirectionTrue);

            // we said 200 magnetic for the wind, so with -20 mag var
            // the true direction for wind should be 220
            Assert.AreEqual(220, scenario.Wind.Direction.DirectionTrue);

        }

        [TestMethod]
        public void TestAngularDifference_WindLeftOfRunway()
        {
            Wind wind = Wind.FromMagnetic(310, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(010, _defaultMagneticVariation);
            Scenario scenario = new Scenario(runway, wind);
            Assert.AreEqual(-60,scenario.WindRunwayAngularDifferenceMagnetic);

        }

                [TestMethod]
        public void TestAngularDifference_WindRightOfRunway()
        {
            Wind wind = Wind.FromMagnetic(010, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(350, _defaultMagneticVariation);
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(20,scenario.WindRunwayAngularDifferenceMagnetic);

        }

        
                [TestMethod]
        public void TestAngularDifference_WindStraightDownRunway()
        {

            Wind wind = Wind.FromMagnetic(10, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(10, _defaultMagneticVariation);  
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(0,scenario.WindRunwayAngularDifferenceMagnetic);
    
        }

        [TestMethod]
        public void TestAngularDifference_WindStraightDownOppositeRunway()
        {

            Wind wind = Wind.FromMagnetic(10, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(190, _defaultMagneticVariation);  
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(-180,scenario.WindRunwayAngularDifferenceMagnetic);
        }

        [TestMethod]
        public void TestAngularDifference_WindGreaterThan180()
        {

            Wind wind = Wind.FromMagnetic(160, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(10, _defaultMagneticVariation);  
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(150,scenario.WindRunwayAngularDifferenceMagnetic);
        }

        [TestMethod]
        public void Test_CrosswindComponent_NoCrosswind()
        {

            Wind wind = Wind.FromMagnetic(160, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(160, _defaultMagneticVariation);  
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(0,scenario.CrosswindComponent);
            Assert.AreEqual(10,scenario.HeadwindComponent);
        }

        [TestMethod]
        public void Test_CrosswindComponent_DirectCrosswind()
        {

            Wind wind = Wind.FromMagnetic(0, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(90, _defaultMagneticVariation);  
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(-10,scenario.CrosswindComponent);
            Assert.AreEqual(0,(int)scenario.HeadwindComponent);
        }

        [TestMethod]
        public void Test_CrosswindComponent_40DegreeCrosswind()
        {

            Wind wind = Wind.FromMagnetic(0, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(40, _defaultMagneticVariation);  
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(-6.4278760968653925,scenario.CrosswindComponent);
            Assert.AreEqual(7.660444431189781,scenario.HeadwindComponent);
        }

        [TestMethod]
        public void Test_PressureAltitude()
        {
            Wind wind = Wind.FromMagnetic(0, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(40, _defaultMagneticVariation);  
            Scenario scenario = new Scenario(runway, wind);          

            scenario.TemperatureCelcius = 11;
            scenario.FieldElevation = 1500;
            scenario.QNH = 1013;

            Assert.AreEqual(1500, scenario.PressureAltitude);
        }
    }
}
