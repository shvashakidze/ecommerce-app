import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../../../core/services/auth.service';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-cart',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './cart.component.html',
})
export class CartComponent {
  checkoutLoading = false;
  selectedProvider = signal<'bog' | 'tbc'>('bog');

  constructor(
    public cartService: CartService,
    private authService: AuthService,
    private http: HttpClient,
    private router: Router
  ) {}

  selectProvider(provider: 'bog' | 'tbc') {
    this.selectedProvider.set(provider);
  }

  checkout() {
    if (!this.authService.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }

    this.checkoutLoading = true;

    const orderDto = {
      shippingAddress: 'თბილისი, საქართველო',
      items: this.cartService.items().map(i => ({
        productId: i.id,
        productName: i.name,
        quantity: i.quantity,
        unitPrice: i.price
      }))
    };

    const provider = this.selectedProvider();

    this.http.post<any>(
      `${environment.apiUrl}/orders?provider=${provider}`, orderDto
    ).subscribe({
      next: (res) => {
        this.cartService.clear();
        window.location.href = res.redirectUrl;
      },
      error: (err) => {
        console.error(err);
        this.checkoutLoading = false;
        alert('შეცდომა! სცადე თავიდან.');
      }
    });
  }
}