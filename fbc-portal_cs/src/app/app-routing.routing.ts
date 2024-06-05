import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PrivateLayoutComponent } from './shared/components/layout/private-layout/private-layout.component';
import { PublicLayoutComponent } from './shared/components/layout/public-layout/public-layout.component';
import { AuthGuard } from './shared/guards/auth.guard';


const appRoutes: Routes = [

  // redirect to home when empty
  {
      path: "",
      redirectTo: "home",
      pathMatch: "full",
  },

  // Public layout
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      { 
        path: 'login',
        loadChildren: () => import("./pages/login/login.module").then((m) => m.LoginModule).catch((error) => console.error(error)),
      },
    ]
  },
  // Private layout
  {
    path: '',
    component: PrivateLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'home', 
        loadChildren: () => import("./pages/home/home.module").then((m) => m.HomeModule).catch((error) => console.error(error)),
      },
      {
        path: 'expenses', 
        loadChildren: () => import("./pages/expenses/expenses.module").then((m) => m.ExpensesModule).catch((error) => console.error(error)),
      },
      {
        path: 'orders', 
        loadChildren: () => import("./pages/orders/orders.module").then((m) => m.OrdersModule).catch((error) => console.error(error)),
      },  
      {
        path: 'approval', 
        loadChildren: () => import("./pages/approval/approval.module").then((m) => m.ApprovalModule).catch((error) => console.error(error)),
      },
      {
        path: 'teams', 
        loadChildren: () => import("./pages/teams/teams.module").then((m) => m.TeamsModule).catch((error) => console.error(error)),
      } 
    ],
  },
  // otherwise redirect to home
  { path: '**', redirectTo: 'home' }
];

@NgModule({

  imports: [RouterModule.forRoot(appRoutes,{ scrollOffset: [0, 0], scrollPositionRestoration: 'top' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }