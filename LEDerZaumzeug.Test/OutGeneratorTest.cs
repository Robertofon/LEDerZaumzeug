using System;
using Xunit;
using LEDerZaumzeug.Outputs;

namespace LEDerZaumzeug.Test
{
    public class OutGeneratortest
    {
        [Fact]
        public void SchlangenArt()
        {
            var o = new Tpm2NetOutput() 
            {
                SizeMode = SizeModes.StaticSetting,
                PixelOrder = PixelArrangement.SNH_TR,
                SizeX=4,
                SizeY=3,
            };

            o.Initialize(null);

            ///foreach ( var tp in o.PxMap)
               // Console.WriteLine(tp);

            Assert.Equal((3,0), o.PxMap[0]);
            Assert.Equal((2,0), o.PxMap[1]);
            Assert.Equal((1,0), o.PxMap[2]);
            Assert.Equal((0,0), o.PxMap[3]);
            Assert.Equal((0,1), o.PxMap[4]);
            Assert.Equal((1,1), o.PxMap[5]);
            Assert.Equal((2,1), o.PxMap[6]);
            Assert.Equal((3,1), o.PxMap[7]);
            Assert.Equal((3,2), o.PxMap[8]);
            Assert.Equal((2,2), o.PxMap[9]);
            Assert.Equal((1,2), o.PxMap[10]);
            Assert.Equal((0,2), o.PxMap[11]);
        }
    }
}