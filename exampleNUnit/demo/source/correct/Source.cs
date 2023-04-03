namespace Source
{
    public class Element1
    {
        public int a;
        public double b;
        public string c;
        public Element2 d;

        public Element1(int a, double b, string c, Element2 d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public override string ToString()
        {
            return "Element1{" +
                    "a=" + a +
                    ", b=" + b +
                    ", c='" + c + '\'' +
                    ", d=" + d +
                    '}';
        }
    }

    public class Element2
    {
        public int a;
        public int b;
        public int c;

        public Element2(int a, int b, int c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public override string ToString()
        {
            return "Element2{" +
                    "a=" + a +
                    ", b=" + b +
                    ", c=" + c +
                    '}';
        }
    }
}