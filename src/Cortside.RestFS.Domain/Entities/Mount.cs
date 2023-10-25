namespace Cortside.RestFS.Domain.Entities {
    public class Mount {
        public string Name { get; set; }
        public string Path { get; set; }

        public bool ReadOnly { get; set; }
    }
}
