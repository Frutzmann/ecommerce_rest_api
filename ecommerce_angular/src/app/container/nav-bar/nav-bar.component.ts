import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User, UserRole } from 'src/app/model/user';
import { AuthService } from 'src/app/service/auth/auth.service';
import { UserService } from 'src/app/service/user/user.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit {
  isLog: Boolean = false;
  role: number = 0;
  constructor(private _authService: AuthService, private _userService: UserService, private router: Router) { }

  ngOnInit(): void {
    this.getCurrentUser();
    this.getUserInfo();
  }

  logout(){
    this._authService.logout();
    this.router.navigateByUrl('login');
  }

  getCurrentUser(){
    this._authService.user.subscribe(data => {
      return this.isLog = data ? true : false;
    })
  }

  getUserInfo() {
    this._userService.getUserInfo().subscribe((data) => {
      this._userService.getUserById(data.id).subscribe((data) => {
        this.role = data.role;
      })
    
    });
  }

  isAdmin(){
    return this.role === 1 ? true : false;
  }

}
