using Cortside.RestFS.Domain.Entities;
using Cortside.RestFS.WebApi.Models;

namespace Cortside.RestFS.WebApi.Mappers {
    public class AddressModelMapper {
        public AddressModel Map(FileSystemSearch dto) {
            if (dto == null) {
                return null;
            }

            return new AddressModel() {
                Street = dto.SearchPattern,
                City = dto.SearchPattern,
                State = dto.SearchPattern,
                Country = dto.SearchPattern,
                ZipCode = dto.SearchPattern
            };
        }
    }
}
