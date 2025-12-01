import { Routes } from '@angular/router';
import { MenuItemsComponent } from './Components/menu-items/menu-items';
import { OrdersComponent } from './Components/orders/orders';

export const routes: Routes = [
  { path: '', redirectTo: 'orders', pathMatch: 'full' },
  { path: 'menu-items', component: MenuItemsComponent },
  { path: 'orders', component: OrdersComponent }
];
