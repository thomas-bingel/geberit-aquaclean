using Geberit.AquaClean.Core.Frames;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace aquaclean_core_tests.Frames
{
    [TestClass]
    public class FrameFactoryTest
    {
        [TestMethod]
        public void ParseHeaderByteForFirstConsFrames()
        {
            //     1110 0000 >> 0000 0111 &   
            // 0   000? ???? => 0 => SINGLE
            // 1   001? ???? => 1 => FIRST
            // 2   010? ???? => 2 => CONS
            // 3   011? ???? => 3 => CONTROL
            // 4   100? ???? => 4 => INFO

            // IsSubframeCount ==>          ???? ???1 
            // SubFrameCountOrIndex ==>     ???? ?11? ==> Used in Single Frame
            // HasMessageTypeByte_b4 ==>    ???1 ????
            // Frame Type ==>               111? ????

            // ConsFrame:
            // IsSubFrameCount ==>          1??? ????
            // SubFrameCountOrIndex ==>     ?11? ????
            // HasMessageTypeByte_b4 ==>    ???? 1???
            // FrameCountOrNuber ==> byte[1]

            //Arrange  
            var frameFactory = new FrameFactory();
            byte headerByte = 0x30;
            var frameType = frameFactory.getFrameTypeFromHeaderByte(headerByte);
            Assert.AreEqual(FrameType.FIRST, frameType);
        }



        [TestMethod]
        public void ParseInfoFrame()
        {
            //Arrange  
            var data = Utils.StringToByteArray("800130140c030003000000003130001200ff0800");
            var frameFactory = new FrameFactory();

            //Act 
            var frame = frameFactory.CreateFrameFromBytes(data);
            //Assert  
            Assert.IsTrue( frame is InfoFrame);
        }

        [TestMethod]
        public void ParseControlFrame()
        {

            //Arrange  
            var data = Utils.StringToByteArray("70000c0c010000000000000000ff090100000d2d");
            var frameFactory = new FrameFactory();

            //Act 
            var frame = frameFactory.CreateFrameFromBytes(data);
            //Assert  
            Assert.IsTrue(frame is FlowControlFrame);
            
        }

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

            foreach (var data in frameData)
            {
                //Act 
                var frame = frameFactory.CreateFrameFromBytes(data);
                //Assert  
                Assert.IsTrue(frame is FirstConsFrame);

            }
        }


        [TestMethod]
        public void ParseSingleFrames()
        {
            //Arrange  
            List<byte[]> frameData = new List<byte[]>
            {
                Utils.StringToByteArray("130500000fb968000100860a32362e30372e3230"), // Single 1/2
                Utils.StringToByteArray("1231382e00000000000000000000000000000000"), // Single 2/2
            };

            var frameFactory = new FrameFactory();

            foreach (var data in frameData)
            {
                //Act 
                var frame = frameFactory.CreateFrameFromBytes(data);
                //Assert  
                Assert.IsTrue(frame is SingleFrame);

            }

        }
    }

}
