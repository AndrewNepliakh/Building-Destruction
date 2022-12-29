using System.Threading.Tasks;

public interface IUserManager
{
    User CurrentUser { get; }
    Task Init(UserData userData);
}