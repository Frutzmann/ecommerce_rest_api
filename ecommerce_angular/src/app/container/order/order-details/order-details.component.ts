import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Basket } from 'src/app/model/basket';
import { Order } from 'src/app/model/order';
import { BasketService } from 'src/app/service/basket/basket.service';
import { OrderService } from 'src/app/service/order/order.service';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss']
})
export class OrderDetailsComponent implements OnInit {

  order: Order = { id: 0, user: null, address: null, orderNumber: 0, totalPrice: 0}
  basket: Basket[] = []; 
  constructor(private router: Router, private _orderService: OrderService, private _basketService: BasketService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(
      params => {
        this._orderService.getOrderById(params["id"]).subscribe((data) => {
          this.order = data;
          this.getBasket(); 
        });
      }
    )
  }

  getBasket() {
    this._basketService.getBasketByOrderNumber(this.order.orderNumber).subscribe((data) => {
      this.basket = data; 
    })
  }

}
