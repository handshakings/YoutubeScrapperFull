using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace YoutubeScrapperFull
{
    class Program
    {
        //fjdjfkdf
        static Random random = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to start youtube channel scrapping.....");
            Console.ReadLine();

            GetDataFromYoutubeAndFillCsv();
        }

        static public void GetDataFromYoutubeAndFillCsv()
        {
            FileStream fileStream1 = new FileStream("../youtube.txt", FileMode.Open);
            StreamReader reader = new StreamReader(fileStream1);
            string a = reader.ReadToEnd();
            string[] youtubeVideoUrlList = a.Split('\n');

            FileStream fileStream = new FileStream("../youtube.csv", FileMode.Append);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.WriteLine("SNo,Channel Name,Channel URL,Subscribers,Total Videos,Total Views,Email,Social Links");

            IWebDriver chromeDriver = CreateChromeDriver();
            IWebDriver firefoxDriver = CreateFirefoxDriver();

            int j = 2;
            foreach (string vidUrl in youtubeVideoUrlList)
            {
                try
                {
                    string videoUrl = vidUrl.Replace("\r", "");
                    List<string> channelData = new List<string>();
                    if (j % 2 == 0)
                    {
                        channelData = Youtube.GetChannelData(videoUrl, firefoxDriver);
                    }
                    else
                    {
                        channelData = Youtube.GetChannelData(videoUrl, chromeDriver);
                    }
                    if (channelData.Count > 0)
                    {
                        writer.WriteLine(j - 1 + "," + channelData.ElementAt(0) + "," + channelData.ElementAt(1) + "," + channelData.ElementAt(2) + "," + channelData.ElementAt(3) + "," + channelData.ElementAt(4) + "," + "\"" + channelData.ElementAt(5) + "\"" + "," + "\"" + channelData.ElementAt(6).Trim() + "\"");
                        writer.Flush();
                        fileStream.Flush();

                        Console.WriteLine(j - 1 + "\t" + channelData.ElementAt(0) + "\t" + channelData.ElementAt(5));
                        //if (j > 4 && j % 5 == 0)
                        //{
                        //    Console.WriteLine("Intentially give a pause to do safe scraping");
                        //    Thread.Sleep(random.Next(15, 25) * 1000);
                        //}
                        j++;
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            writer.Close();
            fileStream.Close();
            chromeDriver.Dispose();
            firefoxDriver.Dispose();
            KillProcesses();
        }


        
        static public void KillProcesses()
        {
            Process[] chromeDriverProcesses = Process.GetProcessesByName("chrome");
            Process[] firefoxDriverProcesses = Process.GetProcessesByName("firefox");
            foreach (var chromeDriverProcess in chromeDriverProcesses)
            {
                chromeDriverProcess.Kill();
            }
            foreach (var firefoxDriverProcess in firefoxDriverProcesses)
            {
                firefoxDriverProcess.Kill();
            }
        }
        static private ChromeDriver CreateChromeDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            //TimeSpan.FromSeconds is the max time for request to timeout
            ChromeDriver driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(random.Next(40, 60)));
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(random.Next(10, 20)));
            driver.Manage().Window.Maximize();
            return driver;
        }
        static private FirefoxDriver CreateFirefoxDriver()
        {
            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            FirefoxDriver driver = new FirefoxDriver(service, options, TimeSpan.FromSeconds(random.Next(40, 60)));
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(random.Next(10, 20)));
            driver.Manage().Window.Maximize();
            return driver;
        }
    }
}
