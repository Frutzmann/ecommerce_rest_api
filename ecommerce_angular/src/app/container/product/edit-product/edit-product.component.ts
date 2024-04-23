import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Product } from 'src/app/model/product';
import { ProductService } from 'src/app/service/product/product.service';

@Component({
  selector: 'app-edit-product',
  templateUrl: './edit-product.component.html',
  styleUrls: ['./edit-product.component.scss']
})
export class EditProductComponent implements OnInit {

  prod: Product = { id: 0, title: '', description: '', link: '', price: 0, currency: '€'}
  constructor(private router: Router, private _productService: ProductService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(
      params => {
        this._productService.getProductById(params['id']).subscribe((data) => {
          this.prod = data;
        });
      }
    )
  }

  editProduct() {
    this._productService.editProduct(this.prod).subscribe(() => {
      this.router.navigate(["admin"])
    })
  }

}
