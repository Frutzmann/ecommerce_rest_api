import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Address } from 'src/app/model/address';
import { Order } from 'src/app/model/order';
import { Product } from 'src/app/model/product';
import { User } from 'src/app/model/user';
import { AddressService } from 'src/app/service/address/address.service';
import { OrderService } from 'src/app/service/order/order.service';
import { ProductService } from 'src/app/service/product/product.service';
import { UserService } from 'src/app/service/user/user.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
  user: User = {id: 0, username: '', role: 0}
  addresses: Address[] = [];
  orders: Order[] = []; 
  products: Product[] = [];

  constructor(private router: Router, private _userService: UserService, private _addressService: AddressService, private _orderService: OrderService, private _productService: ProductService) { }

  ngOnInit(): void {
    this.getUserInfo(); 
    this.isAdmin();
    this.getAddresses(); 
    this.getOrders(); 
    this.getProducts();
  }

  getUserInfo() {
    this._userService.getUserInfo().subscribe((data) => {
      this.user = data;
    });
  }

  isAdmin(){
    return this.user.role === 1 ? true : false;
  }

  getAddresses() {
    this._addressService.getAddress().subscribe((data) => {
      this.addresses = data;
    })

  }

  getOrders() {
    this._orderService.getOrders().subscribe((data) => {
      this.orders = data;
    })
  }

  getProducts() {
    this._productService.getProducts().subscribe((data) => {
      this.products = data;
    })
  }

  editAddress(ad: Address) {
    this.router.navigate(['editAddress'], { queryParams: { id: ad.id } })
  }

  deleteAddress(ad: Address, index: number) {
    this._addressService.deleteAddress(ad.id).subscribe();
    this.addresses.splice(index, 1); 
  }

  editProduct(prod: Product) {
    this.router.navigate(['editProduct'], { queryParams: {id: prod.id }})
  }

  deleteProduct(prod: Product, index: number) {
    this._productService.deleteProduct(prod.id).subscribe();
    this.products.splice(index, 1);
  }

  deleteOrder(or: Order, index: number) {
    this._orderService.deleteOrder(or.id);
    this.orders.splice(index, 1);
  }
}
