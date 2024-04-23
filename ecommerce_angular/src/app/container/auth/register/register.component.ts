import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from 'src/app/service/auth/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  username: string = '';
  password: string = '';

  completeForm: Boolean = true;
  errorForm: Boolean = false;
  usernameError: Boolean = false;

  constructor(private _authService: AuthService, private router: Router) {}

  ngOnInit(): void {}

  register() {
    this._authService
      .register(this.username, this.password, 0)
      .pipe(
        catchError((err: HttpErrorResponse) => {
          if (err.status === 401) this.errorForm = true;
          else if (err.status === 400) this.completeForm = false;
          else if (err.status === 409) this.usernameError = true
          return throwError(err);
        })
      )
      .subscribe((data) => {
        localStorage.setItem('RegisterUserToken', data.toString());
        this._authService.userSubject.next(data.toString());
      });
  }
}
