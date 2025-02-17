namespace StarWars.Models
{
    public class Vehicle
    {
        public string name { get; set; }

        public override string ToString()
        {
            return name ?? "Unknown Vehicle";
        }
    }
}