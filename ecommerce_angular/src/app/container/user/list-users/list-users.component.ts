import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User, UserRole } from 'src/app/model/user';
import { UserService } from 'src/app/service/user/user.service';

@Component({
  selector: 'app-list-users',
  templateUrl: './list-users.component.html',
  styleUrls: ['./list-users.component.scss']
})
export class ListUsersComponent implements OnInit {
  user: User = {id: 0, username: '', role: 0}
  users: User[] = [];

  constructor(private _userService: UserService, private router: Router) { }

  ngOnInit(): void {
    this.getUser();
    this.getUserInfo();
  }

  getUser(){
    this._userService.getUsers().subscribe((data) => {
      this.users = data;
    })

  }

  getUserInfo(){
    this._userService.getUserInfo().subscribe((data) => {
      this.user = data;
    });
  }

  isAdmin(){
    return this.user.role === 1 ? true : false;
  }

  edit(u: User){

  }

  goToProfile(u: User){
    this.router.navigate(['userProfile'], {queryParams: {id: u.id }});
  }

  deleteUser(u: User, index: number){
    this._userService.deleteUser(u.id).subscribe();
    this.users.splice(index, 1);
  }

}
