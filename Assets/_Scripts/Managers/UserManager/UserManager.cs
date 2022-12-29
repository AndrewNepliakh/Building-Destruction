using System;
using System.Threading.Tasks;
using UnityEngine;

public class UserManager : IUserManager
{
    private User _currentUser;

    public User CurrentUser
    {
        get
        {
            try
            {
                return _currentUser;
            }
            catch (Exception e)
            {
                Debug.LogError($"Current user is not initialized, coses: {e.Message}");
                _currentUser = new User();
                return _currentUser;
            }
        }
    }

    public async Task Init(UserData userData)
    {
        try
        {
            _currentUser = userData.User;
            if (userData.User == null) throw new NullReferenceException();
        }
        catch (Exception e)
        {
            _currentUser = new User();
            Debug.LogError($"Current user is not initialized, coses: {e.Message}");
        }
    }
}