import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Address } from 'src/app/model/address';
import { AddressService } from 'src/app/service/address/address.service';

@Component({
  selector: 'app-address-form',
  templateUrl: './address-form.component.html',
  styleUrls: ['./address-form.component.scss']
})
export class AddressFormComponent implements OnInit {
  addr : Address = {id: 0, postalCode: '', country: '', city: '', road: ''}
  formComplete: Boolean = true;
  constructor(private _addressService: AddressService, private router: Router, private fb: FormBuilder) {  }

  ngOnInit(): void {
  }

  AddAddress(){

    if(this.addr.postalCode ==='' || this.addr.country === ''
    || this.addr.city === '' || this.addr.road === '')
      return this.formComplete = false;

    return this._addressService.AddAddress(this.addr).subscribe(() => {
      return this.router.navigate(['profile']);
    })
  }

}
