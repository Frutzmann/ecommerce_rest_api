import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from 'src/app/model/user';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private api_url = environment.API_URL;
  constructor(private http : HttpClient) { }

  getUserInfo(){
    return this.http.get<User>(`${this.api_url}/me`);
  }

  editUser(id: number, user: User){
    return this.http.put(`${this.api_url}/user/${id}`, user);
  }

  getUsers() {
    return this.http.get<User[]>(`${this.api_url}/user`);
  }

  deleteUser(id: number) {
    return this.http.delete(`${this.api_url}/user/${id}`);
  }

  getUserById(id: number) {
    return this.http.get<User>(`${this.api_url}/user/${id}`);
  }
}


