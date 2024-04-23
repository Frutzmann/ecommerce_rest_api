import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Product } from 'src/app/model/product';
import { ProductService } from 'src/app/service/product/product.service';

@Component({
  selector: 'app-create-product',
  templateUrl: './create-product.component.html',
  styleUrls: ['./create-product.component.scss']
})
export class CreateProductComponent implements OnInit {
  prod: Product = {id: 0, title: '', description: '', link: '', price: 0, currency: '€'}

  constructor(private router: Router, private _productService: ProductService) { }

  ngOnInit(): void {
  }

  createProduct() {
    this._productService.AddProduct(this.prod).subscribe();
    this.router.navigate(['/admin']); 
  }

}
