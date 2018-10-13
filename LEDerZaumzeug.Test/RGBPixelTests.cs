using System;
using Xunit;

namespace LEDerZaumzeug.Test
{
    public class RGBPixelTests
    {
        [Fact]
        public void ZeigeGleichheitGeht()
        {
            var a = new RGBPixel(1, 2, 3);
            var b = new RGBPixel(1.0f, 2.0f, 3.0f);
            Assert.Equal(a, b);

            var c = new RGBPixel(4, 3, 2);
            Assert.NotEqual(a, c);
            Assert.NotEqual(c, b);

            Assert.True(a == b);
            Assert.False(a == c);
            Assert.True(a != c);
            Assert.False(a != b);
        }

        [Fact]
        public void TestRGBPixelConverterHtml()
        {
            RGBPixel? p1 =  RGBPixel.FromHtmlColor("#FB5");
            Assert.NotNull(p1); //[1/0,7333333/0,3333333]
            Assert.NotEqual(p1.Value.R, p1.Value.B);
            Assert.NotEqual(p1.Value.R, p1.Value.G);
            Assert.NotEqual(p1.Value.B, p1.Value.G);
            Assert.Equal(p1, new RGBPixel(1f, 0.73333333333f,.33333333333f));

            RGBPixel? p2 = RGBPixel.FromHtmlColor("#AABBCC");
            Assert.NotNull(p2); 

            RGBPixel? p3 = RGBPixel.FromHtmlColor("#33FF55cc");
            Assert.NotNull(p3);

            RGBPixel? p4 = RGBPixel.FromHtmlColor("#444c");
            Assert.NotNull(p4); 

            RGBPixel? p5 = RGBPixel.FromHtmlColor("#222");
            Assert.NotNull(p5); 

            RGBPixel? p6 = RGBPixel.FromHtmlColor("#c040d0");
            Assert.NotNull(p6);


            //Assert.Equal(p.Value.R, 3);
            // Gleichheit von #DDD mit #DDDDDD !!
            // und gleich #444 zu #444444
            RGBPixel? g1 = RGBPixel.FromHtmlColor("#DDD");
            RGBPixel? g2 = RGBPixel.FromHtmlColor("#DDDDDD");
            Assert.Equal(g1, g2);
            RGBPixel? h1 = RGBPixel.FromHtmlColor("#4444");
            RGBPixel? h2 = RGBPixel.FromHtmlColor("#cc444444");
            Assert.Equal(h1, h2);
        }

        [Fact]
        public void TestRGBPixelConverterHtmlScheitert()
        {
            RGBPixel? p1 = RGBPixel.FromHtmlColor("AABBCC");
            Assert.Null(p1);
            RGBPixel? p2 = RGBPixel.FromHtmlColor("#ABBCC");
            Assert.Null(p2);
            RGBPixel? p3 = RGBPixel.FromHtmlColor("#00AAB7BCC");
            Assert.Null(p3);
            RGBPixel? p4 = RGBPixel.FromHtmlColor("owi3zo3i5");
            Assert.Null(p4);
            RGBPixel? p5 = RGBPixel.FromHtmlColor("#owi3zo3i5");
            Assert.Null(p5);
            RGBPixel? p6 = RGBPixel.FromHtmlColor(" #abc ");
            Assert.Null(p6);
            RGBPixel? p7 = RGBPixel.FromHtmlColor(" #aabbcc ");
            Assert.Null(p7);
            RGBPixel? p8 = RGBPixel.FromHtmlColor("# abc");
            Assert.Null(p8);
            RGBPixel? p9 = RGBPixel.FromHtmlColor("");
            Assert.Null(p9);
        }

        [Fact]
        public void TestRGBPixelOperatorSkalar()
        {
            RGBPixel a = new RGBPixel(1, 2, 3);
            RGBPixel b = a * 5;
            Assert.Equal(5, b.R);
            Assert.Equal(10, b.G);
            Assert.Equal(15, b.B);
        }


        [Fact]
        public void TestRGBPixelOperatorMal()
        {
            RGBPixel a = new RGBPixel(1, 2, 3);
            RGBPixel b = a * new RGBPixel(5,10,100);
            Assert.Equal(5, b.R);
            Assert.Equal(20, b.G);
            Assert.Equal(300, b.B);
        }

        [Fact]
        public void TestRGBPixelOperatorPlus()
        {
            RGBPixel a = new RGBPixel(6, 2, 3);
            RGBPixel b = a + new RGBPixel(5,4,100);
            Assert.Equal(11, b.R);
            Assert.Equal(6, b.G);
            Assert.Equal(103, b.B);
        }

        [Fact]
        public void TestRGBPixelOperatorMinus()
        {
            RGBPixel a = new RGBPixel(6, 2, 3);
            RGBPixel b = a - new RGBPixel(5,4,3);
            Assert.Equal(1, b.R);
            Assert.Equal(-2, b.G);
            Assert.Equal(0, b.B);
        }

        [Fact]
        public void TestRGBPixelOperatorUnärMinus()
        {
            RGBPixel a = new RGBPixel(6, 2, 3);
            RGBPixel acta = -a;
            Assert.Equal(new RGBPixel(-6,-2, -3), acta);

            RGBPixel b = new RGBPixel(-4, -22, -0.3f);
            RGBPixel actb = -b;
            Assert.Equal(new RGBPixel(4, 22, 0.3f), actb);
        }

        [Fact]
        public void TestRGBPixelOperatorInvert()
        {
            RGBPixel a = new RGBPixel(0.5f, 0.7f, 0.3f);
            RGBPixel acta = ~a;
            RGBPixel expa = RGBPixel.P1 - a;
            Assert.Equal(expa, acta);

            RGBPixel b = new RGBPixel(-4, -22, -0.3f);
            RGBPixel actb = ~b;
            RGBPixel expb = RGBPixel.P1 - b;
            Assert.Equal(expb, actb);
        }

        [Fact]
        public void TestRGBPixelOperatorP1_P0()
        {
            Assert.Equal(RGBPixel.P1, new RGBPixel(1, 1, 1));
            Assert.Equal(RGBPixel.P0, new RGBPixel(0, 0, 0));
        }

        [Fact]
        public void TestRGBPixelOperatorClip()
        {
            RGBPixel a = new RGBPixel(0.5f, 0.7f, 0.3f);
            RGBPixel acta = a.Clip();
            RGBPixel expa = a;
            Assert.Equal(expa, acta);

            RGBPixel b = new RGBPixel(1.3f, 10.9f, 1.443f);
            RGBPixel actb = b.Clip();
            RGBPixel expb = new RGBPixel(1,1,1);
            Assert.Equal(expb, actb);

            RGBPixel c = new RGBPixel(-4, -22, -0.3f);
            RGBPixel actc = c.Clip();
            RGBPixel expc = new RGBPixel(0,0,0);
            Assert.Equal(expc, actc);
        }

    }
}
