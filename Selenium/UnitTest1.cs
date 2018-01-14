using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Selenium
{
    [TestClass]
    public class UnitTest1
    {
        private string baseURL = "http://cimob-ips.azurewebsites.net";
        private IWebDriver driver;

        private void LoginAsStudent()
        {
            driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(baseURL+"/Login");

            IWebElement loginEmail = driver.FindElement(By.Id("Email"));
            loginEmail.SendKeys("150221022@estudantes.ips.pt");

            IWebElement loginPassword = driver.FindElement(By.Id("input-pw"));
            loginPassword.SendKeys("teste");
            loginPassword.SendKeys(Keys.Enter);
        }

        private void LoginAsTech()
        {
            driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(baseURL+"/Login");

            IWebElement loginEmail = driver.FindElement(By.Id("Email"));
            loginEmail.SendKeys("diogo_oliveira_9@hotmail.com");

            IWebElement loginPassword = driver.FindElement(By.Id("input-pw"));
            loginPassword.SendKeys("teste");
            loginPassword.SendKeys(Keys.Enter);
        }

        [TestMethod]
        public void LoginTest()
        {
            driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(baseURL+"/Login");

            IWebElement loginEmail = driver.FindElement(By.Name("Email"));
            loginEmail.SendKeys("150221022@estudantes.ips.pt");

            IWebElement loginPassword = driver.FindElement(By.Name("Password"));
            loginPassword.SendKeys("teste");
            loginPassword.SendKeys(Keys.Enter);
        }

        
        [TestMethod]
        public void AlreadyRegistered()
        {
            driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(baseURL+"/Register");

            IWebElement loginEmail = driver.FindElement(By.Id("Student_StudentNum"));
            loginEmail.SendKeys("150221022");
            loginEmail.SendKeys(Keys.Enter);

            IWebElement error = driver.FindElement(By.Id("pre-register-message-error"));
            Assert.AreNotEqual("Número registado.", error.Text);
        }

        [TestMethod]
        public void MailNotFound()
        {
            driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(baseURL+"/Login");

            driver.FindElement(By.Id("fyp-link")).Click();

            IWebElement loginEmail = driver.FindElement(By.Id("email-fyp"));
            loginEmail.SendKeys("dddddddddddd@estudantes.ips.pt");
            loginEmail.SendKeys(Keys.Enter);

            Assert.AreNotEqual(driver.FindElement(By.Id("fyp-message")).Text, "Password renovada.\nVerifique a sua caixa de correio.");
        }

        [TestMethod]
        public void RecoverPassword_NoMatch()
        {
            LoginAsStudent();

            driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(baseURL+"/Account/UpdatePassword");

            IWebElement loginEmail = driver.FindElement(By.Id("CurrentPassword"));
            loginEmail.SendKeys("palavra-passe_errada");
            loginEmail.SendKeys(Keys.Enter);
        }
    }
}
