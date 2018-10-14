using System;
using Xunit;
using LEDerZaumzeug.Outputs;
using System.Collections.Generic;

namespace LEDerZaumzeug.Test
{
    public class OutGeneratortest
    {
        [Fact]
        public void SchlangenArtH_TR()
        {
            var pxMap = new List<(int, int)>();
            OutputBase.GenerischeOrder(pxMap, 4, 3, PixelArrangement.SNH_TR);
 
            Assert.Equal((3,0), pxMap[0]);
            Assert.Equal((2,0), pxMap[1]);
            Assert.Equal((1,0), pxMap[2]);
            Assert.Equal((0,0), pxMap[3]);
            Assert.Equal((0,1), pxMap[4]);
            Assert.Equal((1,1), pxMap[5]);
            Assert.Equal((2,1), pxMap[6]);
            Assert.Equal((3,1), pxMap[7]);
            Assert.Equal((3,2), pxMap[8]);
            Assert.Equal((2,2), pxMap[9]);
            Assert.Equal((1,2), pxMap[10]);
            Assert.Equal((0,2), pxMap[11]);
        }

        [Fact]
        public void SchlangenArtH_TL()
        {
            var pxMap = new List<(int, int)>();
            OutputBase.GenerischeOrder(pxMap, 4, 3, PixelArrangement.SNH_TL); 

            Assert.Equal((0,0), pxMap[0]);
            Assert.Equal((1,0), pxMap[1]);
            Assert.Equal((2,0), pxMap[2]);
            Assert.Equal((3,0), pxMap[3]);
            Assert.Equal((3,1), pxMap[4]);
            Assert.Equal((2,1), pxMap[5]);
            Assert.Equal((1,1), pxMap[6]);
            Assert.Equal((0,1), pxMap[7]);
            Assert.Equal((0,2), pxMap[8]);
            Assert.Equal((1,2), pxMap[9]);
            Assert.Equal((2,2), pxMap[10]);
            Assert.Equal((3,2), pxMap[11]);
        }

        [Fact]
        public void SchlangenArtV_BR()
        {
            var pxMap = new List<(int, int)>();
            OutputBase.GenerischeOrder(pxMap, 5, 4, PixelArrangement.SNV_BR);
 
            Assert.Equal((4,3), pxMap[0]);
            Assert.Equal((4,2), pxMap[1]);
            Assert.Equal((4,1), pxMap[2]);
            Assert.Equal((4,0), pxMap[3]);
            Assert.Equal((3,0), pxMap[4]);
            Assert.Equal((3,1), pxMap[5]);
            Assert.Equal((3,2), pxMap[6]);
            Assert.Equal((3,3), pxMap[7]);
            Assert.Equal((2,3), pxMap[8]);
            Assert.Equal((2,2), pxMap[9]);
            Assert.Equal((2,1), pxMap[10]);
            Assert.Equal((2,0), pxMap[11]);
            Assert.Equal((1,0), pxMap[12]);
            Assert.Equal((1,1), pxMap[13]);
            Assert.Equal((1,2), pxMap[14]);
            Assert.Equal((1,3), pxMap[15]);
            Assert.Equal((0,3), pxMap[16]);
            Assert.Equal((0,2), pxMap[17]);
            Assert.Equal((0,1), pxMap[18]);
            Assert.Equal((0,0), pxMap[19]);
        }

        [Fact]
        public void LinearVertikalTL()
        {
            var pxMap = new List<(int, int)>();
            OutputBase.GenerischeOrder(pxMap, 5, 5, PixelArrangement.LNV_TL);

            Assert.Equal((0,0), pxMap[0]);
            Assert.Equal((0,1), pxMap[1]);
            Assert.Equal((0,2), pxMap[2]);
            Assert.Equal((0,3), pxMap[3]);
            Assert.Equal((0,4), pxMap[4]);
            Assert.Equal((1,0), pxMap[5]);
            Assert.Equal((1,1), pxMap[6]);
            Assert.Equal((1,2), pxMap[7]);
            Assert.Equal((1,3), pxMap[8]);
            Assert.Equal((1,4), pxMap[9]);
            Assert.Equal((2,0), pxMap[10]);
            Assert.Equal((2,1), pxMap[11]);
            Assert.Equal((4,3), pxMap[23]);
            Assert.Equal((4,4), pxMap[24]);
        }

        [Fact]
        public void LinearHorizontalTL()
        {
            var pxMap = new List<(int, int)>();
            OutputBase.GenerischeOrder(pxMap, 5, 5, PixelArrangement.LNH_TL);

            Assert.Equal((0,0), pxMap[0]);
            Assert.Equal((1,0), pxMap[1]);
            Assert.Equal((2,0), pxMap[2]);
            Assert.Equal((3,0), pxMap[3]);
            Assert.Equal((4,0), pxMap[4]);
            Assert.Equal((0,1), pxMap[5]);
            Assert.Equal((1,1), pxMap[6]);
            Assert.Equal((2,1), pxMap[7]);
            Assert.Equal((3,1), pxMap[8]);
            Assert.Equal((4,1), pxMap[9]);
            Assert.Equal((0,2), pxMap[10]);
            Assert.Equal((1,2), pxMap[11]);
            Assert.Equal((3,4), pxMap[23]);
            Assert.Equal((4,4), pxMap[24]);
        }
    }
}