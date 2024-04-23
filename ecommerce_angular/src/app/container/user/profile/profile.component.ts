import { AuthService } from 'src/app/service/auth/auth.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User, UserRole } from 'src/app/model/user';
import { UserService } from 'src/app/service/user/user.service';
import { OrderService } from 'src/app/service/order/order.service';
import { Order } from 'src/app/model/order';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
  user: User = {id: 0, username: '', role: 0 };
  order: Order[] = [];

  currentUsername: String = '';
  constructor(
    private _userService: UserService,
    private _orderService: OrderService,
    private _authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.getUserInfo();
    this.getOrder();
  }

  getUserInfo() {
    this._userService.getUserInfo().subscribe((data) => {
      this.user = data;
      this.currentUsername = data.username;
      this.user.role = data.role === UserRole.ROLE_ADMIN ? 1 : 0;
    });
  }

  editUsername() {
    if (this.user.username !== '') {
      this._userService.editUser(this.user.id, this.user).subscribe((data) => {
        this.router.navigate(['login']);
      });
    }
  }

  getOrder() {
    if (this.user === null) this.order = [];
    else this._orderService.getOrderByUser().subscribe((data) => {
      this.order = data;
    });
  }

  deleteOrder(or: Order, index: number) {
    this._orderService.deleteOrder(or.id).subscribe();
    this.order.splice(index, 1);
  }

  detailOrder(or: Order) {
    this.router.navigate(["orderDetails"], { queryParams: {id: or.id}}); 
  }

  deleteAccount(){
    try{
      this._userService.deleteUser(this.user.id).subscribe(() => {
        this._authService.logout();
      });
    } catch (e) {
      console.log("ERROR: ", e);
    }

  }
}
