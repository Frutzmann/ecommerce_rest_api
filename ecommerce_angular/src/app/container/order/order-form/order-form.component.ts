import { formatNumber } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Address } from 'src/app/model/address';
import { Basket } from 'src/app/model/basket';
import { Order } from 'src/app/model/order';
import { AddressService } from 'src/app/service/address/address.service';
import { BasketService } from 'src/app/service/basket/basket.service';
import { OrderService } from 'src/app/service/order/order.service';

@Component({
  selector: 'app-order-form',
  templateUrl: './order-form.component.html',
  styleUrls: ['./order-form.component.scss']
})
export class OrderFormComponent implements OnInit {

  basket: Basket[] = []
  order: Order = { id: 0, user: null, address: null, totalPrice: 0, orderNumber: 0}
  selectedAddress: Address = {id: 0, road: '', postalCode: '', city: '', country: '', user: null}
  address: Address = {id: 0, road: '', postalCode: '', city: '', country: '', user: null}
  userAddress: Address[] = [];
  totalPrice: number = 0;
  formComplete: boolean= true;

  constructor(private router: Router, private _basketService: BasketService, private _orderService: OrderService, private _addressService: AddressService) { }

  ngOnInit(): void {
    this.getUserBasket();
    this.getUserAddress(); 
    this.computeTotalPrice();
  }

  getUserBasket() {
    this._basketService.getBaskets().subscribe((data) => {
      this.basket = data;
      this.computeTotalPrice();
    })
    
  }

  getUserAddress() {
    this._addressService.getAddressByUser().subscribe((data) => {
      this.userAddress = data;
    })
  }

  computeTotalPrice() {
    for (const b of this.basket)
      this.totalPrice += b.quantity * b.product.price;
    return this.totalPrice;
  }

  createOrder() {
    
      this.order.address = this.selectedAddress;
      this.order.totalPrice = this.totalPrice;

      this._orderService.createOrder(this.order).subscribe((data) => {
        var orderNumber = parseInt(data.toString());
        for(const b of this.basket)
        {
          b.orderNumber = orderNumber;
          this._basketService.setBasketOrderNumber(b).subscribe();
        }
        this.router.navigate(['/profile']);
      });
      
         
    } 

  createAddress() {

    if(this.address.postalCode ==='' || this.address.country === ''
    || this.address.city === '' || this.address.road === '')
      return this.formComplete = false;

    return this._addressService.AddAddress(this.address).subscribe(() => {
      this.router.navigate(['orderForm']);
    })
  }
}
