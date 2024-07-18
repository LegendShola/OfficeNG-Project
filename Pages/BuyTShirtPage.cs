using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightDemo.Pages
{
    public class BuyTShirtPage
    {
        private readonly IPage _page;
        private const string BaseUrl = "https://www.saucedemo.com/";
        private const string TShirtUrl = "https://www.saucedemo.com/inventory-item.html?id=1";
        private const string CartUrl = "https://www.saucedemo.com/cart.html";
        private const string CheckoutStepOneUrl = "https://www.saucedemo.com/checkout-step-one.html";
        private const string CheckoutStepTwoUrl = "https://www.saucedemo.com/checkout-step-two.html";
        private const string CheckoutCompleteUrl = "https://www.saucedemo.com/checkout-complete.html";
        
        public BuyTShirtPage(IPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
        }

        // Navigate to the Sauce Labs Sample Application
        public async Task GotoAsync()
        {
            await _page.GotoAsync(BaseUrl, new PageGotoOptions { Timeout = 60000 });
        }

        // Accept cookies if presented
        public async Task AcceptCookiesAsync()
        {
            const string acceptCookiesSelector = "//span[normalize-space()='Accept']";
            if (await _page.QuerySelectorAsync(acceptCookiesSelector) != null)
            {
                await _page.ClickAsync(acceptCookiesSelector, new PageClickOptions { Timeout = 30000 });
            }
        }

        // Login with provided username and password
        public async Task LoginAsync(string username, string password)
        {
            await _page.FillAsync("#user-name", username);
            await _page.FillAsync("#password", password);
            await _page.ClickAsync("#login-button");
            await _page.WaitForURLAsync("https://www.saucedemo.com/inventory.html");
        }

        // Click on the specified T-shirt image
        public async Task ClickTShirtAsync()
        {
            await _page.ClickAsync("img[alt='Sauce Labs Bolt T-Shirt']");
        }

        // Add the selected T-shirt to the cart
        public async Task AddTShirtToCartAsync()
        {
            await _page.ClickAsync("#add-to-cart");
        }

        // Navigate to the cart page
        public async Task GoToCartAsync()
        {
            await _page.ClickAsync(".shopping_cart_link");
            await _page.WaitForURLAsync(CartUrl);
        }

        // Get details of the item in the cart
        public async Task<(string Name, string Price, string Quantity)> GetCartItemDetailsAsync()
        {
            var itemName = await _page.InnerTextAsync("[data-test='inventory-item-name']");
            var itemPrice = await _page.InnerTextAsync("[data-test='inventory-item-price']");
            var itemQuantity = await _page.InnerTextAsync("[data-test='item-quantity']");

            return (itemName, itemPrice, itemQuantity);
        }

        // Proceed to checkout from the cart
        public async Task GoToCheckoutAsync()
        {
            await _page.ClickAsync("#checkout");
            await _page.WaitForURLAsync(CheckoutStepOneUrl);
        }

        // Enter checkout information
        public async Task EnterCheckoutInfoAsync(string firstName, string lastName, string postalCode)
        {
            await _page.FillAsync("#first-name", firstName);
            await _page.FillAsync("#last-name", lastName);
            await _page.FillAsync("#postal-code", postalCode);
        }

        // Continue from checkout step one to step two
        public async Task ContinueToCheckoutStepTwoAsync()
        {
            await _page.ClickAsync("#continue");
            await _page.WaitForURLAsync(CheckoutStepTwoUrl);
        }

        // Get order summary details
        public async Task<(string Name, string Price, string Quantity, string Subtotal, string Tax, string Total)> OrderSummaryDetailsAsync()
        {
            var itemName = await _page.InnerTextAsync("[data-test='inventory-item-name']");
            var itemPrice = await _page.InnerTextAsync("[data-test='inventory-item-price']");
            var itemQuantity = await _page.InnerTextAsync("[data-test='item-quantity']");
            var subtotal = await _page.InnerTextAsync("[data-test='subtotal-label']");
            var tax = await _page.InnerTextAsync("[data-test='tax-label']");
            var total = await _page.InnerTextAsync("[data-test='total-label']");

            return (itemName, itemPrice, itemQuantity, subtotal, tax, total);
        }

        // Complete the purchase by clicking Finish
        public async Task CompletePurchaseAsync()
        {
            await _page.ClickAsync("#finish");
            await _page.WaitForURLAsync(CheckoutCompleteUrl);
        }

        // Verify order confirmation after purchase
        public async Task VerifyOrderConfirmationAsync()
        {
        await _page.WaitForSelectorAsync("h2.complete-header[data-test='complete-header']");
        var header = await _page.InnerTextAsync("h2.complete-header[data-test='complete-header']");
        if (!header.Contains("Thank you for your order!"))
        {
            throw new InvalidOperationException("Order confirmation header is not displayed correctly.");
        }

        await _page.WaitForSelectorAsync("div.complete-text[data-test='complete-text']");
        var text = await _page.InnerTextAsync("div.complete-text[data-test='complete-text']");
        if (!text.Contains("Your order has been dispatched, and will arrive just as fast as the pony can get there!"))
        {
            throw new InvalidOperationException("Order confirmation text is not displayed correctly.");
        }

        var backButton = await _page.QuerySelectorAsync("#back-to-products");
        if (backButton == null || !await backButton.IsVisibleAsync())
        {
            throw new InvalidOperationException("Back to products button is not visible.");
        }
    }

        // Logout from the application
        public async Task LogoutAsync()
        {
            await _page.ClickAsync("#react-burger-menu-btn");
            await _page.ClickAsync("#logout_sidebar_link");
            await _page.WaitForURLAsync(BaseUrl);
        }
    }
}
