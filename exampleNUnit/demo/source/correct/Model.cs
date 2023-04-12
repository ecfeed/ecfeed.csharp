namespace Source
{
    public class Person
    {
        private string name;
        private int age;

        public Person(string name, int age)
        {
            this.name = name;
            this.age = age;
        }

        public override string ToString()
        {
            return "Person{" +
                    "name='" + name + '\'' +
                    ", age=" + age +
                    '}';
        }
    }

    public class Data
    {
        private Person delinquent;
        private int id;

        public Data(Person delinquent, int id)
        {
            this.delinquent = delinquent;
            this.id = id;
        }

        public override string ToString()
        {
            return "Data{" +
                    "delinquent=" + delinquent +
                    ", id=" + id +
                    '}';
        }
    }
}