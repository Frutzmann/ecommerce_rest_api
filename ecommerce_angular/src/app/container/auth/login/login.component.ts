import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from 'src/app/service/auth/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  username: string = '';
  password: string = '';

  completeForm: Boolean = true;
  errorForm: Boolean = false;

  constructor(private _authService: AuthService, private router: Router) {}

  ngOnInit(): void {}

  login() {
    if (this.username === '' || this.password === '') this.completeForm = false;

    this._authService
      .login(this.username, this.password)
      .pipe(
        catchError((err: HttpErrorResponse) => {
          if (err.status === 401) this.errorForm = true;
          return throwError(err);
        })
      )
      .subscribe((data) => {
        localStorage.setItem('UserToken', data.toString());
        this._authService.userSubject.next(data.toString());
        this.router.navigateByUrl('profile');
      });
  }
}
