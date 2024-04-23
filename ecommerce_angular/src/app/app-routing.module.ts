import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './common/auth.guard';
import { AddressFormComponent } from './container/address/address-form/address-form.component';
import { EditAddressFormComponent } from './container/address/edit-address-form/edit-address-form/edit-address-form.component';
import { LoginComponent } from './container/auth/login/login.component';
import { ProfileComponent } from './container/user/profile/profile.component';
import { RegisterComponent } from './container/auth/register/register.component';
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

const routes: Routes = [
  {path : 'login', component: LoginComponent },
  {path: '', redirectTo: '/login', pathMatch: 'full'},
  {path : 'register', component: RegisterComponent},
  {path : 'profile', component: ProfileComponent, canActivate: [AuthGuard]},
  {path: 'user', component: ListUsersComponent, canActivate: [AuthGuard]},
  {path : 'addAddress', component: AddressFormComponent, canActivate: [AuthGuard]},
  {path : 'editAddress', component: EditAddressFormComponent, canActivate: [AuthGuard]},
  {path: 'userProfile', component: UserProfileComponent, canActivate: [AuthGuard] },
  {path: 'productList', component: ProductListComponent, canActivate: [AuthGuard]},
  {path: 'productDetail', component: ProductDetailComponent, canActivate: [AuthGuard]},
  {path: 'basket', component: BasketComponent, canActivate: [AuthGuard]},
  {path: 'orderForm', component: OrderFormComponent, canActivate: [AuthGuard]},
  {path: 'admin', component: AdminComponent, canActivate: [AuthGuard]},
  {path: 'editProduct', component: EditProductComponent, canActivate: [AuthGuard]},
  {path: 'orderDetails', component: OrderDetailsComponent, canActivate: [AuthGuard]},
  {path: 'createProduct', component: CreateProductComponent, canActivate: [AuthGuard]}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
