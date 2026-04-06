using RateDepartment.Extensions;

namespace RateDepartment.Configs;

public class AppSettingsConfig
{
    public string Site { get; set; } = "";
    public OrganisationConfig Organisation { get; set; } = new();
    public TriesConfig Tries { get; set; } = new();
    public TimeoutsConfig Timeouts { get; set; } = new();

    public class OrganisationConfig
    {
        public string Name { get; set; } = "";
        public string Rating { get; set; } = "";
        public List<string> DepartmentsList { get; set; } = [];

        public string Departments
        {
            get => DepartmentsList.Join(",");
            set => DepartmentsList = value.Split(',')
                .Select(d => d.Trim())
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();
        }
    }

    public class TriesConfig
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class TimeoutsConfig
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }
}