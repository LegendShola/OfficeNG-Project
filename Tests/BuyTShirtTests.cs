using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightDemo.Pages;
using Xunit;

namespace PlaywrightDemo.Tests
{
    public class BuyTShirtTests : IAsyncLifetime
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IPage? _page;

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
            }
        }

        [Fact]
        public async Task BuyTshirt()
        {
            if (_page == null)
            {
                throw new InvalidOperationException("Page is not initialized.");
            }

            var buyTShirtPage = new BuyTShirtPage(_page);

            // Navigate to the Sauce Labs Sample Application (https://www.saucedemo.com/) in Incognito mode.
            await buyTShirtPage.GotoAsync();

            // Enter valid credentials to log in.
            var usernames = new[] { "standard_user", "performance_glitch_user", "visual_user" };
            var password = "secret_sauce";
            var random = new Random();
            var username = usernames[random.Next(usernames.Length)];
            await buyTShirtPage.LoginAsync(username, password);

            // Verify that the login is successful and the user is redirected to the products page.
            Assert.Equal("https://www.saucedemo.com/inventory.html", _page.Url);

            // Select a T-shirt by clicking on its image or name.
            await buyTShirtPage.ClickTShirtAsync();

            // Verify that the T-shirt details page is displayed.
            Assert.Equal("https://www.saucedemo.com/inventory-item.html?id=1", _page.Url);
            Assert.True(await _page.Locator("//img[@alt='Sauce Labs Bolt T-Shirt']").IsVisibleAsync());

            // Click the "Add to Cart" button.
            await buyTShirtPage.AddTShirtToCartAsync();

            // Verify that the T-shirt is added to the cart successfully.
            Assert.True(await _page.Locator("#remove").IsVisibleAsync());

            // Navigate to the cart by clicking the cart icon or accessing the cart page directly.
            await buyTShirtPage.GoToCartAsync();

            // Verify that the cart page is displayed.
            Assert.Equal("https://www.saucedemo.com/cart.html", _page.Url);

            // Review the items in the cart and ensure that the T-shirt is listed with the correct details (name, price, quantity, etc.).
            var (itemName, itemPrice, itemQuantity) = await buyTShirtPage.GetCartItemDetailsAsync();
            Assert.Equal("Sauce Labs Bolt T-Shirt", itemName);
            Assert.Equal("$15.99", itemPrice);
            Assert.Equal("1", itemQuantity);

            // Click the "Checkout" button.
            await buyTShirtPage.GoToCheckoutAsync();

            // Verify that the checkout information page is displayed.
            Assert.Equal("https://www.saucedemo.com/checkout-step-one.html", _page.Url);

            // Enter the required checkout information (e.g., name, shipping address, payment details).
            await buyTShirtPage.EnterCheckoutInfoAsync("Shola", "Olagbemisoye", "110123");

            // Click the "Continue" button.
            await buyTShirtPage.ContinueToCheckoutStepTwoAsync();

            // Verify that the order summary page is displayed, showing the T-shirt details and the total amount.
            Assert.Equal("https://www.saucedemo.com/checkout-step-two.html", _page.Url);
            var (sItemName, sItemPrice, sItemQuantity, subtotal, tax, total) = await buyTShirtPage.OrderSummaryDetailsAsync();
            Assert.Equal("Sauce Labs Bolt T-Shirt", sItemName);
            Assert.Equal("$15.99", sItemPrice);
            Assert.Equal("1", sItemQuantity);
            Assert.Equal("Item total: $15.99", subtotal);
            Assert.Equal("Tax: $1.28", tax);
            Assert.Equal("Total: $17.27", total);

            // Click the "Finish" button.
            await buyTShirtPage.CompletePurchaseAsync();

            // Verify that the order confirmation page is displayed, indicating a successful purchase.
            Assert.Equal("https://www.saucedemo.com/checkout-complete.html", _page.Url);
            await buyTShirtPage.VerifyOrderConfirmationAsync();

            // Logout from the application.
            await buyTShirtPage.LogoutAsync();

            // Verify that the user is successfully logged out and redirected to the login page.
            Assert.Equal("https://www.saucedemo.com/", _page.Url);
        }
    }
}
