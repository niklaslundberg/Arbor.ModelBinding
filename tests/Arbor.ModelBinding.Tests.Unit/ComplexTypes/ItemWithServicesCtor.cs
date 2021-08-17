using System.Collections.Generic;
using System.Linq;

namespace Arbor.ModelBinding.Tests.Unit.ComplexTypes
{
    public class ItemWithServicesCtor
    {
        public ItemWithServicesCtor(IEnumerable<Service> services, string description, int numberOfItems)
        {
            Description = description;
            NumberOfItems = numberOfItems;
            Services = services?.ToList();
        }

        public string Description { get; }

        public int NumberOfItems { get; }

        public IEnumerable<Service> Services { get; }

        public override string ToString()
        {
            string services = Services != null
                ? string.Join(", ", Services.Select(service => service.ToString()))
                : "No Services";

            return $"{nameof(Description)}: {Description}, {nameof(Services)}: {services}";
        }
    }
}
