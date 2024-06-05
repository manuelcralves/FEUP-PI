import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { ExpensesComponent } from "./expenses.component";

const routes: Routes = [
  {
      path: "",
      component: ExpensesComponent,
  },
  {
    path: "new",
    loadChildren: () => import("./expense/expense.module").then((m) => m.ExpenseModule).catch((error) => console.error(error)),  
  },
  {
    path: ":id",
    loadChildren: () => import("./expense/expense.module").then((m) => m.ExpenseModule).catch((error) => console.error(error)),  
  },
  {
    path: "approval/:id",
    loadChildren: () => import("./expense/expense.module").then((m) => m.ExpenseModule).catch((error) => console.error(error)),  
  },
  {
    path: "sketch/:id",
    loadChildren: () => import("./expense/expense.module").then((m) => m.ExpenseModule).catch((error) => console.error(error)),  
  }
  
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ExpensesRoutingModule { }

