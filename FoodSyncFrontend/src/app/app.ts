
import { Component, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MenuItemsComponent } from './Components/menu-items/menu-items';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule, CommonModule, MenuItemsComponent], // <-- include RouterModule
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  protected readonly title = signal('FoodSyncFrontend');
}
