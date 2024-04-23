import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Basket } from 'src/app/model/basket';
import { Product } from 'src/app/model/product';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BasketService {

  private api_url = environment.API_URL;
  constructor(private http: HttpClient) { }

  getBasketById(id: number) {
    return this.http.get<Basket[]>(`${this.api_url}/basket/${id}`); 
  }

  getBaskets() {
    return this.http.get<Basket[]>(`${this.api_url}/basket`); 
  }

  updateBasket(basket: Basket){
    return this.http.put<Basket>(`${this.api_url}/basket/${basket.id}`, basket);
  }

  createBasket(basket: Basket){
    return this.http.post<Basket>(`${this.api_url}/basket`, basket); 
  }

  deleteBasket(id: number){
    return this.http.delete(`${this.api_url}/basket/${id}`);
  }

  setBasketOrderNumber(basket: Basket){
    return this.http.put<Basket>(`${this.api_url}/basket/order/${basket.id}`, basket);
  }

  getBasketByOrderNumber(orderNumber: number) {
    return this.http.get<Basket[]>(`${this.api_url}/basket/${orderNumber}`); 
  }
}
