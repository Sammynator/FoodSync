import { Component, OnInit } from '@angular/core';
import { FoodSyncService, Order } from '../../services/food-sync';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.html',
  styleUrls: ['./orders.css']
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];

  constructor(private foodService: FoodSyncService) { }

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders() {
    this.foodService.getOrders().subscribe(orders => {
      this.orders = orders;
    });
  }
}
