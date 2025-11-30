import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface MenuItem {
  id: number;
  name: string;
  price: number;
}

export interface OrderItem {
  id: number;
  menuItemId: number;
  menuItem?: MenuItem;
  quantity: number;
  notes: string;
}

export interface Order {
  id: number;
  userId: number;
  status: string;
  items: OrderItem[];
}

@Injectable({
  providedIn: 'root'
})
export class FoodSyncService {
  private apiUrl = 'https://localhost:5208/api'; 

  constructor(private http: HttpClient) { }

  // Menu Items
  getMenuItems(): Observable<MenuItem[]> {
    return this.http.get<MenuItem[]>(`${this.apiUrl}/MenuItems`);
  }

  addMenuItem(menuItem: {name: string, price: number}): Observable<MenuItem> {
    return this.http.post<MenuItem>(`${this.apiUrl}/MenuItems`, menuItem);
  }

  updateMenuItem(id: number, menuItem: {name?: string, price?: number}): Observable<MenuItem> {
    return this.http.put<MenuItem>(`${this.apiUrl}/MenuItems/${id}`, menuItem);
  }

  deleteMenuItem(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/MenuItems/${id}`);
  }

  // Orders
  getOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.apiUrl}/Orders`);
  }

  getOrder(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.apiUrl}/Orders/${id}`);
  }

  addOrder(order: {userId: number, items: {menuItemId: number, quantity: number, notes: string}[]}): Observable<Order> {
    return this.http.post<Order>(`${this.apiUrl}/Orders`, order);
  }

  updateOrderStatus(id: number, status: string): Observable<Order> {
    return this.http.put<Order>(`${this.apiUrl}/Orders/${id}/status`, { status });
  }
}
