using System.Diagnostics.CodeAnalysis;

namespace APinI.Models
{
    public class BaseTask
    {
        [NotNull]
        public string Name { get; set; }
        [NotNull]
        public string Description { get; set; }
        [NotNull]
        public string Status { get; set; }
        
    }
}
