import { Component, OnInit } from '@angular/core';
import { FoodSyncService, MenuItem } from '../../services/food-sync';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-menu-items',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './menu-items.html',
  styleUrls: ['./menu-items.css']
})
export class MenuItemsComponent implements OnInit {
  menuItems: MenuItem[] = [];

  constructor(private foodService: FoodSyncService) { }

  ngOnInit(): void {
    this.loadMenuItems();
  }

  loadMenuItems() {
    this.foodService.getMenuItems().subscribe(items => {
      this.menuItems = items;
    });
  }

  deleteMenuItem(id: number) {
    this.foodService.deleteMenuItem(id).subscribe(() => {
      this.menuItems = this.menuItems.filter(item => item.id !== id);
    });
  }
}
