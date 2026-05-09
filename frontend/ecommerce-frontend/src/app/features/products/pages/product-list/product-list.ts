import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { CartService } from '../../../cart/services/cart.service';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-product-list',
  imports: [ DecimalPipe],
  templateUrl: './product-list.html',
})
export class ProductListComponent implements OnInit {
  products = signal<any[]>([]);
  loading = signal(true);

  constructor(private http: HttpClient, public cartService: CartService) {}

  ngOnInit() {
    this.http.get<any>(`${environment.apiUrl}/products`).subscribe({
      next: (res) => {
        this.products.set(res.products);
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  addToCart(product: any) {
    this.cartService.addItem(product);
  }
}