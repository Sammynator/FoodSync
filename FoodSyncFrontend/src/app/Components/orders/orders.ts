import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FoodSyncService, Order, MenuItem } from '../../services/food-sync';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './orders.html',
  styleUrls: ['./orders.css']
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];
  currentEditingOrder: Order | null = null; // for editing UI

  // Status values must match server-side values. Labels are user-friendly.
  statuses = [
    { value: 'pending', label: 'Pending' },
    { value: 'preparing', label: 'Preparing' },
    { value: 'done', label: 'Done' },
    { value: 'served', label: 'Served' }
  ];
  // For creating a new order
  menuItems: MenuItem[] = [];
  showNewOrderPanel = false;
  newOrderUserId: number | null = null;
  newOrderSelectedItemId: number | null = null;
  newOrderItems: { menuItemId: number; name?: string; price?: number; quantity: number }[] = [];
  // Editing state
  editedStatus: Record<number, string> = {};
  // For editing an order
  editSelectedItemId: number | null = null;

  constructor(
    private foodService: FoodSyncService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  // Load menu items for creating orders
  loadMenuItems() {
    this.foodService.getMenuItems().subscribe(items => {
      this.menuItems = items;
      this.cdr.markForCheck();
    });
  }

  openNewOrder() {
    this.showNewOrderPanel = true;
    // ensure menu items are available
    if (!this.menuItems || this.menuItems.length === 0) {
      this.loadMenuItems();
    }
  }

  // Start editing an existing order: clone it to currentEditingOrder
  startEdit(order: Order) {
    this.currentEditingOrder = JSON.parse(JSON.stringify(order));
    // initialize edited status for this order
    this.editedStatus[order.id] = order.status;
    // ensure menu items loaded for selection
    if (!this.menuItems || this.menuItems.length === 0) this.loadMenuItems();
  }

  addItemToEdit() {
    if (!this.currentEditingOrder) return;
    if (this.editSelectedItemId == null) return;
    const menu = this.menuItems.find(m => m.id === this.editSelectedItemId);
    if (!menu) return;
    const existing = this.currentEditingOrder.items.find(i => i.menuItemId === menu.id && i.id === 0);
    // If item exists (by menuItemId) increase quantity, otherwise add a new item (id = 0)
    const existingByMenu = this.currentEditingOrder.items.find(i => i.menuItemId === menu.id);
    if (existingByMenu) {
      existingByMenu.quantity = (existingByMenu.quantity || 0) + 1;
    } else {
      this.currentEditingOrder.items.push({ id: 0, menuItemId: menu.id, menuItem: menu as any, quantity: 1, notes: '' } as any);
    }
    this.editSelectedItemId = null;
    this.cdr.markForCheck();
  }

  removeEditItem(index: number) {
    if (!this.currentEditingOrder) return;
    this.currentEditingOrder.items.splice(index, 1);
    this.cdr.markForCheck();
  }

  saveEdit() {
    if (!this.currentEditingOrder) return;
    const id = this.currentEditingOrder.id;
    const payload = {
      userId: this.currentEditingOrder.userId,
      items: this.currentEditingOrder.items.map(i => ({ id: i.id || 0, menuItemId: i.menuItemId, quantity: i.quantity, notes: i.notes || '' }))
    };
    this.foodService.updateOrder(id, payload).subscribe(updated => {
      const index = this.orders.findIndex(o => o.id === id);
      if (index > -1) this.orders[index] = updated;
      this.currentEditingOrder = null;
      this.cdr.markForCheck();
    }, err => console.error('saveEdit failed', err));
  }

  cancelEditMode() {
    this.currentEditingOrder = null;
  }

  // Save status change explicitly (user must press Save Status)
  saveStatus(order: Order) {
    const newStatus = this.editedStatus[order.id];
    if (!newStatus || newStatus === order.status) return;
    this.updateOrderStatus(order.id, newStatus);
  }

  addItemToNewOrder() {
    if (this.newOrderSelectedItemId == null) return;
    const menu = this.menuItems.find(m => m.id === this.newOrderSelectedItemId);
    if (!menu) return;
    const existing = this.newOrderItems.find(i => i.menuItemId === menu.id);
    if (existing) {
      existing.quantity += 1;
    } else {
      this.newOrderItems.push({ menuItemId: menu.id, name: menu.name, price: menu.price, quantity: 1 });
    }
    this.newOrderSelectedItemId = null;
    this.cdr.markForCheck();
  }

  removeNewOrderItem(index: number) {
    this.newOrderItems.splice(index, 1);
    this.cdr.markForCheck();
  }

  saveNewOrder() {
    if (this.newOrderUserId == null || this.newOrderItems.length === 0) {
      console.warn('saveNewOrder: missing userId or no items', { userId: this.newOrderUserId, items: this.newOrderItems });
      return;
    }

    const payload = {
      userId: Number(this.newOrderUserId),
      items: this.newOrderItems.map(i => ({ menuItemId: i.menuItemId, quantity: i.quantity, notes: '' }))
    };

    console.log('saveNewOrder: sending payload', payload);

    this.foodService.addOrder(payload).subscribe({
      next: created => {
        console.log('saveNewOrder: created', created);
        this.orders.push(created);
        this.showNewOrderPanel = false;
        this.newOrderUserId = null;
        this.newOrderItems = [];
        this.cdr.markForCheck();
      },
      error: err => {
        console.error('saveNewOrder: failed', err);
      }
    });
  }

  // Change status helper used by template
  changeStatus(order: Order, newStatus: string) {
    if (!order || !newStatus) return;
    this.updateOrderStatus(order.id, newStatus);
  }

  // Compute total for an order (use from template instead of heavy expressions)
  totalForOrder(order: Order): number {
    if (!order || !order.items) return 0;
    return order.items.reduce((sum, it) => {
      const price = (it.menuItem && it.menuItem.price) ? it.menuItem.price : 0;
      const qty = it.quantity ?? 0;
      return sum + (price * qty);
    }, 0);
  }

  // Load all orders
  loadOrders() {
    this.foodService.getOrders().subscribe(orders => {
      this.orders = orders;
      // initialize editedStatus map so selects show current status
      for (const o of this.orders) {
        this.editedStatus[o.id] = o.status;
      }
      this.cdr.markForCheck();
    });
  }

  // Load single order (optional)
  getOrder(id: number) {
    this.foodService.getOrder(id).subscribe(order => {
      const index = this.orders.findIndex(o => o.id === id);
      if (index > -1) {
        this.orders[index] = order;
      } else {
        this.orders.push(order);
      }
      this.cdr.markForCheck();
    });
  }

  // Add a new order
  addOrder(newOrder: { userId: number; items: { menuItemId: number; quantity: number; notes?: string }[] }) {
    this.foodService.addOrder(newOrder).subscribe(createdOrder => {
      this.orders.push(createdOrder);
      this.cdr.markForCheck();
    });
  }

  // Update existing order (user or items)
  updateOrder(id: number, updatedOrder: { userId?: number; items: { id?: number; menuItemId?: number; quantity?: number; notes?: string }[] }) {
    this.foodService.updateOrder(id, updatedOrder).subscribe(updated => {
      const index = this.orders.findIndex(o => o.id === id);
      if (index > -1) {
        this.orders[index] = updated;
        this.cdr.markForCheck();
      }
    });
  }

  // Update order status
  updateOrderStatus(id: number, status: string) {
    this.foodService.updateOrderStatus(id, { status }).subscribe(updated => {
      const index = this.orders.findIndex(o => o.id === id);
      if (index > -1) {
        this.orders[index] = updated;
        this.cdr.markForCheck();
      }
    });
  }

  // Delete an order
  deleteOrder(id: number) {
    this.foodService.deleteOrder(id).subscribe(() => {
      this.orders = this.orders.filter(o => o.id !== id);
      this.cdr.markForCheck();
    });
  }

  // Optional: cancel editing
  cancelEdit() {
    this.currentEditingOrder = null;
  }
}
