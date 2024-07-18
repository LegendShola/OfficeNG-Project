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

        public async Task GotoAsync()
        {
            await _page.GotoAsync(BaseUrl, new PageGotoOptions { Timeout = 60000 });
        }

        public async Task AcceptCookiesAsync()
        {
            const string acceptCookiesSelector = "//span[normalize-space()='Accept']";
            if (await _page.QuerySelectorAsync(acceptCookiesSelector) != null)
            {
                await _page.ClickAsync(acceptCookiesSelector, new PageClickOptions { Timeout = 30000 });
            }
        }

        public async Task LoginAsync(string username, string password)
        {
            await _page.FillAsync("#user-name", username);
            await _page.FillAsync("#password", password);
            await _page.ClickAsync("#login-button");
            await _page.WaitForURLAsync("https://www.saucedemo.com/inventory.html");
        }

        public async Task ClickTShirtAsync()
        {
            await _page.ClickAsync("img[alt='Sauce Labs Bolt T-Shirt']");
        }

        public async Task ClickAddToCartAsync()
        {
            await _page.ClickAsync("#add-to-cart");
        }

        public async Task ClickCartAsync()
        {
            await _page.ClickAsync(".shopping_cart_link");
            await _page.WaitForURLAsync(CartUrl);
        }

        public async Task ClickCheckoutAsync()
        {
            await _page.ClickAsync("#checkout");
            await _page.WaitForURLAsync(CheckoutStepOneUrl);
        }

        public async Task<(string Name, string Price, string Quantity)> GetCartItemDetailsAsync()
        {
            var itemName = await _page.InnerTextAsync("[data-test='inventory-item-name']");
            var itemPrice = await _page.InnerTextAsync("[data-test='inventory-item-price']");
            var itemQuantity = await _page.InnerTextAsync("[data-test='item-quantity']");

            return (itemName, itemPrice, itemQuantity);
        }

        public async Task EnterCheckoutInfoAsync(string firstname, string lastname, string postalcode)
        {
            await _page.FillAsync("#first-name", firstname);
            await _page.FillAsync("#last-name", lastname);
            await _page.FillAsync("#postal-code", postalcode);
        }

        public async Task ClickContinueAsync()
        {
            await _page.ClickAsync("#continue");
            await _page.WaitForURLAsync(CheckoutStepTwoUrl);
        }

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

        public async Task ClickFinishAsync()
        {
            await _page.ClickAsync("#finish");
            await _page.WaitForURLAsync(CheckoutCompleteUrl);
        }

        public async Task VerifyOrderConfirmationDisplayedAsync()
        {
            var header = await _page.InnerTextAsync("h2.complete-header[data-test='complete-header']");
            if (!header.Contains("Thank you for your order!"))
            {
                throw new Exception("Order confirmation header is not displayed correctly.");
            }

            var text = await _page.InnerTextAsync("div.complete-text[data-test='complete-text']");
            if (!text.Contains("Your order has been dispatched, and will arrive just as fast as the pony can get there!"))
            {
                throw new Exception("Order confirmation text is not displayed correctly.");
            }

            var backButton = _page.Locator("#back-to-products");
            if (!await backButton.IsVisibleAsync())
            {
                throw new Exception("Back to products button is not visible.");
            }
        }

        public async Task ClickLogoutButtonAsync()
        {
            await _page.ClickAsync("#react-burger-menu-btn");
            await _page.ClickAsync("#logout_sidebar_link");
            await _page.WaitForURLAsync(BaseUrl);
        }

        public async Task<string> GetErrorMessageAsync()
        {
            return await _page.InnerTextAsync("//div[@id='notistack-snackbar']");
        }
    }
}
