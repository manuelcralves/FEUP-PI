export interface MenuItem {
    title: string;
    icon: string;
    page: string;
    isExternalLink?: boolean;
    issupportExternalLink?: boolean;
    badge: { type: string, value: string };
    submenu: {
        items: Partial<MenuItem>[];
    };
    section: string;
    permissaoNecessaria: string[];
}
