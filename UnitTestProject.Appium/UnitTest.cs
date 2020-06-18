using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace UnitTestProject.Appium
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void LaunchTest_LayoutCycleException()
        {
            for (int i = 0; i < 50; i++)
            {
                // Launch the app
                AppiumOptions opt = new AppiumOptions();
                opt.AddAdditionalCapability("app", "49586DaveAntoine.SimpleWeatherDebug_9bzempp7dntjg!App");
                //opt.AddAdditionalCapability("app", "49586DaveAntoine.SimpleWeather-Asimpleweatherapp_9bzempp7dntjg!App");
                opt.AddAdditionalCapability("deviceName", "WindowsPC");

                WindowsDriver<WindowsElement> session;

                try
                {
                    session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), opt);
                }
                catch (Exception)
                {
                    try
                    {
                        session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), opt);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                session.LaunchApp();

                //session.FindElementByAccessibilityId("CommandBar").Click();

                session.Manage().Window.Size = new System.Drawing.Size(480, 720);
                session.Manage().Window.Maximize();
                session.Manage().Window.Size = new System.Drawing.Size(480, 720);
                session.Manage().Window.Maximize();

                Task.Delay(1000);

                session.CloseApp();

                Task.Delay(1000);
            }
        }
    }
}