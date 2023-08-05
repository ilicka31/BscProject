using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Contracts
{
    public record UserCreated(long id, string name, string username, string email, string passeord, DateTime birthDate, string address, bool approved, bool denied, int type);
    public record UserUpdated(long id, string name, string username, string email, DateTime birthDate, string address);
    public record UserPasswordChanged(long id, string password);
    public record UserActivated(long id, bool isActive);
    public record UserDeleted(long id);
}
