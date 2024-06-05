import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { OrdersComponent } from './orders.component';

const routes: Routes = [
  {
      path: "",
      component: OrdersComponent,
  },
  {
    path: "new",
    loadChildren: () => import("./order/order.module").then((m) => m.OrderModule).catch((error) => console.error(error)),
  },
  {
    path: ":id",
    loadChildren: () => import("./order/order.module").then((m) => m.OrderModule).catch((error) => console.error(error)),  
  },
  {
    path: "approval/:id",
    loadChildren: () => import("./order/order.module").then((m) => m.OrderModule).catch((error) => console.error(error)),  
  },
  {
    path: "sketch/:id",
    loadChildren: () => import("./order/order.module").then((m) => m.OrderModule).catch((error) => console.error(error)),  
  }
  
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OrdersRoutingModule { }
