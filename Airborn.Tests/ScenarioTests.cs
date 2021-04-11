using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class ScenarioTests
    {

        [TestMethod]
        public void TestScenarioCreation()
        {

            int magneticVariation = -20;

            Wind wind = new Wind(Direction.FromMagnetic(200, magneticVariation), 10);
            Runway runway = new Runway(Direction.FromMagnetic(340, magneticVariation));
            
            Scenario scenario = new Scenario(runway, wind);

            Assert.AreEqual(0, scenario.Runway.RunwayHeading.DirectionTrue);
            Assert.AreEqual(220, scenario.Wind.Direction.DirectionTrue);
        }

        [TestMethod]
        public void TestAngularDifference_WindLeftOfRunway()
        {
            int magneticVariation = -20;
            Wind wind = new Wind(Direction.FromMagnetic(310, magneticVariation), 10);
            Runway runway = new Runway(Direction.FromMagnetic(010, magneticVariation));
            Scenario scenario = new Scenario(runway, wind);
            Assert.AreEqual(-60,scenario.WindRunwayAngularDifferenceMagnetic);

        }

                [TestMethod]
        public void TestAngularDifference_WindRightOfRunway()
        {
            int magneticVariation = 20;
            Wind wind = new Wind(Direction.FromMagnetic(010, magneticVariation), 10);
            Runway runway = new Runway(Direction.FromMagnetic(350, magneticVariation));
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(20,scenario.WindRunwayAngularDifferenceMagnetic);

        }

        
                [TestMethod]
        public void TestAngularDifference_WindStraightDownRunway()
        {

            int magneticVariation = -20;
            Wind wind = new Wind(Direction.FromMagnetic(10, magneticVariation), 10);
            Runway runway = new Runway(Direction.FromMagnetic(10, magneticVariation));  
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(0,scenario.WindRunwayAngularDifferenceMagnetic);
    
        }

        [TestMethod]
        public void TestAngularDifference_WindStraightDownOppositeRunway()
        {

            int magneticVariation = -20;
            Wind wind = new Wind(Direction.FromMagnetic(10, magneticVariation), 10);
            Runway runway = new Runway(Direction.FromMagnetic(190, magneticVariation));  
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(-180,scenario.WindRunwayAngularDifferenceMagnetic);
        }

        [TestMethod]
        public void TestAngularDifference_WindGreaterThan180()
        {

            int magneticVariation = -20;
            Wind wind = new Wind(Direction.FromMagnetic(160, magneticVariation), 10);
            Runway runway = new Runway(Direction.FromMagnetic(10, magneticVariation));  
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(150,scenario.WindRunwayAngularDifferenceMagnetic);
        }

        [TestMethod]
        public void Test_CrosswindComponent_NoCrosswind()
        {

            int magneticVariation = -20;
            Wind wind = new Wind(Direction.FromMagnetic(160, magneticVariation), 10);
            Runway runway = new Runway(Direction.FromMagnetic(160, magneticVariation));  
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(0,scenario.CrosswindComponent);
            Assert.AreEqual(10,scenario.HeadwindComponent);
        }

        [TestMethod]
        public void Test_CrosswindComponent_DirectCrosswind()
        {

            int magneticVariation = -20;
            Wind wind = new Wind(Direction.FromMagnetic(0, magneticVariation), 10);
            Runway runway = new Runway(Direction.FromMagnetic(90, magneticVariation));  
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(-10,scenario.CrosswindComponent);
            Assert.AreEqual(0,(int)scenario.HeadwindComponent);
        }

        [TestMethod]
        public void Test_CrosswindComponent_40DegreeCrosswind()
        {

            int magneticVariation = -20;
            Wind wind = new Wind(Direction.FromMagnetic(0, magneticVariation), 10);
            Runway runway = new Runway(Direction.FromMagnetic(40, magneticVariation));  
            Scenario scenario = new Scenario(runway, wind);          
            Assert.AreEqual(-6.4278760968653925,scenario.CrosswindComponent);
            Assert.AreEqual(7.660444431189781,scenario.HeadwindComponent);
        }
    }
}
