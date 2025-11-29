import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Orders } from './orders';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  protected readonly title = signal('FoodSyncFrontend');

  orders: any[] = [];

  ordersService = inject(Orders);

  constructor() {
    this.ordersService.get().subscribe((orders: any) => {
      this.orders = orders;
    });
  }
}
