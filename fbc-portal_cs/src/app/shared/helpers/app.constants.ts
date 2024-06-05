export class AppConstants {
    public static MOBILE_RESPONSIVE_WIDTH = 992;
    public static MOBILE_RESPONSIVE_WIDTH_HORIZONTAL = 768;
    public static NAVIGATION_TYPE_COLLAPSIBLE = 'menu-collapsible';
    public static NAVIGATION_TYPE_ACCORDATION = 'menu-accordation';
    public static LAYOUT_STYLE_HORIZONTAL = 'horizontal';
    public static LAYOUT_STYLE_VERTICAL = 'vertical';

    public static SESSIONSTORAGEID_TOKEN_OBJECT: string = "TokenObject";

    public static fireRefreshEventOnWindow = function () {
        const evt = new Event('resize', { bubbles: true, cancelable: false });

        window.dispatchEvent(evt);
    };
}
