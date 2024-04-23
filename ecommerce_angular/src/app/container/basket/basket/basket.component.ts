import { ReturnStatement } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Basket } from 'src/app/model/basket';
import { BasketService } from 'src/app/service/basket/basket.service';

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.scss']
})
export class BasketComponent implements OnInit {
  baskets: Basket[] = []

  constructor(private router: Router, private _basketService: BasketService) { }

  ngOnInit(): void {
    this.getUserBasket();
  }

  getUserBasket() {
    this._basketService.getBaskets().subscribe((data) => {
      this.baskets = data;
      console.log(this.baskets);
    })
  }

  deleteBasket(basket: Basket, index: number) {
    this._basketService.deleteBasket(basket.id).subscribe();
    this.baskets.splice(index, 1);
  }
  
  order() {
    this.router.navigate(['orderForm']);
  }
}
