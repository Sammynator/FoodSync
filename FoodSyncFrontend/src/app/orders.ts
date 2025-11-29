import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class Orders {

  constructor() {}

    private http = inject(HttpClient);
    private apiUrl = environment.apiURL + "/orders";

    public get(): Observable<any> {
      return this.http.get(this.apiUrl);
    }
}
