using CIMOB_IPS.Controllers;
using CIMOB_IPS.Models;
using CIMOB_IPS.Views;
using System;
using Xunit;

namespace XUnitCIMOB_IPS
{
    public class UnitTest1
    {
        //public UserController userController = new UserController();
        LoginViewModel model1 = new LoginViewModel { Email = "teste@gmail.com", Password = "teste", RememberMe = true };
        LoginViewModel model2 = new LoginViewModel { Email = "teste@gmail.com", Password = "teste", RememberMe = false };
        LoginViewModel model3 = new LoginViewModel { Email = "teste@gmail.com", Password = "teste1", RememberMe = false };
        LoginViewModel model4 = new LoginViewModel { Email = "teste2@gmail.com", Password = "teste", RememberMe = false };
        LoginViewModel model5 = new LoginViewModel { Email = "brunop.esac@hotmail.com", Password = "teste", RememberMe = false };
        LoginViewModel model6 = new LoginViewModel { Email = "1231234@gmail.com", Password = "teste", RememberMe = true };

        [Fact]
        public void TestStateEmailNotFound()
        {
            LoginState state = Account.IsRegistered(model6.Email, model6.Password);

            Assert.Equal(LoginState.EMAIL_NOTFOUND, state);
        }

        [Fact]
        public void TestStateWrongPassword()
        {
            LoginState state = Account.IsRegistered(model3.Email, model3.Password);

            Assert.Equal(LoginState.WRONG_PASSWORD, state);
        }

        [Fact]
        public void TestStateValidState()
        {
            LoginState state = Account.IsRegistered(model1.Email, model1.Password);

            Assert.NotEqual(LoginState.WRONG_PASSWORD, state);
            Assert.NotEqual(LoginState.EMAIL_NOTFOUND, state);
            Assert.NotEqual(LoginState.CONNECTION_FAILED, state);
        }
    }
}
