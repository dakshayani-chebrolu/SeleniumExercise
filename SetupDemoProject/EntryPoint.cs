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
            driver.FindElement(By.Id("twotabsearchtextbox")).SendKeys("laptop");

            driver.FindElement(By.Id("nav-search-submit-button")).Click();

            driver.FindElement(By.XPath("//div[@data-cel-widget='search_result_0']/descendant::h2/a[contains(@class,'a-text-normal')]")).Click();

            WaitForPageLoad(driver, wait);

            //Check whether the first selected item is on sale or not

            actualPrice= driver.FindElement(By.XPath("//*[@id='priceblock_ourprice'] | //*[@id='priceblock_saleprice']")).Text;
          
            decimal actualLaptopPrice = Decimal.Parse(actualPrice, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, new CultureInfo("en-US"));

            decimal expectedLaptopPrice = Decimal.Parse(expectedPrice, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, new CultureInfo("en-US"));

            SuccessMessage("Actual Price:" + actualLaptopPrice);

            //Assertion for the actual price of the selected item with the expected price
            Assert.Greater(actualLaptopPrice, expectedLaptopPrice, "Assertion was not successfull");

            SuccessMessage("Test Passed: Actual Price-"+ actualLaptopPrice +" is greater than expected laptop price-"+expectedLaptopPrice);

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

}

