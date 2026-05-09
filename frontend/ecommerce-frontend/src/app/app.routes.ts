import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'products', pathMatch: 'full' },
  {
    path: 'products',
    loadComponent: () =>
      import('./features/products/pages/product-list/product-list')
        .then(m => m.ProductListComponent)
  },
  {
    path: 'products/:id',
    loadComponent: () =>
      import('./features/products/pages/product-detail/product-detail')
        .then(m => m.ProductDetail)
  },
  {
    path: 'cart',
    loadComponent: () =>
      import('./features/cart/pages/cart/cart.component')
        .then(m => m.CartComponent)
  },
  {
    path: 'orders',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/orders/pages/order-list/order-list')
        .then(m => m.OrderListComponent)
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/pages/login/login')
        .then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/pages/register/register')
        .then(m => m.RegisterComponent)
  },
  {
  path: 'payment/mock',
  loadComponent: () =>
    import('./features/payment/pages/mock-payment/mock-payment')
      .then(m => m.MockPayment)
},
{
  path: 'payment/success',
  loadComponent: () =>
    import('./features/payment/pages/payment-success/payment-success')
      .then(m => m.PaymentSuccess)
},
{
  path: 'payment/fail',
  loadComponent: () =>
    import('./features/payment/pages/payment-fail/payment-fail')
      .then(m => m.PaymentFail)
},
  { path: '**', redirectTo: 'products' }
];