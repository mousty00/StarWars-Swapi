using StarWars.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public class Character : INotifyPropertyChanged
{
    public string name { get; set; }
    public string height { get; set; }
    public string mass { get; set; }
    public string hair_color { get; set; }
    public string skin_color { get; set; }
    public string eye_color { get; set; }
    public string birth_year { get; set; }
    public string gender { get; set; }
    public string homeworld { get; set; }

    public List<string> vehicles_urls { get; set; } = new List<string>();
    public List<string> starships_urls { get; set; } = new List<string>();

    private Planet planetInfo;
    public Planet PlanetInfo
    {
        get => planetInfo;
        set
        {
            if (planetInfo != value)
            {
                planetInfo = value;
                OnPropertyChanged(nameof(PlanetInfo));
            }
        }
    }

    public List<string> vehiclesList = new List<string>();
    public string Vehicles => vehiclesList.Any() ? string.Join(", ", vehiclesList) : "None";

    public List<string> starshipsList = new List<string>();
    public string Starships => starshipsList.Any() ? string.Join(", ", starshipsList) : "None";

    public string Planet => planetInfo?.ToString() ?? "Unknown";

    public bool IsExpanded { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return $"Name: {name}\n" +
               $"Height: {height}\n" +
               $"Mass: {mass}\n" +
               $"Hair Color: {hair_color}\n" +
               $"Skin Color: {skin_color}\n" +
               $"Eye Color: {eye_color}\n" +
               $"Birth Year: {birth_year}\n" +
               $"Gender: {gender}\n" +
               $"Planet: {Planet}\n" +
               $"Planet URL: {homeworld}\n";
    }
}
