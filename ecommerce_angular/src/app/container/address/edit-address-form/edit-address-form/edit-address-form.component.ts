import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Address } from 'src/app/model/address';
import { AddressService } from 'src/app/service/address/address.service';

@Component({
  selector: 'app-edit-address-form',
  templateUrl: './edit-address-form.component.html',
  styleUrls: ['./edit-address-form.component.scss']
})
export class EditAddressFormComponent implements OnInit {
  addr: Address = {id: 0, postalCode: '', country: '', city: '', road: ''}
  constructor(private _addressService: AddressService, private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(
      params => {
        this._addressService.getAddressById(params['id']).subscribe((data) => {
          this.addr = data;
        });
      }
    )
  }

  editAddress(){
    this._addressService.editAddress(this.addr).subscribe(() => {
      this.router.navigate(["profile"]);
    })
  }

}
