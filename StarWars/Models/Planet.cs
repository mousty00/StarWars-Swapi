namespace StarWars.Models
{
    public class Planet
    {
        public string name { get; set; }
        public string gravity { get; set; }
        public string terrain { get; set; }
        public string surface_water { get; set; }
        public string population { get; set; }

        public override string ToString()
        {
            return name ?? "Unknown Planet";
        }
    }
}