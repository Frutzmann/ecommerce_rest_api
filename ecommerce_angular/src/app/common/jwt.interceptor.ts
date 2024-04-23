import { HttpEvent, HttpHandler, HttpHeaders, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { AuthService } from "../service/auth/auth.service";

@Injectable()
export class JwtInterceptor implements HttpInterceptor{
    constructor(private _authService: AuthService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let token = this._authService.userSubject.value
        if(token) {
            req = req.clone({
                headers: new HttpHeaders({
                    'Content-Type' : 'application/json',
                    'Authorization' : `Bearer ${token}`
                })
            })
        }
        return next.handle(req);
    }
}
