import { MenuItem } from "./menu-item";

export interface MenuConfig {
    horizontal_menu: {
        items: Partial<MenuItem>[]
    };
    vertical_menu: {
        items: Partial<MenuItem>[]
    };
}