import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Product } from 'src/app/model/product';
import { Basket } from 'src/app/model/basket';
import { ProductService } from 'src/app/service/product/product.service';
import { BasketService } from 'src/app/service/basket/basket.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {

  products: Product[] = [];
  basket: Basket = { id: 0, user: undefined, product: undefined, quantity: 1, orderNumber: 0};

  constructor(private router: Router, private _productService: ProductService, private _basketService: BasketService) { }

  ngOnInit(): void {
    this.initProducts();
  }

  initProducts() {
    this._productService.getProducts().subscribe((data) => {
      this.products = data;
    })
  }

  showProductDescription(prod: Product) {
    this.router.navigate(['productDetail'], { queryParams: {id: prod.id} }); 
  }

  getSrc(prod: Product) {
    return prod.link ? prod.link : 'https://store.storeimages.cdn-apple.com/4668/as-images.apple.com/is/iphone-14-pro-model-unselect-gallery-1-202209?wid=5120&hei=2880&fmt=p-jpg&qlt=80&.v=1660753619946'
  }
  

  addProduct(prod: Product) {
    this.basket.product = prod;
    this._basketService.createBasket(this.basket).subscribe();
  }
}
