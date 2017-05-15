using System.Collections.Generic;

namespace Arbor.ModelBinding.Tests.Unit.SampleTypes
{
    public class NewUsersRequest
    {
        public NewUsersRequest(IEnumerable<string> newUsers)
        {
            NewUsers = newUsers;
        }

        public IEnumerable<string> NewUsers { get; }
    }
}
