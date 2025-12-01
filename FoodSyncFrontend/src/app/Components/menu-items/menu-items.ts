import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FoodSyncService, MenuItem } from '../../services/food-sync';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // needed for ngModel

@Component({
  selector: 'app-menu-items',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './menu-items.html',
  styleUrls: ['./menu-items.css']
})
export class MenuItemsComponent implements OnInit {
  menuItems: MenuItem[] = [];

  // Fields for creating a new menu item
  newName: string = '';
  newPrice: number | null = null;

  // For editing an existing item
  currentEditingItem: MenuItem | null = null;

  constructor(private foodService: FoodSyncService,
              private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.loadMenuItems();
  }

  loadMenuItems() {
    this.foodService.getMenuItems().subscribe(items => {
      this.menuItems = items;
      this.cdr.markForCheck();  
    });
  }

  addMenuItem(newItem: { name: string; price: number }) {
    if (!newItem.name || newItem.price <= 0) return;

    this.foodService.addMenuItem(newItem).subscribe(createdItem => {
      this.menuItems.push(createdItem);
      this.cdr.markForCheck();
    });
  }

  // Called by the template to save a new item using bound inputs
  saveNewItem() {
    const price = this.newPrice ?? 0;
    if (!this.newName || price <= 0) {
      return;
    }

    this.addMenuItem({ name: this.newName.trim(), price: price });
    this.newName = '';
    this.newPrice = null;
  }

  editMenuItem(item: MenuItem) {
    this.currentEditingItem = { ...item }; // clone to edit safely
  }

  updateMenuItem(id: number, updatedData: { name?: string; price?: number }) {
    this.foodService.updateMenuItem(id, updatedData).subscribe(updatedItem => {
      const index = this.menuItems.findIndex(i => i.id === id);
      if (index > -1) {
        this.menuItems[index] = updatedItem;
        this.cdr.markForCheck();
      }
    });
  }

  cancelEdit() {
    this.currentEditingItem = null;
  }

  deleteMenuItem(id: number) {
    this.foodService.deleteMenuItem(id).subscribe(() => {
      this.menuItems = this.menuItems.filter(item => item.id !== id);
      this.cdr.markForCheck();
    });
  }
}
