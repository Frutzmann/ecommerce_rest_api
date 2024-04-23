import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Order } from 'src/app/model/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private api_url = environment.API_URL;
  constructor(private http: HttpClient) { }

  createOrder(order: Order) {
    return this.http.post<Order>(`${this.api_url}/order`, order); 
  }

  getOrders() {
    return this.http.get<Order[]>(`${this.api_url}/order`);
  }

  deleteOrder(id: number) {
    return this.http.delete(`${this.api_url}/order/${id}`)
  }

  getOrderByUser() {
    return this.http.get<Order[]>(`${this.api_url}/order/user`); 
  }

  getOrderById(id: number) {
    return this.http.get<Order>(`${this.api_url}/order/${id}`);
  }
}
