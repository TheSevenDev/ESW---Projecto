using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Selenium
{
    [TestClass]
    public class UnitTest1
    {
        private string baseURL = "cimob-ips.azurewebsites.net";
        private IWebDriver driver;

        [TestMethod]
        public void LoginTest()
        {
            driver = new ChromeDriver(@"C:\chromeWebDriver\");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Navigate().GoToUrl(baseURL+"/Login");

            IWebElement loginEmail = driver.FindElement(By.Id("Email"));
            loginEmail.SendKeys("150221022@estudantes.ips.pt");

            IWebElement loginPassword = driver.FindElement(By.Id("input-pw"));
            loginPassword.SendKeys("teste");
            loginPassword.SendKeys(Keys.Enter);
        }
    }
}
