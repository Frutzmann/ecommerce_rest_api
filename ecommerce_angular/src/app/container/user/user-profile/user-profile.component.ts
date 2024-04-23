import { UserService } from 'src/app/service/user/user.service';
import { animate } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { Address } from 'src/app/model/address';
import { User, UserRole } from 'src/app/model/user';
import { AddressService } from 'src/app/service/address/address.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
  user: User = {id: 0, username: '', role: 0};
  currentUser : User = { id: 0, username: '', role: 0}
  address : Address[] = [];
  username: string = '';
  constructor(private _userService: UserService, private _addressService: AddressService, private router: Router, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.getUserInfo();
    this.route.queryParams.subscribe(params => {
      this._userService.getUserById(params['id']).subscribe((data) => {
        this.user = data;
        this.username = data.username;
        this.getAddresses();
      })
    })
  }

  getUserInfo() {
    this._userService.getUserInfo().subscribe((data) => {
      this.currentUser = data;
      this.currentUser.role = data.role === UserRole.ROLE_ADMIN ? 1 : 0;
    })
  }

  getAddresses(){
    this._addressService.getAddress().subscribe((data) => {
      const filter = data.filter(x => x.user.id == this.user.id);
      this.address = filter;
    })
  }

  editUsername(){
    if (this.username !== '' || this.username !== undefined) {
      this._userService.editUser(this.user.id, this.user).subscribe(() => {
        this.router.navigate(['user']);
      })
    }
  }


  isAdmin(){
    return this.currentUser.role === 1 ? true : false;
  }



}
