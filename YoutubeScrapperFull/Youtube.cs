using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace YoutubeScrapperFull
{
    class Youtube
    {
        //abcd
        static Random random = new Random();

       
        public static List<string> GetChannelData(string videoUrl, IWebDriver driver)
        {
            driver.Navigate().GoToUrl(videoUrl);
            Thread.Sleep(random.Next(3, 5) * 1000);

            string channelName = driver.FindElement(By.Id("text-container")).GetAttribute("innerText").Trim().Replace(",", "");
            string subscribers = driver.FindElement(By.Id("subscriber-count")).GetAttribute("innerText").Split(' ')[0].Trim().Replace(",", "");
            string channelUrl = videoUrl.Remove(videoUrl.Length - 7, 7);
            string totalVideos = string.Empty;
            string totalViews = string.Empty;
            string description = string.Empty;
            try
            {
                string allvideourl = driver.FindElement(By.XPath("//*[@id='play-all']/ytd-button-renderer/a")).GetAttribute("href");
                driver.Navigate().GoToUrl(allvideourl);
                Thread.Sleep(random.Next(3, 5) * 1000);
                totalVideos = driver.FindElement(By.XPath("//*[@id='publisher-container']/div/yt-formatted-string/span[3]")).GetAttribute("innerText").Replace(",", "");
                totalViews = driver.FindElement(By.XPath("//*[@id='count']/ytd-video-view-count-renderer/span[1]")).GetAttribute("innerText").Replace(",", "").Split(' ')[0];
                description = driver.FindElement(By.XPath("//*[@id='description']")).GetAttribute("innerText");
            }
            catch (Exception)
            {
                goto ABOUT;
            }

        ABOUT:
            string aboutUrl = videoUrl.Remove(videoUrl.Length - 7, 7) + "/about";
            driver.Navigate().GoToUrl(aboutUrl);
            Thread.Sleep(random.Next(3, 5) * 1000);
            string aboutPageData = driver.FindElement(By.XPath("//*[@id='left-column']")).GetAttribute("innerText");

            string email = StringManipulation.SearchEmail(description + " " + aboutPageData);
            string sLinks = StringManipulation.SearchSocialLinks(description + " " + aboutPageData);

            if(email == null || email == "")
            {
                email = SearchEmailFromGivenLinks(driver, sLinks);
            }

            List<string> channelData = new List<string>()
            {
                channelName,channelUrl,subscribers,totalVideos,totalViews,email,sLinks
            };
            return channelData;
        }

       
        public static string SearchEmailFromGivenLinks(IWebDriver driver, string sLinks)
        {
            string[] links = sLinks.Split('\n');
            IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)driver;
            scriptExecutor.ExecuteScript("window.open()");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            string email = "";
            foreach(string link in links)
            {
                if (email == "")
                {
                    driver.Navigate().GoToUrl(link);
                    Thread.Sleep(5000);
                    string pageContent = driver.PageSource;
                    email = StringManipulation.SearchEmail(pageContent);
                }
                else
                {
                    break;
                }     
            }
            driver.SwitchTo().Window(driver.WindowHandles[0]);
            return email;
        }

    }
}
