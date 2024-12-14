using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Application.Abstractions.Authentication;
public interface IUserContext
{
    string IdentityId { get; }
}
