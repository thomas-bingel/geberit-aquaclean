using Geberit.AquaClean.Core.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace aquaclean_core_tests
{

    [TestClass]
    public class DeserializerTest
    {
        [TestMethod]
        public void DeserializationOfStructTest() 
        {

            //Arrange  
            var sapNumber = "SapNumber".PadLeft(12);
            var serialNumber = "SerialNumber".PadLeft(20);
            var productionDate = "Production".PadLeft(10);
            var description = "Description".PadLeft(40);

            var bytes = Encoding.UTF8.GetBytes(sapNumber + serialNumber + productionDate + description);
            //Act 
            var obj = Geberit.AquaClean.Core.Common.Deserializer.Deserialize<DeviceIdentification>(bytes);

            //Assert  
            Assert.AreEqual("SapNumber", obj.SapNumber);
            Assert.AreEqual("SerialNumber", obj.SerialNumber);
            Assert.AreEqual("Production", obj.ProductionDate);
            Assert.AreEqual("Description", obj.Description);
        }

        [TestMethod]
        public void DeserializationOfSystemParameterListTest()
        {

            //Arrange  
            var bytes = Utils.StringToByteArray("070000000000010000000002000000000300000000040000000005000000000600000000000000000000000000000000000000000000000000000000006F000000");

            //Act 
            var obj = Geberit.AquaClean.Core.Common.Deserializer.Deserialize<SystemParameterList>(bytes);

            //Assert  
            Assert.AreEqual(0, obj.DataArray[0]);
            Assert.AreEqual(0, obj.DataArray[1]);
            Assert.AreEqual(0, obj.DataArray[2]);
            Assert.AreEqual(0, obj.DataArray[3]);
            Assert.AreEqual(0, obj.DataArray[4]);
            Assert.AreEqual(0, obj.DataArray[5]);
        }

    }
}
