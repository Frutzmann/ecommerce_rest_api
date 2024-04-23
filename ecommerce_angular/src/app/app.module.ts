import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import {HttpClient, HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './container/auth/login/login.component';
import { RegisterComponent } from './container/auth/register/register.component';
import { NavBarComponent } from './container/nav-bar/nav-bar.component';
import { ProfileComponent } from './container/user/profile/profile.component';
import { JwtInterceptor } from './common/jwt.interceptor';
import { AddressFormComponent } from './container/address/address-form/address-form.component';
import { EditAddressFormComponent } from './container/address/edit-address-form/edit-address-form/edit-address-form.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatCardModule} from '@angular/material/card';
import { ListUsersComponent } from './container/user/list-users/list-users.component';
import { UserProfileComponent } from './container/user/user-profile/user-profile.component';
import { ProductListComponent } from './container/product/product-list/product-list.component';
import { ProductDetailComponent } from './container/product/product-detail/product-detail.component';
import { BasketComponent } from './container/basket/basket/basket.component';
import { OrderFormComponent } from './container/order/order-form/order-form.component';
import { AdminComponent } from './container/admin/admin/admin.component';
import { EditProductComponent } from './container/product/edit-product/edit-product.component';
import { OrderDetailsComponent } from './container/order/order-details/order-details.component';
import { CreateProductComponent } from './container/product/create-product/create-product.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    NavBarComponent,
    ProfileComponent,
    AddressFormComponent,
    EditAddressFormComponent,
    ListUsersComponent,
    UserProfileComponent,
    ProductListComponent,
    ProductDetailComponent,
    BasketComponent,
    OrderFormComponent,
    AdminComponent,
    EditProductComponent,
    OrderDetailsComponent,
    CreateProductComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MatCardModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    HttpClient,
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
