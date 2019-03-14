using Geberit.AquaClean.Core.Frames;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace aquaclean_core_tests.Frames
{
    [TestClass]
    public class FrameCollectorTest
    {


        [TestMethod]
        public void ParseConsFrames()
        {
            //Arrange  
            List<byte[]> frameData = new List<byte[]>
            {
                Utils.StringToByteArray("300605000057febf00010082523134362e323178"), // First 1/6
                Utils.StringToByteArray("52012e78782e7848423138303645553135333437"), // Cons 2/6
                Utils.StringToByteArray("54023100000000000030342e30362e3230313841"), // Cons 3/6
                Utils.StringToByteArray("5603717561436c65616e204d65726120436f6d66"), // Cons 4/6
                Utils.StringToByteArray("40046f7274000000000000000000000000000000"), // Cons 5/6
                Utils.StringToByteArray("4205000000000000000000000000000000000000"), // Cons 6/6
            };

            var frameFactory = new FrameFactory();
            var frameCollector = new FrameCollector();

            foreach (var data in frameData)
            {
                //Act 
                var frame = frameFactory.CreateFrameFromBytes(data);
                //frameCollector.AddFrame(frame.SubFrameCountOrIndex)
                //Assert  
                Assert.IsTrue(frame is FirstConsFrame);

            }
        }

    }
}
