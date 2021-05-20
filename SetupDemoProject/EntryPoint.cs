using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Globalization;


class EntryPoint
{
    static IWebDriver driver = null;
    

    static void Main()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        String url = "https://www.amazon.com/";
        String actualPrice = "";
        String expectedPrice = "$100";

        ChromeOptions options = new ChromeOptions();
        options.AddArguments("--incognito");

        //initialize the chrome driver 
        driver = new ChromeDriver(options);
        driver.Manage().Window.Maximize();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));

        try
        {

            //Navigate to the url
            driver.Navigate().GoToUrl(url);
            WaitForPageLoad(driver, wait);

            //get the elements using ID and Xpath
            GetWebElement(By.Id("twotabsearchtextbox")).SendKeys("laptop");

            GetWebElement(By.Id("nav-search-submit-button")).Click();

            GetWebElement(By.XPath("//div[@data-cel-widget='search_result_0']/descendant::h2/a[contains(@class,'a-text-normal')]")).Click();

            WaitForPageLoad(driver, wait);

            //Check whether the first selected item is on sale or not

            actualPrice = GetWebElement(By.XPath("//*[@id='priceblock_ourprice'] | //*[@id='priceblock_saleprice']")).Text;

            decimal actualLaptopPrice = Decimal.Parse(actualPrice, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, new CultureInfo("en-US"));

            decimal expectedLaptopPrice = Decimal.Parse(expectedPrice, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, new CultureInfo("en-US"));

            SuccessMessage("Actual Price:" + actualLaptopPrice);

            //Assertion for the actual price of the selected item with the expected price
            Assert.Greater(actualLaptopPrice, expectedLaptopPrice, "Assertion was not successfull");

            SuccessMessage("Test Passed: Actual Price-" + actualLaptopPrice + " is greater than expected laptop price-" + expectedLaptopPrice);

        }

        catch (Exception e)
        {
            FailureMessage("Exception: " + e.Message);
        }

        finally
        {
            driver.Quit();
            SuccessMessage("Browser closed");
        }
        watch.Stop();

        SuccessMessage($"Execution Time: {watch.ElapsedMilliseconds} ms");

    }

    //Page load mechanism to wait for the page to be loaded using explicit wait
    public static void WaitForPageLoad(IWebDriver driver, WebDriverWait wait)
    {
        wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
    }

    //Success message code for readability of messages
    public static void SuccessMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.White;
    }

    //Failure message code for readability of messages
    public static void FailureMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.White;
    }

    //Wait till the element is available by ignoring exception 
    public static IWebElement GetWebElement(By locator)
    {

        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver);
        fluentWait.Timeout = TimeSpan.FromSeconds(5);
        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(400);
        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        fluentWait.Message = "Element to be searched not found";

        IWebElement element = fluentWait.Until(x => x.FindElement(locator));

        return element;
    }
}



