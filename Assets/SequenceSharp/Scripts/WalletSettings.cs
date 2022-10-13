
public class WalletSettings
{
#nullable enable
    /**
        <summary>
         `light` and `dark` are the main themes. To use other available
         themes, you can use the camel case version of the theme names in the wallet settings.
         For example: "Blue Dark" on wallet UI can be passed as "blueDark".
         Note that this setting will not be persisted, use wallet.Open with 'openWithOptions' intent
         to set when you open the wallet for user.
        </summary>
    */
    public string? theme;

    /**
        <summary>
        This image will be displayed on the wallet during the connect/authorize process.
        A 3:1 aspect ratio works best - e.g 1200x400
        </summary>
    */
    public string? bannerUrl;

    /**
        <summary>
        If not specified, all available payment providers will be enabled.
        Note that this setting will not be persisted, use wallet.open with 'openWithOptions' intent
        to set when you open the wallet for user.
        See <see cref="PaymentProviderOption"/>
        </summary>
    */

    public string[]? includedPaymentProviders;
    /**
        <summary>
        Specify a default currency to use with payment providers.
        If not specified, the default is USDC.
        Note that this setting will not be persisted, use wallet.open with 'openWithOptions' intent
        to set when you open the wallet for user.
        See<see cref="CurrencyOption"/>
        </summary>
    */
    public string? defaultFundingCurrency;

    /**
        <summary>
        Specify default purchase amount as an integer, for prefilling the funding amount.
        If not specified, the default is 100.
        Note that this setting will not be persisted, use wallet.open with 'openWithOptions' intent
        to set when you open the wallet for user. 
        </summary>
    */
    public uint? defaultPurchaseAmount;

    /**
        <summary>
        If true, lockFundingCurrencyToDefault disables picking any currency provided by payment
        providers other than the defaultFundingCurrency.
        If false, it allows picking any currency provided by payment providers.
        The default is true.
        Note that this setting will not be persisted, use wallet.open with 'openWithOptions' intent
        to set when you open the wallet for user. 
        </summary>
    */
    public bool? lockFundingCurrencyToDefault;
#nullable disable
}