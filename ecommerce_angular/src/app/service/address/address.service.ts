import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Address } from 'src/app/model/address';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AddressService {
  private api_url = environment.API_URL
  constructor(private http: HttpClient) { }

  getAddress(){
    return this.http.get<Address[]>(`${this.api_url}/address`);
  }

  AddAddress(addr: Address){
    return this.http.post<Address>(`${this.api_url}/address`, addr);
  }

  getAddressById(id: number){
    return this.http.get<Address>(`${this.api_url}/address/${id}`);
  }

  editAddress(addr: Address){
    return this.http.put<Address>(`${this.api_url}/address/${addr.id}`, addr);
  }

  deleteAddress(id: number){
    return this.http.delete(`${this.api_url}/address/${id}`);
  }

  getAddressByUser() {
    return this.http.get<Address[]>(`${this.api_url}/address/user`);
  }
}
