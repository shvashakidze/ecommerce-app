import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-mock-payment',
  imports: [],
  templateUrl: './mock-payment.html',
})
export class MockPayment implements OnInit {
  orderId = '';
  amount = '';
  provider = 'bog';
  loading = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.orderId = this.route.snapshot.queryParams['orderId'];
    this.amount = this.route.snapshot.queryParams['amount'];
    this.provider = this.route.snapshot.queryParams['provider'] ?? 'bog';
  }

  pay(success: boolean) {
    this.loading = true;
    this.http.post(`${environment.apiUrl.replace('/api', '')}/api/payment/mock-confirm`, {
      orderId: this.orderId,
      success
    }).subscribe({
      next: () => {
        this.router.navigate(
          success ? ['/payment/success'] : ['/payment/fail']
        );
      },
      error: () => { this.loading = false; }
    });
  }
}