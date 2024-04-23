import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Product } from 'src/app/model/product';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private api_url = environment.API_URL;
  constructor(private http: HttpClient) {}

  getProducts() {
    return this.http.get<Product[]>(`${this.api_url}/products`); 
  } 

  AddProduct(prod: Product){
    return this.http.post(`${this.api_url}/products`, prod);
  }

  getProductById(id: number){
    return this.http.get<Product>(`${this.api_url}/products/${id}`);
  }

  editProduct(prod: Product){
    return this.http.put<Product>(`${this.api_url}/products/${prod.id}`, prod);
  }

  deleteProduct(id: number){
    return this.http.delete(`${this.api_url}/products/${id}`);
  }
}
