
import { MenuConfig } from "./menu-config";


export const MenuSettingsConfig: MenuConfig = {
  vertical_menu: {
    items: [
      {
        title: 'Home',
        icon: 'la-home',
        page: '/home',
      },
      {
        title: 'Encomendas',
        icon: 'la-cube',
        page: '/orders'
      },
      {
        title: 'Despesas',
        icon: 'la-bar-chart-o',
        page: '/expenses'
      },
      {
        title: 'Aprovação',
        icon: 'la-check-square',
        page: '/approval',
        permissaoNecessaria: ['TEAM', 'ADMIN'],
      },
      {
        title: 'Equipas',
        icon: 'la-users',
        page: '/teams',
        permissaoNecessaria: ['TEAM', 'ADMIN'],
      }
    ]
  },
  horizontal_menu: {
    items: []
  }
};





