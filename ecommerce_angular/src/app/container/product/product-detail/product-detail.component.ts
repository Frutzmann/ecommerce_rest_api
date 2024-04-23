import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Basket } from 'src/app/model/basket';
import { Product } from 'src/app/model/product';
import { BasketService } from 'src/app/service/basket/basket.service';
import { ProductService } from 'src/app/service/product/product.service';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {

  product: Product = { id: 0, title: '', description: '', link: '', price: 0, currency: ''}
  basket: Basket = { id: 0, user: undefined, product: undefined, quantity: 1, orderNumber: 0};
  quantity: number = 1;
  constructor(private router: Router, private route: ActivatedRoute, private _productService: ProductService, private _basketService: BasketService) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(
      params => {
        this._productService.getProductById(params['id']).subscribe((data) => {
          this.product = data; 
        });
      }
    )
  }
  AddToBasket() {
    this._basketService.createBasket({id: 0, user: null, product: this.product, quantity: this.quantity, orderNumber: 0}).subscribe((data) => {
      if (data) 
        this.router.navigate(['/basket']); 
    });
  }

  getSrc() {
    return this.product.link ? this.product.link : 'https://store.storeimages.cdn-apple.com/4668/as-images.apple.com/is/iphone-14-pro-model-unselect-gallery-1-202209?wid=5120&hei=2880&fmt=p-jpg&qlt=80&.v=1660753619946';
  }

  incrementQuantity() {
    this.quantity++; 
  }

  decrementQuantity() {
    if(this.quantity > 1)
      this.quantity--;
  }

}
