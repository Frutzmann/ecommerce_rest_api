import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private api = environment.API_URL

  options = {
    headers : new HttpHeaders({
      'Content-Type' : 'application/json'
    }), responseType: 'text' as 'json'
  }

  public user: Observable<string>
  public userSubject : BehaviorSubject<string>

  constructor(private http: HttpClient, private router: Router) {
    this.userSubject = new BehaviorSubject<string>(localStorage.getItem('UserToken') || '')
    this.user = this.userSubject.asObservable();
   }

  login(username: string, password: string){
    return this.http.post(`${this.api}/authenticate`, {username: username, password: password}, this.options);
  }

  register(username: string, password: string, role: number){
    return this.http.post(`${this.api}/register`, {username: username, password: password, role: role}, this.options);
  }

  logout(){
    localStorage.removeItem('UserToken');
    this.userSubject.next('');
    this.router.navigate(['login'])
  }

}
