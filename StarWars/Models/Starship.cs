namespace StarWars.Models
{
    public class Starship
    {
        public string name { get; set; }

        public override string ToString()
        {
            return name ?? "Unknown Starship";
        }
    }
}