using System;
using Xunit;

namespace LEDerZaumzeug.Test
{
    public class HSVPixelTests
    {

        [Fact]
        public void Farbtests()
        {
            RGBPixel schwarz = RGBPixel.P0;
            RGBPixel weiß = RGBPixel.P1;
            RGBPixel braun = new RGBPixel(.36f, .18f, .09f);
            RGBPixel blau = new RGBPixel(0, 0, 1);
            RGBPixel rot = new RGBPixel(1, 0, 0);
            RGBPixel grün = new RGBPixel(0, 1, 0);
            RGBPixel magenta = new RGBPixel(1, 0, 1);
            RGBPixel cyan = new RGBPixel(0, 1, 1);
            RGBPixel orange = new RGBPixel(1, .5f, 0);
            RGBPixel violett = new RGBPixel(.5f, 0, 1);
            RGBPixel zinober = new RGBPixel(1, 0.25f, 0);
            RGBPixel safran = new RGBPixel(1, 0.75f, 0);

            HSVPixel schwarz1 = HSVPixel.P0;
            HSVPixel weiß1 = new HSVPixel(0, 0, 1);
            HSVPixel braun1 = new HSVPixel(20, 0.75f, 0.36f);
            HSVPixel blau1 = new HSVPixel(240, 1, 1);
            HSVPixel rot1 = new HSVPixel(0, 1, 1);
            HSVPixel grün1 = new HSVPixel(120, 1, 1);
            HSVPixel magenta1 = new HSVPixel(300, 1, 1);
            HSVPixel cyan1 = new HSVPixel(180, 1, 1);
            HSVPixel orange1 = new HSVPixel(30, 1, 1);
            HSVPixel violett1 = new HSVPixel(270, 1, 1);
            HSVPixel zinober1 = new HSVPixel(15, 1, 1);
            HSVPixel safran1 = new HSVPixel(45, 1, 1);

            Assert.Equal(schwarz1.V, HSVPixel.FromRGB(schwarz).V);
            Assert.Equal(schwarz, (RGBPixel)schwarz1);
            Assert.Equal(weiß1.V, HSVPixel.FromRGB(weiß).V);
            Assert.Equal(weiß1.S, HSVPixel.FromRGB(weiß).S);
            Assert.Equal(weiß, (RGBPixel)weiß1);
            Assert.Equal(braun1, HSVPixel.FromRGB(braun));
            Assert.Equal(braun, (RGBPixel)braun1);
            Assert.Equal(blau1, HSVPixel.FromRGB(blau));
            Assert.Equal(blau, (RGBPixel)blau1);
            Assert.Equal(rot1, HSVPixel.FromRGB(rot));
            Assert.Equal(rot, (RGBPixel)rot1);
            Assert.Equal(grün1, HSVPixel.FromRGB(grün));
            Assert.Equal(grün, (RGBPixel)grün1);
            Assert.Equal(magenta1, HSVPixel.FromRGB(magenta));
            Assert.Equal(magenta, (RGBPixel)magenta1);
            Assert.Equal(cyan1, HSVPixel.FromRGB(cyan));
            Assert.Equal(cyan, (RGBPixel)cyan1);
            Assert.Equal(orange1, HSVPixel.FromRGB(orange));
            Assert.Equal(orange, (RGBPixel)orange1);
            Assert.Equal(violett1, HSVPixel.FromRGB(violett));
            Assert.Equal(violett, (RGBPixel)violett1);
            Assert.Equal(zinober1, HSVPixel.FromRGB(zinober));
            Assert.Equal(zinober, (RGBPixel)zinober1);
            Assert.Equal(safran1, HSVPixel.FromRGB(safran));
            Assert.Equal(safran, (RGBPixel)safran1);
        }
    }
}